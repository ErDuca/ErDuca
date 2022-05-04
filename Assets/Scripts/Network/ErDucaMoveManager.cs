using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErDucaMoveManager : MonoBehaviour
{
    //Presa una lista di mosse, la posizione della pedina e la situazione attuale della board
    //(direttamente da ErDucaNetworkManager), restituisce una lista 
    //di coppie di indici (quelli sui quali la pedina potrà muoversi)

    //Settarla allo start (se sono host)
    private bool invertIndex = false;

    public List<Tuple<int, int>> GetAvailableMoves(uint netId, int i, int j, List<Movement> pieceRelativeMoves)
    {
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
                case Ptype.Walk:
                    temp = GetWalkMoves(netId, i, j, m._offsetX, m._offsetY);
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

                default:
                    return null;
            }
        }

        return availableMoves;
    }

    private Tuple<int, int> GetWalkMoves(uint netId, int i, int j, int xOffset, int yOffset)
    {
        if (invertIndex)
        {
            xOffset = -xOffset;
            yOffset = -yOffset;
        }

        //Bounds checking
        if (i + xOffset < 0 || i + xOffset > 5 || j + yOffset < 0 || j + yOffset > 5)
            return null;

        //Controllare se c'è una mia pedina nel posto dove devo andare
        if (ErDucaNetworkManager.singleton.GetMatrixIdAt(i + xOffset, j + yOffset) == netId)
            return null;

        Tuple<int, int> tupleToRet = new Tuple<int, int>(i + xOffset, j + yOffset);

        return tupleToRet;
    }

    private Tuple<int, int> GetJumpMoves(uint netId, int i, int j, int xOffset, int yOffset)
    {
        if (invertIndex)
        {
            xOffset = -xOffset;
            yOffset = -yOffset;
        }

        //Bounds checking
        if (i + xOffset < 0 || i + xOffset > 5 || j + yOffset < 0 || j + yOffset > 5)
            return null;

        //Controllare se c'è una mia pedina nel posto dove devo andare
        if (ErDucaNetworkManager.singleton.GetMatrixIdAt(i + xOffset, j + yOffset) == netId)
            return null;

        Tuple<int, int> tupleToRet = new Tuple<int, int>(i + xOffset, j + yOffset);

        return tupleToRet;
    }

    private Tuple<int, int> GetSlideMoves(uint netId, int i, int j, int xOffset, int yOffset, ref bool hasFoundEnemy,
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

        //Controllare se c'è una mia pedina nel posto dove devo andare
        if (ErDucaNetworkManager.singleton.GetMatrixIdAt(i + xOffset, j + yOffset) == netId)
            return null;

        //Ho trovato una pedina avversaria - Come lo dico allo switch sopra?
        if (ErDucaNetworkManager.singleton.GetMatrixIdAt(i + xOffset, j + yOffset) != netId &&
            ErDucaNetworkManager.singleton.GetMatrixIdAt(i + xOffset, j + yOffset) != 0)
        {
            hasFoundEnemy = true;
        }

        isl += xOffset;
        jsl += yOffset;
        
        Tuple<int, int> tupleToRet = new Tuple<int, int>(i + xOffset, j + yOffset);
        Debug.Log("Mossa disponibile = " + tupleToRet.Item1 + tupleToRet.Item2);
        Debug.Log("C'era un nemico? : " + hasFoundEnemy);

        return tupleToRet;
    }
}
