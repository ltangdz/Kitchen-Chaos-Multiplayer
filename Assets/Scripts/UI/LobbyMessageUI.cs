using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
        
        KitchenGameLobby.Instance.OnCreateLobbyStarted += KitchenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed += KitchenGameLobby_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted += KitchenGameLobby_OnJoinStarted;
        KitchenGameLobby.Instance.OnQuickJoinFailed += KitchenGameLobby_OnQuickJoinFailed;
        KitchenGameLobby.Instance.OnJoinFailed += KitchenGameLobby_OnJoinFailed;
    }

    private void OnDestroy()
    {
        KitchenGameLobby.Instance.OnCreateLobbyStarted -= KitchenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed -= KitchenGameLobby_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted -= KitchenGameLobby_OnJoinStarted;
        KitchenGameLobby.Instance.OnQuickJoinFailed -= KitchenGameLobby_OnQuickJoinFailed;
        KitchenGameLobby.Instance.OnJoinFailed -= KitchenGameLobby_OnJoinFailed;
    }

    private void Start()
    {
        Hide();
    }
    
    private void KitchenGameLobby_OnJoinFailed(object sender, EventArgs e)
    {
        ShowWithMsg("Join Failed. Check whether the code is correct?");
    }

    private void KitchenGameLobby_OnQuickJoinFailed(object sender, EventArgs e)
    {
        ShowWithMsg("Quick Join Failed.");
    }

    private void KitchenGameLobby_OnJoinStarted(object sender, EventArgs e)
    {
        ShowWithMsg("Connecting...");
    }
    
    private void KitchenGameLobby_OnCreateLobbyFailed(object sender, EventArgs e)
    {
        ShowWithMsg("Failed to create lobby: Lobby Name cannot be empty.");
    }

    private void KitchenGameLobby_OnCreateLobbyStarted(object sender, EventArgs e)
    {
        ShowWithMsg("Creating lobby...");
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowWithMsg(string message)
    {
        messageText.text = message;
        gameObject.SetActive(true);
    }
}
