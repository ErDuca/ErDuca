using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    //[SerializeField] private float mTileSize = 1f;
    [SerializeField] private Material mTileMaterial;

    private const int mBoardHeight = 6;
    private const int mBoardWidth = 6;
    private Vector3 mBounds;
    private Vector2 mBoardCenter = Vector2.zero;

    public GridLayout mBoard;
    
    public GameObject[,] mBoardTiles = new GameObject [mBoardHeight, mBoardWidth];

    void Start() {
        int x = 0;
        int y = 0;
        foreach (Transform child in transform) {
            if (x == mBoardHeight) {
                y++;
                x = 0;
            }

            mBoardTiles[x, y] = child.gameObject;
            Tile current = mBoardTiles[x, y].GetComponent<Tile>();
            current.mCoordX = x;
            current.mCoordY = y;
            x++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveUnit(GameObject point, Piece unit) {
        Tile destinationTile = point.GetComponent<Tile>();
        unit.transform.parent = null;
        unit.transform.SetParent(destinationTile.transform);

        // C'è un'unità avversaria (avremo fatto prima il check per non farlo muovere se ci sono unità alleate)
        if (point.transform.childCount > 0) {
            Piece enemyUnit = point.transform.GetChild(0).gameObject.GetComponent<Piece>();
            KillUnit(enemyUnit);
        }
    }

    public void KillUnit(Piece unit) {
        Destroy(unit);
    }

    public void SpawnUnit(GameObject spawn, Piece unit) {
        Tile spawningTile = spawn.GetComponent<Tile>();
        Piece spawnedUnit = Instantiate(unit, new Vector3(0, 0, 0), Quaternion.identity);
        spawnedUnit.transform.SetParent(spawningTile.transform);
        //spawnedUnit.transform.parent = transform;
        spawnedUnit.transform.localPosition = Vector3.zero;
    }

    
}
