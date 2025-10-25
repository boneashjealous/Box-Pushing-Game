using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

//基础管理器组合存档系统，场景系统，设置更新系统，缓存配置系统
//便于跨场景传递信息以及复用
public class BaseManager : MonoBehaviour
{
    private static BaseManager instance;
    private void Awake()
    {
        //初始化 跨场景
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        archiveManager = new ArchiveManager();
        sceneLoader = new SceneLoader();
        settingsManager = new SettingsManager();
        cacheManager = new CacheManager();
    }
    //存档管理
    private ArchiveManager archiveManager;
    //场景切换
    private SceneLoader sceneLoader;
    //设置更新
    private SettingsManager settingsManager;
    //缓存配置
    private CacheManager cacheManager;
    //查询存档
    public ArchiveMetaData[] QueryArchiveMetaDatas()
    {
        return archiveManager.LoadArchiveMetaData();
    }
    //获取存档
    public ArchiveMetaData[] GetArchiveMetaDatas()
    {
        return archiveManager.archiveMetaDatas;
    }
    //加载游戏
    public void LoadGame(ArchiveMetaData archiveMetaData)
    {
        archiveManager.LoadFromJson(archiveMetaData.id);
        archiveManager.currentArchiveMetaData = archiveMetaData;
        LoadLevel(archiveManager.currentArchiveData.lastTimeLevel);
    }
    //新游戏
    public void NewGame()
    {
        LoadGame(archiveManager.NewArchiveData());
    }
    //保存元数据
    public void SaveMetaData(ArchiveMetaData archiveMetaData)
    {
        archiveManager.SaveArchiveMetaData(archiveMetaData);
    }
    //删除存档
    public void DeleteArchive(string saveid)
    {
        archiveManager.DeleteSaveFiles(saveid);
    }
    //设置分辨率全屏
    public void ChangeResolution(int width, int height, bool fullScreen)
    {
        settingsManager.ChangeResolution(width, height, fullScreen);
    }
    //进入关卡
    public void LoadLevel(int level)
    {
        archiveManager.currentArchiveData.lastTimeLevel = level;
        SaveArchiveData();
        sceneLoader.LoadScene(1);
    }
    //返回主菜单
    public void BackMainMenu()
    {
        sceneLoader.LoadScene(0);
    }
    //获得存档数据
    public ArchiveData GetCurrentArchiveData()
    {
        return archiveManager.currentArchiveData;
    }
    //保存存档数据
    public void SaveArchiveData()
    {
        archiveManager.SaveData();
    }
    //重置关卡
    public void ResetLevel()
    {
        LoadLevel(archiveManager.currentArchiveData.lastTimeLevel);
    }


    //定义存档管理器
    public class ArchiveManager
    {
        //初始化加载元数据
        private ArchiveMetaData[] _archiveMetaDatas;
        //获取只读元数据
        public ArchiveMetaData[] archiveMetaDatas { get { return _archiveMetaDatas; } }
        //当前存档
        public ArchiveData currentArchiveData;
        public ArchiveMetaData currentArchiveMetaData;
        //加载存档元数据
        public ArchiveMetaData[] LoadArchiveMetaData()
        {
            List<ArchiveMetaData> archiveMetaData = new();
            string saveDir = Application.persistentDataPath;

            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);

