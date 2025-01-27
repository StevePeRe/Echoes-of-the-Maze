using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePipe : MonoBehaviour, IInteractuable, IMessageInteraction
{
    Player auxPlayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        if (auxPlayer != null) 
        {
            auxPlayer.setPosition(new Vector3(-6f, 1.71f, -5.55f));
        }
    }

    public string getMessageToShow()
    {
        return "Usar: E";
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player) 
        {
            auxPlayer = player;
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player)
        {
            auxPlayer = null;
        }
    }

}
