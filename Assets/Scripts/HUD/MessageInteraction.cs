using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class MessageInteraction : NetworkBehaviour
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
        messageInteractionsPlayer();
    }

    private void messageInteractionsPlayer()
    {
        if (!IsOwner) return;

        if (!wObject && messageInt.text != "") messageInt.text = ""; // reset text
        wObject = false;

        if (Player.LocalInstance.getRaycastPlayer() is IMessageInteraction messInteract)
        {
            wObject = true;
            messageInt.text = messInteract.getMessageToShow();
        }
    }

}
