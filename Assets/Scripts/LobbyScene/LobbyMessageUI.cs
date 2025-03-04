using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageInfoT;
    [SerializeField] private Button closeButton;
    [SerializeField] private LobbyCreateUI lobbyCreateUI; // interfaz final para crear una lobby

    private void Awake()
    {
        closeButton.onClick.AddListener(hide);
    }

    private void Start()
    {
        MazeGameLobby.Instance.OnLobbyCreateStarted += MazeGameLobby_OnLobbyCreateStarted;
        MazeGameLobby.Instance.OnLobbyCreateFailed += MazeGameLobby_OnLobbyCreatedFailed;
        MazeGameLobby.Instance.OnLobbyJoinStarted += MazeGameLobby_OnLobbyJoinStarted;
        MazeGameLobby.Instance.OnLobbyJoinFailed += MazeGameLobby_OnLobbyJoinFailed;

        closeButton.gameObject.SetActive(false);
        hide();
    }

    private void MazeGameLobby_OnLobbyJoinFailed(object sender, System.EventArgs e)
    {
        closeButton.gameObject.SetActive(true);
        showMessageInfo("Failed to join lobby");
    }

    private void MazeGameLobby_OnLobbyJoinStarted(object sender, System.EventArgs e)
    {
        closeButton.gameObject.SetActive(false);
        showMessageInfo("Joining lobby...");
    }

    private void MazeGameLobby_OnLobbyCreatedFailed(object sender, System.EventArgs e)
    {
        closeButton.gameObject.SetActive(true);
        showMessageInfo("Failed to create lobby");
    }

    private void MazeGameLobby_OnLobbyCreateStarted(object sender, System.EventArgs e)
    {
        closeButton.gameObject.SetActive(false);
        showMessageInfo("Creating lobby...");
    }

    public void showMessageInfo(string message)
    {
        messageInfoT.text = message;
        show();
    }

    public void show()
    {
        lobbyCreateUI.hide(); // oculto la interfaz de detras para verse mejor
        gameObject.SetActive(true);
    }

    public void hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        MazeGameLobby.Instance.OnLobbyCreateStarted -= MazeGameLobby_OnLobbyCreateStarted;
        MazeGameLobby.Instance.OnLobbyCreateFailed -= MazeGameLobby_OnLobbyCreatedFailed;
        MazeGameLobby.Instance.OnLobbyJoinStarted -= MazeGameLobby_OnLobbyJoinStarted;
        MazeGameLobby.Instance.OnLobbyJoinFailed -= MazeGameLobby_OnLobbyJoinFailed;
    }
}
