using Cinemachine;
using System.Collections;
using System.Collections.Generic;
//using Unity.Netcode;
using UnityEngine;

public class CMCameraConfig : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineC;

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

    //// Start is called before the first frame update
    //// De esta forma cada camara sigue a su propio cliente
    //public override void OnNetworkSpawn()
    //{
    //    if (!IsOwner || Player.LocalInstance == null) return;
       
        
    //}

}
