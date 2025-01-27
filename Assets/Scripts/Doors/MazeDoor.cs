using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeDoor : MonoBehaviour, IInteractuable, IMessageInteraction
{
    [SerializeField] private Player player;
    [SerializeField] Transform movePlayerMaze;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        player.setPosition(movePlayerMaze);
    }

    public string getMessageToShow()
    {
        return "Abrir: E";
    }
}
