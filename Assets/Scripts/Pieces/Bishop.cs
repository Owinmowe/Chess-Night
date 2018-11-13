﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Chessman {

    bool underCheck = false;

    private void Start()
    {
        BoardManager.Instance.kingUnderAttack += Bishop_kingUnderAttack;
        BoardManager.Instance.kingOK += Instance_kingOK;
    }

    private void Instance_kingOK(bool isKingWhite)
    {
        if(isWhite == isKingWhite)
        {
            underCheck = false;
        }
    }

    private void Bishop_kingUnderAttack(bool isKingWhite)
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
        int i, j;

        //Top Left
        i = CurrentX;
        j = CurrentY;
        while (true)
        {
            i--;
            j++;
            if (i < 0 || j >= 8)
            {
                break;
            }
            c = BoardManager.Instance.Chessmen[i, j];
            if (c == null)
            {
                BishopCheck(i, j, ref r);
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    BishopCheck(i, j, ref r);
                }
                break;
            }
        }

        //Top Right
        i = CurrentX;
        j = CurrentY;
        while (true)
        {
            i++;
            j++;
            if (i >= 8 || j >= 8)
            {
                break;
            }
            c = BoardManager.Instance.Chessmen[i, j];
            if (c == null)
            {
                BishopCheck(i, j, ref r);
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    BishopCheck(i, j, ref r);
                }
                break;
            }
        }

        //Down Left
        i = CurrentX;
        j = CurrentY;
        while (true)
        {
            i--;
            j--;
            if (i < 0 || j < 0)
            {
                break;
            }
            c = BoardManager.Instance.Chessmen[i, j];
            if (c == null)
            {
                BishopCheck(i, j, ref r);
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    BishopCheck(i, j, ref r);
                }
                break;
            }
        }

        //Down Right
        i = CurrentX;
        j = CurrentY;
        while (true)
        {
            i++;
            j--;
            if (i >= 8 || j < 0)
            {
                break;
            }
            c = BoardManager.Instance.Chessmen[i, j];
            if (c == null)
            {
                BishopCheck(i, j, ref r);
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    BishopCheck(i, j, ref r);
                }
                break;
            }
        }
        return r;
    }

    private void BishopCheck(int x, int y, ref bool[,] r)
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
