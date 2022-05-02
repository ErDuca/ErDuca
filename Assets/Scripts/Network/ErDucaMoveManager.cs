using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErDucaMoveManager : MonoBehaviour
{
    //Presa una lista di mosse, la posizione della pedina e la situazione attuale della board
    //(direttamente da ErDucaNetworkManager), restituisce una lista 
    //di coppie di indici (quelli sui quali la pedina potrà muoversi)

    public List<Tuple<int, int>> GetAvailableMoves(uint netId, int i, int j, List<Movement> pieceRelativeMoves)
    {
        List<Tuple<int, int>> availableMoves = new List<Tuple<int, int>>();

        foreach (Movement m in pieceRelativeMoves)
        {
            if (m._mType.Equals(Ptype.Walk))
            {
                Tuple<int, int> temp = GetWalkMoves(netId, i, j, m._OffsetX, m._OffsetY);
                if(temp != null)
                    availableMoves.Add(temp);
            }
        }

        return availableMoves;
    }

    private Tuple<int, int> GetWalkMoves(uint netId, int i, int j, int xOffset, int yOffset)
    {
        //Bounds checking
        if (i + xOffset < 0 || i + xOffset > 5 || j + yOffset < 0 || j + yOffset > 5)
            return null;

        //Controllare se c'è una mia pedina nel posto dove devo andare
        if (ErDucaNetworkManager.singleton.getMatrixIdAt(i + xOffset, j + yOffset) == netId)
            return null;

        Tuple<int, int> tupleToRet = new Tuple<int, int>(i + xOffset, j + yOffset);

        return tupleToRet;
    }
}
