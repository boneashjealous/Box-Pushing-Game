using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnidirectionalSripts : TerrainObject
{
    public override ReturnState PassivityMove(MonsterObject monsterObject, Direction direction)
    {
        if (direction.ToVector3() == GetFacingDirection())
        {
            return new ReturnState(true, false,null,null);
        }
        else
        {
            return new ReturnState(false,false,null,null);
        }
    }
    public override ReturnState PassivityMove(BoxObject boxObject, Direction direction)
    {
        if (direction.ToVector3() == GetFacingDirection())
        {
            return new ReturnState(true, false, null, null);
        }
        else
        {
            return new ReturnState(false, false, null, null);
        }
    }
    // 获取当前对象面朝方向的单位向量
    public Vector3 GetFacingDirection()
    {
        // 将角度转换为弧度
        float angleInRadians = transform.eulerAngles.z * Mathf.Deg2Rad;

        // 计算单位向量
        Vector3 direction =new Vector3(
            Mathf.Round(Mathf.Cos(angleInRadians)),
            Mathf.Round(Mathf.Sin(angleInRadians))
        );

        return direction.normalized;
    }
}
