using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static GameSripts;


//定义所有游戏对象的基类
[RequireComponent(typeof(BoxCollider2D))]
public abstract class GameSripts : MonoBehaviour
{
    protected virtual void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }
    //被动移动
    public abstract ReturnState PassivityMove(MonsterObject monsterObject, Direction direction);
    public abstract bool PassivityMove(SpectreSripts spectreSripts);
    public abstract ReturnState PassivityMove(BoxObject boxObject, Direction direction);

    //返回能否移动和是否死亡的数据，箱子串接数据，怪物数据
    public class ReturnState
    {
        public bool canMove;
        public bool canDie;
        public List<BoxObject> boxObjects;
        public MonsterObject monsterObject;
        public ReturnState(bool canMove, bool canDie, List<BoxObject> boxObjects, MonsterObject monsterObject)
        {
            this.canMove = canMove;
            this.canDie = canDie;
            this.boxObjects = boxObjects;
            this.monsterObject = monsterObject;
        }
    }
}

//定义地形对象基类
public abstract class TerrainObject : GameSripts
{
    public override ReturnState PassivityMove(MonsterObject monsterObject, Direction direction)
    {
        monsterObject.willBehavior += Ability;
        return new ReturnState(true, false, null, null);
    }

    public override bool PassivityMove(SpectreSripts spectreSripts)
    {
        return true;
    }

    public override ReturnState PassivityMove(BoxObject boxObject, Direction direction)
    {
        boxObject.willBehavior += Ability;
        return new ReturnState(true, false, null, null);
    }

    protected virtual void Ability(MonsterObject monsterObject)
    {
        return;
    }
    protected virtual void Ability(BoxObject boxObject)
    {
        return;
    }
}

//定义移动对象基类
public abstract class MoveObject : GameSripts
{
    //移动
    public abstract void Move(Direction direction);
    //检查移动
    public abstract ReturnState CheckMove(Direction direction);
    //转向
    protected void Turn(Direction direction)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction.ToVector3());
    }
}
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
//定义能力者
public abstract class AbilityObject : MoveObject
{
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected AudioSource audioSource;
    protected static Dictionary<string, AudioClip> audioClip;
    protected int _abilityValue;
    [SerializeField]
    public int abilityMaxValue;
    //用于展示能力值的图片
    [HideInInspector] public Sprite sprite;
    public int abilityValue
    {
        get { return _abilityValue; }
        set
        {
            if (abilityMaxValue == -1)
            {
                GameUIManager.Instance.UpdateAbilityValue(-1, this.sprite);
                return;
            }
            if (value > abilityMaxValue)
            {
                _abilityValue = abilityMaxValue;
                GameUIManager.Instance.UpdateAbilityValue(abilityMaxValue, this.sprite);
            }
            else
            {
                if (value == 0)
                {
                    FreedSkill();
                }
                _abilityValue = value;
                GameUIManager.Instance.UpdateAbilityValue(value, this.sprite);
            }
        }
    }
    protected UnityAction<string> animationEnd;
    public virtual void Skill()
    {
        if (abilityMaxValue != -1)
        {
            if (abilityValue <= 0)
            {
                Debug.Log("能力值不足，无法使用技能");
                return;
            }
        }
        Ability();
    }
    //释放技能
    public virtual void FreedSkill()
    {
        return;
    }
    protected abstract void Ability();
    //消耗能力值
    protected virtual void ConsumeAbilityValue()
    {
        abilityValue--;
    }
    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        sprite = spriteRenderer.sprite;
        if (audioClip == null)
        {
            audioClip = new Dictionary<string, AudioClip>();
            //加载音频文件
            Resources.LoadAll<AudioClip>("Audio/SoundEffects").ToList().ForEach(clip =>
            {
                if (!audioClip.ContainsKey(clip.name))
                {
                    audioClip.Add(clip.name, clip);
                }
            });
            if (audioClip.Count == 0)
            {
                Debug.LogError("音频文件加载失败，请检查路径和文件名是否正确");
            }
        }
        //初始化能力值
        if (abilityMaxValue == -1)
        {
            _abilityValue = -1; //-1表示无限制
        }
        else
        {
            _abilityValue = abilityMaxValue;
        }
        //初始化动画结束事件
        animationEnd = new UnityAction<string>(MoveEnd);
        animationEnd += HitWallEnd;
    }
    //移动动画结束
    protected virtual void MoveEnd(string animationName)
    {
        if (animationName != "Move")
            return;
        transform.position = transform.position + new Vector3(Mathf.Round(transform.up.x), Mathf.Round(transform.up.y), 0);
        End();
    }
    //碰壁动画结束
    protected virtual void HitWallEnd(string animationName)
    {
        if (animationName == "HitWall")
            End();
    }

    //控制请求
    public virtual void Play(string animationName)
    {
        PlayerController.instance.enableRequest++;
        animator.Play(animationName);
        StartCoroutine(WaitForAnimationCompletion(animationName, animationEnd));
        if (audioClip.ContainsKey(animationName))
        {
            audioSource.clip = audioClip[animationName];
            audioSource.Play();
        }
    }
    private IEnumerator WaitForAnimationCompletion(string animationName, UnityAction<string> animationEnd)
    {
        // 等待动画开始播放
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            yield return null;
        }
        // 获取动画长度
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        // 等待动画播放完成
        yield return new WaitForSeconds(animationLength);
        // 执行回调函数
        animationEnd?.Invoke(animationName);
    }
    //动画结束控制器释放
    protected virtual void End()
    {
        PlayerController.instance.enableRequest--;
    }
}



