using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
///脚本：MainMenuUI.cs
///时间：2023.9.19
///功能：
/// </summary>

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button multiPlayerButton; 
    [SerializeField] private Button singlePlayButton; 
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        multiPlayerButton.onClick.AddListener(() =>
        {
            // KitchenGameMultiPlayer.isMultiPlayer = true;
            Loader.Load(Loader.Scene.LobbyScene);
        });

        // 需要更改传输层 目前有问题
        // singlePlayButton.onClick.AddListener(() =>
        // {
        //     KitchenGameMultiPlayer.isMultiPlayer = false;
        //     Loader.Load(Loader.Scene.LobbyScene);
        // });
        
        quitButton.onClick.AddListener(Application.Quit);

        Time.timeScale = 1f;
    }

}
