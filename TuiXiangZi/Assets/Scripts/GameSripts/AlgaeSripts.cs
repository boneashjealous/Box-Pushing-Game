using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgaeSripts : TerrainObject
{
    public override ReturnState PassivityMove(MonsterObject monsterObject, Direction direction)
    {
        if (monsterObject._habitat == Habitat.Aerial)
        {
            return new ReturnState(true,false,null,null);
        }
        monsterObject.willBehavior += Ability;
        return new ReturnState(true, true, null, null);
    }
    protected override void Ability(MonsterObject monsterObject)
    {
        Destroy(gameObject);
    }
}
