using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static GameSripts;


//����������Ϸ����Ļ���
[RequireComponent(typeof(BoxCollider2D))]
public abstract class GameSripts : MonoBehaviour
{
    protected virtual void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }
    //�����ƶ�
    public abstract ReturnState PassivityMove(MonsterObject monsterObject, Direction direction);
    public abstract bool PassivityMove(SpectreSripts spectreSripts);
    public abstract ReturnState PassivityMove(BoxObject boxObject, Direction direction);

    //�����ܷ��ƶ����Ƿ����������ݣ����Ӵ������ݣ���������
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

//������ζ������
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

//�����ƶ��������
public abstract class MoveObject : GameSripts
{
    //�ƶ�
    public abstract void Move(Direction direction);
    //����ƶ�
    public abstract ReturnState CheckMove(Direction direction);
    //ת��
    protected void Turn(Direction direction)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction.ToVector3());
    }
}
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
//����������
public abstract class AbilityObject : MoveObject
{
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected AudioSource audioSource;
    protected static Dictionary<string, AudioClip> audioClip;
    protected int _abilityValue;
    [SerializeField]
    public int abilityMaxValue;
    //����չʾ����ֵ��ͼƬ
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
                Debug.Log("����ֵ���㣬�޷�ʹ�ü���");
                return;
            }
        }
        Ability();
    }
    //�ͷż���
    public virtual void FreedSkill()
    {
        return;
    }
    protected abstract void Ability();
    //��������ֵ
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
            //������Ƶ�ļ�
            Resources.LoadAll<AudioClip>("Audio/SoundEffects").ToList().ForEach(clip =>
            {
                if (!audioClip.ContainsKey(clip.name))
                {
                    audioClip.Add(clip.name, clip);
                }
            });
            if (audioClip.Count == 0)
            {
                Debug.LogError("��Ƶ�ļ�����ʧ�ܣ�����·�����ļ����Ƿ���ȷ");
            }
        }
        //��ʼ������ֵ
        if (abilityMaxValue == -1)
        {
            _abilityValue = -1; //-1��ʾ������
        }
        else
        {
            _abilityValue = abilityMaxValue;
        }
        //��ʼ�����������¼�
        animationEnd = new UnityAction<string>(MoveEnd);
        animationEnd += HitWallEnd;
    }
    //�ƶ���������
    protected virtual void MoveEnd(string animationName)
    {
        if (animationName != "Move")
            return;
        transform.position = transform.position + new Vector3(Mathf.Round(transform.up.x), Mathf.Round(transform.up.y), 0);
        End();
    }
    //���ڶ�������
    protected virtual void HitWallEnd(string animationName)
    {
        if (animationName == "HitWall")
            End();
    }

    //��������
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
        // �ȴ�������ʼ����
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            yield return null;
        }
        // ��ȡ��������
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        // �ȴ������������
        yield return new WaitForSeconds(animationLength);
        // ִ�лص�����
        animationEnd?.Invoke(animationName);
    }
    //���������������ͷ�
    protected virtual void End()
    {
        PlayerController.instance.enableRequest--;
    }
}



//������޻���
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
    //���ù�������
    protected abstract void SetAttributes();
    //�ױ��ƶ�
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
            //�ƶ�����
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
    //�����ƶ�
    protected void HitWallMove()
    {
        if (willBehavior1 != null)
        {
            willBehavior1?.Invoke(this);
            return;
        }
        Play("HitWall");
    }
    //�ƶ�
    public override void Move(Direction direction)
    {
        willBehavior = null;
        willBehavior1 = null;
        Turn(direction);
        ChangeableMove(direction);
    }
    //����ƶ�
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

    //���﹥��
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
    //��������
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
        //ˮ�����ˮ�ؿ�
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
    //�Ƚ�������������ͷ�������
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

//�������ӻ���
[RequireComponent(typeof(SpriteRenderer))]
public abstract class BoxObject : MoveObject
{
    public float moveSpeed = 0.2f; //�����ƶ��ٶ�
    protected (Coroutine, Vector3) coroutine;//��ǰЭ����Ϣ(���ڽ�������)
    //��ʾ����״̬��ͼƬ����
    private Sprite[] boxSprite;
    private SpriteRenderer spriteRenderer;
    private bool currentEnd = false;
    private int _currentState; //��ǰͼƬ״̬
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
                currentState = 0; //���û�п������ӵĶ���������Ϊ��ʼ״̬
            }
            else
            {
                currentState = 2; //����п������ӵĶ���������Ϊ����״̬
            }
            _moveObject = value;
        }
    } //��ǰ�������ӵĶ���
    protected int currentState
    {
        get { return _currentState; }
        set
        {
            if (value < 0 || value >= boxSprite.Length)
            {
                Debug.LogError("��������״̬,����������Χ");
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
    //��鵱ǰ�ŵ��Ƿ���END��֪ͨmanager(��ȡ����ֵת��ģʽ)
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
        //�ƶ�����
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
    //�����ƶ�����Э��
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
    //���ӱ�ը
    public void Die()
    {

    }
}

//�����������
//��С
public enum Size
{
    small,
    medium,
    large,
    huge
}
//ʳ��
public enum DietType
{
    Carnivore,  // ʳ��
    Herbivore   // ʳ��
}
//��Ϣ��
[Flags]
public enum Habitat
{
    Aquatic = 1,     // ˮ��
    Terrestrial = 2, // ½��
    Aerial = 4       // ����
}

//���巽��ö�ٺ���չ����
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
