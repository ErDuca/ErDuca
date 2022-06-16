using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ErDucaTile : NetworkBehaviour
{
    private Material material;
    private Color originalColor;

    [SerializeField]
    [SyncVar]private int i;
    [SerializeField]
    [SyncVar]private int j;

    public int I
    {
        get => i;
        set
        {
                i = value;
        }
    }
    public int J
    {
        get => j;
        set
        {
            j = value;
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
