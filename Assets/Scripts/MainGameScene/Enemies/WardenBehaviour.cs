using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WardenBehaviour : MonoBehaviour, IDamageable
{
    private NavMeshAgent agent;
    private List<Vector3> wayPoints;
    private Vector3 lastWayPoint;

    // Interfaces
    [SerializeField] private int _health = 100;
    public int Health
    {
        get { return _health; }
        set { _health = Mathf.Clamp(value, 0, 100); }
    }

    private void Awake()
    {
        // Get the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();
        wayPoints = new List<Vector3>();
        lastWayPoint = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnerObjectMazeManager.instance.OnCompletedWayPointsList += SpawnerObjectMazeManager_OnCompletedWayPointsList;
    }

    private void SpawnerObjectMazeManager_OnCompletedWayPointsList(object sender, System.EventArgs e)
    {
        //Debug.Log("Empiezo la corrutina AAAAAAAAAAA");
        StartCoroutine(wardenWanderBehaviour());
    }

    // crear una state mnachine sencillla para gestionar el comportameinto del warden

    private IEnumerator wardenWanderBehaviour()
    {
        // Selecciona un waypoint inicial
        Vector3 targetWayPoint = wayPoints[Random.Range(0, wayPoints.Count)];
        agent.SetDestination(targetWayPoint);

        while (true)
        {
            // Si el agente ha llegado al destino
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                // Selecciona un nuevo waypoint
                Vector3 newWayPoint;
                do
                {
                    newWayPoint = wayPoints[Random.Range(0, wayPoints.Count)];
                } while (newWayPoint == targetWayPoint); // Evita seleccionar el mismo waypoint consecutivamente

                targetWayPoint = newWayPoint;
                agent.SetDestination(targetWayPoint);
                Debug.Log("Nuevo destino: " + targetWayPoint);
            }

            // ver si ve a algun player
            // VER la  idea de maquina de estado pero dentro del IEnumerator, no lo habia pensado que podia estar aqui dentro
            // anayadir fase de ataque, que si esta demasiado cerca el jugador que lo pille para matarlo
            //. pero si recibe danyo se cmbio a fase de seguir al que le hace danyo

            // Continúa moviéndose hacia el destino actual
            yield return null; // Espera al siguiente frame para reevaluar
        }
    }

    private void detectPlayer()
    {

    }

    public void addWayPoint(Vector3 wayPoint)
    {
        // Add a waypoint to the list
        wayPoints.Add(wayPoint);
    }

    public int getSizeListWayPoints()
    {
        // Get the number of waypoints in the list
        return wayPoints.Count;
    }

    public void dealtDamage(int damage)
    {
        Health -= damage;
        Debug.Log("vida actual Warden: " + Health);
    }
}
