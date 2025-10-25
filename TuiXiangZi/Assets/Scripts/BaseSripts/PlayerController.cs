using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerController : MonoBehaviour
{
    private CameraFollow cameraFollow;
    private AbilityObject _currentControlObjects;
    public AbilityObject currentControlObjects{
        get
        { 
            return _currentControlObjects;
        }
        set
        {
            if (value == null)
            {
                SpectreSripts.spectre.transform.position = currentControlObjects.gameObject.transform.position;
                SpectreSripts.spectre.transform.rotation= currentControlObjects.gameObject.transform.rotation;
                _currentControlObjects = SpectreSripts.spectre;
                currentControlObjects.gameObject.SetActive(true);
                SpectreSripts.spectre.Appear();
                currentControlObjects.abilityValue =_currentControlObjects.abilityValue;
                cameraFollow.SetTarget(currentControlObjects.transform);
                return;
            }
            value.abilityValue = value.abilityValue;
            cameraFollow.SetTarget(value.transform);
            _currentControlObjects = value;
        }
        }
    [HideInInspector]public int enableRequest;
    //单例
    public static PlayerController instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        enableRequest =0;
    }
    private void Start()
    {
        cameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        AbilityObject abilityObject;
        if (!GameObject.Find("Spectre").TryGetComponent<AbilityObject>(out abilityObject))
        {
            enableRequest++;
            Debug.LogError("未获取到幽灵脚本");
        }
        else
        {
            currentControlObjects = abilityObject;
        }
    }

    //检测输入
    private void Update()
    {
        if (enableRequest==0)
        {
            if (Input.GetButtonDown("w_key"))
            {
                currentControlObjects.Move(Direction.Up);
            }
            else if (Input.GetButtonDown("a_key"))
            {
                currentControlObjects.Move(Direction.Left);
            }
            else if (Input.GetButtonDown("s_key"))
            {
                currentControlObjects.Move(Direction.Down);
            }
            else if (Input.GetButtonDown("d_key"))
            {
                currentControlObjects.Move(Direction.Right);
            }
            else if (Input.GetButtonDown("skill"))
            {
                currentControlObjects.Skill();
            }
        }
    }
}
