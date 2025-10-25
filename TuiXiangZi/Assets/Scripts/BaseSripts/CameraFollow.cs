using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Ҫ�����Ŀ�����
    private Transform target;

    // ����ͷ�ƶ���ƽ���ȣ��룩
    public float smoothTime = 0.3f;

    // �����Ŀ���ƫ����
    public Vector3 offset = new Vector3(0, 0, -10);

    // ����ͷ�ƶ��ı߽�����
    public bool enableBounds = false;
    public float minX = 0f;
    public float maxX = 10f;
    public float minY = 0f;
    public float maxY = 10f;

    private Vector3 velocity = Vector3.zero;
    private Coroutine followCoroutine;

    void Start()
    {
        // ��������Э��
        if (target != null)
        {
            followCoroutine = StartCoroutine(FollowTarget());
        }
        else
        {
            Debug.LogError("SmoothCameraFollow: û������Ŀ�����!");
        }
    }

    void OnDisable()
    {
        // ֹͣЭ��
        if (followCoroutine != null)
        {
            StopCoroutine(followCoroutine);
        }
    }

    // ʹ��Э��ʵ��ƽ������
    private IEnumerator FollowTarget()
    {
        while (true)
        {
            // ��������������ͷλ��
            Vector3 targetPosition = target.position + offset;

            // �����Ҫ�߽����ƣ���Ӧ�ñ߽�
            if (enableBounds)
            {
                targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
                targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
            }

            // ʹ��SmoothDampƽ���ƶ�����ͷ
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                smoothTime
            );

            // �ȴ���һ֡
            yield return null;
        }
    }

    // ��Scene��ͼ�л��Ʊ߽磨�������ñ߽�ʱ��
    void OnDrawGizmosSelected()
    {
        if (enableBounds)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
            Vector3 size = new Vector3(maxX - minX, maxY - minY, 0);
            Gizmos.DrawWireCube(center, size);
        }
    }

    // ������������̬�ı�Ŀ��
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (followCoroutine != null)
        {
            StopCoroutine(followCoroutine);
        }
        if (gameObject.activeInHierarchy)
        {
            followCoroutine = StartCoroutine(FollowTarget());
        }
    }
}