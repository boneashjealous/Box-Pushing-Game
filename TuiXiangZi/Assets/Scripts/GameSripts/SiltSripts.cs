using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiltSripts : TerrainObject
{
    public override ReturnState PassivityMove(MonsterObject monsterObject, Direction direction)
    {
        monsterObject.willBehavior += Ability;
        return new ReturnState(true,false,null,null);
    }

    protected override void Ability(MonsterObject monsterObject)
    {
        monsterObject.abilityValue = 0;
    }
}
