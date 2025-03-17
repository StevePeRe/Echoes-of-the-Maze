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

public class MazeGameLobby : MonoBehaviour
{
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";

    public static MazeGameLobby Instance { get; private set; }

    private Lobby joinedLobby;
    private float hearbeatTimer;
    private float listLobbiesTimer;

    public event EventHandler OnLobbyCreateStarted; 
    public event EventHandler OnLobbyCreateFailed; 
    public event EventHandler OnLobbyJoinStarted; 
    public event EventHandler OnLobbyJoinFailed; 

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); // accedere desde otras escenas
        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions options = new InitializationOptions();
            options.SetProfile(UnityEngine.Random.Range(0,1000).ToString()); // por si se crean builds en la misma pc

            await UnityServices.InitializeAsync(options);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private void Update()
    {
        handleHeartbeat();
        updatePeriodicListLobbies();
    }

    public void updatePeriodicListLobbies()
    {
        if (joinedLobby == null 
            && AuthenticationService.Instance.IsSignedIn 
            && SceneManager.GetActiveScene().name == Loader.Scene.LobbyScene.ToString()) // solo reviso las lobbies estando en la escena de lobbies
        {
            listLobbiesTimer -= Time.deltaTime;
            if (listLobbiesTimer <= 0f)
            {
                float listLobbiesTimerMax = 3f; // tiempo de actualizacion
                listLobbiesTimer = listLobbiesTimerMax;
                listLobbies();
            }
        }

    }

    // Para que las lobbies publicas no caduquen
    private void handleHeartbeat() {
        if (IsLobbyHost())
        {
            hearbeatTimer -= Time.deltaTime;
            if(hearbeatTimer <= 0f)
            {
                float handlerHeartbeatMax = 15f;
                hearbeatTimer = handlerHeartbeatMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async Task<Allocation> allocateRelay()
    {
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4 - 1); // max connections less the host
            return allocation;
        } catch (RelayServiceException e) {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<string> getRelayJoinCode(Allocation allocation)
    {
        try {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (RelayServiceException e) {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<JoinAllocation> joinRelay(string joinCode)
    {
        try {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch (RelayServiceException e) {
            Debug.Log(e);
            return default;
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        OnLobbyCreateStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 4, new CreateLobbyOptions
            {
                IsPrivate = isPrivate
            });

            // Relay function
            Allocation allocation = await allocateRelay();
            string relayJoinCode = await getRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            // Creo al Host
            MazeGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.MainGameScene);

        } catch (LobbyServiceException e)
        {
            OnLobbyCreateFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(e);
        }
        
    }

    public async void QuickJoin()
    {
        OnLobbyJoinStarted?.Invoke(this, EventArgs.Empty);
        try { 
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            // Relay 
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await joinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            // Inicio el cliente
            MazeGameMultiplayer.Instance.StartClient();
        } catch (LobbyServiceException e)
        {
            OnLobbyJoinFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(e);
        }
    }

    public async void joinWithCode(string lobbyCode)
    {
        OnLobbyJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            
            // Relay 
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await joinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            
            // Inicio el cliente
            MazeGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            OnLobbyJoinFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(e);
        }
        
    }

    public async void joinWithId(string lobbyId)
    {
        OnLobbyJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            
            //TODO ESTO PUEDE dar fallo con la recoleccion del valor del codigo de relay si el cliente se conecta muy rapido

            // Relay 
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await joinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            
            // Inicio el cliente
            MazeGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            OnLobbyJoinFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(e);
        }

    }

    public async void deleteLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void leaveLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void KickPlayer(string playerId) // TODO
    {
        if (IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    private async void listLobbies()
    {
        try
        {
            QueryLobbiesOptions queryOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter> {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);

            //Debug.Log(queryResponse.Results.Count);
            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
            {
                lobbyList = queryResponse.Results
            });
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public Lobby getLobby()
    {
        return joinedLobby;
    }

}
