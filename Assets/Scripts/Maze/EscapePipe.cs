using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class EscapePipe : NetworkBehaviour, IInteractuable, IMessageInteraction
{
    private bool isCooldown = false;
    private const float cooldownTimeMAX = 30f;
    private float cooldownTime;
    [SerializeField] private TextMeshPro cooldownT;

    private void Awake()
    {
        cooldownT.text = "";
        cooldownTime = cooldownTimeMAX;
    }

    private void Update()
    {
        if (isCooldown)
        {
            cooldownTime -= Time.deltaTime; // si lo pongo dentro de clientrpc el tiempo se vuelve loco ya que depende de cada pc del cliente
            setTimeCooldownServerRpc();
        }
    }
    public void Interact()
    {
        if (isCooldown)
        {
            Debug.Log("Interact is on cooldown.");
            return;
        }

        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.setPositionPlayerServerRpc(new Vector3(-2.2f, 20.12f, -5.55f));
            setCooldownServerRpc();
        }
    }

    // Tiempo del Cooldown
    [ServerRpc(RequireOwnership = false)] // aunque no sea dueño del objeto el cliente puede llamar a este metodo
    private void setTimeCooldownServerRpc()
    {
        setTimeCooldownClientRpc();
    }

    [ClientRpc]
    private void setTimeCooldownClientRpc()
    {
        cooldownT.text = Mathf.Ceil(cooldownTime).ToString();

        if (cooldownTime <= 0)
        {
            cooldownTime = cooldownTimeMAX;
            cooldownT.text = "";
            isCooldown = false;
        }
    }

    // Empezar Cooldown
    [ServerRpc(RequireOwnership = false)] // aunque no sea dueño del objeto el cliente puede llamar a este metodo
    private void setCooldownServerRpc()
    {
        setCooldownClientRpc();
    }

    [ClientRpc]
    private void setCooldownClientRpc()
    {
        isCooldown = true;
    }

    public string getMessageToShow()
    {
        return "Usar: E";
    }
}

