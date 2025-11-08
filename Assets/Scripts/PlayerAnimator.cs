using Unity.Netcode;
using UnityEngine;

/// <summary>
///脚本：PlayerAnimator.cs
///时间：2023.9.10
///功能：角色动画控制
/// </summary>

public class PlayerAnimator : NetworkBehaviour
{
    private const string IS_WALKING = "isWalking";
    private Animator animator;
    [SerializeField] private Player player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!IsOwner) return;
        animator.SetBool(IS_WALKING, player.IsWalking);
    }
}
