using Unity.Netcode.Components;
using UnityEngine;

/// <summary>
///脚本：ClientNetworkTransform.cs
///时间：
///功能：
/// </summary>
namespace Unity.Multiplayer.Samples.Utilities.ClientAuthority
{
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}