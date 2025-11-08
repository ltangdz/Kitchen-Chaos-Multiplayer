using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 持久场景
/// </summary>
public class PersistentSceneManager : MonoBehaviour
{
    private void Start()
    {
        Loader.Load(Loader.Scene.MainMenuScene);
    }
}
