using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
//初始化，管理关卡，存档，游戏胜利等
public class GameManager : MonoBehaviour
{
    //单例
    private static GameManager instance;
    public static GameManager Instance{ get{ return instance; }private set { instance = value; } }
    //当前关卡数据
    private ArchiveData _archiveData;
    public ArchiveData archiveData { get { return _archiveData; }private set { _archiveData = value; } }
    //抵达终点箱子数量
    private int arrivalEnd;
    [SerializeField]
    private int levelMax;
    public int ArrivalEnd
    {
        get { return arrivalEnd; }
        set
        {
            arrivalEnd = value;
            if (arrivalEnd >= arrivalTarget)
            {
                //如果抵达目标箱子数量大于等于目标需求箱子数量
                //则游戏胜利
                GameWin();
            }
        }
    }
    //抵达目标需求箱子数量
    private int arrivalTarget;
    private AudioSource audioSource;
    // 音频集合
    private AudioClip[] audioClips;
    private BaseManager baseManager;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        if(GameObject.Find("BaseManager") != null)
        {
            BaseManager baseManager = GameObject.Find("BaseManager").GetComponent<BaseManager>();
            this.baseManager = baseManager ?? throw new Exception("baseManager参数不能为null");
            archiveData = baseManager.GetCurrentArchiveData();
            Instantiate(Resources.Load<GameObject>($"LevelMap/Level_{archiveData.lastTimeLevel}"));
        }
        audioSource = GetComponent<AudioSource>();
        // 加载Resources/Audio/Music文件夹下的所有音频
        audioClips = Resources.LoadAll<AudioClip>("Audio/Music");
        if (audioClips == null || audioClips.Length == 0)
        {
            Debug.LogWarning("未找到任何音频文件，请检查路径：Resources/Audio/Music");
        }
        StartCoroutine(SwitchMusic());
        
    }

    private void Start()
    {
        arrivalTarget =GameObject.FindGameObjectsWithTag("End").Length;
    }
    //游戏胜利
    private void GameWin()
    {
        //当前存档的关卡解锁+1
        if(archiveData.lastTimeLevel == archiveData.largestLevelUnlocked)
        {
            if (archiveData.largestLevelUnlocked < levelMax)
            {
                archiveData.largestLevelUnlocked = archiveData.largestLevelUnlocked + 1;
                //通知ui管理器
                GameUIManager.Instance.GameWin();
            }
            else
            {
                //通知ui管理器
                GameUIManager.Instance.GameEnd();
            }
        }
        else
        {
            //通知ui管理器
            GameUIManager.Instance.GameWin();
        }
        //保存存档
        baseManager.SaveArchiveData();
    }
    // 随机获取一个音频
    public AudioClip GetRandomAudioClip()
    {
        if (audioClips == null || audioClips.Length == 0)
            return null;
        int index = UnityEngine.Random.Range(0, audioClips.Length);
        return audioClips[index];
    }
    //音乐协程切换音乐
    IEnumerator SwitchMusic()
    {
        AudioClip audioClip=GetRandomAudioClip();
        audioSource.clip = audioClip;
        audioSource.Play();
        yield return new WaitForSeconds(audioClip.length);
        StartCoroutine(SwitchMusic());
    }
}
