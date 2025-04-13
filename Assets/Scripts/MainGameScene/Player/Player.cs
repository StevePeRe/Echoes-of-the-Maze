using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IDamageable
{
    private Camera cameraPlayer;
    private CharacterController cController;
    [SerializeField] private List<Vector3> positionPlayerSpawns;

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
    public static event EventHandler OnEPressed;
    //public static event EventHandler OnBuyerInteraction;

    // Danyo caida
    private float fallStartHeight; // Altura inicial de la caída
    private bool isFalling; // Indica si el jugador está cayendo
    [SerializeField] private float fallDamageThreshold; // Distancia mínima para aplicar daño
    [SerializeField] private float fallDamageMultiplier; // Multiplica

    // Interfaces
    [SerializeField] private int _health = 100;
    public int Health
    {
        get { return _health; }
        set { _health = Mathf.Clamp(value, 0, 100); }
    }

    // Player Singleton
    public static Player LocalInstance { get; private set; }

    public static event EventHandler OnAnyPlayerSpawned; // cuando la escena cambia, este evento no lo limpiaremos, por ello hay que hacerlo manual en resetStaticData

    public static void ResetStaticData() {
        OnAddItem = null;
        OnDropItem = null;
        OnEPressed = null;
        OnAnyPlayerSpawned = null;
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

            // para que el jugador no se caiga al spawnear, igual se cae no se por quexd
        }
        setPositionPlayerServerRpc(positionPlayerSpawns[UnityEngine.Random.Range(0, 3)]); // POS FIJA DE CADA PLAYER
        // ver video de PlayerData de code monkey, habla osbre esto min 4:35:00

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

        OnEPressed?.Invoke(this, EventArgs.Empty); // algunos script necesitan solo el evento de la E

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
    }

    // Update is called once per frame
    private void Update()
    {
        // movement Player
        if (!IsOwner) return;

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
        Vector3 forward = cameraPlayer.transform.forward;
        Vector3 right = cameraPlayer.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        if (cameraPlayer != null) moveDirection = forward * direction.y + right * direction.x;
        if (cController.isGrounded)
        {
            // Detectar aterrizaje
            if (isFalling)
            {
                isFalling = false;
                float fallDistance = fallStartHeight - transform.position.y; // Calcular la distancia de caída

                if (fallDistance > fallDamageThreshold)
                {
                    Debug.Log($"datos: ({fallDistance} - {fallDamageThreshold}) * {fallDamageMultiplier}");
                    int fallDamage = Mathf.RoundToInt((fallDistance - fallDamageThreshold) * fallDamageMultiplier);
                    dealtDamage(fallDamage); // Aplicar daño al jugador
                    Debug.Log($"Daño por caída: {fallDamage}");
                }
            }

            #region sprint
            if (!crouch)
                speedPlayer = sprint ? 8f : 4.8f;
            #endregion

            velocity.y = -1f;
            if (jump) velocity.y = jumpForce;
        }
        else
        {
            // Detectar inicio de la caída
            if (!isFalling)
            {
                isFalling = true;
                fallStartHeight = transform.position.y; // Guardar la altura inicial de la caída
            }

            speedPlayer = sprint ? 8f : 4.8f;
            velocity.y -= gravity * -2f * Time.deltaTime;
        }
        cController.Move(moveDirection * speedPlayer * Time.deltaTime);
        //Debug.Log("en update player: " + transform.position + " velocidad: " + velocity + " movement: " + moveDirection);
        cController.Move(velocity * Time.deltaTime);
        #endregion

        #region rotation
        // player rotation
        if (cameraPlayer != null) {
            Vector3 eulerRotation = cameraPlayer.transform.eulerAngles;
            transform.rotation = Quaternion.Euler(0, eulerRotation.y, 0); // solo rotacion en eje Y
            gameObject.transform.GetChild(3).rotation = cameraPlayer.transform.rotation; // rotacion de la mano
        }
        #endregion
    }

    public Collider getRaycastPlayer() // de aqui ya obtengo el componente sin null
    {
        if (cameraPlayer == null) return null;

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
    }

    public void jumpRaycastPlayer() // de aqui ya obtengo el componente sin null
    {

        if (Physics.Raycast(cameraPlayer.transform.position, cameraPlayer.transform.forward, out RaycastHit hit, 3f, ~0, QueryTriggerInteraction.Ignore)) // para evitar los trigger
        {
            
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void setPositionPlayerServerRpc(Vector3 pos)
    {
        setPositionPlayerClientRpc(pos);
    }

    [ClientRpc]
    private void setPositionPlayerClientRpc(Vector3 pos)
    {
        if (cController) // si existe, desactivo y cambio pos, y vuelvo a activar
        {
            cController.enabled = false;
            transform.position = pos;
            cController.enabled = true;
        } else
        {
            transform.position = pos;
        }

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

    // Recibir danyo
    public void dealtDamage(int damage) {
        Health -= damage;
        Debug.Log("vida actual: " + Health);
    }

}
