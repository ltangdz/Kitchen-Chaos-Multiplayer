using UnityEngine;

/// <summary>
///脚本：FollowTransform.cs
///时间：2023.12.6
///功能：视觉跟随移动（不改变父物体）
/// </summary>

public class FollowTransform : MonoBehaviour
{
    private Transform targetTransform;

    public void SetTargetTransform(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }

    private void LateUpdate()
    {
        if (targetTransform == null) return;

        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }
}
