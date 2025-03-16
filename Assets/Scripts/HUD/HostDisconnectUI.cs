//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.Netcode;
//using UnityEngine;

//public class HostDisconnectUI : MonoBehaviour
//{
//    void Start()
//    {
//        MazeGameMultiplayer.OnDisconnectHostAction += MazeGameMultiplayer_OnDisconnectHostAction;
//        //NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
//        hide();
//    }

//    //private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
//    //{
//    //    if (clientId == NetworkManager.ServerClientId)
//    //    {
//    //        Debug.Log("me desconecto; " + clientId);
//    //    }
//    //}
//    private void MazeGameMultiplayer_OnDisconnectHostAction(object sender, EventArgs e)
//    {
//        //Debug.Log("entro en disconnecty");
//        //show();
//        //NetworkManager.Singleton.Shutdown();
//        //MazeGameLobby.Instance.leaveLobby(); // dejar la lobby
//        //Loader.Load(Loader.Scene.MainMenuScene);
//    }

//    private void hide()
//    {
//        gameObject.SetActive(false);
//    }

//    private void show()
//    {
//        gameObject.SetActive(true);
//    }

//    private void OnDestroy()
//    {
//        MazeGameMultiplayer.OnDisconnectHostAction -= MazeGameMultiplayer_OnDisconnectHostAction;
//    }
//}
