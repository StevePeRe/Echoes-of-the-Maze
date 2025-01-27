using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyerBehaviour : MonoBehaviour, IMessageInteraction
{
    [SerializeField] private Player player;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Inventory inventory;

    // crear un evento que le avise desde el daymanager cuando cambio de dia
    private int targetQuota;
    private int ownQuota;
    private bool hasReachedQuota;

    // Start is called before the first frame update
    void Start()
    {
        // Game Input
        gameInput.OnInteractionAction += GameInput_OnInteractionAction; // E
        targetQuota = 30;
    }
    private void GameInput_OnInteractionAction(object sender, System.EventArgs e)
    {
        if (hasReachedQuota)
        {
            Debug.Log("Has alcanzado la cuota del dia");
            return;
        }

        if (player.getRaycastPlayer() == this)
        {
            ICollectable auxCollect = inventory.getItemOnHand();
            if (auxCollect != null)
            {
                ownQuota += auxCollect.CostObject;
                inventory.eraseItemFromInventory(); // borro el item del inventario
                auxCollect.setActive(false); // destruir objeto al entregarlo

                Debug.Log("Llevas " + ownQuota + " cantidad de " + targetQuota);
                if (ownQuota >= targetQuota)
                {
                    hasReachedQuota = true; // enviar mensaje al dayamaneger para que se pueda pasar de dia al ya tener toda la cuota
                    ownQuota -= targetQuota; // el sobrante para el siguiente dia

                    //increaseTargetQuota(); // aumentarla para cuando se pase de dia si o si
                }
            }
            else
            {
                Debug.Log("Tienes que tener un item en la mano");
            }

        }
    }
    public string getMessageToShow()
    {
        return "Entregar:E";
    }
    public bool getHasReachedQuota()
    {
        return hasReachedQuota;
    }
    private void increaseTargetQuota()
    {
        targetQuota=+Random.Range(16, 42);
    }
}