using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Chessman {

    bool underCheck = false;

    private void Start()
    {
        BoardManager.Instance.kingUnderAttack += Rook_kingUnderAttack;
        BoardManager.Instance.kingOK += Instance_kingOK;
    }

    private void Instance_kingOK(bool isKingWhite)
    {
        if(isWhite == isKingWhite)
        {
            underCheck = false;
        }
    }

    private void Rook_kingUnderAttack(bool isKingWhite)
    {
        if (isKingWhite == isWhite)
        {
            underCheck = true;
        }
    }

    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];

        Chessman c;
        int i;

        //right move
        i = CurrentX;
        while (true)
        {
            i++;
            if (i >= 8)
            {
                break;
            }

            c = BoardManager.Instance.Chessmen[i, CurrentY];
            if (c == null)
            {
                RookCheck(i, CurrentY, ref r);
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    RookCheck(i, CurrentY, ref r);
                }
                break;
            }
        }

        //left move
        i = CurrentX;
        while (true)
        {
            i--;
            if (i < 0)
            {
                break;
            }

            c = BoardManager.Instance.Chessmen[i, CurrentY];
            if (c == null)
            {
                RookCheck(i, CurrentY, ref r);
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    RookCheck(i, CurrentY, ref r);
                }
                break;
            }
        }

        //Up
        i = CurrentY;
        while (true)
        {
            i++;
            if (i >= 8)
            {
                break;
            }

            c = BoardManager.Instance.Chessmen[CurrentX, i];
            if (c == null)
            {
                RookCheck(CurrentX, i, ref r);
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    RookCheck(CurrentX, i, ref r);
                }
                break;
            }
        }

        //Down
        i = CurrentY;
        while (true)
        {
            i--;
            if (i < 0)
            {
                break;
            }
            c = BoardManager.Instance.Chessmen[CurrentX, i];
            if (c == null)
            {
                RookCheck(CurrentX, i, ref r);
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    RookCheck(CurrentX, i, ref r);
                }
                break;
            }
        }
        return r;
    }

    private void RookCheck(int x, int y, ref bool[,] r)
    {
        BoardManager.Instance.selectedChessman = BoardManager.Instance.Chessmen[this.CurrentX, this.CurrentY];
        BoardManager.Instance.PieceCheckChangePosition(x, y);
        BoardManager.Instance.PieceCheckTest(isWhite);
        if (underCheck)
        {
            r[x, y] = false;
        }
        else
        {
            r[x, y] = true;
        }
        BoardManager.Instance.PieceKingIsOk(isWhite);
        BoardManager.Instance.PieceCheckUndoLastMove(x, y);
    }
}
