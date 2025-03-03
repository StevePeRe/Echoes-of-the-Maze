using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Se llama desde un cliente siempre
public class SpawnerObjectMazeManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsList;
    private GameObject gameObjectAux;
    //private Dictionary<Transform, bool> positions;
    private List<Transform> listPositions;
    private int objectsToSpawn;
    private int objectsSpawned;

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
        listPositions = new List<Transform>();
    }

    public void Start()
    {
        //objectsToSpawn = 40; // empieza con 60
        //positions = new Dictionary<Transform, bool>();
    }

    public void Update()
    {
    }

    public void spawnObjectsInMaze()
    {
        if (listPositions.Count <= 0 && objectsList.Count <= 0)
        {
            Debug.Log("No hay posiciones existentes u objetos en la lista");
            return;
        }

        Shuffle(listPositions);

        while (objectsSpawned < objectsToSpawn) 
        {
            gameObjectAux = getRandomWeightedItem(objectsList);

            GameObject spwObj = Instantiate(gameObjectAux, listPositions[objectsSpawned].position, listPositions[objectsSpawned].rotation);
            spwObj.GetComponent<NetworkObject>().Spawn(true);
            Debug.Log("nombre: " + spwObj.name);

            objectsSpawned++;

            if (objectsSpawned >= listPositions.Count) break; // evita errores al no haber mas posiciones que objetos a spawnear
        }

        resetValues();
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

    // Método para barajar una lista (Fisher-Yates Shuffle)
    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]); // Intercambio
        }
    }

    public void resetValues()
    {
        listPositions.Clear();
        objectsSpawned = 0;
    }

    public void addPositions(Transform pos)
    {
        //Debug.Log("Poss:" + pos.position);
        Debug.Log("Poss:" + listPositions.Count);
        listPositions.Add(pos); // por defecto false, ya que no hay objeto en esa pos
    }

    public void increaseObjectsToSpawn()
    {
        objectsToSpawn += 25;
    }

    public int getObjectsToSpawn()
    {
        return objectsToSpawn;
    }
}
