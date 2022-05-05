using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ErDucaTile : NetworkBehaviour
{
    private Material material;
    private Color originalColor;

    [SerializeField]
    [SyncVar]private int _i;
    [SerializeField]
    [SyncVar]private int _j;
    public int I
    {
        get => _i;
        set
        {
                _i = value;
        }
    }
    public int J
    {
        get => _j;
        set
        {
            _j = value;
        }
    }

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        originalColor = material.color;
    }
    public void SetMaterialColor(Color color)
    {
        material.color = color;
    }
    public void SetOriginalColor()
    {
        material.color = originalColor;
    }
}
