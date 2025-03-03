using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayersUI : MonoBehaviour
{
    [SerializeField] private Button startGame;

    private void Awake()
    {
        startGame.onClick.AddListener(() =>
        {
            Loader.LoadNetwork(Loader.Scene.MainGameScene);
        });
    }

    public void show()
    {
        gameObject.SetActive(true);
    }

    public void hide()
    {
        gameObject.SetActive(false);
    }
}
