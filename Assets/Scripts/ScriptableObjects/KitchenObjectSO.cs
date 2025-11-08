using UnityEngine;

/// <summary>
///脚本：KitchenObjectSO.cs
///时间：2023.9.11
///功能：
/// </summary>

[CreateAssetMenu()]
public class KitchenObjectSO : ScriptableObject
{
    public Transform prefab;
    public Sprite sprite;
    public string objectName;
}
