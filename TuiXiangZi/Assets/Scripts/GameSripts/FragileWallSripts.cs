using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragileWallSripts : TerrainObject
{
    public override ReturnState PassivityMove(MonsterObject monsterObject, Direction direction)
    {
        if (monsterObject._habitat == Habitat.Aerial)
        {
            return new ReturnState(true, false, null, null);
        }
        return new ReturnState(false, false, null, null);
    }
    public override ReturnState PassivityMove(BoxObject boxObject, Direction direction)
    {
        return new ReturnState(false, false, null, null);
    }
    protected override void Ability(MonsterObject monsterObject)
    {
        return;
    }
}
