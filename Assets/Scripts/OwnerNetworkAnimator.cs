using Unity.Netcode.Components;
using UnityEngine;

/// <summary>
///脚本：OwnerNetworkAnimator.cs
///时间：2023.10.16
///功能：
/// </summary>

public class OwnerNetworkAnimator : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
