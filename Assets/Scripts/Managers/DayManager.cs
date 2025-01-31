//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class DayManager : MonoBehaviour
//{

//    // quitar de que sea un singleton, y cuando cambie de dia avisar con un evento a todos los que quieran enterarse. mas optimo
//    public event EventHandler OnGoToNextDay;

//    private const int maxDaysToPlay = 3;
//    private int actualDaysPlaying;

//    private const float maxTimePerDay = 1500f; // 25min todo el dia
//    private float gamePlayingTimer;

//    //private bool goToNextDay; // al dormir para pasar directamente al siguiente dia

//    private void Awake()
//    {
        
//    }

//    // Start is called before the first frame update
//    void Start()
//    {
//        //goToNextDay = false;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (!MazeGameManager.instance.getGamePlaying()) return;

//        gamePlayingTimer += Time.deltaTime;
//        //Debug.Log("Tiempo pasado: " + gamePlayingTimer);

//        // cuando legue al limite, todos los que no esten en el refugio mueren

//        if (gamePlayingTimer >= maxTimePerDay/* || goToNextDay*/)
//        {
//            OnGoToNextDay?.Invoke(this, EventArgs.Empty); // avisa a todos los que esten suscritos una solo vez que ha pasado la jornada
            
//            //llamar a estos metodos dentro del script del refugio que es donde se vera si estan todos dentro
//            MazeGameManager.instance.setGeneratePreMaze(); // reiniciar dia
//            SpawnerObjectMazeManager.instance.resetListPositions(); // resetaer el disc de transform para colocarlos los objetos
//            // si todos los jugadores no estan en el refugio(crear script de refugio y que le llegue la notificacion) mueren

//            // - si se acaba el tiempo se pasa directamente al siguiente dia, viendo quienes estan dentro del refugio y quienes no y comprobando si se ha cumplido la cuota
//            // - si han cumplido la cuota y quieren acabar antes el dia, pueden hacerlo pulsando el boton de nuevo para acabarlo
//            // - no podran pulsar el boton de nuevo a menos que hayan cumplido la quota
//            // - se llevan un plus si cumplen la cuota mucho antes
 
//        }
//}

//    // metodo de escucha al buyer por si se ha cumplido la cuota, asi borra todo y reincia el dia antes de que acabe el tiempo

//    public void resetDayTimer()
//    {
//        gamePlayingTimer = 0;
//    }

//    public void resetWorkingDays()
//    {
//        actualDaysPlaying = 0;
//    }
//    public void addActualDaysPlaying()
//    {
//        actualDaysPlaying++;
//    }
//}


////    resetDayTimer();
////    addActualDaysPlaying();
////    //Debug.Log("Se ha acabado el dia, pasando al siguiente"); // si pasa avisarle l mazegameManager para uue haga lo debido

////    if(actualDaysPlaying >= maxDaysToPlay) { 
////        resetWorkingDays();
////        //Debug.Log("Pasando a la sigueinte jornada");
////        // le sumo dinero a la cuota
////    }