using System.Collections;
using UnityEngine;

/// <summary>
/// 玩家控制器
/// 重构：只保留输入和移动逻辑，拾取功能委托给ItemInteractor和ItemPickupHandler
/// </summary>
public class Player : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;

    //public float rotationSpeed = 10f;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private Camera mainCamera;

    [Header("物品交互")]
    private ItemInteractor itemInteractor;  //捡东西时候用
    private ItemPickupHandler pickupHandler;//放东西时候用

    private bool isCooldown = false;
    private float cooldownTime = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        // 获取组件引用
        itemInteractor = GetComponent<ItemInteractor>();
        pickupHandler = GetComponent<ItemPickupHandler>();

        // 如果没有，自动添加
        if (itemInteractor == null)
        {
            itemInteractor = gameObject.AddComponent<ItemInteractor>();
        }
        if (pickupHandler == null)
        {
            pickupHandler = gameObject.AddComponent<ItemPickupHandler>();
        }
    }

    void Update()
    {
        HandleMovementInput();
        HandleItemInteraction();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    // ============= 移动相关（保持不变）=============

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
        //RotatePlayer();
    }

    private void MovePlayer()
    {
        Vector3 targetPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);
    }

    /*
    private void RotatePlayer()
    {
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        if (cameraForward.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }
    }
    */

    // ============= 物品交互（重构后）=============

    /// <summary>
    /// 处理物品交互输入
    /// </summary>
    private void HandleItemInteraction()
    {
        if (!Input.GetKeyDown(KeyCode.F) || isCooldown) return;

        // 拾取物品
        if (itemInteractor.HasItemInRange())
        {
            bool success = itemInteractor.TryPickupItem();
            if (success)
            {
                StartCoroutine(Cooldown());
            }
        }
        // 放置物品
        else if (pickupHandler.HeldItem != null)
        {
            Vector3 dropPos = transform.position + transform.forward * 2f;
            pickupHandler.DropItem(dropPos);
            StartCoroutine(Cooldown());
        }
    }

    private IEnumerator Cooldown()
    {
        if (isCooldown) yield break;
        isCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isCooldown = false;
    }
}
