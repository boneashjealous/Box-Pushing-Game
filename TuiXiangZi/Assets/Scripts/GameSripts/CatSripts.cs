using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatSripts : MonsterObject
{
    private Direction currentDirection;
    //进行过方向选择
    private bool isChooseDirection = false;
    private bool isMove = false;
    private int MoveCount = 0;
    private ReturnState lastReturnState;

    private void UseSkill()
    {
        if (isChooseDirection)
        {
            _size++;
            MoveCount = 1;
            SkillMove();
            ConsumeAbilityValue();
            isChooseDirection = false;
        }
    }


    private void SkillMove()
    {
        willBehavior = null;
        ReturnState returnState = CheckMove(currentDirection);
        if (returnState.monsterObject != null)
        {
            OnAttack(returnState.canDie, returnState.monsterObject);
            _size--;
            return;
        }
        if (returnState.canMove)
        {
            //移动动画
            Play("Move");
            if (MoveCount > 0)
            {
                isMove = true;
            }
            else
            {
                _size--;
            }
            returnState.boxObjects?.ForEach(box => box.Move(currentDirection));
            if (returnState.canDie)
            {
                if (MoveCount == 0)
                {
                    isDead = true;
                }
            }
            MoveCount--;
            lastReturnState = returnState;
        }
        else
        {
            if (lastReturnState.canDie)
            {
                if (MoveCount == 0)
                {
                    isDead = true;
                }
            }
            Play("HitWall");
            _size--;
            return;
        }
    }
    protected override void MoveEnd(string animationName)
    {
        base.MoveEnd(animationName);
        if (isMove)
        {
            isMove = false;
            SkillMove();
        }
    }
    protected override void Ability()
    {
        PlayerController.instance.enableRequest++;
        StartCoroutine(SubController());
    }

    protected override void SetAttributes()
    {
        _size = Size.medium;
        _dietType = DietType.Carnivore;
        _habitat = Habitat.Terrestrial;
    }

    IEnumerator SubController()
    {
        while (true)
        {
            yield return null;
            if (Input.GetButtonDown("w_key"))
            {
                ChooseDirection(Direction.Up);
            }
            else if (Input.GetButtonDown("a_key"))
            {
                ChooseDirection(Direction.Left);
            }
            else if (Input.GetButtonDown("s_key"))
            {
                ChooseDirection(Direction.Down);
            }
            else if (Input.GetButtonDown("d_key"))
            {
                ChooseDirection(Direction.Right);
            }
            if (Input.GetButtonUp("skill"))
            {
                End();
                UseSkill();
                yield break;
            }
        }
    }

    //选择跳跃方向方法
    private void ChooseDirection(Direction direction)
    {
        Turn(direction);
        currentDirection = direction;
        isChooseDirection = true;
    }
}
