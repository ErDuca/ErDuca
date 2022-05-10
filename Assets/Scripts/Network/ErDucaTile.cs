using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ErDucaTile : NetworkBehaviour
{
    private Material _material;
    private Color _originalColor;

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
        _material = GetComponent<MeshRenderer>().material;
        _originalColor = _material.color;
    }
    public void SetMaterialColor(Color color)
    {
        _material.color = color;
    }
    public void SetOriginalColor()
    {
        _material.color = _originalColor;
    }
}
