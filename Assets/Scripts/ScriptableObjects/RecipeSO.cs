using System.Collections.Generic;
using UnityEngine;

/// <summary>
///脚本：RecipeSO.cs
///时间：2023.9.14
///功能：食谱
/// </summary>

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject
{
    public List<KitchenObjectSO> kitchenObjectSOList;
    public string recipeName;
}
