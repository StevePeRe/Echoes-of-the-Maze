using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lintern : MonoBehaviour, ICollectable, IMessageInteraction
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

    public void CollectItem(Transform transf)
    {
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.detectCollisions = false;
        followTransform.SetTargetTransform(transf);
        //transform.position = handPosition.position;
        //transform.SetParent(handPosition);
    }

    public void DropItem()
    {
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.detectCollisions = true;
        transform.SetParent(null);
    }

    public void setActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void UseItem(bool use)
    {
        if (use) { Debug.Log("Uso el objeto linterna"); }
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
