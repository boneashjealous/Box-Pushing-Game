using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSripts : GameSripts
{
    public override ReturnState PassivityMove(MonsterObject monsterObject, Direction direction)
    {
        return new ReturnState(false,false,null,null) ;
    }

    public override bool PassivityMove(SpectreSripts spectreSripts)
    {
        return false ;
    }

    public override ReturnState PassivityMove(BoxObject boxObject, Direction direction)
    {
        return new ReturnState(false,false,null,null) ;
    }
}
