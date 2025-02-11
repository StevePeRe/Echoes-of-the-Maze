using System.Collections;
using System.Collections.Generic;
using TMPro;
//using Unity.Netcode;
using UnityEngine;

public class MessageInteraction : MonoBehaviour
{
    private TextMeshProUGUI messageInt;
    private bool wObject;

    // Start is called before the first frame update
    private void Awake()
    {
        messageInt = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.LocalInstance == null) return;
        messageInteractionsPlayer();
    }

    private void messageInteractionsPlayer()
    {
        if (!wObject && messageInt.text != "") messageInt.text = ""; // reset text
        wObject = false;

        Collider hitPlayer = Player.LocalInstance.getRaycastPlayer();

        if (hitPlayer != null && hitPlayer.GetComponent<IMessageInteraction>() is IMessageInteraction message)
        {
            wObject = true;
            messageInt.text = message.getMessageToShow();
        }
    }

}
