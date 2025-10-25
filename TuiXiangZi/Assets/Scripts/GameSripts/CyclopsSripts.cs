using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsSripts : MonsterObject
{
    //缓冲的动画控制器
    private RuntimeAnimatorController animator0;
    private Sprite sprite0;
    private bool obito;
    private FragileWallSripts hasWall;
    //需要改变动画控制器
    private bool needChange;
    private Direction currentDirection;
    //进行过方向选择
    private bool isChooseDirection = false;
    protected override void Awake()
    {
        base.Awake();
        animator0 = Resources.Load<RuntimeAnimatorController>("Animations/Monster/Cyclops/Cyclops_1");
        sprite0 = Resources.LoadAll<Sprite>("Sprite/GameSprite/HitWallCyclops_1")[0];
    }
    protected override void ChangeableMove(Direction direction)
    {
        hasWall = null;
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
        else if (hasWall != null)
        {
            obito = true;
            needChange = true;
            Play("Attack");
            Destroy(hasWall.gameObject);
            return;
        }
        else
        {
            HitWallMove();
            return;
        }
    }
    public override ReturnState CheckMove(Direction direction)
    {
        ReturnState ReturnState = new ReturnState(true, false, null, null);
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + direction.ToVector3(), Vector2.zero);
        if (hits != null && hits.Length != 0)
        {
            foreach (var item in hits)
            {
                GameSripts gameSripts;
                item.collider.gameObject.TryGetComponent<GameSripts>(out gameSripts);
                if (gameSripts != null)
                {
                    if (gameSripts is FragileWallSripts)
                    {
                        hasWall = gameSripts as FragileWallSripts;
                    }
                    ReturnState returnState = gameSripts.PassivityMove(this, direction);
                    if (returnState.monsterObject != null)
                    {
                        return returnState;
                    }
                    if (!returnState.canMove)
                    {
                        ReturnState.canMove = false;
                        return ReturnState;
                    }
                    if (returnState.boxObjects != null && returnState.boxObjects.Count != 0)
                    {
                        ReturnState.boxObjects = returnState.boxObjects;
                    }
                    if (returnState.canDie)
                    {
                        ReturnState.canDie = true;
                    }
                }
            }
        }
        return ReturnState;
    }
    //覆盖陷阱
    protected override void Ability()
    {
        if (obito)
        {
            PlayerController.instance.enableRequest++;
            StartCoroutine(SubController());
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
    private void UseSkill()
    {
        if (isChooseDirection)
        {
            //放土操作
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + currentDirection.ToVector3(), Vector2.zero);
            if (hits != null && hits.Length != 0)
            {
                foreach (var item in hits)
                {
                    TerrainObject terrainObject;
                    item.collider.gameObject.TryGetComponent<TerrainObject>(out terrainObject);
                    if (terrainObject != null)
                    {
                        Destroy(terrainObject.gameObject);
                    }
                }
            }
            //切换动画控制
            spriteRenderer.sprite = sprite;
            audioSource.clip = null;
            gameObject.SetActive(false);
            gameObject.SetActive(true);
            RuntimeAnimatorController animator1 = animator.runtimeAnimatorController;
            animator.runtimeAnimatorController = animator0;
            animator0 = animator1;
            ConsumeAbilityValue();
            isChooseDirection = false;
            obito = false;
        }
    }

    //选择方向方法
    private void ChooseDirection(Direction direction)
    {
        Turn(direction);
        currentDirection = direction;
        isChooseDirection = true;
    }
    protected override void AttackEnd(string animationName)
    {
        base.AttackEnd(animationName);
        if (needChange)
        {
            //切换动画控制
            spriteRenderer.sprite = sprite0;
            audioSource.clip = null;
            gameObject.SetActive(false);
            gameObject.SetActive(true);
            RuntimeAnimatorController animator1 = animator.runtimeAnimatorController;
            animator.runtimeAnimatorController = animator0;
            animator0 = animator1;
            ConsumeAbilityValue();
            needChange = false;
        }
    }

    protected override void SetAttributes()
    {
        _size = Size.huge;
        _dietType = DietType.Carnivore;
        _habitat = Habitat.Terrestrial;
    }
}
