using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CMCameraConfig : NetworkBehaviour
{
    private CinemachineVirtualCamera cinemachineC;

    // Start is called before the first frame update
    // De esta forma cada camara sigue a su propio cliente
    private void Start()
    {
        if (!IsOwner) return;

        cinemachineC = GetComponent<CinemachineVirtualCamera>();

        if(cinemachineC != null)
        {
            cinemachineC.Follow = Player.LocalInstance.transform;
        }
    }

}
