using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class StartDayButton : NetworkBehaviour, IInteractuable, IMessageInteraction
{
    public static event EventHandler OnStartDay;

    //private bool flagCanEndDay;
    bool aux = false;

    public static void ResetStaticData()
    {
        OnStartDay = null;
    }

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

        if (MazeGameManager.instance.getWaitingToStart()) // cuando se acabe el dia vuelve a este estado
        {
            aux = true; // pensar que poner en aux para poder volver a pulsar el boton
            MazeGameManager.instance.generatePreMazeServerRpc();
            SpawnerObjectMazeManager.instance.spawnObjectsInMaze();
            sendEventsServerRpc();
            MazeGameLobby.Instance.deleteLobby(); // borro lobby al empezar la partida
            //flagCanEndDay = true;
            Debug.Log("Empieza el dia.");
        } else
        {
            //send message day its alkready started
        }

        // solo poder darle cuando ya has cumplid la cuota
        //if (flagCanEndDay)
        //{
        //    MazeGameManager.instance.setGeneratePreMaze(); // reiniciar dia
        //    SpawnerObjectMazeManager.instance.resetListPositions(); // resetaer el disc de transform para colocarlos los objetos

        //    flagCanEndDay = false;
        //}
    }

    [ServerRpc(RequireOwnership = false)]
    public void sendEventsServerRpc()
    {
        sendEventsClientRpc();
    }

    [ClientRpc]
    private void sendEventsClientRpc()
    {
        OnStartDay?.Invoke(this, EventArgs.Empty);
    }

    public string getMessageToShow()
    {
        return "Activar: E";
    }
}
