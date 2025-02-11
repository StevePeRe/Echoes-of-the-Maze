using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RefugeDoor : NetworkBehaviour, IInteractuable, IMessageInteraction
{
    [SerializeField] private Transform doorHinge; // Punto de rotación de la puerta
    [SerializeField] private float openAngle = 90f; // Ángulo al que se abre la puerta
    [SerializeField] private float transitionDuration = 1f; // Duración de la apertura/cierre
    [SerializeField] private bool isOpen = false; // Estado inicial de la puerta

    //private Quaternion actualRotation; // siempre empienzan cerradas las puertas
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine doorCoroutine;
    private bool doorMoving = false;

    void Start()
    {
        if (!isOpen) {
            closedRotation = doorHinge.localRotation;
            openRotation = Quaternion.Euler(doorHinge.localRotation.eulerAngles + new Vector3(0, openAngle, 0));
        } else
        {
            closedRotation = Quaternion.Euler(doorHinge.localRotation.eulerAngles + new Vector3(0, -openAngle, 0));
            openRotation = doorHinge.localRotation;
        }
    }

    public void ToggleDoor()
    {
        if (doorMoving) return; // no empezar ninguna corutina hasta que acabe la anterior

        doorMoving = true;
        doorCoroutine = StartCoroutine(RotateDoor(isOpen ? openRotation : closedRotation, isOpen ? closedRotation : openRotation));

        //doorCoroutine = StartCoroutine(RotateDoor(actualRotation, isOpen ? openRotation : closedRotation));
        isOpen = !isOpen;
    }

    private IEnumerator RotateDoor(Quaternion startRotation, Quaternion endRotation)
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            doorHinge.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        doorMoving = false;
        doorHinge.localRotation = endRotation; // Asegurarse de terminar exactamente en la rotación final
    }

    public void Interact()
    {
        Debug.Log("Entro interact");
        if (MazeGameManager.instance.getGamePlaying())
        {
            Debug.Log("Interactuo con la puerta");
            //ToggleDoor();
            InteractServerRpc();
        }
        else
        {
            Debug.Log("Debes empezar el dia antes");
        }
    }

    [ServerRpc(RequireOwnership = false)] // aunque no sea dueño del objeto el cliente puede llamar a este metodo
    private void InteractServerRpc()
    {
        InteractClientRpc();
    }

    [ClientRpc]
    private void InteractClientRpc()
    {
        ToggleDoor();
    }

    public string getMessageToShow()
    {
        return "Interactuar: E";
    }

}
