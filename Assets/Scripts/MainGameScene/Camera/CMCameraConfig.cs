using Cinemachine;
using System.Collections;
using System.Collections.Generic;
//using Unity.Netcode;
using UnityEngine;

public class CMCameraConfig : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineC;
    private CinemachineInputProvider inputCamera;

    private void Awake()
    {
        inputCamera = GetComponent<CinemachineInputProvider>();
    }

    // para que la camara siga al player
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // desaparace el mouse
        Cursor.visible = false;

        if (Player.LocalInstance != null)
        {
            cinemachineC = GetComponent<CinemachineVirtualCamera>();

            if (cinemachineC != null)
            {
                cinemachineC.Follow = Player.LocalInstance.transform.GetChild(2); // obtengo el tranform del GO de camaraPos
            }
        }
        else
        {
            Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
        }

        // Al pausar el juego, la camara se desactiva
        PauseMenu.instance.OnAblePauseAction += PauseMenu_OnAblePauseAction;
        PauseMenu.instance.OnDisablePauseAction += PauseMenu_OnDisablePauseAction;
    }

    private void PauseMenu_OnAblePauseAction(object sender, System.EventArgs e)
    {
        if (inputCamera != null)
        {
            inputCamera.enabled = false; // Desactivar el componente de entrada de la cámara
        }
    }

    private void PauseMenu_OnDisablePauseAction(object sender, System.EventArgs e)
    {
        if (inputCamera != null)
        {
            inputCamera.enabled = true; // Reactivar el componente de entrada de la cámara
        }
    }

    private void Player_OnAnyPlayerSpawned(object sender, System.EventArgs e)
    {
        if (Player.LocalInstance != null)
        {
            cinemachineC = GetComponent<CinemachineVirtualCamera>();

            if (cinemachineC != null)
            {
                cinemachineC.Follow = Player.LocalInstance.transform.GetChild(2); // obtengo el tranform del GO de camaraPos
            }
        }
    }
}
