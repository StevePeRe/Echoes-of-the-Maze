using Kartograph.Entities;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StartDayButton : NetworkBehaviour, IInteractuable, IMessageInteraction
{
    //private bool flagCanEndDay;
    bool aux = false;

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
            return;
        }

        if (!aux)
        {
            aux = true; // pensar que poner en aux para poder volver a pulsar el boton
            MazeGameManager.instance.generatePreMazeServerRpc();
            SpawnerObjectMazeManager.instance.spawnObjectsInMaze();
            //flagCanEndDay = true;
            Debug.Log("Empieza el dia.");
        }

        // solo poder darle cuando ya has cumplid la cuota
        //if (flagCanEndDay)
        //{
        //    MazeGameManager.instance.setGeneratePreMaze(); // reiniciar dia
        //    SpawnerObjectMazeManager.instance.resetListPositions(); // resetaer el disc de transform para colocarlos los objetos

        //    flagCanEndDay = false;
        //}
    }

    public string getMessageToShow()
    {
        return "Activar: E";
    }
}
