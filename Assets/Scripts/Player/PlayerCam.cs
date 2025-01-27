using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    //[SerializeField] private float sensX, sensY;
    [SerializeField] private Transform orientation;
    //[SerializeField] private CinemachineVirtualCamera virtualCamera;

    //private float xRotation, yRotation;
    // Separar logica de rotacion de camara para el jugador

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // desaparace el mouse
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 eulerRotation = this.transform.rotation.eulerAngles;
        orientation.rotation = Quaternion.Euler(0, eulerRotation.y, 0);
    }
}
