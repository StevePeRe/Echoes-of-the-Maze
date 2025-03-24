using Kartograph.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine.AI;

public class MazeGameManager : NetworkBehaviour
{
    public static MazeGameManager instance { get; private set; }

    [SerializeField] private Transform playerPrefab;
    [SerializeField] LevelGenerator3D generator;

    NavMeshSurface navMeshMaze;
    //[SerializeField] private Section prefab;

    // pruebas
    public Transform lintern;
    //public Transform polola;

    private enum State
    {
        WaitingToStart,
        GamePlaying,
        GameOver // cuando ningun jugador sobrevive
    }

    // estado default generar el laberinto
    private State state;
    private int seed;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Transform spwObj = Instantiate(lintern);
            spwObj.GetComponent<NetworkObject>().Spawn(false);
            spwObj.transform.position = new Vector3(0.02f, 13.23f, 7.62f);

            //Transform spwObj2 = Instantiate(polola);
            //spwObj2.GetComponent<NetworkObject>().Spawn(false);
            //spwObj2.transform.position = new Vector3(-2.02f, 13.23f, 7.62f);

            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += NetworkManager_OnLoadEventCompleted; ; // al unico que le va a cargar la escena de primeras es al host
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback; // cuando el jugador cliente se conecta
        }
    }

    private void NetworkManager_OnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        foreach (ulong clientid in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playertransform = Instantiate(playerPrefab);
            playertransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientid);
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientid)
    {
        Transform playertransform = Instantiate(playerPrefab);
        playertransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientid);
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("MazeGameManager Instance already exist");
        }
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        state = State.WaitingToStart;
        seed = 0;
        navMeshMaze = generator.gameObject.GetComponent<NavMeshSurface>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsClient) return;

        //generator.RegisterNewSection(prefab);

        switch (state) {
            //case State.GeneratePreMaze:
            //    Debug.Log("estado generatePreMaze");
            //    //seed = Random.Range(-2147483643, 2147483643);
            //    //syncDungeonWithClientsServerRpc(seed);
            //    break;
            case State.WaitingToStart:
                //espero hasta que se inicie el dia
                break;
            case State.GamePlaying:
                //Debug.Log("estado GamePlaying");
                // mientras este jugando el contador de daymanager sigue 
                // mientras estes jugando y no se te acabe el dia
                break;
            case State.GameOver:
                // muestra la muerte de todos, unabreve animacion de que se meten y matan a todos, luego reinicia
                // todos los dias y cuotas
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void generatePreMazeServerRpc(ServerRpcParams rpcParams = default)
    {
        MazeGameLobby.Instance.deleteLobby(); // elimino la lobby al empezar el dia y generar el maze
        seed = UnityEngine.Random.Range(-2147483643, 2147483643);
        generatePreMazeClientRpc(seed, rpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void generatePreMazeClientRpc(int seed, ulong clientId, ClientRpcParams clientRpcParams = default)
    {
        state = State.GamePlaying;
        generator.SetSeed(seed);
        generator.Generate(() =>
        {
            Debug.Log("despues de generar maze");
            // el spawn de objetos en el maze solo lo hace el servidor
            if (IsServer)
            {
                SpawnerObjectMazeManager.instance.spawnObjectsInMaze();
                //await BuildNavMeshAsync(navMeshMaze);
                StartCoroutine(buildNavMesh()); //  se hace en el servidor ya que la IA la manejara el servidor y la replicara a los clientes
            }
        });
    }

    private IEnumerator buildNavMesh()
    {
        yield return new WaitForEndOfFrame();

        navMeshMaze.BuildNavMesh(); Debug.Log("navmesh construido"); 
    }

    //private async Task BuildNavMeshAsync(NavMeshSurface surface)
    //{
    //    var tcs = new TaskCompletionSource<bool>();
    //    NavMeshBuilder.UpdateNavMeshDataAsync(surface.navMeshData, surface.GetBuildSettings(), surface.GetSources(), surface.GetWorldBounds(), (data) =>
    //    {
    //        surface.navMeshData = data;
    //        tcs.SetResult(true);
    //    });
    //    await tcs.Task;
    //}

    //public Task UpdateNavMeshAsync(this NavMeshSurface surface, NavMeshData data)
    //{
    //    var tcs = new TaskCompletionSource<bool>();
    //    surface.navMeshData = data;
    //    surface.navMeshDataInstance = NavMesh.AddNavMeshData(data);
    //    surface.BuildNavMeshAsync(data, tcs.SetResult);
    //    return tcs.Task;
    //}

    //public bool getGeneratePreMaze() { return state == State.GeneratePreMaze; } // 
    public bool getWaitingToStart() { return state == State.WaitingToStart; }
    public bool getGamePlaying() {  return state == State.GamePlaying; }
    public bool getGameOver() { return state == State.GameOver; }


    // Se modifica para todos, el inicio del dia
    //[ServerRpc(RequireOwnership = false)] public void setGamePlayingServerRpc() { setGamePlayingClientRpc(); }
    //[ClientRpc] private void setGamePlayingClientRpc() { state = State.GamePlaying; }


    // local para cada jugador
    public void setWaitingToStart() { state = State.WaitingToStart; }
    public void setGamePlaying() { state = State.GamePlaying; }
    public void setGameOver() { state = State.GameOver; }

}
