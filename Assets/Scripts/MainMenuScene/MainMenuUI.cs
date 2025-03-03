using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button onlineButton;
    [SerializeField] private Button lanButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        onlineButton.onClick.AddListener(() =>
        {
            //MazeGameMultiplayer.Instance.StartHost();
            Loader.Load(Loader.Scene.LobbyScene);
        });

        lanButton.onClick.AddListener(() =>
        {
            //MazeGameMultiplayer.Instance.StartClient();
            Debug.Log("No implementado de momento");
        });

        exitButton.onClick.AddListener(() =>
        {
           Application.Quit();
        });
    }
}
