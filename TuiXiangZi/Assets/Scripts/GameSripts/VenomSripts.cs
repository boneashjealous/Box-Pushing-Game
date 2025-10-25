using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VenomSripts : TerrainObject
{
    public override ReturnState PassivityMove(MonsterObject monsterObject, Direction direction)
    {
        return new ReturnState(true, true,null,null);
    }
}
    

