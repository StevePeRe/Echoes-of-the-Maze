using TMPro;
using Unity.Netcode;
using UnityEngine;

public class BuyerBehaviour : NetworkBehaviour, IInteractuable, IMessageInteraction
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private TextMeshPro quotaT;

    int targetQuota;
    int ownQuota;
    bool hasReachedQuota; 

    private void Awake()
    {
        targetQuota = 30;
        ownQuota = 0;
        hasReachedQuota = false;
        quotaT.text = "Quota: " + ownQuota + "/" + targetQuota;
    }

    public void Interact()
    {
        if (hasReachedQuota)
        {
            Debug.Log("Has alcanzado la cuota del dia");
            return;
        }

        ICollectable auxCollect = inventory.getItemOnHand();
        if (auxCollect != null)
        {
            //ownQuota.Value += auxCollect.CostObject;
            increaseQuotaServerRpc(auxCollect.CostObject);
            inventory.eraseItemFromInventory(); // borro el item del inventario
            auxCollect.setActive(false); // destruir objeto al entregarlo

            
            if (ownQuota >= targetQuota)
            {
                quotaReachedServerRpc(true); // enviar mensaje al dayamaneger para que se pueda pasar de dia al ya tener toda la cuota
                decreaseQuotaServerRpc(targetQuota); // el sobrante para el siguiente dia
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
        hasReachedQuota = value;
    }

    // IncreaseQuota
    [ServerRpc(RequireOwnership = false)] // Permite que cualquier cliente lo llame
    public void increaseQuotaServerRpc(int amount)
    {
        increaseQuotaClientRpc(amount);
    }
    [ClientRpc]
    private void increaseQuotaClientRpc(int amount)
    {
        ownQuota += amount;
        quotaT.text = "Quota: " + ownQuota + "/" + targetQuota;
        Debug.Log("Llevas " + ownQuota + " cantidad de " + targetQuota);
    }

    // DecreaseQuota - cuota sobrante para el siguiente dia
    [ServerRpc(RequireOwnership = false)] // Permite que cualquier cliente lo llame
    public void decreaseQuotaServerRpc(int amount)
    {
        decreaseQuotaClientRpc(amount);
    }
    [ClientRpc]
    private void decreaseQuotaClientRpc(int amount)
    {
        ownQuota -= amount;
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
        targetQuota = +Random.Range(16, 42);
    }

    public string getMessageToShow()
    {
        return "Entregar: E";
    }
    public bool getHasReachedQuota()
    {
        return hasReachedQuota;
    }
}