using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Lintern : NetworkBehaviour, ICollectable, IMessageInteraction
{
    [SerializeField] private Light lightFlashlight;
    private bool useItem;
    private Rigidbody rb;
    private FollowTransform followTransform;

    [SerializeField] private string _name;
    public string Name
    {
        get
        {
            return _name;
        }
    }

    [SerializeField] private Sprite _image;
    public Sprite Image
    {
        get
        {
            return _image;
        }
    }

    [SerializeField] private int _costObject = 2;
    public int CostObject
    {
        get
        {
            return _costObject;
        }
    }

    [SerializeField] private int _weigthObject;
    public int WeigthObject
    {
        get
        {
            return _weigthObject;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        useItem = false;
        lightFlashlight.enabled = false;
        rb = GetComponent<Rigidbody>();
        followTransform = GetComponent<FollowTransform>();
    }

    public void CollectItem(NetworkObject netObj) // le podria pasar por partes el transform, ya que no puedo serializar un tipo compuesto, pero si nativo como position o rotation
    {
        CollectItemServerRpc(netObj);
    }

    [ServerRpc(RequireOwnership = false)] // aunque no sea dueño del objeto el cliente puede llamar a este metodo
    private void CollectItemServerRpc(NetworkObjectReference netObj)
    {
        CollectItemClientRpc(netObj);
    }

    [ClientRpc]
    private void CollectItemClientRpc(NetworkObjectReference netObj)
    {
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.detectCollisions = false;

        netObj.TryGet(out NetworkObject playerNetworkObject); // obetngo la referencia del objeto de la red 
        followTransform.SetTargetTransform(playerNetworkObject.gameObject.transform.GetChild(3) /*la pos de la mano*/); // y lo puedo usar para acceder a sus componentes
    }

    public void DropItem()
    {
        DropItemServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] // aunque no sea dueño del objeto el cliente puede llamar a este metodo
    private void DropItemServerRpc()
    {
        DropItemClientRpc();
    }

    [ClientRpc] // lo hace para el que lo llamo pero en el servidor para que los demas lo vean replicado
    private void DropItemClientRpc()
    {
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.detectCollisions = true;
        followTransform.SetTargetTransform(null); // al suelo otra vez
    }

    public void setActive(bool active)
    {
        setActiveServerRpc(active);
    }

    [ServerRpc(RequireOwnership = false)] // aunque no sea dueño del objeto el cliente puede llamar a este metodo
    private void setActiveServerRpc(bool active)
    {
        setActiveClientRpc(active);
    }

    [ClientRpc]
    private void setActiveClientRpc(bool active)
    {
        gameObject.SetActive(active);
    }

    public void UseItem()
    {
        useItemServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] // aunque no sea dueño del objeto el cliente puede llamar a este metodo
    private void useItemServerRpc()
    {
        useItemClientRpc();
    }

    [ClientRpc]
    private void useItemClientRpc()
    {
        useItem = !useItem;
        Debug.Log("Valor useItem: " + useItem);
        if (useItem)
        {
            Debug.Log("enciendo");
            lightFlashlight.enabled = true;
        }
        else {
            Debug.Log("apago");
            lightFlashlight.enabled = false; 
        }
        
    }

    public string getMessageToShow()
    {
        return "Coger: E";
    }

    public void setDestruction()
    {
        setDestructionServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] // aunque no sea dueño del objeto el cliente puede llamar a este metodo
    private void setDestructionServerRpc()
    {
        setDestructionClientRpc();
    }

    [ClientRpc]
    private void setDestructionClientRpc()
    {
        Destroy(gameObject);
    }

    public NetworkObject getNetworkObject()
    {
        return NetworkObject;
    }
}
