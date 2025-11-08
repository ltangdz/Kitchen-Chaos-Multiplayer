using UnityEngine;

/// <summary>
///脚本：ResetStaticDataManager.cs
///时间：2023.9.20
///功能：重置静态数据
/// </summary>

public class ResetStaticDataManager : MonoBehaviour
{
    private void Awake()
    {
        CuttingCounter.ResetStaticData();
        BaseCounter.ResetStaticData();
        TrashCounter.ResetStaticData();
        Player.ResetStaticData();
    }
}
