using UnityEngine;
using ToolSystem.Actions;

namespace ToolSystem
{
    /// <summary>
    /// 工具物品基类 - 继承自Item
    /// </summary>
    public class ToolItem : Item
    {
        [Header("工具设置")]
        [Tooltip("工具类型")]
        [SerializeField] protected ToolType toolType = ToolType.None;
        public ToolType ToolType => toolType;

        [Header("工具行为")]
        [Tooltip("该工具可执行的行为（ScriptableObject配置）")]
        [SerializeField] protected ScriptableToolAction toolAction;
        public ScriptableToolAction ToolAction => toolAction;

        protected virtual void Awake()
        {
            itemType = ItemType.Tool;
        }

        protected virtual void Update()
        {
            // 只有被持有时才检测输入
            if (!IsPickedUp || CurrentHolder == null)
            {
                return;
            }

            // 鼠标左键使用工具
            if (Input.GetMouseButtonDown(0))
            {
                // 尝试检测目标
                IToolTarget target = DetectTarget();
                TryUse(CurrentHolder, target);
            }
        }

        /// <summary>
        /// 检测工具作用目标（子类可重写）
        /// </summary>
        protected virtual IToolTarget DetectTarget()
        {
            // 从鼠标位置发射射线检测目标
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                Debug.Log("Interacted with " + hit.collider.name);
                return hit.collider.GetComponent<IToolTarget>();
            }
            return null;
        }

        /// <summary>
        /// 尝试使用工具
        /// </summary>
        public virtual bool TryUse(GameObject user, IToolTarget target = null)
        {
            if (target != null && !target.CanInteract(this))
            {
                Debug.Log($"{itemName} 无法作用于该目标");
                return false;
            }

            OnUse(user, target);
            return true;
        }

        /// <summary>
        /// 执行工具使用（子类可重写）
        /// </summary>
        protected virtual void OnUse(GameObject user, IToolTarget target)
        {
            if (toolAction != null)
            {
                toolAction.Execute(this, user, target);
            }

            Debug.Log($"{user.name} 使用了 {itemName}");
        }
    }
}
