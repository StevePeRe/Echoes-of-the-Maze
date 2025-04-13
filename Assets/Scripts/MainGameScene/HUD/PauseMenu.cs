using System;
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

    public event EventHandler OnAblePauseAction;
    public event EventHandler OnDisablePauseAction;

    public static PauseMenu instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            Debug.LogError("PauseMenu Instance already exist");
        }
        instance = this;

        resumeButton.onClick.AddListener(() =>
        {
            hide();
            Cursor.lockState = CursorLockMode.Locked; // desaparace el mouse
            Cursor.visible = false;
            OnDisablePauseAction?.Invoke(this, EventArgs.Empty);
        });

        quitButton.onClick.AddListener(() =>
        {
            OnDisablePauseAction?.Invoke(this, EventArgs.Empty);
            MazeGameLobby.Instance.leaveLobby(); // dejar la lobby
            NetworkManager.Singleton.Shutdown();
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
            OnAblePauseAction?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            hide();
            Cursor.lockState = CursorLockMode.Locked; // desaparace el mouse
            Cursor.visible = false;
            OnDisablePauseAction?.Invoke(this, EventArgs.Empty);
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
