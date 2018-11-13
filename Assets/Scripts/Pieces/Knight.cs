using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Chessman
{

    bool underCheck = false;

    private void Start()
    {
        BoardManager.Instance.kingUnderAttack += Knight_kingUnderAttack;
        BoardManager.Instance.kingOK += Instance_kingOK;
    }

    private void Instance_kingOK(bool isKingWhite)
    {
        if(isWhite == isKingWhite)
        {
            underCheck = false;
        }
    }

    private void Knight_kingUnderAttack(bool isKingWhite)
    {
        if (isKingWhite == isWhite)
        {
            underCheck = true;
        }
    }

    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];

        //UpLeft
        KnightMove(CurrentX - 1, CurrentY + 2, ref r);

        //UpRight
        KnightMove(CurrentX + 1, CurrentY + 2, ref r);

        //RightUp
        KnightMove(CurrentX + 2, CurrentY + 1, ref r);

        //RightDown
        KnightMove(CurrentX + 2, CurrentY - 1, ref r);

        //DownLeft
        KnightMove(CurrentX - 1, CurrentY - 2, ref r);

        //DownRight
        KnightMove(CurrentX + 1, CurrentY - 2, ref r);

        //LeftUp
        KnightMove(CurrentX - 2, CurrentY + 1, ref r);

        //LeftDown
        KnightMove(CurrentX - 2, CurrentY - 1, ref r);

        return r;
    }

    public void KnightMove(int x, int y, ref bool[,] r)
    {
        Chessman c;
        if(x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            c = BoardManager.Instance.Chessmen[x, y];
            if (c == null)
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
            else if (isWhite != c.isWhite)
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
    }
}
