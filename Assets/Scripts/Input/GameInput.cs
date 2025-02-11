using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnInteractionAction; // E
    public event EventHandler OnDropAction; // G
    public event EventHandler<OnMouseValuesEventArgs> OnWheelMouseAction; // wheel
    public event EventHandler<OnMouseValuesEventArgs> OnRightClickAction; // right click


    private Vector2 movementInput;
    PlayerInputActions playerInputActions;

    public static GameInput instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            Debug.LogError("GameInput Instance already exist");
        }
        //Debug.Log("Entro en GameInput");
        instance = this;

        // start - la primera ejecucion al pulsarlo
        // performed - el instante despues del started
        // canceled - cuando deja de pulsar

        playerInputActions = new PlayerInputActions(); // del nuevo input que he creado, creo la instancia para usarla
        playerInputActions.Player.Enable(); // habilito el input del player
        playerInputActions.Player.Interaction.performed += Interaction_performed; // el nuevo sistema de input puede funcionar tmb con events,
                                                                                  // con esto no tengo que estar todo el rato atento si pulsa la interaccion
        playerInputActions.Player.Drop.performed += Drop_performed;
        playerInputActions.Player.WeelMouse.performed += WeelMouse_performed; 
        playerInputActions.Player.UseItem.performed += UseItem_performed;

    }

    public class OnMouseValuesEventArgs : EventArgs
    {
        public float value;
    }

    private void Drop_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        
        OnDropAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interaction_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //Debug.Log(obj.ReadValue<float>());
        OnInteractionAction?.Invoke(this, EventArgs.Empty); // how is sending this event, eventArgs if we want to
    }

    private void WeelMouse_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnWheelMouseAction?.Invoke(this, new OnMouseValuesEventArgs
        {
            value = obj.ReadValue<Vector2>().y // solo giro en vertical
        });
    }

    private void UseItem_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnRightClickAction?.Invoke(this, new OnMouseValuesEventArgs
        {
            value = obj.ReadValue<float>()
        });
    }

    public Vector2 GetMovementVector()
    {
        movementInput = playerInputActions.Player.Move.ReadValue<Vector2>();
        return movementInput.normalized;
    }

    public bool GetJump()
    {
        return playerInputActions.Player.Jump.triggered;
        //return playerInputActions.Player.Jump.ReadValue<float>() > 0;
    }

    public bool GetCrouch()
    {
        return playerInputActions.Player.Crouch.ReadValue<float>() > 0;
    }

    public bool GetSprint()
    {
        return playerInputActions.Player.Sprint.ReadValue<float>() > 0;
    }

    //public Vector2 GetMovementWeelMouse()
    //{
    //    return playerInputActions.Player.WeelMouse.ReadValue<Vector2>();
    //}

    //public bool GetRigthClickMouse()
    //{
    //    return playerInputActions.Player.UseItem.triggered;
    //}

}
