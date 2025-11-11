using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button createPrivateButton;
    [SerializeField] private Button createPublicButton;

    private string lobbyName = string.Empty;
    private void Awake()
    {
        lobbyNameInputField.onEndEdit.AddListener((text) =>
        {
            lobbyName = text;
        });
        
        closeButton.onClick.AddListener(Hide);
        
        createPrivateButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.CreateLobby(lobbyName, true);
        });
        
        createPublicButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.CreateLobby(lobbyName, false);
        });
    }

    private void Start()
    {
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
