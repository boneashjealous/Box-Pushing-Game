using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//°ïÖúUI¹ÜÀíÆ÷¼àÌıÇĞ»»¹Ø¿¨
[RequireComponent(typeof(Button))]
public class UILevel : MonoBehaviour
{
    [SerializeField]
    private int level;
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => GameUIManager.Instance.ChangeLevelButton(level));
        if (level <= GameManager.Instance.archiveData.largestLevelUnlocked)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}
