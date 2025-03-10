using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour
{
    // Para no duplicar objetos una vez se vuelve a la escena principal
    private void Awake()
    {
        if (NetworkManager.Singleton != null) Destroy(NetworkManager.Singleton.gameObject);

        if (MazeGameMultiplayer.Instance != null) Destroy(MazeGameMultiplayer.Instance.gameObject);

        if (MazeGameLobby.Instance != null) Destroy(MazeGameLobby.Instance.gameObject);

        // RESET STATIC DATA - para limpiar las subs y que no se lance el evento el doble
        Player.ResetStaticData();
        StartDayButton.ResetStaticData();
        MazeGameMultiplayer.ResetStaticData();
    }
}
