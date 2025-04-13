using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MazeGameMultiplayer : NetworkBehaviour
{
    public static MazeGameMultiplayer Instance { get; private set; }

    public static event EventHandler OnDisconnectHostAction;

    public static void ResetStaticData()
    {
        OnDisconnectHostAction = null;
    }

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject); // no se destruye al pasar de escenas
    }

    void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        //hide();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        //if (IsServer)
        //{
        //    Debug.Log("trato de desconectarme: " + clientId);
        //    MazeGameLobby.Instance.leaveLobby(); // dejar la lobby
        //    NetworkManager.Singleton.Shutdown();
        //    Loader.Load(Loader.Scene.MainMenuScene);
        //}
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("trato de desconectarme: " + clientId);
            OnDisconnectHostAction?.Invoke(this, EventArgs.Empty);
            MazeGameLobby.Instance.leaveLobby(); // dejar la lobby
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        }
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    // Disconnection
    //[ServerRpc(RequireOwnership = false)]
    //public void hostDisconnectServerRpc()
    //{
    //    Debug.Log("me ");
    //    hostDisconnectClientRpc();
    //}

    [ServerRpc(RequireOwnership = false)]
    public void hostDisconnectServerRpc()
    {
        hostDisconnectClientRpc();
    }

    [ClientRpc]
    private void hostDisconnectClientRpc()
    {
    }

    //[ClientRpc]
    //private void hostDisconnectClientRpc()
    //{
    //    Debug.Log("desconectar el cliente");
    //    NetworkManager.Singleton.Shutdown();
    //    MazeGameLobby.Instance.leaveLobby(); // dejar la lobby
    //    Loader.Load(Loader.Scene.MainMenuScene);
    //}
}
