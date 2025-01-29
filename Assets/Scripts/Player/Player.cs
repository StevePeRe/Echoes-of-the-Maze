using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Player : NetworkBehaviour
{
    [SerializeField] private Transform orientation;
    private Transform camPosition;
    private GameInput gameInput;
    private float speedPlayer = 4.8f;
    private Vector3 moveDirection;

    private CharacterController cController;
    private Vector3 velocity;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity = 19.62f;

    // jump
    [SerializeField] private float smoothCrouch;
    private bool crouch;
    private float targetLocalScaleY;

    // Inventory
    public event EventHandler<OnInventoryItemEventArgs> OnAddItem;
    public event EventHandler OnDropItem;

    // Raycast
    //[SerializeField] private TextMeshProUGUI messageInterc;
    //private bool wObject;

    public static event EventHandler OnAnyPlayerSpawned; // static ya que queremos que no perternezca a una clase en concreto sino en general

    public static Player LocalInstance { get; private set; }

    private void Awake()
    {
        gameInput = GameInput.instance;
        cController = GetComponent<CharacterController>(); // busca dentro de donde este el script
        camPosition = this.transform; //  de moemnto para el error xd
    }

    // cuando se inicie la conexion se lanzara este metodo
    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            LocalInstance = this;
        }

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Game Input
        gameInput.OnInteractionAction += GameInput_OnInteractionAction; // E
        gameInput.OnDropAction += GameInput_OnDropAction; // G
    }

    // TODO Cambiar en un futuro refactorizar en otro docuemnto solo interacciones
    private void GameInput_OnDropAction(object sender, EventArgs e)
    {
        OnDropItem?.Invoke(this, EventArgs.Empty);
    }

    // Interfaces que intertactuen con la E
    private void GameInput_OnInteractionAction(object sender, EventArgs e)
    {
        // mas adelante ver si poner esto en el inventario para mejor organizacion
        var monoB = getRaycastPlayer();

        if (monoB is ICollectable collectable)
        {
            OnAddItem?.Invoke(this, new OnInventoryItemEventArgs
            {
                inventoryItem = collectable
            });
        }

        if (monoB is IInteractuable interectuable)
        {
            interectuable.Interact();
        }
        
    }

    // Dibujado de raycast de la cam
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(camPosition.position, camPosition.position + camPosition.forward * 3f);
    }

    // Update is called once per frame
    private void Update()
    {
        //// raycast Player
        //messageInteractionsPlayer();
        // movement Player
        MovementPlayer(gameInput.GetMovementVector(), gameInput.GetJump(), gameInput.GetCrouch(), gameInput.GetSprint());
    }

    private void MovementPlayer(Vector2 direction, bool jump, bool crouch, bool sprint)
    {
        #region crouch
        // player smooth crouch
        if (crouch)
        {
            targetLocalScaleY = 0.65f;
            speedPlayer = 2f;
        }
        else
        {
            targetLocalScaleY = 1f;
            speedPlayer = 4.8f;
        }
        float newScaleY = Mathf.Lerp(transform.localScale.y, targetLocalScaleY, Time.deltaTime * smoothCrouch);
        transform.localScale = new Vector3(1, newScaleY, 1);
        #endregion

        #region movement
        // walk in the direction you are looking
        moveDirection = orientation.forward * direction.y + orientation.right * direction.x;

        if (cController.isGrounded)
        {
            #region sprint
            if (!crouch)
                speedPlayer = sprint ? 8f : 4.8f;
            #endregion

            velocity.y = -1f;
            if (jump) velocity.y = jumpForce;
        }
        else
        {
            velocity.y -= gravity * -2f * Time.deltaTime;
        }

        cController.Move(moveDirection * speedPlayer * Time.deltaTime);
        cController.Move(velocity * Time.deltaTime);
        #endregion

        #region rotation
        // player rotation
        transform.rotation = orientation.rotation;
        #endregion
    }

    //// TODO mover RAYCAST a otro fichero para mejorar organizacion, colocar ahi todas las interacciones
    //private void messageInteractionsPlayer()
    //{
    //    if(!wObject && messageInterc.text != "") messageInterc.text = ""; // reset text
    //    wObject = false;

    //    if (getRaycastPlayer() is IMessageInteraction messInteract)
    //    {
    //        wObject = true;
    //        messageInterc.text = messInteract.getMessageToShow();
    //    }
    //}

    public MonoBehaviour getRaycastPlayer() 
    {
        // lanzo una sola comprobacion cuando quiera saber que esta viendo el player
        if (Physics.Raycast(camPosition.position, camPosition.forward, out RaycastHit hit, 3f, ~0, QueryTriggerInteraction.Ignore)) // para evitar los trigger
        {
            return hit.collider.GetComponent<MonoBehaviour>();
        }
        return null;
    }

    public void setPosition(Transform pos) {
        cController.enabled = false;
        transform.position = pos.position;
        cController.enabled = true;
    }
    public void setPosition(Vector3 pos)
    {
        cController.enabled = false;
        transform.position = pos;
        cController.enabled = true;
    }

    public void setRotation(Quaternion rotation) {
        this.orientation.rotation = rotation;
    }

    public void setCMposition(Vector3 CMposition) {
        camPosition.position = CMposition;
    }

}
