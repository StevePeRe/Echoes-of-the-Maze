using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameInput;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.Progress;


// necesario Inventario
// Se gestiona el HUD la parte visible del inventario, intercambio del objeto de la mano
// se hace de forma local para cada jugador, cosas relacionadas al HUD no hace falta hacerlas NetworkObejct
public class HUD : MonoBehaviour
{
    [SerializeField] private Inventory inventory; // para los eventos de Inventario
    [SerializeField] private Transform inventoryItems; // GO del inventario para obtener los SLOTS

    private const int MAX_SELECTION = 2; // ver como conectar con lo demas porque quioero poner una mejora de poner mas espacios
    private ICollectable itemOnHand; // segruamente me toque crearlo en el player, o verificar si sirve para player local
    private int selection;
    private Image imageItem;

    // Start is called before the first frame update
    void Start()
    {
        // sirve para player local
        itemOnHand = null; // empieza sin item en la mano
        selection = 0; // pos por defecto
        imageItem = inventoryItems.GetChild(selection).GetChild(0).GetComponent<Image>(); // img de la pos por defecto del inv
        inventoryItems.GetChild(selection).localScale = new Vector3(1.1f, 1.1f, 1.1f); // item destacado por defecto

        GameInput.instance.OnWheelMouseAction += Instance_OnWheelMouseAction;
        GameInput.instance.OnRightClickAction += Instance_OnRightClickAction;
    }

    // para mover la seleccion de objetos - se quita del update para mayor optimizacion
    private void Instance_OnWheelMouseAction(object sender, OnMouseValuesEventArgs wheelMouse)
    {
        #region desmarcar item seleccionado y disabled item
        inventoryItems.GetChild(selection).localScale = new Vector3(1f, 1f, 1f);
        if (inventory.getInventory()[selection] != null)
            {
            //inventory.getInventory()[selection].setActive(false);
                HUDMultiplayer.Instance.setVisibleObjectFromInventory(inventory.getInventory()[selection].getNetworkObject(), false);
            }
        #endregion

        #region seleccion item de inventario
            if (wheelMouse.value > 0) { selection++; }
            else if (wheelMouse.value < 0) { selection--; }

            if (selection < 0) selection = MAX_SELECTION;
            else if (selection > MAX_SELECTION) selection = 0;

            imageItem = inventoryItems.GetChild(selection).GetChild(0).GetComponent<Image>();

            // seleccionar item en mano 
            if (inventory.getInventory()[selection] != null)
            {
                itemOnHand = inventory.getInventory()[selection];
                //inventory.getInventory()[selection].setActive(true);
                HUDMultiplayer.Instance.setVisibleObjectFromInventory(inventory.getInventory()[selection].getNetworkObject(), true);
                imageItem.sprite = inventory.getInventory()[selection].Image;
            }
            else
            {
                itemOnHand = null;
            }
        #endregion

        //Debug.Log("Valor seleccion: " + selection);
        //Debug.Log("Valor itemOnHand: " + inventory.getInventory()[selection]);

        #region marcar item seleccionado
        inventoryItems.GetChild(selection).localScale = new Vector3(1.1f, 1.1f, 1.1f);
        #endregion

    }

    private void Instance_OnRightClickAction(object sender, OnMouseValuesEventArgs e)
    {
        if(itemOnHand == null) return;

        bool use = (e.value > 0) ? true : false;
        #region Use item
        inventory.getInventory()[selection].UseItem(use);
        #endregion
    }

    // Update is called once per frame
    void Update()
    {

    }


    public ICollectable getItemOnHand() { return itemOnHand; }
    public void setItemOnHand(ICollectable item) { itemOnHand = item; }
    
    public Image getItemImage() { return imageItem; }
    public void setItemImage(Sprite sprite) { imageItem.sprite = sprite; }

    public int getSelection() { return selection; }

    public void resetItemOnHand()
    {
        if (itemOnHand != null)
        {
            itemOnHand = null;
            imageItem.sprite = null;
        }
    }
}
