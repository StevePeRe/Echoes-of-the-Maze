using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
//using Unity.Netcode;
using UnityEngine;

public class MainCameraConfig : MonoBehaviour
{
    private void Start()
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.setcameraPlayer(GetComponent<Camera>());
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
            Player.LocalInstance.setcameraPlayer(GetComponent<Camera>());
        }
    }
    //public override void OnNetworkSpawn()
    //{
    //    if (!IsOwner) return;

    //    if (Player.LocalInstance != null)
    //    {
    //        Player.LocalInstance.setcameraPlayer(GetComponent<Camera>());
    //    }
    //    else 
    //    {
    //        Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
    //    }


    //}
}
