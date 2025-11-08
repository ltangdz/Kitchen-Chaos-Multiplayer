using UnityEngine;
using UnityEngine.UI;

/// <summary>
///脚本：PlateIconsSingleUI.cs
///时间：2023.9.14
///功能：
/// </summary>

public class PlateIconsSingleUI : MonoBehaviour
{
    [SerializeField] private Image image;

    public void SetKitchenObjectSO(KitchenObjectSO kitchenObjectSO)
    {
        image.sprite = kitchenObjectSO.sprite;
    }
}
