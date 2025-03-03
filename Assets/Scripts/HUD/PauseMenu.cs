using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;
    private bool pause;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() =>
        {
            hide();
            Cursor.lockState = CursorLockMode.Locked; // desaparace el mouse
            Cursor.visible = false;
        });

        quitButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            MazeGameLobby.Instance.leaveLobby(); // dejar la lobby
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        GameInput.instance.OnPauseAction += GameInput_OnPauseAction; ; // Esc - P
        hide();
    }

    private void GameInput_OnPauseAction(object sender, System.EventArgs e)
    {
        pause = !pause;
        if (pause)
        {
            show();
            Cursor.lockState = CursorLockMode.None; // aparace el mouse
            Cursor.visible = true;
        }
        else
        {
            hide();
            Cursor.lockState = CursorLockMode.Locked; // desaparace el mouse
            Cursor.visible = false;
        }
    }

    private void hide()
    {
        gameObject.SetActive(false);
        pause = false;
    }

    private void show()
    {
        gameObject.SetActive(true);
        pause = true;
    }
}
