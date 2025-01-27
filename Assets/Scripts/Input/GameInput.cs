using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnInteractionAction; // E
    public event EventHandler OnDropAction; // G

    private Vector2 movementInput;
    PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions(); // del nuevo input que he creado, creo la instancia para usarla
        playerInputActions.Player.Enable(); // habilito el input del player
        playerInputActions.Player.Interaction.performed += Interaction_performed; // el nuevo sistema de input puede funcionar tmb con events,
                                                                                  // con esto no tengo que estar todo el rato atento si pulsa la interaccion
        playerInputActions.Player.Drop.performed += Drop_performed;
    }

    private void Drop_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnDropAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interaction_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractionAction?.Invoke(this, EventArgs.Empty); // how is sending this event, eventArgs if we want to
    }

    //private void OnEnable()
    //{

    //}

    public Vector2 GetMovementVector()
    {
        movementInput = playerInputActions.Player.Move.ReadValue<Vector2>();

        return movementInput.normalized;
    }

    //public Vector2 GetMovementVectorCamara()
    //{
    //    movementInputCam = playerInputActions.Player.CamaraMove.ReadValue<Vector2>();

    //    return movementInputCam.normalized;
    //}

    public Vector2 GetMovementWeelMouse()
    {
        return playerInputActions.Player.WeelMouse.ReadValue<Vector2>();
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
    public bool GetRigthClickMouse()
    {
        return playerInputActions.Player.UseItem.triggered;
    }

}
