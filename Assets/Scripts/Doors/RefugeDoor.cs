using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefugeDoor : MonoBehaviour, IInteractuable, IMessageInteraction
{
    [SerializeField] private Transform doorHinge; // Punto de rotación de la puerta
    [SerializeField] private float openAngle = 90f; // Ángulo al que se abre la puerta
    [SerializeField] private float transitionDuration = 1f; // Duración de la apertura/cierre
    [SerializeField] private bool isOpen = false; // Estado inicial de la puerta

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine doorCoroutine;

    void Start()
    {
        // Guardar las rotaciones inicial (cerrada) y abierta
        closedRotation = doorHinge.localRotation;
        openRotation = Quaternion.Euler(doorHinge.localRotation.eulerAngles + new Vector3(0, openAngle, 0));
    }

    public void ToggleDoor()
    {
        if (doorCoroutine != null)
        {
            StopCoroutine(doorCoroutine);
        }

        doorCoroutine = StartCoroutine(RotateDoor(isOpen ? openRotation : closedRotation, isOpen ? closedRotation : openRotation));
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

        doorHinge.localRotation = endRotation; // Asegurarse de terminar exactamente en la rotación final
    }

    public void Interact()
    {
        if(MazeGameManager.instance.getGamePlaying())
        {
            ToggleDoor();
        }
        else
        {
            Debug.Log("Debes empezar el dia antes");
        }
    }

    public string getMessageToShow()
    {
        return "Interactuar: E";
    }

}
