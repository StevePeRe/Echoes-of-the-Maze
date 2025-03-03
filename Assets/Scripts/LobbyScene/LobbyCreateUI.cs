using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyName;
    [SerializeField] private Button isPublic;
    [SerializeField] private Button isPrivate;
    [SerializeField] private Button closeLobbyUI;
    //[SerializeField] private LobbyPlayersUI lobbyPlayersUI; // por si se complica el tener la info con la partida empezada

    private void Awake()
    {
        isPublic.onClick.AddListener(() =>
        {
            MazeGameLobby.Instance.CreateLobby(lobbyName.text, false);
            //hide();
            //lobbyPlayersUI.show();
        });

        isPrivate.onClick.AddListener(() =>
        {
            MazeGameLobby.Instance.CreateLobby(lobbyName.text, true);
            //hide();
            //lobbyPlayersUI.show();
        });

        closeLobbyUI.onClick.AddListener(() =>
        {
            hide();
        });
            
    }

    private void Start()
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
