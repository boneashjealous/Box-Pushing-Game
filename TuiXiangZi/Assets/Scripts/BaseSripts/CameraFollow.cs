using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // 要跟随的目标对象
    private Transform target;

    // 摄像头移动的平滑度（秒）
    public float smoothTime = 0.3f;

    // 相对于目标的偏移量
    public Vector3 offset = new Vector3(0, 0, -10);

    // 摄像头移动的边界限制
    public bool enableBounds = false;
    public float minX = 0f;
    public float maxX = 10f;
    public float minY = 0f;
    public float maxY = 10f;

    private Vector3 velocity = Vector3.zero;
    private Coroutine followCoroutine;

    void Start()
    {
        // 启动跟随协程
        if (target != null)
        {
            followCoroutine = StartCoroutine(FollowTarget());
        }
        else
        {
            Debug.LogError("SmoothCameraFollow: 没有设置目标对象!");
        }
    }

    void OnDisable()
    {
        // 停止协程
        if (followCoroutine != null)
        {
            StopCoroutine(followCoroutine);
        }
    }

    // 使用协程实现平滑跟随
    private IEnumerator FollowTarget()
    {
        while (true)
        {
            // 计算期望的摄像头位置
            Vector3 targetPosition = target.position + offset;

            // 如果需要边界限制，则应用边界
            if (enableBounds)
            {
                targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
                targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
            }

            // 使用SmoothDamp平滑移动摄像头
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                smoothTime
            );

            // 等待下一帧
            yield return null;
        }
    }

    // 在Scene视图中绘制边界（仅在启用边界时）
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

    // 公共方法：动态改变目标
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