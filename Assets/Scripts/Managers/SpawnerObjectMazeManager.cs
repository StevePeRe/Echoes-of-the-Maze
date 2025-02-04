using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnerObjectMazeManager : MonoBehaviour
{

    //[SerializeField] private List<GameObject> objectsList;
    //private GameObject gameObjectAux;
    //private Dictionary<Transform, bool> positions;
    //private int objectsToSpawn;
    //private int objectsSpawned;

    //public static SpawnerObjectMazeManager instance { get; private set; }

    //private void Awake()
    //{
    //    if (instance != null)
    //    {
    //        Debug.LogError("SpawnerObjectMazeManager Instance already exist");
    //    }
    //    instance = this;
    //}

    //public void Start()
    //{
    //    objectsToSpawn = 40; // empieza con 60
    //    positions = new Dictionary<Transform, bool>();
    //}

    //public void Update()
    //{
    //}

    //public void resetListPositions()
    //{
    //    positions.Clear();
    //}

    //public void addPositions(Transform position)
    //{
    //    //Debug.Log("Poss:" + position.position);
    //    positions.Add(position, false); // por defecto false, ya que no hay objeto en esa pos
    //}

    //public void spawnObjectsInMaze()
    //{
    //    if (positions.Count <= 0)
    //    {
    //        Debug.Log("No hay posiciones existentes");
    //        return;
    //    }

    //    //añadir objetos aleatoriamente en el labertinto // se llamara al empezar el dia // la creacion del laberinto se hace al empezar el juego
    //    var listKeys = positions.Keys.ToList();

    //    while (objectsSpawned < objectsToSpawn)
    //    {
    //        int i = Random.Range(0, listKeys.Count);
    //        if (!positions[listKeys[i]])
    //        {
    //            gameObjectAux = getRandomWeightedItem(objectsList);
    //            Debug.Log("El transform es: " + listKeys[i].position);
    //            positions[listKeys[i]] = true;
    //            Instantiate(gameObjectAux, listKeys[i].position, listKeys[i].rotation).SetActive(true);
    //            objectsSpawned++;
    //        }
    //    }
    //}

    //private GameObject getRandomWeightedItem(List<GameObject> items)
    //{
    //    float totalWeight = 0;
    //    // Suma todos los pesos
    //    foreach (var weightedItem in items)
    //    {
    //        totalWeight += weightedItem.GetComponent<ICollectable>().WeigthObject;
    //    }
    //    // Genera un número aleatorio entre 0 y el peso total
    //    float randomValue = Random.Range(0, totalWeight);
    //    float cumulativeWeight = 0;
    //    // Encuentra el ítem correspondiente al número aleatorio
    //    foreach (var weightedItem in items)
    //    {
    //        cumulativeWeight += weightedItem.GetComponent<ICollectable>().WeigthObject;
    //        if (randomValue < cumulativeWeight)
    //        {
    //            return weightedItem;
    //        }
    //    }
    //    // En caso de que algo falle (no debería), devuelve el primer ítem como fallback
    //    return items[0];
    //}

    //public void increaseObjectsToSpawn()
    //{
    //    objectsToSpawn += 25;
    //}

    //public int getObjectsToSpawn()
    //{
    //    return objectsToSpawn;
    //}
}
