using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DayManager : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI passedTimeT;
    public event EventHandler OnGoToNextDay;

    private const float maxTimePerDay = 1500f; // 25min todo el dia
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    void Update()
    {
        // se hará en el server
        if (IsServer)
        {
            if (!MazeGameManager.instance.getGamePlaying()) return;

            gamePlayingTimer.Value += Time.deltaTime;

            if (gamePlayingTimer.Value >= maxTimePerDay)
            {
                //OnGoToNextDay?.Invoke(this, EventArgs.Empty);
                //MazeGameManager.instance.setGeneratePreMazeServerRpc();
                // todavia por decidir que va a pasar, para hacer el juego mas tetrico
                resetDayTimer();
            }
        }

        // Mostrar el tiempo en la interfaz de cada cliente localmente
        passedTimeT.text = Mathf.Ceil(gamePlayingTimer.Value).ToString();
    }

    public void resetDayTimer()
    {
        gamePlayingTimer.Value = 0;
    }
}
