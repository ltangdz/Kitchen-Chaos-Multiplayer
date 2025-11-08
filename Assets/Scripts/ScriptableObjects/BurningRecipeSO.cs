using UnityEngine;

/// <summary>
///脚本：BurningRecipeSO.cs
///时间：2023.9.13
///功能：切割食谱
/// </summary>

[CreateAssetMenu()]
public class BurningRecipeSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float burningTimeMax;
}