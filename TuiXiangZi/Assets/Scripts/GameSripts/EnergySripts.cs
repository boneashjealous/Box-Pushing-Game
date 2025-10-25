using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySripts : TerrainObject
{
    protected override void Ability(MonsterObject monsterObject)
    {
        monsterObject.abilityValue++;
        Destroy(this.gameObject);
    }
}
