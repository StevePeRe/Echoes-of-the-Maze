using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// clase para gestionar los mensajes mostrados al jugador con todos los objetos interactuables
public interface IMessageInteraction
{
    public string getMessageToShow();
}
