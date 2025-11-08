using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            KitchenGameMultiPlayer.Instance.ChangePlayerColor(colorId);
        });
    }

    private void Start()
    {
        KitchenGameMultiPlayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiPlayer_OnPlayerDataNetworkListChanged;
        image.color = KitchenGameMultiPlayer.Instance.GetPlayerColor(colorId);
        UpdateSelected();
    }

    private void OnDestroy()
    {
        KitchenGameMultiPlayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiPlayer_OnPlayerDataNetworkListChanged;
    }

    private void KitchenGameMultiPlayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdateSelected();
    }

    private void UpdateSelected()
    {
        if (KitchenGameMultiPlayer.Instance.GetPlayerData().colorId == colorId)
        {
            selectedGameObject.SetActive(true);
        }
        else
        {
            selectedGameObject.SetActive(false);
        }
    }
    
}
