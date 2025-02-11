using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Lintern : NetworkBehaviour, ICollectable, IMessageInteraction
{
    //[SerializeField] private Transform handPosition;
    //[SerializeField] private Transform freePos;
    private BoxCollider boxCollider;
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
    void Start()
    {
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

    [ClientRpc]
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

    public void UseItem(bool use)
    {
        useItemServerRpc(use);
    }

    [ServerRpc(RequireOwnership = false)] // aunque no sea dueño del objeto el cliente puede llamar a este metodo
    private void useItemServerRpc(bool use)
    {
        useItemClientRpc(use);
    }

    [ClientRpc]
    private void useItemClientRpc(bool use)
    {
        if (use) { Debug.Log("Uso el objeto " + gameObject.name); }
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
