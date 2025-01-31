using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour
{
    // limpiar el buffer para los eventos statics que al cambiar de escena puede dar fallos
    private void Awake()
    {
        Player.ResetStaticData();
    }
}