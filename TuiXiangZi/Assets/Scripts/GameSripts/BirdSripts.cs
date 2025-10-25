using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSripts : MonsterObject
{
    private GameObject nest;

    protected override void Awake()
    {
        base.Awake();
        nest=Resources.Load<GameObject>("Prefabs/Nest_0");
    }
    protected override void Ability()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);
        if (hits != null && hits.Length != 0)
        {
            foreach (var item in hits)
            {
                GameSripts gameSripts;
                item.collider.gameObject.TryGetComponent<GameSripts>(out gameSripts);
                if (gameSripts!=null&&gameSripts!=this)
                {
                    return;
                }
            }
        }
        Instantiate(nest, transform.position, Quaternion.identity);
        ConsumeAbilityValue();
    }

    protected override void SetAttributes()
    {
        _size = Size.large;
        _dietType = DietType.Carnivore;
        _habitat= Habitat.Aerial;
    }
}
