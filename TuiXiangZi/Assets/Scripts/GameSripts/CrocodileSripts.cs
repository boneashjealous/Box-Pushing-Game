using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrocodileSripts : MonsterObject
{
    private Direction currentDirection;
    //进行过方向选择
    private bool isChooseDirection = false;
    //毒预制体
    private GameObject venom;

    protected override void Awake()
    {
        base.Awake();
        venom = Resources.Load<GameObject>("Prefabs/Venom_0");
    }
    protected override void Ability()
    {
        PlayerController.instance.enableRequest++;
        StartCoroutine(SubController());
    }
    IEnumerator SubController()
    {
        while (true)
        {
            yield return null;
            if (Input.GetButtonDown("w_key"))
            {
                ChooseDirection(Direction.Up);
            }
            else if (Input.GetButtonDown("a_key"))
            {
                ChooseDirection(Direction.Left);
            }
            else if (Input.GetButtonDown("s_key"))
            {
                ChooseDirection(Direction.Down);
            }
            else if (Input.GetButtonDown("d_key"))
            {
                ChooseDirection(Direction.Right);
            }
            if (Input.GetButtonUp("skill"))
            {
                End();
                UseSkill();
                yield break;
            }
        }
    }
    private void UseSkill()
    {
        if (isChooseDirection)
        {
            Vector3 vector3 = transform.position + currentDirection.ToVector3();
            RaycastHit2D[] hits = Physics2D.RaycastAll(vector3, Vector2.zero);
            if (hits != null && hits.Length != 0)
            {
                foreach (var item in hits)
                {
                    TerrainObject terrainObject;
                    item.collider.gameObject.TryGetComponent<TerrainObject>(out terrainObject);
                    MonsterObject monsterObject;
                    item.collider.gameObject.TryGetComponent<MonsterObject>(out monsterObject);
                    if (terrainObject != null||monsterObject!=null)
                    {
                        return;
                    }
                }
            }
            Instantiate(venom, vector3, Quaternion.identity);
            ConsumeAbilityValue();
        }
    }
    //选择跳跃方向方法
    private void ChooseDirection(Direction direction)
    {
        Turn(direction);
        currentDirection = direction;
        isChooseDirection = true;
    }


    protected override void SetAttributes()
    {
        _size = Size.large;
        _dietType=DietType.Carnivore;
        _habitat = Habitat.Terrestrial | Habitat.Aquatic;
    }
}
