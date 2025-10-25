using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//����������
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
                Debug.LogError("���ظ����������");
            }
        }
    }
    //�ƶ�
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
            //ִ���ƶ�����
            Play("Move");
        }
        else
        {
            //ִ�����ڶ���
            Play("HitWall");
        }
    }
    //��������
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
                    //ת�ƿ�����
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

    //��ʧ����
    private void DisappearEnd()
    {
        //��ʧ����
        End();
        audioSource.clip = null;
        gameObject.SetActive(false);
    }
    
    //���ֶ���
    public void Appear()
    {
        Play("Appear");
    }
    private void AppearEnd()
    {
        spriteRenderer.sprite=sprite;
        gameObject.SetActive(false);//ˢ��idle״̬
        gameObject.SetActive(true);
        End();
    }
    //����idle
    public void ResetIdle()
    {
        spriteRenderer.sprite=null;
    }
    //���ᱻ����
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
        return; // ���鼼�ܲ���Ҫ�ͷ�
    }

    public override ReturnState CheckMove(Direction direction)
    {
        throw new System.NotImplementedException();
    }
}