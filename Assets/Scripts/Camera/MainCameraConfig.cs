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
        //Comunicado directo con player, al principio el player es null ya que tengo que crearlo con el boton de host, mas adelante cuando se pase a la escena lo hara automatico y entrara
        //primero en el if
        //pero como no, se suscribe al evento y se queda esperadndo a que un jugador spawnee
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.setCameraPlayer(GetComponent<Camera>());
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
            Player.LocalInstance.setCameraPlayer(GetComponent<Camera>());
        }
    }
    //public override void OnNetworkSpawn() // onNetworkSpawn solo sirve para este objeto, osea que no sirve para todo esta logica 
    //{
    //    if (!IsOwner) return; si hiciese if (!IsClient) a lo mejor entra en el Player.LocalInstance que no debe, y le coloca la camara del host al segundo que ha entrado
    //    por ello es mejor hacerlo con evento de cuando un cliente se ha conectado al juegoy colocarle a ese jugador la camara

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
