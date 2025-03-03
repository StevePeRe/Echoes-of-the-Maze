using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainMenuScene,
        MainGameScene,
        LoadingScene,
        LobbyScene
    }

    public static Scene targetScene;
    //public static bool isNetworked;

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;
        //isNetworked = false;
        SceneManager.LoadScene(Scene.LoadingScene.ToString()); // carga primero la escena intermedia
    }

    public static void LoadNetwork(Scene targetScene)
    {
        //Loader.targetScene = targetScene;
        //isNetworked = true;
        //SceneManager.LoadScene(Scene.LoadingScene.ToString());
        // el cargar una escena intermedia LOADING para cambio de escena online se pone complicado
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
        //if (!isNetworked) SceneManager.LoadScene(targetScene.ToString()); // cuando llega despues del primer frame de LoadingScene, carga la siguiente escena
        //else NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }


}
