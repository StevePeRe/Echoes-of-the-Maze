using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BuyerBehaviour : NetworkBehaviour, IInteractuable, IMessageInteraction
{
    [SerializeField] private Inventory inventory;

    // si son NV solo las puede moficar el servidor por seguridad -> necesario serverRPC
    NetworkVariable<int> targetQuota = new NetworkVariable<int>(30, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    NetworkVariable<int> ownQuota = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    NetworkVariable<bool> hasReachedQuota = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); // variable multijugador

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Interact()
    {
        if (hasReachedQuota.Value)
        {
            Debug.Log("Has alcanzado la cuota del dia");
            return;
        }

        //Debug.Log("entrewgo objeto a vendedor");
        //BuyerBehaviourServerRpc();
        ICollectable auxCollect = inventory.getItemOnHand();
        if (auxCollect != null)
        {
            //ownQuota.Value += auxCollect.CostObject;
            increaseQuotaServerRpc(auxCollect.CostObject);
            inventory.eraseItemFromInventory(); // borro el item del inventario
            auxCollect.setActive(false); // destruir objeto al entregarlo

            Debug.Log("Llevas " + ownQuota.Value + " cantidad de " + targetQuota.Value);
            if (ownQuota.Value >= targetQuota.Value)
            {
                quotaReachedServerRpc(true); // enviar mensaje al dayamaneger para que se pueda pasar de dia al ya tener toda la cuota
                decreaseQuotaServerRpc(targetQuota.Value);
                //ownQuota.Value -= targetQuota.Value; // el sobrante para el siguiente dia
                increaseTargetQuotaServerRpc(); // aumentarla para cuando se pase de dia
            }
        }
        else
        {
            Debug.Log("Tienes que tener un item en la mano");
        }
    }

    // QuotaReached
    [ServerRpc(RequireOwnership = false)] // Permite que cualquier cliente lo llame
    public void quotaReachedServerRpc(bool value)
    {
        quotaReachedClientRpc(value);
    }
    [ClientRpc]
    private void quotaReachedClientRpc(bool value)
    {
        hasReachedQuota.Value = value;
    }

    // IncreaseQuota
    [ServerRpc(RequireOwnership = false)] // Permite que cualquier cliente lo llame
    public void increaseQuotaServerRpc(int amount)
    {
        Debug.Log("Value: " + amount);
        ownQuota.Value += amount;
        Debug.Log("ownQuota: " + ownQuota.Value);
        //increaseQuotaClientRpc(amount);
    }
    [ClientRpc]
    private void increaseQuotaClientRpc(int amount)
    {
        Debug.Log("Value: " + amount);
        ownQuota.Value += amount;
    }

    // DecreaseQuota
    [ServerRpc(RequireOwnership = false)] // Permite que cualquier cliente lo llame
    public void decreaseQuotaServerRpc(int amount)
    {
        decreaseQuotaClientRpc(amount);
    }
    [ClientRpc]
    private void decreaseQuotaClientRpc(int amount)
    {
        ownQuota.Value -= amount;
    }

    // TargetQuota
    [ServerRpc(RequireOwnership = false)]
    public void increaseTargetQuotaServerRpc()
    {
        increaseTargetQuotaClientRpc();
    }
    [ClientRpc]
    private void increaseTargetQuotaClientRpc()
    {
        targetQuota.Value = +Random.Range(16, 42);
    }

    public string getMessageToShow()
    {
        return "Entregar: E";
    }
    public bool getHasReachedQuota()
    {
        return hasReachedQuota.Value;
    }
    //private void increaseTargetQuota()
    //{
    //    targetQuota.Value = +Random.Range(16, 42);
    //}
}