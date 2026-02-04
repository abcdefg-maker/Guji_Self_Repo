using UnityEngine;

/// 重构：移除事件订阅，只保留状态和回调
/// </summary>

/// <summary>
/// 物品类型枚举
/// </summary>
public enum ItemType
{
    Material,      // 材料
    Tool,          // 工具
    Consumable,    // 消耗品
    Seed,          // 种子
    Crop           // 农作物
}

/// <summary>
/// 物品基类 - 场景中的物品表现
/// <summary>

public class Item : MonoBehaviour
{
    [Header("物品信息")]
    [Tooltip("物品唯一标识")]
    public string itemID = "item_001";

    [Tooltip("物品显示名称")]
    public string itemName = "Test Item";

    [Tooltip("物品类型")]
    public ItemType itemType = ItemType.Material;

    [Header("物品栏显示")]
    [Tooltip("物品图标（用于UI显示）")]
    public Sprite itemIcon;

    [Header("拾取设置")]
    [Tooltip("是否可以被拾取")]
    public bool canBePickedUp = true;

    [Tooltip("拾取时的附着点偏移")]
    public Vector3 attachOffset = Vector3.zero;

    [Header("状态")]
    private bool isPickedUp = false;
    public bool IsPickedUp => isPickedUp;

    private GameObject currentHolder;
    public GameObject CurrentHolder => currentHolder;

    /// <summary>
    /// 被拾取时调用（虚方法，子类可重写）
    /// </summary>
    public virtual void OnPickedUp(GameObject picker)
    {
        isPickedUp = true;
        currentHolder = picker;

        Debug.Log($"{itemName} was picked up by {picker.name}");
    }

    /// <summary>
    /// 被放置时调用（虚方法，子类可重写）
    /// </summary>
    public virtual void OnDropped(Vector3 dropPosition)
    {
        isPickedUp = false;
        currentHolder = null;

        // 获取当前手的世界坐标
        Vector3 currentPos = transform.position;

        // 解除父子关系
        transform.SetParent(null);

        // 使用射线检测地面，从当前位置向下发射
        Vector3 finalPosition = currentPos;
        Ray ray = new Ray(new Vector3(currentPos.x, currentPos.y + 1f, currentPos.z), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            // X、Z 保持手的位置，Y 稍微高于地面
            finalPosition = new Vector3(currentPos.x, hit.point.y + 0.1f, currentPos.z);
        }

        transform.position = finalPosition;

        // 恢复碰撞体
        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        // 冻结 Rigidbody，防止物品移动
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Debug.Log($"{itemName} was dropped at {finalPosition}");
    }
}


