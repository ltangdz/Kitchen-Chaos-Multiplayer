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
    private bool isFirstUpdate = true;
    // [SerializeField] private float loadTime = 1f;       // 假装在加载 可以用异步方法代替

    private void Update()
    {
        if (isFirstUpdate)
        {
            // StartCoroutine(Loading());
            StartCoroutine(Loader.LoadSceneCoroutine());
            isFirstUpdate = false;
        }
    }

    // private IEnumerator Loading()
    // {
    //     // yield return new WaitForSeconds(loadTime);
    //     yield return Loader.LoadSceneCoroutine();
    // }
}
