using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


//用于在关卡开始时初始化百科信息
public class EncyclopediaManager : MonoBehaviour
{
    public GameObject EncyclopediaPanel;
    public GameObject EncyclopediaInfoPanel;
    private void Awake()
    {
        initEncyclopedia("Algae_0");
        initEncyclopedia("Bird_0");
        initEncyclopedia("Birdie_0");
        initEncyclopedia("Box_0");
        initEncyclopedia("Cat_0");
        initEncyclopedia("CoverBox_0");
        initEncyclopedia("Crocodile_0");
        initEncyclopedia("Cyclops_0");
        initEncyclopedia("End_0");
        initEncyclopedia("Energy");
        initEncyclopedia("FragileWall_0");
        initEncyclopedia("Frog_0");
        initEncyclopedia("GlassBox_0");
        initEncyclopedia("Hippo_0");
        initEncyclopedia("IronBox_0");
        initEncyclopedia("MagnctScorpion_0");
        initEncyclopedia("Mushroom_0");
        initEncyclopedia("Nest_0");
        initEncyclopedia("Pit_0");
        initEncyclopedia("ReturnSoul_0");
        initEncyclopedia("River_0");
        initEncyclopedia("Silt_0");
        initEncyclopedia("Spectre");
        initEncyclopedia("Unidirectional_0");
        initEncyclopedia("Venom_0");
    }
    //初始化百科
    private void initEncyclopedia(string objectName)
    {
        try
        {
            if (GameObject.Find(objectName))
            {
                EncyclopediaInformation encyclopedia = Resources.Load<EncyclopediaInformation>($"EncyclopediaInfo/{objectName}");
                Transform transform = Instantiate(EncyclopediaInfoPanel).transform;
                transform.SetParent(EncyclopediaPanel.transform);
                transform.localScale = Vector3.one;   // 仅重置缩放
                LayoutRebuilder.ForceRebuildLayoutImmediate(EncyclopediaPanel.transform as RectTransform);
                transform.GetChild(0).GetComponent<Image>().sprite = encyclopedia.sprite;
                transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = encyclopedia.Info;
            }
        }
        catch 
        {
            Debug.LogError($"初始化百科失败");
        }
        
    }
}
