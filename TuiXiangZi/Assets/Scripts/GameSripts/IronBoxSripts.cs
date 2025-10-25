using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronBoxSripts : BoxObject
{
    public override ReturnState PassivityMove(MonsterObject monsterObject, Direction direction)
    {
        if (monsterObject._habitat==Habitat.Aerial)
        {
            return new ReturnState(true, false, null, null);
        }
        if(monsterObject.size >=Size.large)
        {
            return CheckMove(direction);
        }
        else
        {
            return new ReturnState(false, false, null, null);
        }
    }

    public override bool PassivityMove(SpectreSripts spectreSripts)
    {
        return true;
    }

    public override ReturnState PassivityMove(BoxObject boxObject, Direction direction)
    {
        return new ReturnState(false, false, null, null);
    }

    protected override void SetboxName()
    {
        boxName=new string[] { "IronBox_0", "IronBox_1", "IronBox_2" };
    }
}
