using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmItem : MonoBehaviour, ICollectable, IMessageInteraction
{
    [SerializeField] private Transform handPosition;
    [SerializeField] private Transform freePos;

    private Rigidbody rb;

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
        
    }

    public void CollectItem()
    {
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.detectCollisions = false;
        transform.position = handPosition.position;
        transform.SetParent(handPosition);
    }

    public void DropItem()
    {
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.detectCollisions = true;
        transform.SetParent(freePos);
    }

    public void setActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void UseItem(bool use)
    {
        if (use) { Debug.Log("Uso el objeto " + gameObject.name); }
    }

    public string getMessageToShow()
    {
        return "Coger: E";
    }
    public void setDestruction()
    {
        Destroy(gameObject);
    }
}
