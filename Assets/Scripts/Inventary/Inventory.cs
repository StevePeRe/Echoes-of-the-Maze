using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using static UnityEditor.Timeline.Actions.MenuPriority;

// el inventario se puede eliminar y no afectaria al player
// necesario HUD
// Gestiona el anadir y borrar objetos del inventario del jugador
public class Inventory : MonoBehaviour
{
    [SerializeField] private HUD hud;

    private ICollectable[] inventPlayer; // inventario local de cada jugador
    private int MAX_SIZE_iNVENTORY = 3;

    // Start is called before the first frame update
    void Start()
    {
        inventPlayer = new ICollectable[MAX_SIZE_iNVENTORY];

        // Player - desacoplado del player
        Player.OnAddItem += Player_OnAddItem;
        Player.OnDropItem += Player_OnDropItem;
    }

    // !!!! en este caso elegir player o Player.LocalInstance da igual, ya que es local de todas formas, si este evento es llamase en el esrvidor si importaria
    // ya que el player serie el que haya invocado el evento y Player.LocalInstance seria siempre el local que tengo ese archivo, y se ejecutaria para el local solo, no para el que lo invoco
    private void Player_OnAddItem(object sender, OnInventoryItemEventArgs e)
    {
        Player player = sender as Player; // necesario para saber la pos del player que invoca y va a ser el padre del objeto
        if (hud.getItemOnHand() == null)
        {
            e.inventoryItem.CollectItem(player.getNetworkObject()); /// le paso el tranform de la mano del player que ha recogido el objeto
            hud.setItemOnHand(e.inventoryItem); // establezco valores del HUD
            hud.setItemImage(e.inventoryItem.Image);
            inventPlayer[hud.getSelection()] = e.inventoryItem; // meto el item en el inventario del player
        }
    }

    private void Player_OnDropItem(object sender, EventArgs e)
    {
        // no hace falta saber el player que lo invoca ya que la pos al dropear el obj sera null
        if (hud.getItemOnHand() != null)
        {
            inventPlayer[hud.getSelection()].DropItem(); // esto no se si funcionara para cada uno
            eraseItemFromInventory(); // le paso el juagdor que ha invocado el metodo
        }
    }

    public void eraseItemFromInventory()
    {
        for (int i = 0; i < inventPlayer.Length; i++)
        {
            if (inventPlayer[i] == hud.getItemOnHand())
            {
                inventPlayer[i] = null;
            }
        }
        hud.resetItemOnHand(); // borro del objeto todo en el HUD
    }

    public ICollectable getItemOnHand() { return hud.getItemOnHand(); } // opr si se quiere acceder directamente desde el inventario el objeto que tiene ne la mano

    public ICollectable[] getInventory() { return inventPlayer; } 

}

public class OnInventoryItemEventArgs : EventArgs
{
    public ICollectable inventoryItem;
}