//定义怪兽基类
public abstract class MonsterObject : AbilityObject
{
    protected Size _size;
    [HideInInspector] public DietType _dietType;
    [HideInInspector] public Habitat _habitat;
    public Size size
    {
        get { return _size; }
        set
        {
            if (value == Size.huge + 1)
                return;
            else
                _size = value;
        }
    }
    [HideInInspector] public bool isDead = false;
    public UnityAction<MonsterObject> willBehavior;
    public UnityAction<MonsterObject> willBehavior1;

    protected override void Awake()
    {
        base.Awake();
        animationEnd += DieEnd;
        animationEnd += AttackEnd;
        SetAttributes();
    }

    public override ReturnState PassivityMove(MonsterObject monsterObject, Direction direction)
    {
        return React(monsterObject); ;
    }
    public override bool PassivityMove(SpectreSripts spectreSripts)
    {
        return true;
    }
    public override ReturnState PassivityMove(BoxObject boxObject, Direction direction)
    {
        if (_habitat.HasFlag(Habitat.Aerial))
        {
            return new ReturnState(true, false, null, null);
        }
        return new ReturnState(false, false, null, null);
    }
    //设置怪物属性
    protected abstract void SetAttributes();
    //易变移动
    protected virtual void ChangeableMove(Direction direction)
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
    //碰壁移动
    protected void HitWallMove()
    {
        if (willBehavior1 != null)
        {
            willBehavior1?.Invoke(this);
            return;
        }
        Play("HitWall");
    }
    //移动
    public override void Move(Direction direction)
    {
        willBehavior = null;
        willBehavior1 = null;
        Turn(direction);
        ChangeableMove(direction);
    }
    //检查移动
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

