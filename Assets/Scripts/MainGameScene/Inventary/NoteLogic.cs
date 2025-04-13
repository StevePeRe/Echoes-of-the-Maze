using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteLogic : MonoBehaviour, IInteractuable, IMessageInteraction
{
    [SerializeField] private int noteID; // id unico para cada nota
    private bool active;
    private bool interactionLocked; // Nuevo estado para bloquear interacciones


    private void Awake()
    {
        active = false;
    }

    private void Start()
    {
        Player.OnEPressed += Player_OnEPressed;
    }

    private void Player_OnEPressed(object sender, System.EventArgs e)
    {
        if (!active) return;

        active = false;
        NoteMessage.Instance.showNote(noteID, active);

        // Bloquear interacciones temporalmente
        interactionLocked = true;
        StartCoroutine(UnlockInteraction());
    }

    public void Interact()
    {
        //Debug.Log("interactuo con nota: " + noteID);
        if (interactionLocked) return; // Evitar interacción si está bloqueada

        if (active) return;

        active = true; 
        NoteMessage.Instance.showNote(noteID, active);
    }

    // Cuando salga del rango, automaticamente la nota se deja de leer
    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("salgo del trigger: " + other.tag);
        if (other.CompareTag("Player"))
        {
            Debug.Log("salgo del trigger por el player: " + noteID);
            active = false;
            NoteMessage.Instance.showNote(noteID, active);
        }
    }

    public string getMessageToShow()
    {
        //Debug.Log("leo la nota: " + noteID);
        return "Read note: E";
    }

    // Desbloquear interacción después de un breve retraso
    private IEnumerator UnlockInteraction()
    {
        yield return new WaitForSeconds(0.2f); // Ajusta el tiempo según sea necesario
        interactionLocked = false;
    }
}
