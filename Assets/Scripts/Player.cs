using System.Collections;
using UnityEngine;
using InventorySystem;
using Core.Constants;
using Core.Items;
using ItemSystem;

namespace Core.Player
{
    /// <summary>
    /// 玩家控制器
    /// 重构：只保留输入和移动逻辑，拾取功能委托给ItemInteractor和ItemPickupHandler
    /// </summary>
    public class Player : MonoBehaviour
    {
        #region Serialized Fields
        [Header("移动设置")]
        public float moveSpeed = 5f;
        #endregion

        #region Private Fields
        private Rigidbody rb;
        private Vector3 moveDirection;
        private Camera mainCamera;

        private ItemInteractor itemInteractor;
        private ItemPickupHandler pickupHandler;
        private InventoryManager inventoryManager;

        private bool isCooldown = false;
        private float cooldownTime = GameConstants.DefaultCooldownTime;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            InitializeComponents();
        }

        private void Update()
        {
            HandleMovementInput();
            HandleItemInteraction();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            rb = GetComponent<Rigidbody>();
            mainCamera = Camera.main;

            itemInteractor = GetComponent<ItemInteractor>();
            pickupHandler = GetComponent<ItemPickupHandler>();
            inventoryManager = GetComponent<InventoryManager>();

            if (itemInteractor == null)
            {
                itemInteractor = gameObject.AddComponent<ItemInteractor>();
            }
            if (pickupHandler == null)
            {
                pickupHandler = gameObject.AddComponent<ItemPickupHandler>();
            }
            if (inventoryManager == null)
            {
                inventoryManager = InventoryManager.Instance;
            }
        }
        #endregion

        #region Movement
        private void HandleMovementInput()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        }

        private void HandleMovement()
        {
            if (moveDirection.magnitude <= 0) return;
            MovePlayer();
        }

        private void MovePlayer()
        {
            Vector3 targetPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);
        }
        #endregion

        #region Item Interaction
        private void HandleItemInteraction()
        {
            if (!Input.GetKeyDown(KeyCode.Space) || isCooldown) return;

            if (itemInteractor.HasItemInRange())
            {
                TryPickupItem();
            }
            else if (inventoryManager != null)
            {
                TryDropItem();
            }
        }

        private void TryPickupItem()
        {
            bool success = itemInteractor.TryPickupItem();
            if (success)
            {
                StartCoroutine(Cooldown());
            }
        }

        private void TryDropItem()
        {
            Item selectedItem = inventoryManager.GetSelectedItem();
            if (selectedItem != null)
            {
                Vector3 dropPos = transform.position + transform.forward * GameConstants.DefaultDropDistance;
                DropSelectedItem(dropPos);
                StartCoroutine(Cooldown());
            }
        }

        private void DropSelectedItem(Vector3 dropPosition)
        {
            Item item = inventoryManager.RemoveItem(inventoryManager.SelectedIndex);
            if (item != null)
            {
                item.gameObject.SetActive(true);
                item.OnDropped(dropPosition);
                Debug.Log($"丢弃物品: {item.itemName}");
            }
        }

        private IEnumerator Cooldown()
        {
            if (isCooldown) yield break;
            isCooldown = true;
            yield return new WaitForSeconds(cooldownTime);
            isCooldown = false;
        }
        #endregion
    }
}
