using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Chessman : MonoBehaviour {

    protected void RaiseKingUnderAttack(bool isKingWhite)
    {
        BoardManager.Instance.KingUnderAttackBoardManager(isKingWhite);
    }

    public int CurrentX { set; get; }
    public int CurrentY { set; get; }
    public bool isWhite;
    public bool firstTurn = true;
    public bool enPassantLeft;
    public bool enPassantRight;

    public void SetPosition(int x, int y)
    {
        CurrentX = x;
        CurrentY = y;
    }

    public virtual bool[,] PossibleMove()
    {
        return new bool [8,8];
    }

    public virtual bool[,] CheckTest()
    {
        return new bool[8, 8];
    }

    public virtual bool[,] GameCheckTest()
    {
        return new bool[8, 8];
    }
}
