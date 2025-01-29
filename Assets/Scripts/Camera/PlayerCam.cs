using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerCam : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // desaparace el mouse
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        Debug.Log("Coloco rotation y pos de CM");

        Vector3 eulerRotation = this.transform.rotation.eulerAngles;
        //orientation.rotation = Quaternion.Euler(0, eulerRotation.y, 0);
        Player.LocalInstance.setRotation(Quaternion.Euler(0, eulerRotation.y, 0)); // pasarle la rot de la camara en cada frame al player
        Player.LocalInstance.setCMposition(this.transform.position); // pasarle la pos de la camara en cada frame al player
    }
}
