using Unity.Netcode;
using UnityEngine;

/// <summary>
///脚本：IKitchenObjectParent.cs
///时间：
///功能：
/// </summary>

public interface IKitchenObjectParent
{
    public Transform GetKitchenObjectFollowTransform();

    public void SetKitchenObject(KitchenObject kitchenObject);

    public KitchenObject GetKitchenObject(); 

    public void ClearKitchenObject();

    public bool HasKitchenObject();

    public NetworkObject GetNetworkObject();
}