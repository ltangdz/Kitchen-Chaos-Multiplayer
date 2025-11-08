using UnityEngine;

/// <summary>
///脚本：FryingRecipeSO.cs
///时间：2023.9.12
///功能：切割食谱
/// </summary>

[CreateAssetMenu()]
public class FryingRecipeSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float fryingTimeMax;
}