using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PitScripts : TerrainObject
{
    public override ReturnState PassivityMove(MonsterObject monsterObject, Direction direction)
    {
        if (monsterObject._habitat != Habitat.Aerial)
        {
            return new ReturnState(true, true, null, null);
        }
        return new ReturnState(true, false, null, null);
    }

    protected override void Ability(MonsterObject monsterObject)
    {
        return;
    }
}