            foreach (string path in Directory.GetFiles(saveDir, "*.meta"))
            {
                try
                {
                    string jsonData = File.ReadAllText(path, Encoding.UTF8);
                    ArchiveMetaData meta = JsonUtility.FromJson<ArchiveMetaData>(jsonData);
                    archiveMetaData.Add(meta);
                }
                catch (Exception e)
                {
                    Debug.LogError($"加载存档元数据失败: {path}\n{e.Message}");
                }
            }
            ArchiveMetaData[] archiveMetaDatas = new ArchiveMetaData[3];
            if (archiveMetaData == null || archiveMetaData.Count == 0)
            {
                _archiveMetaDatas = archiveMetaDatas;
                return archiveMetaDatas;
            }
            try
            {
                // 按最后修改时间降序排序（最近的在最前）
                archiveMetaData = archiveMetaData.OrderByDescending(save => save.lastModified).ToList();

                // 填充结果数组
                for (int i = 0; i < 3; i++)
                {
                    if (i < archiveMetaData.Count)
                    {
                        archiveMetaDatas[i] = archiveMetaData[i];
                    }
                    else
                    {
                        // 存档不足3个时填充空值
                        archiveMetaDatas[i] = null;
                    }
                }
                _archiveMetaDatas = archiveMetaDatas;
                return archiveMetaDatas;
            }
            catch (Exception e)
            {
                Debug.LogError($"获取最近存档时出错: {e.Message}");
                _archiveMetaDatas = archiveMetaDatas;
                return archiveMetaDatas;
            }
        }
        //读档
        public void LoadFromJson(string saveId)
        {
            string path = Path.Combine(Application.persistentDataPath, $"{saveId}.sav");
            try
            {
                var json = File.ReadAllText(path);
                var data = JsonUtility.FromJson<ArchiveData>(json);
                currentArchiveData = data;
                return;
            }
            catch (Exception e)
            {
                Debug.LogError($"读档于{path}时，发生{e}错误");
                return;
            }

        }
        //保存数据
        public void SaveData()
        {
            SaveJson(currentArchiveData, currentArchiveMetaData.id);
            currentArchiveMetaData.lastModified = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            SaveArchiveMetaData(currentArchiveMetaData);
        }
        //存档
        public void SaveJson(ArchiveData archiveData, string saveId)
        {
            string path = Path.Combine(Application.persistentDataPath, $"{saveId}.sav");
            try
            {
                var json = JsonUtility.ToJson(archiveData);
                File.WriteAllText(path, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"存档于{path}时，发生{e}错误");
            }
        }
        // 删除存档
        public void DeleteSaveFiles(string saveId)
        {
            string saveDir = Application.persistentDataPath;

            // 删除元数据文件
            string metaPath = Path.Combine(saveDir, $"{saveId}.meta");
            DeleteFile(metaPath);

            // 删除存档文件
            string savePath = Path.Combine(saveDir, $"{saveId}.sav");
            DeleteFile(savePath);
        }
        // 删除文件
        private void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"文件已删除: {path}");
            }
            else
            {
                Debug.LogError($"未找到路径上的文件: {path}");
            }
        }
        //保存元数据
        public void SaveArchiveMetaData(ArchiveMetaData archiveMetaData)
        {
            string path = Path.Combine(Application.persistentDataPath, $"{archiveMetaData.id}.meta");
            try
            {
                string json = JsonUtility.ToJson(archiveMetaData, true);
                File.WriteAllText(path, json, Encoding.UTF8);
            }
            catch (Exception e)
            {
                Debug.LogError($"元数据存档于{path}时，发生{e}错误");
            }
        }
        //新建存档
        public ArchiveMetaData NewArchiveData()
        {
            string saveId = Guid.NewGuid().ToString();
            // 创建元数据
            ArchiveMetaData meta = new ArchiveMetaData
            {
                id = saveId,
                saveName = "新存档",
                lastModified = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            };
            ArchiveData archiveData = new ArchiveData()
            {
                lastTimeLevel = 1, // 初始关卡为0
                largestLevelUnlocked = 1 // 初始解锁最大关卡为0
            };//等待构造
            SaveArchiveMetaData(meta);
            SaveJson(archiveData, saveId);
            return meta;
        }
    }
    //定义场景管理器
    public class SceneLoader
    {
        //切换场景
        public void LoadScene(int sceneIndex)
        {
            if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(sceneIndex);
            else
                Debug.LogError($"场景加载器试图切换为不存在的场景{sceneIndex}");
        }
    }
    //定义设置管理器
    public class SettingsManager
    {
        //修改分辨率
        public void ChangeResolution(int width, int height, bool fullScreen)
        {
            Screen.SetResolution(width, height, fullScreen);
        }
        //保存设置
        public void SaveSettings()
        {
            PlayerPrefs.SetInt("ScreenWidth", Screen.width);
            PlayerPrefs.SetInt("ScreenHeight", Screen.height);
            PlayerPrefs.SetInt("FullScreenMode", (int)Screen.fullScreenMode);
        }
        //读取设置
        public (int, int, int) LoadSettings()
        {
            return (PlayerPrefs.GetInt("ScreenWidth"), PlayerPrefs.GetInt("ScreenHeight"), PlayerPrefs.GetInt("FullScreenMode"));
        }
        //恢复默认设置
        public void RestoreDefaultSettings()
        {
            Screen.SetResolution(Screen.width, Screen.height, Screen.fullScreenMode);
        }
    }
    //定义缓存管理器
    public class CacheManager
    {
        public ArchiveData archiveData;
        //地图缓存
        //元素缓存
        //查询地图缓存
        //查询元素缓存
        //获取地图缓存
        //获取元素缓存
        //输出地图缓存
        //输出元素缓存
    }
}

//存档数据
[Serializable]
public class ArchiveData
{
    public int lastTimeLevel; // 上次关卡
    public int largestLevelUnlocked; //解锁的最大关卡
}

//存档元数据
[Serializable]
public class ArchiveMetaData
{
    public string id; // GUID
    public string saveName; // 支持中文
    public string lastModified;
}


