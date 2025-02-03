using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Timeline.Actions.MenuPriority;

// el inventario se puede eliminar y no afectaria al player
// necesario HUD
// Gestiona el anadir y borrar objetos del inventario del jugador
public class Inventory : MonoBehaviour
{
    //[SerializeField] private Player player;
    [SerializeField] private HUD hud;

    //private int MAX_SIZE_iNVENTORY = 3;
    //private ICollectable[] inventPlayer;

    public static event EventHandler<OnInventoryItemEventArgs> OnAddInventoryItem; // para el HUD
    public static void ResetStaticData()
    {
        OnAddInventoryItem = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        //inventPlayer = new ICollectable[MAX_SIZE_iNVENTORY];
        // Player - desacoplado del player
        Player.OnAddItem += Player_OnAddItem;
        Player.OnDropItem += Player_OnDropItem;
    }

    private void Player_OnAddItem(object sender, OnInventoryItemEventArgs e)
    {
        Debug.Log("Add item");
        if (hud.getItemOnHand() == null)
        {
            e.inventoryItem.CollectItem(); /// hago que desaparezca el item
            OnAddInventoryItem?.Invoke(sender, e); // a HUD 
        }
    }

    private void Player_OnDropItem(object sender, EventArgs e)
    {
        Debug.Log("Drop item");
        //if (hud.getItemOnHand() != null)
        //{
        //    hud.getItemOnHand().DropItem();
        //    eraseItemFromInventory();
        //}
    }

    //public void eraseItemFromInventory()
    //{
    //    for (int i = 0; i < inventPlayer.Length; i++)
    //    {
    //        if (inventPlayer[i] == hud.getItemOnHand())
    //        {
    //            inventPlayer[i] = null;
    //        }
    //    }
    //    hud.resetItemOnHand();
    //}

    //public ICollectable getItemOnHand() { return hud.getItemOnHand(); } // opr si se quiere acceder directamente desde el inventario el objeto que tiene ne la mano

    //public ICollectable[] getInventory() { return inventPlayer; }

    // lo cambia desde hud
    public void setItemIntoInventory(ICollectable inventItem, int index)
    {
        //hayq ue verificar que jugador es
        Player.LocalInstance.getInventory()[index] = inventItem;
        //inventPlayer[index] = inventItem;
    }

    // Update is called once per frame
    void Update()
    {
        // los que no estan en la mano que esten desactivados
    }


}

public class OnInventoryItemEventArgs : EventArgs
{
    public ICollectable inventoryItem;
}
