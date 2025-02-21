using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEntranceMaze : MonoBehaviour, IInteractuable, IMessageInteraction
{
    [SerializeField] Transform movePlayerMaze;

    public void Interact()
    {
        Player.LocalInstance.setPositionPlayerServerRpc(movePlayerMaze.position);
    }

    public string getMessageToShow()
    {
        return "Abrir: E";
    }
}
