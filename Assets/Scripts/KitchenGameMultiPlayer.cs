using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///脚本：KitchenGameMultiPlayer.cs
///时间：2023.12.6
///功能：
/// </summary>

public class KitchenGameMultiPlayer : SingletonNetwork<KitchenGameMultiPlayer>
{
    private const int MAX_PLAYER_AMOUNT = 4;

    public event EventHandler OnPlayerDataNetworkListChanged;

    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;
    [SerializeField] private List<Color> playerColorList;
    
    private NetworkList<PlayerData> playerDataNetworkList;
    protected override void Awake()
    {
        base.Awake();
        
        DontDestroyOnLoad(this);
        
        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeevent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }


    public override void OnDestroy()
    {
        // NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
        base.OnDestroy();
    }

    // 处理延迟加入
    #region LateJoin

    public void StartHost()
    {
        // 先清除之前的注册回调
        NetworkManager.Singleton.ConnectionApprovalCallback = null;

        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;

        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
            {
                playerDataNetworkList.RemoveAt(i);
                return;
            }
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = clientId,
            colorId = GetFirstUnusedColorId(),
        });
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "游戏已经开始 不能加入!";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "玩家数量已达上限!";
            return;
        }
        
        connectionApprovalResponse.Approved = true;

        // // 在游戏未开始时 允许加入
        // if (GameManager.Instance.IsWaitingToStart())
        // {
        //     connectionApprovalResponse.Approved = true;
        //     connectionApprovalResponse.CreatePlayerObject = true;
        // }
        // else
        // {
        //     connectionApprovalResponse.Approved = false;
        //     connectionApprovalResponse.Reason = "游戏已经开始 不能加入";
        // }
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
    }
    
    
    private void NetworkManager_Client_OnClientDisconnectCallback(ulong obj)
    {
        string reason = NetworkManager.Singleton.DisconnectReason;
        Debug.Log($"断开连接 原因: {(reason == "" ? "服务器断开连接" : reason)}");
    }

    #endregion


    /// <summary>
    /// 生成厨房对象
    /// </summary>
    /// <param name="kitchenObjectSO">厨房对象SO文件</param>
    /// <param name="kitchenObjectParent">厨房对象父对象</param>
    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        // 客户端请求生成厨房对象 需要进行服务器验证 调用ServerRpc
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }

    /// <summary>
    /// KitchenGameMultiPlayer属于公共资源 客户端尝试调用ServerRpc 需要开放权限
    /// </summary>
    /// <param name="kitchenObjectSOIndex">厨房对象在厨房对象列表中的下标</param>
    /// <param name="kitchenObjectParentNetworkObjectReference">
    /// 在客户端与服务器之间安全传递 NetworkObject 引用
    /// 在网络通信中，直接传递 NetworkObject 实例（如作为 RPC 参数）是无效的（因为对象在客户端和服务器上的内存地址不同）。
    /// NetworkObjectReference 通过存储 NetworkObject 的唯一标识符（NetworkObjectId），实现跨端对同一网络对象的引用。
    /// </param>
    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        // NetworkObjectReference 通过 TryGet 方法解析本地 NetworkObject 实例
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }

    public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO)
    {
        return kitchenObjectListSO.KitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    public KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex)
    {
        return kitchenObjectListSO.KitchenObjectSOList[kitchenObjectSOIndex];
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        // 销毁厨房对象之前 通知所有客户端 引用该对象的父对象 解除对改厨房对象的引用
        ClearKitchenObjectParentClientRpc(kitchenObjectNetworkObjectReference);

        // 销毁厨房对象 (只能在服务器)
        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void ClearKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        
        kitchenObject.ClearKitchenObjectParent();
    }

    public bool IsPlayerIndexConnected(int playerIndex)
    {
        // return NetworkManager.Singleton.ConnectedClientsIds.Count > playerIndex;
        return playerIndex < playerDataNetworkList.Count;
    }

    /// <summary>
    /// 获取玩家编号 (0-3)
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    public int GetPlayerIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
            {
                return i;
            }
        }
        
        return -1;
    }
    
    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (var playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }
        }
        
        return default;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }

    public Color GetPlayerColor(int colorId)
    {
        return playerColorList[colorId];
    }

    public void ChangePlayerColor(int colorId)
    {
        ChangePlayerColorServerRpc(colorId);
    }

    /// <summary>
    /// 客户端请求改变颜色
    /// </summary>
    /// <param name="colorId"></param>
    /// <param name="serverRpcParams">clientId</param>
    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
    {
        if(!IsColorAvailable(colorId)) return;

        int playerDataIndex = GetPlayerIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.colorId = colorId;
        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private bool IsColorAvailable(int colorId)
    {
        // 被使用的颜色不能用
        foreach (var playerData in playerDataNetworkList)
        {
            if (playerData.colorId == colorId)
            {
                // 已经被使用了
                return false;
            }
        }
        return true;
    }

    private int GetFirstUnusedColorId()
    {
        for (int i = 0; i < playerColorList.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }
        return -1;
    }

    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId, "你已被房主踢出!!!");
        
        // 手动触发回调
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }
}