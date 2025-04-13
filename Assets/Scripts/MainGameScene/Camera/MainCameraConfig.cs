using System.Collections;
//using Unity.Netcode;
using UnityEngine;

// Asignarle la camara al player
public class MainCameraConfig : MonoBehaviour
{
    private void Start()
    {
        //Comunicado directo con player, al principio el player es null ya que tengo que crearlo con el boton de host, mas adelante cuando se pase a la escena lo hara automatico y entrara
        //primero en el if
        //pero como no, se suscribe al evento y se queda esperadndo a que un jugador spawnee
        if (Player.LocalInstance != null && GetComponent<Camera>() != null)
        {
            Player.LocalInstance.setCameraPlayer(GetComponent<Camera>());
        }
        else
        {
            // ver cuantos listeners tiene, no entiendo porque al seguir conectado el host, no le afecta que este evento sea reseteado
            Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
        }
    }

    private void Player_OnAnyPlayerSpawned(object sender, System.EventArgs e)
    {
        if (Player.LocalInstance != null && GetComponent<Camera>() != null)
        {
            Player.LocalInstance.setCameraPlayer(GetComponent<Camera>());
        }
    }

    //private void OnDestroy()
    //{
    //    Player.OnAnyPlayerSpawned -= Player_OnAnyPlayerSpawned;
    //}
}
