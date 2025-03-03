using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMenuUI : MonoBehaviour
{
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinLobbyButton;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private Button backToMainMenuB;
    [SerializeField] private TMP_InputField joinCodeIF;
    [SerializeField] private LobbyCreateUI lobbyCreateUI; // interfaz final para crear una lobby
    [SerializeField] private Transform lobbyContainer; // contenedor de la lista de lobbies
    [SerializeField] private Transform lobbyTemplate;

    private void Awake()
    {
        createLobbyButton.onClick.AddListener(() =>
        {
            //MazeGameLobby.Instance.CreateLobby("Mi primera Lobby", false);
            lobbyCreateUI.show();
        });

        quickJoinLobbyButton.onClick.AddListener(() =>
        {
            MazeGameLobby.Instance.QuickJoin();
        });

        joinCodeButton.onClick.AddListener(() =>
        {
            MazeGameLobby.Instance.joinWithCode(joinCodeIF.text);
        });

        backToMainMenuB.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        lobbyTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        MazeGameLobby.Instance.OnLobbyListChanged += Instance_OnLobbyListChanged;
        updateLobbyList(new List<Lobby>()); // ninguna lobby al principio que mostrar
    }

    private void Instance_OnLobbyListChanged(object sender, MazeGameLobby.OnLobbyListChangedEventArgs e)
    {
        //Debug.Log(e.lobbyList.Count);
        updateLobbyList(e.lobbyList);
    }

    private void updateLobbyList(List<Lobby> lobbyList)
    {
        // si no son exactamente el lobbyTemplate los elimino para resetear la lista // eliminando antes de volver a crear la lista
        foreach (Transform child in lobbyContainer)
        {
            if (child == lobbyTemplate) continue;
            Destroy(child.gameObject);
        }

        if (lobbyList.Count <= 0)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "There are no lobbies available";
        }
        else
        {
            // crea instancias segun lobbies existan, y a cada una cuando se crea le asigna su lobby 
            foreach (Lobby lobby in lobbyList)
            {
                Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
                lobbyTransform.gameObject.SetActive(true);
                lobbyTransform.GetComponent<LobbyListSingleTemplateUI>().setLobby(lobby);
            }
        }
    }

    private void OnDestroy()
    {
        MazeGameLobby.Instance.OnLobbyListChanged -= Instance_OnLobbyListChanged;
    }
}
