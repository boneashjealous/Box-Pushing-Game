using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverBoxSripts : BoxObject
{
    public int usageCount;
    private bool canMove=false;
    //特殊箱子，需要覆盖基类的一些方法
    protected override void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }
    public override ReturnState PassivityMove(MonsterObject monsterObject, Direction direction)
    {
        if (monsterObject._habitat.HasFlag(Habitat.Aerial)|| canMove)
        {
            return new ReturnState(true, false, null, null);
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
        if(canMove)
        {
            return new ReturnState(true, false, null, null);
        }
        return new ReturnState(false, false, null, null);
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
        PlayerController.instance.enableRequest++;
        willBehavior += CheckTerrainObject;
        coroutine = (StartCoroutine(BoxMove(vector3)), vector3);
    }
    protected override IEnumerator BoxMove(Vector3 vector3)
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
        coroutine = (null, Vector3.zero);
        willBehavior?.Invoke(this);
    }
    private void CheckTerrainObject(BoxObject boxObject)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);
        if (hits != null && hits.Length != 0)
        {
            foreach (var item in hits)
            {
                TerrainObject terrainObject;
                item.collider.gameObject.TryGetComponent<TerrainObject>(out terrainObject);
                if (terrainObject != null)
                {
                    Destroy(terrainObject.gameObject);
                    if (usageCount > 1)
                    {
                        usageCount--;
                    }
                    else
                    {
                        canMove=true;
                    }
                }
            }
        }
    }

    protected override void SetboxName()
    {
        return;
    }
}
