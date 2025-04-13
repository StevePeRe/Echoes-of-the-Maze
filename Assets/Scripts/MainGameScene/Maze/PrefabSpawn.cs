using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Se llama desde un cliente siempre
public class PrefabSpawn : MonoBehaviour
{
    [Header("Spawn de Objetos")]
    [SerializeField] private List<Transform> objectPositions;

    [Header("Spawn de notas")]
    [SerializeField] private List<Transform> notePositions;

    // Start is called before the first frame update
    private void Awake()
    {
        if (SpawnerObjectMazeManager.instance == null) return;

        //// llama al spawner object maze para anadir las posiciones donde se van a generar los objetos aleatorios
        if(objectPositions.Count > 0)
        {
            for (int i = 0; i < objectPositions.Count; i++)
            {
                SpawnerObjectMazeManager.instance.addObjectPositions(objectPositions[i]);
            }
        }

        if (notePositions.Count > 0)
        {
            for (int i = 0; i < notePositions.Count; i++)
            {
                SpawnerObjectMazeManager.instance.addNotePositions(notePositions[i]);
            }
        }
    }
}
