using Kartograph.Entities;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StartDayButton : NetworkBehaviour, IInteractuable, IMessageInteraction
{
    //private bool flagCanEndDay;

    // Start is called before the first frame update
    void Start()
    {
        //flagCanEndDay = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //TODO mejorar logica
    public void Interact()
    {
        if (!IsClient) // cualquiera puede empezar el dia
        {
            //Debug.Log("No eres el Host start daybutton.");
            return;
        }
        // solo poder darle cuando ya has cumplid la cuota
        //if (flagCanEndDay)
        //{
        //    MazeGameManager.instance.setGeneratePreMaze(); // reiniciar dia
        //    SpawnerObjectMazeManager.instance.resetListPositions(); // resetaer el disc de transform para colocarlos los objetos

        //    flagCanEndDay = false;
        //}

        if (!MazeGameManager.instance.getGamePlaying())
        {
            MazeGameManager.instance.setGamePlaying(); // iniciar dia
            //SpawnerObjectMazeManager.instance.spawnObjectsInMaze(); // spwn objetos aleatorios
            //flagCanEndDay = true;
            Debug.Log("Empieza el dia.");
        }
    }

    public string getMessageToShow()
    {
        return "Activar: E";
    }
}