    //怪物攻击
    protected virtual void OnAttack(bool canDie, MonsterObject other)
    {
        other.transform.rotation = Quaternion.Euler(0, 0, this.transform.eulerAngles.z + 180f);
        other.Play("Attack");
        Play("Attack");
        if (canDie)
        {
            isDead = true;
        }
        else
        {
            other.isDead = true;
        }
    }
    protected virtual void AttackEnd(string animationName)
    {
        if (animationName != "Attack")
            return;
        End();
    }
    //怪物死亡
    protected virtual void Die()
    {
        Play("Die");
    }
    protected virtual void DieEnd(string animationName)
    {
        if (animationName != "Die")
            return;
        if (PlayerController.instance.currentControlObjects == this)
            PlayerController.instance.currentControlObjects = null;
        End();
        Destroy(gameObject);
    }
    protected override void MoveEnd(string animationName)
    {
        if (animationName != "Move")
            return;
        transform.position = transform.position + new Vector3(Mathf.Round(transform.up.x), Mathf.Round(transform.up.y), 0);
        //水生检查水地块
        if (_habitat == Habitat.Aquatic)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);
            isDead = true;
            if (hits != null && hits.Length != 0)
            {
                foreach (var item in hits)
                {
                    RiverSripts riverSripts;
                    item.collider.gameObject.TryGetComponent<RiverSripts>(out riverSripts);
                    if (riverSripts != null)
                    {
                        isDead = false;
                    }
                }
            }
        }
        willBehavior?.Invoke(this);
        willBehavior = null;
        End();
    }
    protected override void End()
    {
        base.End();
        if (isDead)
        {
            Die();
            isDead = false;
        }
    }
    //比较俩个怪物的类型返回数据
    private ReturnState React(MonsterObject other)
    {
        ReturnState returnState = new ReturnState(true, false, null, null);
        if (_habitat == Habitat.Aerial)
        {
            if (other._habitat == Habitat.Aerial)
            {
                if (other._dietType == DietType.Carnivore)
                {
                    if (other._size > _size)
                    {
                        returnState.monsterObject = this;
                        returnState.canDie = false;
                    }
                    else if (other._size == _size)
                    {
                        returnState.canMove = false;
                    }
                    else
                    {
                        if (_dietType == DietType.Carnivore)
                        {
                            returnState.monsterObject = this;
                            returnState.canDie = true;
                        }
                        else
                        {
                            returnState.canMove = false;
                        }
                    }
                }
                else if (_dietType == DietType.Carnivore)
                {
                    if (_size > other._size)
                    {
                        returnState.monsterObject = this;
                        returnState.canDie = true;
                    }
                    else
                    {
                        returnState.canMove = false;
                    }
                }
                else
                {
                    returnState.canMove = false;
                }
            }
            else
            {
                if (other._size == Size.huge)
                {
                    returnState.monsterObject = this;
                    returnState.canDie = false;
                }
                else
                {
                    if (other._size < _size)
                    {
                        returnState.monsterObject = this;
                        returnState.canDie = true;
                    }
                }
            }
        }
        else
        {
            if (other._habitat == Habitat.Aerial)
            {
                if (_size == Size.huge)
                {
                    returnState.monsterObject = this;
                    returnState.canDie = true;
                }
                else
                {
                    if (other._dietType == DietType.Carnivore)
                    {
                        if (other._size > _size)
                        {
                            returnState.monsterObject = this;
                            returnState.canDie = false;
                        }
                    }
                }
            }
            else
            {
                if (other._dietType == DietType.Carnivore)
                {
                    if (_dietType == DietType.Carnivore)
                    {
                        if (other._size > _size)
                        {
                            returnState.monsterObject = this;
                            returnState.canDie = false;
                        }
                        else if (other._size == _size)
                        {
                            returnState.canMove = false;
                        }
                        else
                        {
                            returnState.monsterObject = this;
                            returnState.canDie = true;
                        }
                    }
                    else
                    {
                        if (other._size > _size)
                        {
                            returnState.monsterObject = this;
                            returnState.canDie = false;
                        }
                        else if (other._size == _size)
                        {
                            returnState.canMove = false;
                        }
                        else
                        {
                            returnState.canMove = false;
                        }
                    }
                }
                else
                {
                    if (_dietType == DietType.Carnivore)
                    {
                        if (_size > other._size)
                        {
                            returnState.monsterObject = this;
                            returnState.canDie = true;
                        }
                        else
                        {
                            returnState.canMove = false;
                        }
                    }
                    else
                    {
                        returnState.canMove = false;
                    }
                }
            }
        }
        return returnState;
    }
}

