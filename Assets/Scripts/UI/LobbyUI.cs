using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using Lobby = Unity.Services.Lobbies.Models.Lobby;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    
    [SerializeField] private LobbyCreateUI lobbyCreateUI;

    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;

    private string lobbyCode = string.Empty;
    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.LeaveLobby();
            Loader.Load(Loader.Scene.MainMenuScene);
        });        
        
        createLobbyButton.onClick.AddListener(() =>
        {
            // KitchenGameLobby.Instance.CreateLobby("Name", false);
            lobbyCreateUI.Show();
        });
        
        quickJoinButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.QuickJoin();
        });
        
        joinCodeButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.JoinWithCode(lobbyCode);
        });
        
        joinCodeInputField.onEndEdit.AddListener((text) =>
        {
            lobbyCode = text;
        });
        
        lobbyTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        playerNameInputField.text = KitchenGameMultiPlayer.Instance.GetPlayerName();
        playerNameInputField.onEndEdit.AddListener((text) =>
        {
            KitchenGameMultiPlayer.Instance.SetPlayerName(text);
        });
        
        KitchenGameLobby.Instance.OnLobbyListChanged += KitchenGameLobby_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }

    private void OnDestroy()
    {
        KitchenGameLobby.Instance.OnLobbyListChanged -= KitchenGameLobby_OnLobbyListChanged;
    }

    private void KitchenGameLobby_OnLobbyListChanged(object sender, KitchenGameLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        // clean up
        foreach (Transform child in lobbyContainer)
        {
            if(child == lobbyTemplate) continue;
            Destroy(child.gameObject);
        }
        
        // new
        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer, false);
            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
            lobbyTransform.gameObject.SetActive(true);
        }
    }
}
