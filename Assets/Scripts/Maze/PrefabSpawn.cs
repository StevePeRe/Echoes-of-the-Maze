using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Se llama desde un cliente siempre
public class PrefabSpawn : MonoBehaviour
{
    [SerializeField] private List<Transform> positions;

    // Start is called before the first frame update
    private void Awake()
    {
        if (positions.Count <= 0 || SpawnerObjectMazeManager.instance == null) return;

        //// llama al spawner object maze para anadir las posiciones donde se van a generar los objetos aleatorios
        for (int i = 0; i < positions.Count; i++)
        {
            //Debug.Log("tam pos: " + positions.Count);
            SpawnerObjectMazeManager.instance.addPositions(positions[i]);
        }
    }
}
