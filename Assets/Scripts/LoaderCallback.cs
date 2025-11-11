using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///脚本：LoaderCallback.cs
///时间：2023.9.19
///功能：
/// </summary>

public class LoaderCallback : MonoBehaviour
{
    // private bool isFirstUpdate = true;
    // [SerializeField] private float loadTime = 1f;       // 假装在加载 可以用异步方法代替
    
    [SerializeField] private LoadingUI loadingUI;

    private void Start()
    {
        StartCoroutine(LoadSceneCoroutine());
    }

    // private void Update()
    // {
    //     if (isFirstUpdate)
    //     {
    //         // StartCoroutine(Loading());
    //         StartCoroutine(Loader.LoadSceneCoroutine());
    //         isFirstUpdate = false;
    //     }
    // }

    // private IEnumerator Loading()
    // {
    //     // yield return new WaitForSeconds(loadTime);
    //     yield return Loader.LoadSceneCoroutine();
    // }
    
    private IEnumerator LoadSceneCoroutine()
    {
        var asyncOp = SceneManager.LoadSceneAsync(Loader.targetScene.ToString());

        if (asyncOp == null)
        {
            Debug.LogError("目标场景为空");
            yield break;
        }
        while (!asyncOp.isDone)
        {
            // 进度值范围：0.0f（未开始）~ 0.9f（加载完成），最后 0.1f 用于场景激活
            int progress = (int)(100 * Mathf.Clamp01(asyncOp.progress / 0.9f)); // 映射到 0-100
            loadingUI.SetProgress(progress);

            yield return null; // 等待下一帧，避免阻塞主线程
        }
    }
    
    // private IEnumerator LoadSceneCoroutine()
    // {
    //     var asyncOp = SceneManager.LoadSceneAsync(Loader.targetScene.ToString());
    //     
    //     // // 可选：禁止场景加载完成后自动激活（用于手动控制切换时机）
    //     // asyncOp.allowSceneActivation = false;
    //
    //     // 循环获取加载进度
    //     while (!asyncOp.isDone)
    //     {
    //         // 进度值范围：0.0f（未开始）~ 0.9f（加载完成），最后 0.1f 用于场景激活
    //         float progress = Mathf.Clamp01(asyncOp.progress / 0.9f); // 映射到 0~1
    //         Debug.Log($"加载进度：{progress * 100:F1}%");
    //
    //         // 当加载进度达到 100%（即 asyncOp.progress >= 0.9f）时，手动激活场景
    //         // if (progress >= 1.0f)
    //         // {
    //         //     Debug.Log("加载完成，按任意键继续...");
    //         //     if (Input.anyKeyDown)
    //         //     {
    //         //         asyncOp.allowSceneActivation = true; // 激活场景
    //         //     }
    //         // }
    //
    //         yield return null; // 等待下一帧，避免阻塞主线程
    //     }
    // }
}
