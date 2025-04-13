using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class StartDayButton : NetworkBehaviour, IInteractuable, IMessageInteraction
{
    public static event EventHandler OnStartDay;

    public static void ResetStaticData()
    {
        OnStartDay = null;
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
            MazeGameManager.instance.generatePreMazeServerRpc();
            sendEventsServerRpc();
        } else
        {
            //send message day its alkready started
        }
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
