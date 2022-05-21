using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Given a list of different moves, the position of a piece and the current board values,
//return a list of indexes (where the piece will be able to move)
public class ErDucaMoveManager : MonoBehaviour
{
    private bool invertIndex = false;

    public Dictionary<Tuple<int, int>, Ptype> GetAvailableMoves(int netId, int i, int j, List<Movement> pieceRelativeMoves)
    {
        //"Host situation" (the board is seen upside down)
        if (netId == 1)
        {
            invertIndex = true;
        }

        Dictionary<Tuple<int, int>, Ptype> availableMoves = new Dictionary<Tuple<int, int>, Ptype>();
        Tuple<int, int> temp;

        foreach (Movement m in pieceRelativeMoves)
        {
            Debug.Log("Analizzo una mossa di tipo " + m._mType);
            switch (m._mType)
            {
                case Ptype.Walk:
                    temp = GetWalkMoves(netId, i, j, m._offsetX, m._offsetY);
                    if (temp != null)
                        availableMoves.Add(temp, Ptype.Walk);
                    break;

                case Ptype.Jump:
                    temp = GetJumpMoves(netId, i, j, m._offsetX, m._offsetY);
                    if (temp != null)
                        availableMoves.Add(temp, Ptype.Jump);
                    break;

                case Ptype.Slide:
                    int isl = i; 
                    int jsl = j;
                    bool foundEnemyPiece = false;
                    do
                    {
                        temp = GetSlideMoves(netId, isl, jsl, m._offsetX, m._offsetY, ref foundEnemyPiece, ref isl, ref jsl);

                        if (temp != null)
                            availableMoves.Add(temp, Ptype.Slide);
                    }
                    while (temp != null && !foundEnemyPiece);
                    break;

                //TEMPORARY LIKE JUMP (DEBUGGING)!
                case Ptype.Fly:
                    temp = GetJumpMoves(netId, i, j, m._offsetX, m._offsetY);
                    if (temp != null)
                        availableMoves.Add(temp, Ptype.Fly);
                    break;

                //(DEBUGGING)!
                case Ptype.Strike:
                    temp = GetStrikeMoves(netId, i, j, m._offsetX, m._offsetY);
                    if (temp != null)
                        availableMoves.Add(temp, Ptype.Strike);
                    break;

                default:
                    return null;
            }
        }

        return availableMoves;
    }

    //APPARENTLY WORKING
    private Tuple<int, int> GetWalkMoves(int netId, int i, int j, int xOffset, int yOffset)
    {
        if (invertIndex)
        {
            xOffset = -xOffset;
            yOffset = -yOffset;
        }

        int absoluteDirX = Math.Sign(xOffset);
        int absoluteDirY = Math.Sign(yOffset);

        Debug.Log("Direzione X = " + absoluteDirX);
        Debug.Log("Direzione Y = " + absoluteDirY);

        //Bounds checking
        if (i + xOffset < 0 || i + xOffset > 5 || j + yOffset < 0 || j + yOffset > 5)
        {
            Debug.Log("Out of Bounds");
            return null;
        }

        //Checking if there's a piece of mine, in the tile i want to go
        if (ErDucaNetworkManager.singleton.GetMatrixIdAt(i + xOffset, j + yOffset) == netId)
        {
            Debug.Log("Nella mia destinazione finale c'è una mia pedina");
            return null;
        }

        for(int x = i + absoluteDirX, y = j + absoluteDirY; x <= 5 && x >= 0 && y <= 5 && y >= 0; x += absoluteDirX, y += absoluteDirY)
        {
                Debug.Log("Controllo se posso muovermi in: " + x + " " + y);

                if ((x == i + xOffset && y == j + yOffset))
                {
                    Debug.Log("Sono dovevo volevo essere, aggiungo la mossa");
                    Tuple<int, int> tupleToRet = new Tuple<int, int>(i + xOffset, j + yOffset);

                    return tupleToRet;
                }

                if (ErDucaNetworkManager.singleton.GetMatrixIdAt(x, y) != 0)
                {
                    Debug.Log("In " + x + " " + y + " c'è qualcuno, ritorno Null");
                    return null;
                }
        }

        return null;
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

    private Tuple<int, int> GetStrikeMoves(int netId, int i, int j, int xOffset, int yOffset)
    {
        if (invertIndex)
        {
            xOffset = -xOffset;
            yOffset = -yOffset;
        }

        //Bounds checking
        if (i + xOffset < 0 || i + xOffset > 5 || j + yOffset < 0 || j + yOffset > 5)
            return null;

        //Checking if there's a piece of mine, where i want to shoot/strike
        if (ErDucaNetworkManager.singleton.GetMatrixIdAt(i + xOffset, j + yOffset) == netId)
            return null;

        //Checking if there's an enemy piece, where i want to shoot/strike
        if (ErDucaNetworkManager.singleton.GetMatrixIdAt(i + xOffset, j + yOffset) != 0)
        {
            Tuple<int, int> tupleToRet = new Tuple<int, int>(i + xOffset, j + yOffset);

            return tupleToRet;
        }

        return null;
    }
}
