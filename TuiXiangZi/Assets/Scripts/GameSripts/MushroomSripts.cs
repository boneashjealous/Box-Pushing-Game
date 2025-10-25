using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomSripts :TerrainObject
{
    protected override void Ability(MonsterObject monsterObject)
    {
        if (monsterObject.size != Size.huge)
        {
            monsterObject.size++;
            Destroy(this.gameObject);
        }
    }
}
