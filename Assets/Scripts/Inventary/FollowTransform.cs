using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    private Transform targetTransform;

    public void SetTargetTransform(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }

    // para que se haga al final de cada frame
    // ya que son netwokr objects, no se le puede asignar el parent como antes, asi que tiene que seguirlo en ejecucion
    private void LateUpdate()
    {
        if(targetTransform == null) return;

        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }
}
