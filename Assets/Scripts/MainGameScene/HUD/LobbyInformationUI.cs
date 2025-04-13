using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyInformationUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private TextMeshProUGUI lobbyCode;



    private void Start()
    {
        Lobby lobby = MazeGameLobby.Instance.getLobby();

        lobbyName.text = "Lobby Name: " + lobby.Name;
        lobbyCode.text = "Lobby Code: " + lobby.LobbyCode;

        StartDayButton.OnStartDay += StartDayButton_OnStartDay;
        // aqui estara el onendday, que vendra del daymanager al acabarse el dia y vovler a estado w8ing
    }

    private void StartDayButton_OnStartDay(object sender, System.EventArgs e)
    {
        hide();
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
