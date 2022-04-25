using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ErDucaTile : NetworkBehaviour
{
    [SyncVar] private bool isOccupied = false;
}
