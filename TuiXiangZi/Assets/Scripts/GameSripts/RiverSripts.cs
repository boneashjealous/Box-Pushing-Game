using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverSripts : TerrainObject
{
    public override ReturnState PassivityMove(MonsterObject monsterObject, Direction direction)
    {
        if (monsterObject._habitat==Habitat.Terrestrial)
        {
            return new ReturnState (true,true,null,null);
        }
        return new ReturnState (true,false,null,null);
    }

    protected override void Ability(MonsterObject monsterObject)
    {
        return;
    }
}
