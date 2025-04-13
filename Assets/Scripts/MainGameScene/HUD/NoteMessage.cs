using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMessage : MonoBehaviour
{
    public static NoteMessage Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void showNote(int idNote, bool active)
    {
        Debug.Log("muestro la nota: " + idNote);
        gameObject.transform.GetChild(idNote).gameObject.SetActive(active);
    }

}
