using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static GameSripts;

public class FrogSripts : MonsterObject
{
    //绑定的对象
    private (BoxSripts, Direction) _boxSripts;
    public (BoxSripts, Direction) boxSripts
    {
        get { return _boxSripts; }
        set
        {
            if (value.Item1 == null)
            {
                if (_boxSripts.Item1 != null)
                {
                    //如果之前绑定的对象不为空，则将其移动对象设置为null
                    _boxSripts.Item1.moveObject = null;
                }
            }
            else
            {
                value.Item1.moveObject = this;
            }
            _boxSripts = value;
        }
    }
    protected override void SetAttributes()
    {
        _size = Size.small;
        _dietType = DietType.Herbivore;
        _habitat = Habitat.Aquatic | Habitat.Terrestrial;
    }
    //重构被移动方法
    public override ReturnState PassivityMove(BoxObject boxObject, Direction direction)
    {
        if (boxObject == boxSripts.Item1)
        {
            return new ReturnState(true, false, null, null);
        }
        return new ReturnState(false, false, null, null);
    }
    protected override void Ability()
    {
        if (boxSripts.Item1 == null)
        {
            //播放伸舌头动画
            Play("Skill_0");
            //启动副控制器协程
            StartCoroutine(SubController());
        }
        else
        {
            //主动释放
            FreedSkill();
        }
    }

    public override void FreedSkill()
    {
        boxSripts = (null, 0);
    }
    protected override void ChangeableMove(Direction direction)
    {
        ReturnState returnState = CheckMove(direction);
        if (returnState.monsterObject != null)
        {
            OnAttack(returnState.canDie, returnState.monsterObject);
            return;
        }
        if (returnState.canMove)
        {
            if (boxSripts.Item1 != null)
            {
                if (boxSripts.Item2.ToVector3() + direction.ToVector3() == new Vector3(0, 0, 0))
                {
                    boxSripts.Item1.Move(direction);
                    ConsumeAbilityValue();
                }else if (boxSripts.Item2!=direction)
                {
                    ReturnState returnState1 = boxSripts.Item1.CheckMove(direction);
                    if (returnState1.canMove)
                    {
                        foreach (var box in returnState1.boxObjects)
                        {
                            box.Move(direction);
                        }
                        ConsumeAbilityValue();
                    }
                    else
                    {
                        returnState.canMove = false;
                    }
                }
                else
                {
                    ConsumeAbilityValue();
                }
            }
            if (returnState.canMove)
            {
                ExecuteMove();
                if (returnState.boxObjects != null)
                {
                    foreach (var box in returnState.boxObjects)
                    {
                        box.Move(direction);
                    }
                }
                if (returnState.canDie)
                {
                    isDead= true;
                }
            }
            else
            {
                HitWallMove();
            }
        }
        else
        {
            HitWallMove();
        }
    }

    //执行移动
    private void ExecuteMove()
    {
        //移动动画
        Play("Move");
    }
    //副控制器协程
    IEnumerator SubController()
    {
        while (true)
        {
            yield return null;
            if (Input.GetButtonDown("w_key"))
            {
                ChooseBox(Direction.Up);
            }
            else if (Input.GetButtonDown("a_key"))
            {
                ChooseBox(Direction.Left);
            }
            else if (Input.GetButtonDown("s_key"))
            {
                ChooseBox(Direction.Down);
            }
            else if (Input.GetButtonDown("d_key"))
            {
                ChooseBox(Direction.Right);
            }
            if (Input.GetButtonUp("skill"))
            {
                End();
                yield break;
            }
        }
    }
    //选择box方法
    private void ChooseBox(Direction direction)
    {
        Turn(direction);
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + direction.ToVector3(), Vector2.zero);
        if (hits != null && hits.Length != 0)
        {
            foreach (var item in hits)
            {
                BoxSripts boxSripts;
                item.collider.TryGetComponent<BoxSripts>(out boxSripts);
                if (boxSripts != null)
                {
                    this.boxSripts = (boxSripts, direction);
                    return;
                }
            }
        }
        if (this.boxSripts.Item1 != null)
        {
            FreedSkill();
        }
    }

}
