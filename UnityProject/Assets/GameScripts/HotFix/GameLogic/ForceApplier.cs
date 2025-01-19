using UnityEngine;
using UnityEngine.UI;

public class ForceApplier : MonoBehaviour
{
    [Header("力场参数")]
    public float forceRadius = 5f;        // 力场作用半径
    public float forceMagnitude = 10f;    // 力的大小
    public ForceMode forceMode = ForceMode.Impulse; // 力的应用模式
    
    // 新增力场方向模式的枚举
    public enum ForceDirectionMode
    {
        Radial,             // 径向力（从中心向外）
        Directional,        // 定向力（固定方向）
        Attractive         // 吸引力（向中心）
    }
    
    [Header("力场方向设置")]
    public ForceDirectionMode directionMode = ForceDirectionMode.Radial;
    public Vector3 customDirection = Vector3.forward; // 用于定向力模式
    
    [Header("UI组件")]
    public Button triggerButton;          // 触发按钮
    
    private void Start()
    {
        // 确保按钮被赋值
        if (triggerButton != null)
        {
            triggerButton.onClick.AddListener(ApplyForceToNearbyObjects);
        }
        else
        {
            Debug.LogWarning("请在Inspector中指定触发按钮！");
        }
    }
    
    private void ApplyForceToNearbyObjects()
    {
        // 使用物体自身位置作为力场中心
        Vector3 center = transform.position;
        
        // 获取范围内所有的碰撞体
        Collider[] colliders = Physics.OverlapSphere(center, forceRadius);
        
        foreach (Collider col in colliders)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 forceDirection;
                
                // 根据不同模式计算力的方向
                switch (directionMode)
                {
                    case ForceDirectionMode.Radial:
                        forceDirection = (col.transform.position - center).normalized;
                        break;
                    case ForceDirectionMode.Directional:
                        forceDirection = customDirection.normalized;
                        break;
                    case ForceDirectionMode.Attractive:
                        forceDirection = (center - col.transform.position).normalized;
                        break;
                    default:
                        forceDirection = Vector3.zero;
                        break;
                }
                
                rb.AddForce(forceDirection * forceMagnitude, forceMode);
            }
        }
    }
    
    private Vector3 CalculateForceDirection(Vector3 center, Vector3 targetPosition)
    {
        switch (directionMode)
        {
            case ForceDirectionMode.Radial:
                return (targetPosition - center).normalized;
            case ForceDirectionMode.Directional:
                return customDirection.normalized;
            case ForceDirectionMode.Attractive:
                return (center - targetPosition).normalized;
            default:
                return Vector3.zero;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 center = transform.position;
        
        // 绘制力场范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, forceRadius);
        
        // 绘制每个碰撞体的受力方向
        Collider[] colliders = Physics.OverlapSphere(center, forceRadius);
        Gizmos.color = Color.red;
        
        foreach (Collider col in colliders)
        {
            if (col.GetComponent<Rigidbody>() != null)
            {
                Vector3 forceDirection = CalculateForceDirection(center, col.transform.position);
                // 从碰撞体位置绘制一条表示力方向的线
                Gizmos.DrawLine(col.transform.position, 
                    col.transform.position + forceDirection * 2f); // 2f是箭头长度
                
                // 绘制箭头
                Vector3 arrowEnd = col.transform.position + forceDirection * 2f;
                Vector3 right = Vector3.Cross(forceDirection, Vector3.up).normalized;
                Vector3 arrowHead1 = arrowEnd - forceDirection * 0.5f + right * 0.2f;
                Vector3 arrowHead2 = arrowEnd - forceDirection * 0.5f - right * 0.2f;
                Gizmos.DrawLine(arrowEnd, arrowHead1);
                Gizmos.DrawLine(arrowEnd, arrowHead2);
            }
        }
    }
}