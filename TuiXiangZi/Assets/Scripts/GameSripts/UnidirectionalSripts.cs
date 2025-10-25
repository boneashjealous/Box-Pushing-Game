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
    // ��ȡ��ǰ�����泯����ĵ�λ����
    public Vector3 GetFacingDirection()
    {
        // ���Ƕ�ת��Ϊ����
        float angleInRadians = transform.eulerAngles.z * Mathf.Deg2Rad;

        // ���㵥λ����
        Vector3 direction =new Vector3(
            Mathf.Round(Mathf.Cos(angleInRadians)),
            Mathf.Round(Mathf.Sin(angleInRadians))
        );

        return direction.normalized;
    }
}
