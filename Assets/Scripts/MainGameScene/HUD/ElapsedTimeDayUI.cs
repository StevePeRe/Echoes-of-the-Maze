using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ElapsedTimeDayUI : MonoBehaviour
{
    private void Start()
    {
        StartDayButton.OnStartDay += StartDayButton_OnStartDay;
        hide();
    }

    private void StartDayButton_OnStartDay(object sender, System.EventArgs e)
    {
        show();
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
