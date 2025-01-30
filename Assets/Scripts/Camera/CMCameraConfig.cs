using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CMCameraConfig : NetworkBehaviour
{
    private CinemachineVirtualCamera cinemachineC;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // desaparace el mouse
        Cursor.visible = false;
    }

    // Start is called before the first frame update
    // De esta forma cada camara sigue a su propio cliente
    public override void OnNetworkSpawn()
    {
        if (!IsOwner || Player.LocalInstance == null) return;
       
        cinemachineC = GetComponent<CinemachineVirtualCamera>();

        if(cinemachineC != null)
        {
            cinemachineC.Follow = Player.LocalInstance.transform;
            //Player.LocalInstance.setCinemachineVPlayer(cinemachineC);
        }
    }

}
