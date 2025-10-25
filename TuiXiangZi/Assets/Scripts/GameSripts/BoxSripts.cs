using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoxSripts : BoxObject
{
    
    public override ReturnState PassivityMove(MonsterObject monsterObject, Direction direction)
    {
        if (monsterObject._habitat.HasFlag(Habitat.Aerial))
        {
            return new ReturnState(true, false, null, null);
        }
        else if (monsterObject.size == Size.huge)
        {
            monsterObject.willBehavior1 += BeDestroy;
            return new ReturnState(false, false, null, null);
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

    public override ReturnState PassivityMove(BoxObject boxObject, Direction direction)
    {
        return new ReturnState(false, false, null, null);
    }
    //±»´Ý»Ù
    private void BeDestroy(MonsterObject monsterObject)
    {
        monsterObject.Play("Attack");
        Destroy(gameObject);
    }

    protected override void SetboxName()
    {
        boxName=new string[] { "Box_0","Box_1" ,"Box_2"};
    }
}
