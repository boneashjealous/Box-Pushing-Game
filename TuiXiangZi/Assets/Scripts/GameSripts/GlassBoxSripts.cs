using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GlassBoxSripts : BoxObject
{
    public override ReturnState PassivityMove(MonsterObject monsterObject, Direction direction)
    {
        if (monsterObject._habitat.HasFlag(Habitat.Aerial))
        {
            return new ReturnState(true, false, null, null);
        }
        else
        {
            return CheckMove(direction);
        }
    }

    public override bool PassivityMove(SpectreSripts spectreSripts)
    {
        return true;
    }

    public override ReturnState PassivityMove(BoxObject boxObject,Direction direction)
    {
        ReturnState returnState = CheckMove(direction);
        if (returnState.canMove)
        {
            return returnState;
        }
        else
        {
            return new ReturnState(false, false, new List<BoxObject> { this}, null);
        }
    }
    
    protected override void SetboxName()
    {
        boxName = new string[] { "GlassBox_0", "GlassBox_1", "GlassBox_2" };
    }
}
