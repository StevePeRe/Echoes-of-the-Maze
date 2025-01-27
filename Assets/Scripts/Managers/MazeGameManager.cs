using Kartograph.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGameManager : MonoBehaviour
{
    [SerializeField] LevelGenerator3D generator;
    public static MazeGameManager instance { get; private set; }

    private enum State
    {
        GeneratePreMaze,
        WaitingToStartDay,
        GamePlaying,
        GamePaused,
        GameOver // cuando ningun jugador sobrevive
    }

    private State state;

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
        state = State.GeneratePreMaze;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state) {
            case State.GeneratePreMaze:
                //generator.Generate(() => { }); // iniciar el laberinto, hay que crearlo de nuevo
                state = State.WaitingToStartDay;
                break;
            case State.WaitingToStartDay:
                // state = gamePlaying
                // cuando toqueun boton que cierra toda la casa
                // situacoin de compra
                break;
            case State.GamePlaying:
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

    public bool getGeneratePreMaze() { return state == State.GeneratePreMaze; }
    public bool getWaitingToStartDay() { return state == State.WaitingToStartDay; }
    public bool getGamePlaying() {  return state == State.GamePlaying; }
    public bool getGamePaused() { return state == State.GamePaused; }
    public bool getGameOver() { return state == State.GameOver; }


    public void setGamePlaying() { state = State.GamePlaying; }
    public void settWaitingToStartDay() { state = State.WaitingToStartDay; }
    public void setGeneratePreMaze() { state = State.GeneratePreMaze; }
    public void setGamePaused() { state = State.GamePaused; }
    public void setGameOver() { state = State.GameOver; }

    //public void setStateGame(State toState) { state = toState; }
}
