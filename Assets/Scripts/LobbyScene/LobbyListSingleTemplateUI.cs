using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListSingleTemplateUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameT;
    private Lobby lobby;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            // me uno con el Id porque es una lobby publica y no tiene codigo
            MazeGameLobby.Instance.joinWithId(lobby.Id);
        });
    }

    public void setLobby(Lobby lobby)
    {
        this.lobby = lobby;
        lobbyNameT.text = lobby.Name;
    }
}
