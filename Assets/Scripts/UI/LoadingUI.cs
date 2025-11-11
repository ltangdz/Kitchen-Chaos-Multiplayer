using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI progressText;

    private void Start()
    {
        SetProgress(0);
    }

    /// <summary>
    /// 设置加载进度
    /// </summary>
    /// <param name="progress">0-100的整数</param>
    public void SetProgress(int progress)
    {
        progressText.text = $"{progress}%";
    }
}
