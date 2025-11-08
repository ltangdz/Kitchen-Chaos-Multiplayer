using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///脚本：HostDisconnectedUI.cs
///时间：2023.12.10
///功能：
/// </summary>

public class HostDisconnectedUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Time.timeScale = 1;
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

        Hide();
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback; 
    }

    /// <summary>
    /// 处理主机断连
    /// </summary>
    /// <param name="clientId">客户端的id</param>
    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        // if (clientId == NetworkManager.ServerClientId)
        // {
        //     // Server is shutting down
        //     Debug.Log(NetworkManager.Singleton.DisconnectReason);
        //     Show();
        // }
        
        // 判断是否是本地客户端断开（主机断连会触发本地客户端断开）
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            // Debug.Log($"NetworkManager.Singleton.IsConnectedClient: {NetworkManager.Singleton.IsConnectedClient}"); 
            // Debug.Log("与主机断开连接：" + NetworkManager.Singleton.DisconnectReason);
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
