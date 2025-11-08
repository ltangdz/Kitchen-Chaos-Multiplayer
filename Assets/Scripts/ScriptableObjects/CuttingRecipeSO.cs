using UnityEngine;

/// <summary>
///脚本：CuttingRecipeSO.cs
///时间：2023.9.12
///功能：切割食谱
/// </summary>

[CreateAssetMenu()]
public class CuttingRecipeSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public int cuttingProgressMax;
}