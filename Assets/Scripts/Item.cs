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

        // 恢复物品到场景
        transform.SetParent(null);
        transform.position = dropPosition;

        // 恢复碰撞体
        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        Debug.Log($"{itemName} was dropped at {dropPosition}");
    }
}


