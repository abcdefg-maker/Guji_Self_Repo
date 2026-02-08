using UnityEngine;
using Core.Items;

namespace InventorySystem
{
    /// <summary>
    /// 物品栏槽位数据
    /// </summary>
    [System.Serializable]
    public class InventorySlot
    {
        [Header("物品引用")]
        public Item itemRef;            // 物品实例引用

        [Header("堆叠信息")]
        public int count = 0;           // 当前数量
        public int maxStack = 99;       // 最大堆叠数

        /// <summary>
        /// 槽位是否为空
        /// </summary>
        public bool IsEmpty => itemRef == null || count <= 0;

        /// <summary>
        /// 槽位是否已满
        /// </summary>
        public bool IsFull => count >= maxStack;

        /// <summary>
        /// 物品ID（快捷访问）
        /// </summary>
        public string ItemID => itemRef != null ? itemRef.itemID : "";

        /// <summary>
        /// 物品类型（快捷访问）
        /// </summary>
        public ItemType ItemType => itemRef != null ? itemRef.itemType : ItemType.Material;

        /// <summary>
        /// 清空槽位
        /// </summary>
        public void Clear()
        {
            itemRef = null;
            count = 0;
        }

        /// <summary>
        /// 设置物品
        /// </summary>
        public void SetItem(Item item, int amount = 1)
        {
            itemRef = item;
            count = amount;

            // 工具不可堆叠
            if (item != null && item.itemType == ItemType.Tool)
            {
                maxStack = 1;
            }
            else
            {
                maxStack = 99;
            }
        }

        /// <summary>
        /// 检查是否可以与指定物品堆叠
        /// </summary>
        public bool CanStackWith(Item item)
        {
            if (IsEmpty) return false;
            if (IsFull) return false;
            if (item == null) return false;

            // 工具不可堆叠
            if (item.itemType == ItemType.Tool) return false;

            // 相同物品ID才可堆叠
            return itemRef.itemID == item.itemID;
        }

        /// <summary>
        /// 增加数量
        /// </summary>
        /// <returns>实际增加的数量</returns>
        public int AddCount(int amount)
        {
            int canAdd = Mathf.Min(amount, maxStack - count);
            count += canAdd;
            return canAdd;
        }

        /// <summary>
        /// 减少数量
        /// </summary>
        /// <returns>实际减少的数量</returns>
        public int RemoveCount(int amount)
        {
            int canRemove = Mathf.Min(amount, count);
            count -= canRemove;

            if (count <= 0)
            {
                Clear();
            }

            return canRemove;
        }
    }
}
