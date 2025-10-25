using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NestSripts : TerrainObject
{
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(HatchingEggs());
    }
    public override ReturnState PassivityMove(MonsterObject monsterObject, Direction direction)
    {
        if (monsterObject._habitat == Habitat.Aerial)
        {
            return new ReturnState(true, false, null, null);
        }
        else
        {
            monsterObject.willBehavior += Ability;
            return new ReturnState(true, false, null, null);
        }
    }
    protected override void Ability(MonsterObject monsterObject)
    {
        Destroy(this.gameObject);
    }
    protected override void Ability(BoxObject boxObject)
    {
        Destroy(this.gameObject);
    }

    IEnumerator HatchingEggs()
    {
        yield return new WaitForSeconds(20f);
        if(this != null)
        {
            GameObject monsterPrefab = Resources.Load<GameObject>("Prefabs/Birdie_0"); 
            Instantiate(monsterPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
