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
    [SerializeField] private Button playButton; 
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            // Loader.Load(Loader.Scene.GameScene);
            Loader.Load(Loader.Scene.LobbyScene);
        });
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        Time.timeScale = 1f;
    }

}
