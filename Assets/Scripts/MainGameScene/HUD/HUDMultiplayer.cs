using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HUDMultiplayer : NetworkBehaviour
{
    public static HUDMultiplayer Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Inventory
    public void setVisibleObjectFromInventory(NetworkObjectReference netObj, bool visibility)
    {
        setVisibleObjectFromInventoryServerRpc(netObj, visibility);
    }

    [ServerRpc(RequireOwnership = false)] // aunque no sea dueño del objeto el cliente puede llamar a este metodo
    private void setVisibleObjectFromInventoryServerRpc(NetworkObjectReference netObj, bool visibility)
    {
        setVisibleObjectFromInventoryClientRpc(netObj, visibility);
    }

    [ClientRpc]
    private void setVisibleObjectFromInventoryClientRpc(NetworkObjectReference netObj, bool visibility)
    {
        netObj.TryGet(out NetworkObject inventoryItem);
        inventoryItem.gameObject.SetActive(visibility);
    }
}
