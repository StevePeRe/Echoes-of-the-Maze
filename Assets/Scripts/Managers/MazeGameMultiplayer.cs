using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MazeGameMultiplayer : NetworkBehaviour
{
    public static MazeGameMultiplayer Instance { get; private set; }

    private void Awake()
    {
        //if (Instance != null)
        //{
        //    //Debug.LogError("MazeGameManager Instance already exist");
        //    Destroy(MazeGameMultiplayer.Instance.gameObject);
        //}
        Instance = this;

        DontDestroyOnLoad(gameObject); // no se destruye al pasar de escenas
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }


}
