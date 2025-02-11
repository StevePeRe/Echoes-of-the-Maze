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
using System.Diagnostics;

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
    //public static event EventHandler OnBuyerInteraction;

    // pruebas
    public Transform lintern;
    public Transform polola;
    public Transform door;

    // Player Singleton
    public static Player LocalInstance { get; private set; }
    public static event EventHandler OnAnyPlayerSpawned;
    //public static event EventHandler OnAnyPlayerMoveWheelMouse;
    //public static event EventHandler OnAnyPlayerRightClick;

    public static void ResetStaticData() {
        OnAddItem = null;
        OnDropItem = null;
        OnAnyPlayerSpawned = null;
        //OnBuyerInteraction = null;
        //OnAnyPlayerMoveWheelMouse = null;
        //OnAnyPlayerRightClick = null;
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
            if (IsServer)
            {
                Transform spwObj = Instantiate(lintern);
                spwObj.GetComponent<NetworkObject>().Spawn(true);
                spwObj.transform.position = new Vector3(0.02f, 13.23f, 7.62f);

                Transform spwObj2 = Instantiate(polola);
                spwObj2.GetComponent<NetworkObject>().Spawn(true);
                spwObj2.transform.position = new Vector3(-2.02f, 13.23f, 7.62f);

                //Transform spwObj3 = Instantiate(door);
                //spwObj3.GetComponent<NetworkObject>().Spawn(true);
                //spwObj3.transform.position = new Vector3(-2.11f, 14.6f, 1f);
                
            }
        }
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Awake()
    {
        cController = GetComponent<CharacterController>(); // busca dentro de donde este el script
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Game Input // no hace falta sync, sirve para los todos los jugadores
        GameInput.instance.OnInteractionAction += GameInput_OnInteractionAction; // E
        GameInput.instance.OnDropAction += GameInput_OnDropAction; // G
    }

    // TODO Cambiar en un futuro refactorizar en otro docuemnto solo interacciones
    // estos metodos tieneN que estar aqui para saber quien los invoca con el isOwner
    private void GameInput_OnDropAction(object sender, EventArgs e)
    {
        if (!IsOwner) return;
        OnDropItem?.Invoke(this, EventArgs.Empty);
    }

    // Interfaces que intertactuen con la E
    private void GameInput_OnInteractionAction(object sender, EventArgs e)
    {
        // Solo pasa la interaccion que ha hecho el jugador propietario
        if (!IsOwner) return;

        var hitPlayer = getRaycastPlayer();
        if (hitPlayer == null) return;
        //hitPlayer.GetComponent<ICollectable>() is ICollectable collectable -> busca algun componente que posea ese tipo de clase especifica o heredada
        //hitPlayer.GetComponent<ICollectable>()-> solo busca el componente pasado T, si no existe como componente devuele null

        if (hitPlayer.GetComponent<ICollectable>() is ICollectable collectable)
        {
            OnAddItem?.Invoke(this, new OnInventoryItemEventArgs
            {
                inventoryItem = collectable
            });
        }

        if (hitPlayer.GetComponent<IInteractuable>() is IInteractuable interactuable)
        {
            interactuable.Interact();
        }

        //if (hitPlayer.GetComponent<BuyerBehaviour>() is BuyerBehaviour buyer)
        //{
        //    UnityEngine.Debug.Log("entro buyer");
        //    buyer.Interact();
        //}
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

    //public MonoBehaviour getRaycastPlayer() 
    //{
    //    //if (!IsClient) return null;
    //    // lanzo una sola comprobacion cuando quiera saber que esta viendo el player
    //    //if (Physics.Raycast(cameraPlayer.transform.position, cameraPlayer.transform.forward, out RaycastHit hit, 3f, ~0, QueryTriggerInteraction.Ignore)) // para evitar los trigger
    //    //{
    //    //    //if(hit.collider.GetComponent<MonoBehaviour>() != null)
    //    //    //{
    //    //    if (hit.collider.TryGetComponent<IMessageInteraction>(out var interactable)) // poniendo directamente lo que busco si lo encuentra e imprimer por pantalla el mesanje
    //    //        // pero si pongo monobehaviour no lo encuentra
    //    //    {
    //    //        //UnityEngine.Debug.Log($"Objeto detectado: {hit.collider.GetComponent<MonoBehaviour>().name}, " +
    //    //        //    $"Tipo: {hit.collider.GetComponent<MonoBehaviour>().GetType()}");
    //    //        //return hit.collider.GetComponent<MonoBehaviour>();
    //    //        return interactable as MonoBehaviour;
    //    //    }
    //    //}
    //    //return null;

    //    if (Physics.Raycast(cameraPlayer.transform.position, cameraPlayer.transform.forward, out RaycastHit hit, 3f, ~0, QueryTriggerInteraction.Ignore)) // para evitar los trigger
    //    {
    //        if (hit.collider != null)
    //        {
    //            //UnityEngine.Debug.Log($"Objeto detectado: {hit.collider.GetComponent<MonoBehaviour>().name}, " +
    //            //    $"Tipo: {hit.collider.GetComponent<MonoBehaviour>().GetType()}");
    //            return hit.collider.GetComponent<MonoBehaviour>();
    //        }
    //    }
    //    return null;

    //}

    public Collider getRaycastPlayer() // de aqui ya obtengo el componente sin null
    {
        if (Physics.Raycast(cameraPlayer.transform.position, cameraPlayer.transform.forward, out RaycastHit hit, 3f, ~0, QueryTriggerInteraction.Ignore)) // para evitar los trigger
        {
            if (hit.collider != null)
            {
                //UnityEngine.Debug.Log($"Objeto detectado: {hit.collider.GetComponent<MonoBehaviour>().name}, " +
                //    $"Tipo: {hit.collider.GetComponent<MonoBehaviour>().GetType()}");
                return hit.collider;
            }
        }
        return null;

        //if (Physics.Raycast(cameraPlayer.transform.position, cameraPlayer.transform.forward, out RaycastHit hit, 3f, ~0, QueryTriggerInteraction.Ignore)) // para evitar los trigger
        //{
        //    if (hit.collider != null)
        //    {
        //        //UnityEngine.Debug.Log($"Objeto detectado: {hit.collider.GetComponent<MonoBehaviour>().name}, " +
        //        //    $"Tipo: {hit.collider.GetComponent<MonoBehaviour>().GetType()}");
        //        return hit.collider.gameObject;
        //    }
        //}
        //return null;

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

    // Referencia de networkObject del objeto de la red
    public NetworkObject getNetworkObject()
    {
        return NetworkObject;
    }

    // Inventario
    //public ICollectable[] getInventory() { return inventPlayer; }

    // Dibujado de raycast de la cam
    private void OnDrawGizmos()
    {
        if (cameraPlayer == null) return;
        Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(camPosition.position, camPosition.position + camPosition.forward * 3f);
        Gizmos.DrawLine(cameraPlayer.transform.position, cameraPlayer.transform.position + cameraPlayer.transform.forward * 3f);
    }

}
