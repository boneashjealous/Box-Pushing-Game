using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HippoSripts : MonsterObject
{
    private Direction currentDirection;
    private BoxObject boxObject;
    private Direction rotateDirection;
    protected override void Ability()
    {
        if(boxObject == null)
        {
            PlayerController.instance.enableRequest++;
            StartCoroutine(SubController());
        }
        else
        {
            FreedSkill();
        }
        
    }

    protected override void SetAttributes()
    {
        _size=Size.large;
        _dietType=DietType.Herbivore;
        _habitat=Habitat.Aquatic;
    }

    private void UseSkill()
    {
        if (boxObject==null)
        {
            ConsumeAbilityValue();
        }
    }
    protected override void ChangeableMove(Direction direction)
    {
        if(boxObject == null)
        {
            ReturnState returnState = CheckMove(direction);
            if (returnState.monsterObject != null)
            {
                OnAttack(returnState.canDie, returnState.monsterObject);
                return;
            }
            if (returnState.canMove)
            {
                //移动动画
                Play("Move");
                returnState.boxObjects?.ForEach(box => box.Move(direction));
                if (returnState.canDie)
                {
                    isDead = true;
                }
            }
            else
            {
                HitWallMove();
                return;
            }
        }
        else
        {
            if(direction== Direction.Left)
            {
                rotateDirection = rotateDirection.Rotate(3);
                MoveBox(rotateDirection, false);
            }else if(direction== Direction.Right)
            {
                rotateDirection = rotateDirection.Rotate(1);
                MoveBox(rotateDirection, true);
            }
        }
    }
    public override void FreedSkill()
    {
        boxObject.moveObject= null;
        boxObject = null;
        ConsumeAbilityValue();
    }
    //旋转箱子
    private void MoveBox(Direction direction,bool b)
    {
        ReturnState returnState = boxObject.CheckMove(direction);
        if (returnState.canMove)
        {
            returnState.boxObjects?.ForEach(box => box.Move(direction));
            if (b)
            {
                direction = direction.Rotate(1);
            }
            else
            {
                direction = direction.Rotate(3);
            }
            boxObject.willBehavior += box =>
            {
                returnState = box.CheckMove(direction);
                if (returnState.canMove)
                {
                    returnState.boxObjects?.ForEach(box => box.Move(direction));
                }
                else
                {
                    FreedSkill();
                }
            };

        }
        else
        {
            FreedSkill();
        }
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

    //选择方向方法
    private void ChooseDirection(Direction direction)
    {
        Turn(direction);
        currentDirection = direction;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + currentDirection.ToVector3(), Vector2.zero);
        if (hits != null && hits.Length != 0)
        {
            foreach (var item in hits)
            {
                BoxObject boxObject;
                item.collider.TryGetComponent<BoxObject>(out boxObject);
                if (boxObject != null)
                {
                    this.boxObject = boxObject;
                    boxObject.moveObject = this;
                    rotateDirection = currentDirection;
                    break;
                }
            }
        }
    }
}
