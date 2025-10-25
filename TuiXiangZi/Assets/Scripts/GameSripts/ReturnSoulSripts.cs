using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnSoulSripts : TerrainObject
{
    protected override void Ability(MonsterObject monsterObject)
    {
        PlayerController.instance.currentControlObjects = null;
        Destroy(gameObject);
    }
}
