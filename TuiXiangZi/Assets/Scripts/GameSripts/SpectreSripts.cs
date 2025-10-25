using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//定义幽灵类
public class SpectreSripts : AbilityObject
{
    private static SpectreSripts _spectre;
    public static SpectreSripts spectre { get { return _spectre; } }
    protected override void Awake()
    {
        base.Awake();
        if (_spectre == null)
        {
            _spectre = this;
        }
        else
        {
            if(_spectre !=this)
            {
                Destroy(gameObject);
                Debug.LogError("有重复的幽灵产生");
            }
        }
    }
    //移动
    public override void Move(Direction direction)
    {
        Turn(direction);
        bool canMove = true;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position+ direction.ToVector3(), Vector2.zero);
        if (hits != null && hits.Length != 0)
        {
            foreach (var item in hits)
            {
                GameSripts gameSripts;
                item.collider.gameObject.TryGetComponent<GameSripts>(out gameSripts);
                if (gameSripts != null)
                {
                    bool b = gameSripts.PassivityMove(this);
                    if (!b)
                    {
                        canMove = false;
                        break;
                    }
                }
            }
        }
        if (canMove)
        {
            //执行移动动画
            Play("Move");
        }
        else
        {
            //执行碰壁动画
            Play("HitWall");
        }
    }
    //能力附身
    protected override void Ability()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position,Vector2.zero);
        if (hits != null && hits.Length != 0)
        {
            foreach (var item in hits)
            {
                MonsterObject monsterObject;
                item.collider.gameObject.TryGetComponent<MonsterObject>(out monsterObject);
                if (monsterObject != null)
                {
                    //转移控制器
                    PlayerController.instance.currentControlObjects=monsterObject;
                    Play("Disappear");
                    return;
                }
            }
        }
    }
    protected override void ConsumeAbilityValue()
    {
        return;
    }

    //消失结束
    private void DisappearEnd()
    {
        //消失动画
        End();
        audioSource.clip = null;
        gameObject.SetActive(false);
    }
    
    //出现动画
    public void Appear()
    {
        Play("Appear");
    }
    private void AppearEnd()
    {
        spriteRenderer.sprite=sprite;
        gameObject.SetActive(false);//刷新idle状态
        gameObject.SetActive(true);
        End();
    }
    //重置idle
    public void ResetIdle()
    {
        spriteRenderer.sprite=null;
    }
    //不会被调用
    public override ReturnState PassivityMove(MonsterObject monsterObject, Direction direction)
    {
        throw new System.NotImplementedException();
    }
    public override bool PassivityMove(SpectreSripts spectreSripts)
    {
        throw new System.NotImplementedException();
    }
    public override ReturnState PassivityMove(BoxObject boxObject, Direction direction)
    {
        throw new System.NotImplementedException();
    }

    public override void FreedSkill()
    {
        return; // 幽灵技能不需要释放
    }

    public override ReturnState CheckMove(Direction direction)
    {
        throw new System.NotImplementedException();
    }
}