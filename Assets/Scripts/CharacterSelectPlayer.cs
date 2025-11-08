using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyTextGameObject;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Button kickButton;

    private void Awake()
    {
        kickButton.onClick.AddListener(() =>
        {
            PlayerData playerData = KitchenGameMultiPlayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            KitchenGameMultiPlayer.Instance.KickPlayer(playerData.clientId);
        });
    }

    private void Start()
    {
        KitchenGameMultiPlayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiPlayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;
        
        // 房主才能踢人 而且不能踢自己
        bool isActive = NetworkManager.Singleton.IsServer && KitchenGameMultiPlayer.Instance.GetPlayerIndexFromClientId(NetworkManager.Singleton.LocalClientId) != playerIndex;
        kickButton.gameObject.SetActive(isActive);
        
        UpdatePlayer();
    }

    private void OnDestroy()
    {
        KitchenGameMultiPlayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiPlayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged -= CharacterSelectReady_OnReadyChanged;

    }

    private void CharacterSelectReady_OnReadyChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void KitchenGameMultiPlayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (KitchenGameMultiPlayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();

            PlayerData playerData = KitchenGameMultiPlayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            readyTextGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
            
            playerVisual.SetPlayerColor(KitchenGameMultiPlayer.Instance.GetPlayerColor(playerData.colorId));
        }
        else
        {
            Hide();
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
