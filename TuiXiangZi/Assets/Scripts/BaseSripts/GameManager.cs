using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
//��ʼ��������ؿ����浵����Ϸʤ����
public class GameManager : MonoBehaviour
{
    //����
    private static GameManager instance;
    public static GameManager Instance{ get{ return instance; }private set { instance = value; } }
    //��ǰ�ؿ�����
    private ArchiveData _archiveData;
    public ArchiveData archiveData { get { return _archiveData; }private set { _archiveData = value; } }
    //�ִ��յ���������
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
                //����ִ�Ŀ�������������ڵ���Ŀ��������������
                //����Ϸʤ��
                GameWin();
            }
        }
    }
    //�ִ�Ŀ��������������
    private int arrivalTarget;
    private AudioSource audioSource;
    // ��Ƶ����
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
            this.baseManager = baseManager ?? throw new Exception("baseManager��������Ϊnull");
            archiveData = baseManager.GetCurrentArchiveData();
            Instantiate(Resources.Load<GameObject>($"LevelMap/Level_{archiveData.lastTimeLevel}"));
        }
        audioSource = GetComponent<AudioSource>();
        // ����Resources/Audio/Music�ļ����µ�������Ƶ
        audioClips = Resources.LoadAll<AudioClip>("Audio/Music");
        if (audioClips == null || audioClips.Length == 0)
        {
            Debug.LogWarning("δ�ҵ��κ���Ƶ�ļ�������·����Resources/Audio/Music");
        }
        StartCoroutine(SwitchMusic());
        
    }

    private void Start()
    {
        arrivalTarget =GameObject.FindGameObjectsWithTag("End").Length;
    }
    //��Ϸʤ��
    private void GameWin()
    {
        //��ǰ�浵�Ĺؿ�����+1
        if(archiveData.lastTimeLevel == archiveData.largestLevelUnlocked)
        {
            if (archiveData.largestLevelUnlocked < levelMax)
            {
                archiveData.largestLevelUnlocked = archiveData.largestLevelUnlocked + 1;
                //֪ͨui������
                GameUIManager.Instance.GameWin();
            }
            else
            {
                //֪ͨui������
                GameUIManager.Instance.GameEnd();
            }
        }
        else
        {
            //֪ͨui������
            GameUIManager.Instance.GameWin();
        }
        //����浵
        baseManager.SaveArchiveData();
    }
    // �����ȡһ����Ƶ
    public AudioClip GetRandomAudioClip()
    {
        if (audioClips == null || audioClips.Length == 0)
            return null;
        int index = UnityEngine.Random.Range(0, audioClips.Length);
        return audioClips[index];
    }
    //����Э���л�����
    IEnumerator SwitchMusic()
    {
        AudioClip audioClip=GetRandomAudioClip();
        audioSource.clip = audioClip;
        audioSource.Play();
        yield return new WaitForSeconds(audioClip.length);
        StartCoroutine(SwitchMusic());
    }
}