//定义箱子基类
[RequireComponent(typeof(SpriteRenderer))]
public abstract class BoxObject : MoveObject
{
    public float moveSpeed = 0.2f; //箱子移动速度
    protected (Coroutine, Vector3) coroutine;//当前协程信息(用于结束动画)
    //表示箱子状态的图片数据
    private Sprite[] boxSprite;
    private SpriteRenderer spriteRenderer;
    private bool currentEnd = false;
    private int _currentState; //当前图片状态
    public UnityAction<BoxObject> willBehavior;
    private MoveObject _moveObject;
    public MoveObject moveObject
    {
        get
        {
            return _moveObject;
        }
        set
        {
            if (value == null)
            {
                currentState = 0; //如果没有控制箱子的对象则设置为初始状态
            }
            else
            {
                currentState = 2; //如果有控制箱子的对象则设置为控制状态
            }
            _moveObject = value;
        }
    } //当前控制箱子的对象
    protected int currentState
    {
        get { return _currentState; }
        set
        {
            if (value < 0 || value >= boxSprite.Length)
            {
                Debug.LogError("设置箱子状态,索引超出范围");
                return;
            }
            if (currentEnd == true && value == 0)
            {
                _currentState = 1;
            }
            else
            {
                _currentState = value;
            }
            spriteRenderer.sprite = boxSprite[_currentState];
        }
    }
    protected string[] boxName;
    protected override void Awake()
    {
        base.Awake();
        SetboxName();
        boxSprite = new Sprite[3] { Resources.Load<Sprite>($"Sprite/GameSprite/{boxName[0]}"), Resources.Load<Sprite>($"Sprite/GameSprite/{boxName[1]}"), Resources.Load<Sprite>($"Sprite/GameSprite/{boxName[2]}") };
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    protected virtual void Start()
    {
        CheckEndToManager(true);
    }
    protected abstract void SetboxName();
    //检查当前脚底是否有END并通知manager(获取布尔值转换模式)
    protected void CheckEndToManager(bool b)
    {
        if (CheckEnd())
        {
            if (b)
            {
                GameManager.Instance.ArrivalEnd++;
                if (currentState != 2)
                {
                    currentState = 1;
                }
                return;
            }
            else
            {
                currentEnd = false;
                GameManager.Instance.ArrivalEnd--;
                if (currentState != 2)
                {
                    currentState = 0;
                }
                return;
            }
        }
    }
    private bool CheckEnd()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);
        if (hits != null && hits.Length != 0)
        {
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("End"))
                {
                    currentEnd = true;
                    return true;
                }
            }
        }
        return false;
    }
    public override void Move(Direction direction)
    {
        if (coroutine.Item1 != null)
        {
            PlayerController.instance.enableRequest--;
            StopCoroutine(coroutine.Item1);
            transform.position = coroutine.Item2;
        }
        Vector3 vector3 = transform.position + direction.ToVector3();
        //移动动画
        CheckEndToManager(false);
        PlayerController.instance.enableRequest++;
        coroutine = (StartCoroutine(BoxMove(vector3)), vector3);
    }

    public override ReturnState CheckMove(Direction direction)
    {
        willBehavior = null;
        ReturnState ReturnState = new ReturnState(true, false, new List<BoxObject>() { this }, null);
        Vector3 vector3 = transform.position + direction.ToVector3();
        RaycastHit2D[] hits = Physics2D.RaycastAll(vector3, Vector2.zero);
        if (hits != null && hits.Length != 0)
        {
            foreach (var item in hits)
            {
                GameSripts gameSripts;
                item.collider.gameObject.TryGetComponent<GameSripts>(out gameSripts);
                if (gameSripts != null)
                {
                    ReturnState returnState = gameSripts.PassivityMove(this, direction);
                    if (!returnState.canMove)
                    {
                        ReturnState.canMove = false;
                    }
                    if (returnState.boxObjects != null && returnState.boxObjects.Count != 0)
                    {
                        ReturnState.boxObjects.AddRange(returnState.boxObjects);
                    }
                }
            }
        }
        return ReturnState;
    }
    //箱子移动动画协程
    protected virtual IEnumerator BoxMove(Vector3 vector3)
    {
        Vector3 startPosition = transform.position;
        float elapsed = 0f;
        while (elapsed < moveSpeed)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveSpeed);
            transform.position = Vector3.Lerp(startPosition, vector3, t);
            yield return null;
        }
        transform.position = vector3;
        PlayerController.instance.enableRequest--;
        CheckEndToManager(true);
        coroutine = (null, Vector3.zero);
        willBehavior?.Invoke(this);
    }
    //箱子爆炸
    public void Die()
    {

    }
}

//定义怪兽类型
//大小
public enum Size
{
    small,
    medium,
    large,
    huge
}
//食类
public enum DietType
{
    Carnivore,  // 食肉
    Herbivore   // 食草
}
//栖息地
[Flags]
public enum Habitat
{
    Aquatic = 1,     // 水生
    Terrestrial = 2, // 陆生
    Aerial = 4       // 空中
}

//定义方向枚举和拓展方法
public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
public static class DirectionExtensions
{
    public static Vector3 ToVector3(this Direction dir)
    {
        return dir switch
        {
            Direction.Up => Vector3.up,
            Direction.Down => Vector3.down,
            Direction.Left => Vector3.left,
            Direction.Right => Vector3.right,
            _ => Vector3.zero
        };
    }
    public static Direction Rotate(this Direction dir, int count)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException("count");
        for (int i = 0; i < count; i++)
        {
            dir = Rotate1(dir);
        }
        return dir;
    }
    private static Direction Rotate1(this Direction dir)
    {
        return dir switch
        {
            Direction.Up => Direction.Right,
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Up,
            Direction.Right => Direction.Down,
            _ => throw new System.ArgumentException("Invalid direction")
        };
    }
}
