using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

// Se llama desde un cliente siempre
public class SpawnerObjectMazeManager : MonoBehaviour
{
    [Header("Lista de Objetos")]
    [SerializeField] private List<GameObject> objectsList;

    [Header("Lista de Notas")]
    [SerializeField] private List<GameObject> notesList;

    [SerializeField] private GameObject enemyWarden;

    private GameObject gameObjectAux;
    private List<Transform> listObjectPositions;
    private List<Transform> listNotePositions;
    private int objectsToSpawn;
    private int objectsSpawned;

    public event EventHandler OnCompletedWayPointsList;

    public static SpawnerObjectMazeManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("SpawnerObjectMazeManager Instance already exist");
        }
        instance = this;
        objectsToSpawn = 40; // empieza con 60
        objectsSpawned = 0;
        listObjectPositions = new List<Transform>();
        listNotePositions   = new List<Transform>();
    }

    public void Start()
    {
    }

    public void Update()
    {
    }

    // Siempre lo llama solo el server
        // OBJECTS
    public void spawnObjectsInMaze()
    {
        if (listObjectPositions.Count <= 0 || objectsList.Count <= 0)
        {
            Debug.Log("No hay posiciones existentes u objetos en la lista");
            return;
        }

        Shuffle(listObjectPositions);

        while (objectsSpawned < objectsToSpawn) 
        {
            gameObjectAux = getRandomWeightedItem(objectsList);
            //Debug.Log("objectsSpawned: " + objectsSpawned);
            GameObject spwObj = Instantiate(gameObjectAux, listObjectPositions[objectsSpawned].position, listObjectPositions[objectsSpawned].rotation);
            spwObj.GetComponent<NetworkObject>().Spawn(true);
            objectsSpawned++;

            if (objectsSpawned >= listObjectPositions.Count) break; // evita errores al no haber mas posiciones que objetos a spawnear
        }

        resetObjectValues();
    }

    private GameObject getRandomWeightedItem(List<GameObject> items)
    {
        float totalWeight = 0;
        // Suma todos los pesos
        foreach (var weightedItem in items)
        {
            totalWeight += weightedItem.GetComponent<ICollectable>().WeigthObject;
        }
        // Genera un número aleatorio entre 0 y el peso total
        float randomValue = UnityEngine.Random.Range(0, totalWeight);
        float cumulativeWeight = 0;
        // Encuentra el ítem correspondiente al número aleatorio
        foreach (var weightedItem in items)
        {
            cumulativeWeight += weightedItem.GetComponent<ICollectable>().WeigthObject;
            if (randomValue < cumulativeWeight)
            {
                return weightedItem;
            }
        }
        // En caso de que algo falle (no debería), devuelve el primer ítem como fallback
        return items[0];
    }

    private void resetObjectValues()
    {
        listObjectPositions.Clear();
        objectsSpawned = 0;
    }

    public void addObjectPositions(Transform posObjects)
    {
        //Debug.Log("Poss:" + listObjectPositions.Count);
        listObjectPositions.Add(posObjects);
    }

    public void increaseObjectsToSpawn()
    {
        objectsToSpawn += 25;
    }

    public int getObjectsToSpawn()
    {
        return objectsToSpawn;
    }


        // NOTES
    public void spawnNotesInMaze()
    {
        if (listNotePositions.Count <= 0 || notesList.Count <= 0)
        {
            Debug.Log("No hay posiciones existentes u notas que colocar en la lista");
            return;
        }

        Shuffle(listNotePositions); // para que las notas nunca spawneen en el mismo sitio

        
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
        int vertexIndex = UnityEngine.Random.Range(0, triangulation.vertices.Length);
        //Debug.Log("vertexIndex: " + vertexIndex + ", triangulation.vertices.Length:" + triangulation.vertices.Length);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(triangulation.vertices[vertexIndex], out hit, 2f, -1))
        {
            enemyWarden.GetComponent<NavMeshAgent>().Warp(hit.position); // Coloca el enemigo en una posición válida
            //Debug.Log("Colocado en una pos validaal agente");
            //OnWardenPositioned?.Invoke(this, EventArgs.Empty);
        }

        GameObject enemySpawn = Instantiate(enemyWarden);
        enemySpawn.GetComponent<NetworkObject>().Spawn(true);

        for (int i = 0; i < notesList.Count; i++)
        {
            if(i >= listNotePositions.Count) break; // evita errores al no haber mas posiciones que objetos a spawnear
            GameObject objSpawn = Instantiate(notesList[i], listNotePositions[i].position, listNotePositions[i].rotation); // con la rotacion de la lista, mas importante tener en cuenta en las notas
            objSpawn.GetComponent<NetworkObject>().Spawn(true);

            // le añado el waypoint a la lista de waypoints para deambular
            if (enemySpawn.GetComponent<WardenBehaviour>())
            {
                enemySpawn.GetComponent<WardenBehaviour>().addWayPoint(listNotePositions[i].position);
                //Debug.Log("Size ListWayPoints: " + enemySpawn.GetComponent<WardenBehaviour>().getSizeListWayPoints());
            }

        }

        if (enemySpawn.GetComponent<WardenBehaviour>().getSizeListWayPoints() > 0)
        {
            // lanzo el evento para que el warden empiece a deambular
            StartCoroutine(waitAndSubscribe()); // ya que al instanciar en la misma funcion, no le da tiempo a escuchar el evento lanzado
        }

        resetNoteValues();
    }

    public void addNotePositions(Transform posNotes)
    {
        Debug.Log("Poss Notes:" + listNotePositions.Count);
        listNotePositions.Add(posNotes);
    }

    private void resetNoteValues()
    {
        listNotePositions.Clear();
        //objectsSpawned = 0;
    }

        // WAYPOINTS


    // necesario para GameObjects que se crean en el mismo momento en el que se lanza un evento, en este caso el enemyWarden
    private IEnumerator waitAndSubscribe()
    {
        // Espera un ciclo de ejecución
        yield return null;

        // Ahora se suscribe al evento
        OnCompletedWayPointsList?.Invoke(this, EventArgs.Empty);
    }

    private void spawnEnemyWarden()
    {
        //NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
        //int vertexIndex = UnityEngine.Random.Range(0, triangulation.vertices.Length);

        //NavMeshHit hit;
        //if (NavMesh.SamplePosition(triangulation.vertices[vertexIndex], out hit, 2f, 0))
        //{
        //    enemyWarden.GetComponent<NavMeshAgent>().Warp(hit.position); // Coloca el enemigo en una posición válida
        //    GameObject enemySpawn = Instantiate(enemyWarden, listNotePositions[0].position, enemyWarden.transform.rotation); // con la rotacion de la lista, mas importante tener en cuenta en las notas
        //    enemySpawn.GetComponent<NetworkObject>().Spawn(true);
        //    OnWardenPositioned?.Invoke(this, EventArgs.Empty);
        //}


        // spawn 1 enemigo Warden
        //GameObject enemySpawn = Instantiate(enemyWarden, listNotePositions[0].position, enemyWarden.transform.rotation); // con la rotacion de la lista, mas importante tener en cuenta en las notas
        //enemySpawn.GetComponent<NetworkObject>().Spawn(true);

        //WardenBehaviour wardenBeh = enemySpawn.GetComponent<WardenBehaviour>();
    }

        // Método para barajar una lista (Fisher-Yates Shuffle)
    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]); // Intercambio
        }
    }
}
