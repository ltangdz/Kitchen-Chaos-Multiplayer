using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///脚本：Loader.cs
///时间：2023.9.19
///功能：
/// </summary>

public static class Loader
{
    public enum Scene
    {
        MainMenuScene,
        GameScene,
        LoadingScene,
        LobbyScene,
        CharacterSelectScene,
    }

    public static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }
    
    /// <summary>
    /// 多人模式的场景加载
    /// </summary>
    /// <param name="targetScene"></param>
    public static void LoadNetwork(Scene targetScene)
    {
        Loader.targetScene = targetScene;

        // SceneManager.LoadScene(Scene.LoadingScene.ToString());

        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
        // SceneManager.LoadSceneAsync(targetScene.ToString());
    }
    
}
