using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Given a list of different moves, the position of a piece and the current board values,
//return a list of indexes (where the piece will be able to move)
public class ErDucaMoveManager : MonoBehaviour
{
    private bool invertIndex = false;

    public List<Tuple<int, int>> GetAvailableMoves(int netId, int i, int j, List<Movement> pieceRelativeMoves)
    {
        //"Host situation" (the board is seen upside down)
        if (netId == 1)
        {
            invertIndex = true;
        }

        List<Tuple<int, int>> availableMoves = new List<Tuple<int, int>>();
        Tuple<int, int> temp;

        foreach (Movement m in pieceRelativeMoves)
        {
            switch (m._mType)
            {
                //BROKEN! DA MODIFICARE; IO VADO Lì SE POSSO ANDARCI!
                //TEMPORARY LIKE JUMP (DEBUGGING)!
                case Ptype.Walk:
                    /*
                    temp = GetWalkMoves(netId, i, j, m._offsetX, m._offsetY);
                    if (temp != null)
                        availableMoves.Add(temp);
                    break;
                    */
                    temp = GetJumpMoves(netId, i, j, m._offsetX, m._offsetY);
                    if (temp != null)
                        availableMoves.Add(temp);
                    break;

                case Ptype.Jump:
                    temp = GetJumpMoves(netId, i, j, m._offsetX, m._offsetY);
                    if (temp != null)
                        availableMoves.Add(temp);
                    break;

                case Ptype.Slide:
                    int isl = i; 
                    int jsl = j;
                    bool foundEnemyPiece = false;
                    do
                    {
                        temp = GetSlideMoves(netId, isl, jsl, m._offsetX, m._offsetY, ref foundEnemyPiece, ref isl, ref jsl);

                        if (temp != null)
                            availableMoves.Add(temp);
                    }
                    while (temp != null && !foundEnemyPiece);
                    break;

                //TEMPORARY LIKE JUMP (DEBUGGING)!
                case Ptype.Fly:
                    temp = GetJumpMoves(netId, i, j, m._offsetX, m._offsetY);
                    if (temp != null)
                        availableMoves.Add(temp);
                    break;

                //TEMPORARY LIKE JUMP (DEBUGGING)!
                case Ptype.Strike:
                    temp = GetJumpMoves(netId, i, j, m._offsetX, m._offsetY);
                    if (temp != null)
                        availableMoves.Add(temp);
                    break;

                default:
                    return null;
            }
        }

        return availableMoves;
    }

    //BROKEN
    private Tuple<int, int> GetWalkMoves(int netId, int i, int j, int xOffset, int yOffset)
    {
        if (invertIndex)
        {
            xOffset = -xOffset;
            yOffset = -yOffset;
        }

        //Bounds checking
        if (i + xOffset < 0 || i + xOffset > 5 || j + yOffset < 0 || j + yOffset > 5)
            return null;

        //Checking if there's a piece of mine, in the tile i want to go
        if (ErDucaNetworkManager.singleton.GetMatrixIdAt(i + xOffset, j + yOffset) == netId)
            return null;

        Tuple<int, int> tupleToRet = new Tuple<int, int>(i + xOffset, j + yOffset);

        return tupleToRet;
    }

    private Tuple<int, int> GetJumpMoves(int netId, int i, int j, int xOffset, int yOffset)
    {
        if (invertIndex)
        {
            xOffset = -xOffset;
            yOffset = -yOffset;
        }

        //Bounds checking
        if (i + xOffset < 0 || i + xOffset > 5 || j + yOffset < 0 || j + yOffset > 5)
            return null;

        //Checking if there's a piece of mine, in the tile i want to go
        if (ErDucaNetworkManager.singleton.GetMatrixIdAt(i + xOffset, j + yOffset) == netId)
            return null;

        Tuple<int, int> tupleToRet = new Tuple<int, int>(i + xOffset, j + yOffset);

        return tupleToRet;
    }

    private Tuple<int, int> GetSlideMoves(int netId, int i, int j, int xOffset, int yOffset, ref bool hasFoundEnemy,
        ref int isl, ref int jsl)
    {
        if (invertIndex)
        {
            xOffset = -xOffset;
            yOffset = -yOffset;
        }

        //Bounds checking
        if (i + xOffset < 0 || i + xOffset > 5 || j + yOffset < 0 || j + yOffset > 5)
            return null;

        //Checking if there's a piece of mine, in the tile i want to go
        if (ErDucaNetworkManager.singleton.GetMatrixIdAt(i + xOffset, j + yOffset) == netId)
            return null;

        //Found an enemy piece
        if (ErDucaNetworkManager.singleton.GetMatrixIdAt(i + xOffset, j + yOffset) != netId &&
            ErDucaNetworkManager.singleton.GetMatrixIdAt(i + xOffset, j + yOffset) != 0)
        {
            hasFoundEnemy = true;
        }

        isl += xOffset;
        jsl += yOffset;
        
        Tuple<int, int> tupleToRet = new Tuple<int, int>(i + xOffset, j + yOffset);

        return tupleToRet;
    }
}
