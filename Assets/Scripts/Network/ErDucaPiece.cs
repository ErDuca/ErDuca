using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ErDucaPiece : NetworkBehaviour
{
    [SerializeField]
    [SyncVar]private uint _myPlayerNetId;

    public uint MyPlayerNetId
    {
        get => _myPlayerNetId;
        set
        {
            _myPlayerNetId = value;
        }
    }
}
