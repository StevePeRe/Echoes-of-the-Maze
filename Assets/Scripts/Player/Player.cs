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
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.InputSystem.LowLevel;

public class Player : NetworkBehaviour
{
    private Camera cameraPlayer;
    private CharacterController cController;
    
    // person characteristics
    [SerializeField] private float smoothCrouch;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity = 19.62f;
    private float speedPlayer = 4.8f;
    private Vector3 moveDirection;
    private bool crouch;
    private float targetLocalScaleY;
    private Vector3 velocity;

    // Inventory
    public static event EventHandler<OnInventoryItemEventArgs> OnAddItem;
    public static event EventHandler OnDropItem;
    private int MAX_SIZE_iNVENTORY = 3;
    private ICollectable[] inventPlayer;

    // Player Singleton
    public static Player LocalInstance { get; private set; }
    public static event EventHandler OnAnyPlayerSpawned;
    public static void ResetStaticData() { 
        OnAnyPlayerSpawned = null;
        OnAddItem = null;
        OnDropItem = null;
    }

    // Methods
    //IsOwner funciona para cargas como el movimiento y mecanicas que solo tiene que ser instanciadas por un solo jugador
    //IsClient funciona para clientes en general
    // cuando se inicie la conexion se lanzara este metodo
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
            transform.position = new Vector3(UnityEngine.Random.Range(-4f, 1f), 16f, UnityEngine.Random.Range(-5f, -1f));
        }

        Debug.Log(transform.position + " Player spawn");
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
    }
    private void Awake()
    {
        cController = GetComponent<CharacterController>(); // busca dentro de donde este el script
    }

    // Start is called before the first frame update
    private void Start()
    {
        inventPlayer = new ICollectable[MAX_SIZE_iNVENTORY]; // creo que es uno individual para cada uno xd

        // Game Input // no hace falta sync, sirve para los todos los jugadores
        GameInput.instance.OnInteractionAction += GameInput_OnInteractionAction; // E
        GameInput.instance.OnDropAction += GameInput_OnDropAction; // G
    }

    // TODO Cambiar en un futuro refactorizar en otro docuemnto solo interacciones
    private void GameInput_OnDropAction(object sender, EventArgs e)
    {
        if (!IsOwner) return;
        OnDropItem?.Invoke(this, EventArgs.Empty);
    }

    // Interfaces que intertactuen con la E
    private void GameInput_OnInteractionAction(object sender, EventArgs e)
    {
        if (!IsOwner) return;
        //Player player = sender as Player;
        //if(player == null) return;
        // mas adelante ver si poner esto en el inventario para mejor organizacion
        var monoB = getRaycastPlayer();

        //Debug.Log("Pulso E");

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

    // Update is called once per frame
    private void Update()
    {
        //// raycast Player

        // movement Player
        if(!IsOwner) return;

        MovementPlayer(GameInput.instance.GetMovementVector(), GameInput.instance.GetJump(), GameInput.instance.GetCrouch(), GameInput.instance.GetSprint());
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
        //moveDirection = orientation.forward * direction.y + orientation.right * direction.x;
        moveDirection = cameraPlayer.transform.forward * direction.y + cameraPlayer.transform.right * direction.x;
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
        Vector3 eulerRotation = cameraPlayer.transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0, eulerRotation.y, 0); // solo rotacion en eje Y
        #endregion
    }

    public MonoBehaviour getRaycastPlayer() 
    {   
        // lanzo una sola comprobacion cuando quiera saber que esta viendo el player
        if (Physics.Raycast(cameraPlayer.transform.position, cameraPlayer.transform.forward, out RaycastHit hit, 3f, ~0, QueryTriggerInteraction.Ignore)) // para evitar los trigger
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

    public void setCameraPlayer(Camera cameraP)
    {
        cameraPlayer = cameraP;
    }

    public ICollectable[] getInventory() { return inventPlayer; }

    // Dibujado de raycast de la cam
    private void OnDrawGizmos()
    {
        if (cameraPlayer == null) return;
        Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(camPosition.position, camPosition.position + camPosition.forward * 3f);
        Gizmos.DrawLine(cameraPlayer.transform.position, cameraPlayer.transform.position + cameraPlayer.transform.forward * 3f);
    }

}
