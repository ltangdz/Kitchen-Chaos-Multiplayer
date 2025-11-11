using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class KitchenGameLobby : Singleton<KitchenGameLobby>
{
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    
    private Lobby joinedLobby;
    public Lobby JoinedLobby => joinedLobby;

    private float heartbeatTimer;
    private readonly float heartbeatTimerMax = 15f;
    private float listLobbiesTimer;
    private readonly float listLobbiesTimerMax = 5f;

    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler OnJoinFailed;
    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        
        InitializeUnityAuthentication();
    }

    /// <summary>
    /// 初始化unity服务
    /// </summary>
    private async void InitializeUnityAuthentication()
    {
        try
        {
            if (UnityServices.State == ServicesInitializationState.Initialized) return;
            var initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(Random.Range(0, 10000).ToString());
        
            // 初始化 Unity 服务（需在所有 Lobby 操作前调用）
            await UnityServices.InitializeAsync();

            // 确保玩家已登录（匿名登录，无需账号体系）
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void Update()
    {
        HandleHeartbeat();
        HandlePeriodicListLobbies();
    }

    private void HandlePeriodicListLobbies()
    {
        // 只有当前房间为空 且认证通过 且当前场景为LobbyScene才更新
        if (joinedLobby != null || !AuthenticationService.Instance.IsSignedIn ||
            SceneManager.GetActiveScene().name != Loader.Scene.LobbyScene.ToString()) return;
        
        listLobbiesTimer -= Time.deltaTime;
        if (listLobbiesTimer <= 0f)
        {
            listLobbiesTimer = listLobbiesTimerMax;
            ListLobbies();
        }
    }

    private void HandleHeartbeat()
    {
        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer <= 0f)
            {
                heartbeatTimer = heartbeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };
        
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
            {
                lobbyList = queryResponse.Results,
            });
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(KitchenGameMultiPlayer.MAX_PLAYER_AMOUNT - 1);
            return allocation;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            var relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return default;
        }
    }
    
    /// <summary>
    /// 创建大厅
    /// </summary>
    /// <param name="lobbyName"></param>
    /// <param name="isPrivate"></param>
    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);

            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiPlayer.MAX_PLAYER_AMOUNT,
                new CreateLobbyOptions
                {
                    IsPrivate = isPrivate,
                });
            
            Allocation allocation = await AllocateRelay();
            
            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)},
                }
            });
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            
            KitchenGameMultiPlayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
        }

    }

    public async void QuickJoin()
    {
        try
        {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);
            
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            
            // relay
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            KitchenGameMultiPlayer.Instance.StartClient();
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex.Message);
            
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
    public async void JoinWithId(string lobbyId)
    {
        try
        {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);

            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            
            // relay
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            KitchenGameMultiPlayer.Instance.StartClient();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }
    public async void JoinWithCode(string lobbyCode)
    {
        try
        {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);

            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            // relay
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            KitchenGameMultiPlayer.Instance.StartClient();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void DeleteLobby()
    {
        try
        {
            if (joinedLobby != null)
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                
                joinedLobby = null;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public async void LeaveLobby()
    {
        try
        {
            if (joinedLobby == null) return;
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex.Message);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
    
    public async void KickPlayer(string playerId)
    {
        try
        {
            if (!IsLobbyHost()) return;
            
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
                
            joinedLobby = null;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
}


