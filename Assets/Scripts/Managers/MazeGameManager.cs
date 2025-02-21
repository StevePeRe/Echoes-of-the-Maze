using Kartograph.Entities;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MazeGameManager : NetworkBehaviour
{
    [SerializeField] LevelGenerator3D generator;
    public static MazeGameManager instance { get; private set; }

    private enum State
    {
        NONE,
        GeneratePreMaze,
        GamePlaying,
        GamePaused,
        GameOver // cuando ningun jugador sobrevive
    }

    // estado default generar el laberinto
    private State state;
    private int seed;
    //private int seed = 0;

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
        state = State.NONE;
        seed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsClient) return;
        
        switch (state) {
            //case State.GeneratePreMaze:
            //    Debug.Log("estado generatePreMaze");
            //    //seed = Random.Range(-2147483643, 2147483643);
            //    //syncDungeonWithClientsServerRpc(seed);
            //    break;
            case State.GamePlaying:
                //Debug.Log("estado GamePlaying");
                // mientras este jugando el contador de daymanager sigue 
                // mientras estes jugando y no se te acabe el dia
                break;
            case State.GamePaused:

                // pausa del juego, muestra HUD de pausa
                break;
            case State.GameOver:
                // muestra la muerte de todos, unabreve animacion de que se meten y matan a todos, luego reinicia
                // todos los dias y cuotas
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void generatePreMazeServerRpc()
    {
        seed = Random.Range(-2147483643, 2147483643);
        generatePreMazeClientRpc(seed);
    }

    [ClientRpc]
    private void generatePreMazeClientRpc(int seed)
    {
        state = State.GamePlaying;
        //Debug.Log("estado: " + state);
        generator.SetSeed(seed);
        generator.Generate(() => { Debug.Log("despues de generar maze"); });
    }

    //public bool getGeneratePreMaze() { return state == State.GeneratePreMaze; } // 
    public bool getGamePlaying() {  return state == State.GamePlaying; }
    public bool getGamePaused() { return state == State.GamePaused; }
    public bool getGameOver() { return state == State.GameOver; }


    // Se modifica para todos, el inicio del dia
    //[ServerRpc(RequireOwnership = false)] public void setGamePlayingServerRpc() { setGamePlayingClientRpc(); }
    //[ClientRpc] private void setGamePlayingClientRpc() { state = State.GamePlaying; }


    // local para cada jugador
    public void setGamePlaying() { state = State.GamePlaying; }
    public void setGamePaused() { state = State.GamePaused; }
    public void setGameOver() { state = State.GameOver; }

}
