using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectable
{
    string Name { get; }
    Sprite Image { get; }
    int CostObject { get; }

    int WeigthObject { get; }

    public void CollectItem(Transform transf); // recogerlo

    public void DropItem(); // soltarlo

    public void UseItem(bool use); // usar el item

    public void setActive(bool active); // visualizacion en HUD

    public void setDestruction(); // destruir el item
}
