using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    //����
    private static GameUIManager instance;
    public static GameUIManager Instance { get; private set; }

    [SerializeField] private GameObject AbilityValue;
    [SerializeField] private GameObject SettingBackground;
    [SerializeField] private GameObject SettingButton;
    [SerializeField] private GameObject ResetButton;
    [SerializeField] private List<Image> images;
    [SerializeField] private GameObject GameWinPanel;
    [SerializeField] private GameObject NextLevelButton;
    [SerializeField] private GameObject HelpButton;
    [SerializeField] private GameObject HelpPanel;
    [SerializeField] private GameObject EncyclopediaButton;
    [SerializeField] private GameObject EncyclopediaPanel;
    [SerializeField] private GameObject BackButton;
    [SerializeField] private ScriptableRendererFeature GaussianBlurFeature;
    private BaseManager baseManager;

    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            images = new();
        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
        EnableGaussianBlur(false);
    }
    private void Start()
    {
        if(GameObject.Find("BaseManager") != null)
        {
            BaseManager baseManager = GameObject.Find("BaseManager").GetComponent<BaseManager>();
            this.baseManager = baseManager ?? throw new Exception("baseManager��������Ϊnull");
        }
    }
    //��˹ģ���Ŀ����͹ر�
    private void EnableGaussianBlur(bool b)
    {
        GaussianBlurFeature.SetActive(b);
    }

    //�л��ؿ���ť
    public void ChangeLevelButton(int level)
    {
        baseManager.LoadLevel(level);
    }
    //���ùؿ���ť
    public void ResetLevelButton()
    {
        baseManager.ResetLevel();
    }
    //����ֵ����
    public void UpdateAbilityValue(int value, Sprite sprite)
    {
        Transform transform = AbilityValue.transform;
        List<Image> images = this.images;
        if (value == -1)
        {
            if (images.Count == 0)
            {
                GameObject abilityValueItem = new GameObject("AbilityValueItem0" );
                abilityValueItem.transform.SetParent(transform);
                Image image = abilityValueItem.AddComponent<Image>();
                images.Add(image);
                image.sprite = sprite;
            }
            else
            {
                images[0].gameObject.SetActive(true);
                images[0].sprite = sprite;
            }
            images[0].rectTransform.localScale = new Vector3(1, -1, 1);
        }
        else if (images.Count < value)
        {
            for (int i = 0; i < images.Count; i++)
            {
                images[i].gameObject.SetActive(true);
                images[i].sprite = sprite;
                images[i].rectTransform.localScale = new Vector3(1, 1,1);
            }
            for (int i = images.Count; i < value; i++)
            {
                GameObject abilityValueItem = new GameObject("AbilityValueItem" + i);
                abilityValueItem.transform.SetParent(transform);
                Image image = abilityValueItem.AddComponent<Image>();
                images.Add(image);
                image.sprite = sprite;
                images[i].rectTransform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            for (int i = 0; i < value; i++)
            {
                images[i].gameObject.SetActive(true);
                images[i].sprite = sprite;
                images[i].rectTransform.localScale = new Vector3(1, 1,1);
            }
            for (int i = value; i < images.Count; i++)
            {
                images[i].gameObject.SetActive(false);
            }
        }

    }
    //������
    public void OpenSettings()
    {
        ShowPanel();
        SettingBackground.SetActive(true);
        PlayerController.instance.enableRequest++;
    }
    //�򿪰ٿ����
    public void OpenEncyclopedia()
    {
        ShowPanel();
        EncyclopediaPanel.SetActive(true);
        PlayerController.instance.enableRequest++;
    }
    //�򿪰������
    public void OpenHelp()
    {
        ShowPanel();
        HelpPanel.SetActive(true);
        PlayerController.instance.enableRequest++;
    }
    //�ر��������
    private void ClosePanel()
    {
        SettingBackground.SetActive(false);
        HelpPanel.SetActive(false);
        EncyclopediaPanel.SetActive(false);
    }
    //�����
    private void ShowPanel()
    {
        ClosePanel();
        SettingButton.SetActive(false);
        ResetButton.SetActive(false);
        HelpButton.SetActive(false);
        EncyclopediaButton.SetActive(false);
        BackButton.SetActive(true);
        EnableGaussianBlur(true);
    }
    //������Ϸ
    public void BackGame()
    {
        ClosePanel();
        SettingButton.SetActive(true);
        ResetButton.SetActive(true);
        HelpButton.SetActive(true);
        EncyclopediaButton.SetActive(true);
        BackButton.SetActive(false);
        PlayerController.instance.enableRequest--;
        EnableGaussianBlur(false);
    }
    //�������˵�
    public void BackMainMenu()
    {
        baseManager.BackMainMenu();
    }
    //��Ϸʤ��
    public void GameWin()
    {
        ClosePanel() ;
        ResetButton.SetActive(false);
        AbilityValue.SetActive(false);
        PlayerController.instance.enableRequest++;
        GameWinPanel.SetActive(true);
        GameWinPanel.GetComponent<Animator>().Play("GameWin");
        StartCoroutine(WaitAndShowNextLevelButton(null));
    }
    //��Ϸ����
    public void GameEnd()
    {
        ClosePanel();
        ResetButton.SetActive(false);
        AbilityValue.SetActive(false);
        PlayerController.instance.enableRequest++;
        GameWinPanel.SetActive(true);
        GameWinPanel.GetComponent<Animator>().Play("GameWin");
        StartCoroutine(WaitAndShowNextLevelButton(BackMainMenu));
    }
    IEnumerator WaitAndShowNextLevelButton(Action action)
    {
        yield return new WaitForSeconds(1.5f);
        ShowNextLevelButton();
        GameWinPanel.SetActive(false);
        action?.Invoke();
    }
    //��ʾ��һ�ذ�ť
    public void ShowNextLevelButton()
    {
        ArchiveData archiveData = GameManager.Instance.archiveData;
        if (archiveData.lastTimeLevel + 1 <= archiveData.largestLevelUnlocked)
        {
            NextLevelButton.SetActive(true);
        }
        else
        {
            SettingBackground.SetActive(true);
        }
    }
    //��һ��
    public void NextLevel()
    {
        baseManager.LoadLevel(GameManager.Instance.archiveData.lastTimeLevel + 1);
    }
}
