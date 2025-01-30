using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainCameraConfig : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner || Player.LocalInstance == null) return;

        Player.LocalInstance.setcameraPlayer(GetComponent<Camera>());
    }
}
