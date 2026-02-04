using System;
using UnityEngine;
using ToolSystem;

namespace InventorySystem
{
    /// <summary>
    /// 物品栏管理器
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [Header("槽位设置")]
        [SerializeField] private int maxSlots = 10;
        public int MaxSlots => maxSlots;

        [SerializeField] private InventorySlot[] slots;
        public InventorySlot[] Slots => slots;

        [Header("选中状态")]
        [SerializeField] private int selectedIndex = 0;
        public int SelectedIndex => selectedIndex;

        [Header("装备设置")]
        [SerializeField] private Transform handPosition;
        private ToolItem currentEquippedTool;

        // 事件
        public event Action<int> OnSlotChanged;          // 槽位内容变化
        public event Action<int> OnSelectionChanged;     // 选中槽位变化

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializeSlots();
        }

        private void Start()
        {
            // 查找手部位置
            if (handPosition == null)
            {
                var handler = GetComponent<ItemPickupHandler>();
                if (handler != null)
                {
                    handPosition = handler.handPosition;
                }
            }

            // 默认选中第一个槽位
            SelectSlot(0);
        }

        private void Update()
        {
            HandleInput();
        }

        /// <summary>
        /// 初始化槽位
        /// </summary>
        private void InitializeSlots()
        {
            slots = new InventorySlot[maxSlots];
            for (int i = 0; i < maxSlots; i++)
            {
                slots[i] = new InventorySlot();
            }
        }

        /// <summary>
        /// 处理输入
        /// </summary>
        private void HandleInput()
        {
            // 数字键1-9选择槽位1-9
            for (int i = 1; i <= 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i - 1))
                {
                    SelectSlot(i - 1);
                    return;
                }
            }

            // 数字键0选择槽位10
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SelectSlot(9);
                return;
            }

            // 滚轮切换
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f)
            {
                SelectSlot((selectedIndex - 1 + maxSlots) % maxSlots);
            }
            else if (scroll < 0f)
            {
                SelectSlot((selectedIndex + 1) % maxSlots);
            }
        }

        /// <summary>
        /// 添加物品到物品栏
        /// </summary>
        public bool AddItem(Item item)
        {
            if (item == null) return false;

            // 1. 尝试堆叠到已有槽位
            if (item.itemType != ItemType.Tool)
            {
                for (int i = 0; i < maxSlots; i++)
                {
                    if (slots[i].CanStackWith(item))
                    {
                        slots[i].AddCount(1);
                        item.gameObject.SetActive(false);
                        OnSlotChanged?.Invoke(i);
                        Debug.Log($"物品 {item.itemName} 堆叠到槽位 {i + 1}");
                        return true;
                    }
                }
            }

            // 2. 找空槽位
            for (int i = 0; i < maxSlots; i++)
            {
                if (slots[i].IsEmpty)
                {
                    slots[i].SetItem(item, 1);
                    item.gameObject.SetActive(false);
                    OnSlotChanged?.Invoke(i);
                    Debug.Log($"物品 {item.itemName} 放入槽位 {i + 1}");

                    // 如果是当前选中槽位且是工具，自动装备
                    if (i == selectedIndex && item.itemType == ItemType.Tool)
                    {
                        EquipTool(item as ToolItem);
                    }

                    return true;
                }
            }

            Debug.Log("物品栏已满！");
            return false;
        }

        /// <summary>
        /// 移除指定槽位的物品
        /// </summary>
        public Item RemoveItem(int index, int amount = 1)
        {
            if (index < 0 || index >= maxSlots) return null;
            if (slots[index].IsEmpty) return null;

            Item item = slots[index].itemRef;
            slots[index].RemoveCount(amount);

            // 如果是当前装备的工具，卸下
            if (item is ToolItem tool && tool == currentEquippedTool)
            {
                UnequipTool();
            }

            OnSlotChanged?.Invoke(index);
            return item;
        }

        /// <summary>
        /// 选择槽位
        /// </summary>
        public void SelectSlot(int index)
        {
            if (index < 0 || index >= maxSlots) return;

            int oldIndex = selectedIndex;
            selectedIndex = index;

            // 切换装备的工具
            UnequipTool();

            var slot = slots[selectedIndex];
            if (!slot.IsEmpty && slot.itemRef is ToolItem tool)
            {
                EquipTool(tool);
            }

            if (oldIndex != selectedIndex)
            {
                OnSelectionChanged?.Invoke(selectedIndex);
                Debug.Log($"选中槽位 {selectedIndex + 1}");
            }
        }

        /// <summary>
        /// 装备工具
        /// </summary>
        private void EquipTool(ToolItem tool)
        {
            if (tool == null) return;

            currentEquippedTool = tool;
            tool.gameObject.SetActive(true);

            if (handPosition != null)
            {
                tool.transform.SetParent(handPosition);
                tool.transform.localPosition = tool.attachOffset;
                tool.transform.localRotation = Quaternion.identity;
            }

            Debug.Log($"装备工具: {tool.itemName}");
        }

        /// <summary>
        /// 卸下工具
        /// </summary>
        private void UnequipTool()
        {
            if (currentEquippedTool != null)
            {
                currentEquippedTool.gameObject.SetActive(false);
                currentEquippedTool = null;
            }
        }

        /// <summary>
        /// 获取当前选中的物品
        /// </summary>
        public Item GetSelectedItem()
        {
            if (selectedIndex < 0 || selectedIndex >= maxSlots) return null;
            return slots[selectedIndex].itemRef;
        }

        /// <summary>
        /// 获取当前装备的工具
        /// </summary>
        public ToolItem GetEquippedTool()
        {
            return currentEquippedTool;
        }

        /// <summary>
        /// 检查物品栏是否已满
        /// </summary>
        public bool IsFull()
        {
            for (int i = 0; i < maxSlots; i++)
            {
                if (slots[i].IsEmpty) return false;
            }
            return true;
        }

        /// <summary>
        /// 获取指定槽位
        /// </summary>
        public InventorySlot GetSlot(int index)
        {
            if (index < 0 || index >= maxSlots) return null;
            return slots[index];
        }
    }
}
