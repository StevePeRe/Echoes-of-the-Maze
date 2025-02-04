using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;


// necesario Inventario
// Se gestiona el HUD la parte visible del inventario, intercambio del objeto de la mano
public class HUD : MonoBehaviour
{
    [SerializeField] private Inventory inventory; // para los eventos de Inventario

    private Image imageItem;
    [SerializeField] private Transform inventoryItems; // GO del inventario para obtener los SLOTS
    private const int MAX_SELECTION = 2; // ver como conectar con lo demas porque quioero poner una mejora de poner mas espacios

    private ICollectable itemOnHand; // segruamente me toque crearlo en el player, o verificar si sirve para player local
    private int selection;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        itemOnHand = null; // empieza sin item en la mano
        // sirve para player local
        selection = 0;
        imageItem = inventoryItems.GetChild(selection).GetChild(0).GetComponent<Image>();

        // subs
        Inventory.OnAddInventoryItem += Inventory_OnAddInventoryItem;
    }

    private void Inventory_OnAddInventoryItem(object sender, OnInventoryItemEventArgs e)
    {
        if (itemOnHand == null)
        {
            Debug.Log("Agrego item en HUD");
            itemOnHand = e.inventoryItem;
            imageItem.sprite = e.inventoryItem.Image;
            inventory.setItemIntoInventory(e.inventoryItem, selection); // guardarlo en el array en la pos exacta
        }
    }

    private void useItemInventory()
    {
        if (itemOnHand != null)
        {
            itemOnHand.UseItem(GameInput.instance.GetRigthClickMouse());
        }
    }

    // Update is called once per frame
    void Update()
    {
        ////TODO OPTIMIZAR QUE SI NO HA CAMBAIDO QUE SOLO SE HAGA UNA VEZ
        //#region desmarcar item seleccionado y disabled item
        //inventoryItems.GetChild(selection).localScale = new Vector3(1f, 1f, 1f);
        //if (inventory.getInventory()[selection] != null)
        //{
        //    inventory.getInventory()[selection].setActive(false);
        //}
        //#endregion

        //#region seleccion item de inventario
        //Vector2 weelMouse = GameInput.instance..GetMovementWeelMouse();
        //if (weelMouse.y > 0) { selection++; }
        //else if (weelMouse.y < 0) { selection--; }

        //if (selection < 0) selection = MAX_SELECTION;
        //else if (selection > MAX_SELECTION) selection = 0;

        //imageItem = inventoryItems.GetChild(selection).GetChild(0).GetComponent<Image>();

        //// seleccionar item en mano 
        //if (inventory.getInventory()[selection] != null)
        //{
        //    itemOnHand = inventory.getInventory()[selection];
        //    inventory.getInventory()[selection].setActive(true);
        //}
        //else
        //{
        //    itemOnHand = null;
        //}
        //#endregion

        //#region Use item
        //useItemInventory();
        //#endregion

        //#region marcar item seleccionado
        //inventoryItems.GetChild(selection).localScale = new Vector3(1.1f, 1.1f, 1.1f);
        //#endregion
    }

    public ICollectable getItemOnHand() { return itemOnHand; }

    public void resetItemOnHand()
    {
        if (itemOnHand != null)
        {
            itemOnHand = null;
            imageItem.sprite = null;
        }
    }
}
