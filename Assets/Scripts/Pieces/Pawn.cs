using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Chessman {

    bool underCheck = false;

    private void Start()
    {
        BoardManager.Instance.kingUnderAttack += Pawn_kingUnderAttack;
        BoardManager.Instance.kingOK += Instance_kingOK;
    }

    private void Instance_kingOK(bool isKingWhite)
    {
        if(isWhite == isKingWhite)
        {
            underCheck = false;
        }
    }

    private void Pawn_kingUnderAttack(bool isKingWhite)
    {
        if (isKingWhite == isWhite)
        {
            underCheck = true;
        }
    }

    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];
        Chessman c, c2;

        //White
        if (isWhite)
        {
            //Diagonal left
            if (CurrentX != 0 && CurrentY != 7)
            {
                c = BoardManager.Instance.Chessmen[CurrentX - 1, CurrentY + 1];
                if (c != null && !c.isWhite)
                {
                    PawnCheck(CurrentX - 1, CurrentY + 1, ref r);
                }
                else
                {
                    r[CurrentX - 1, CurrentY + 1] = false;
                    if (enPassantLeft)
                    {
                        PawnCheck(CurrentX - 1, CurrentY + 1, ref r);
                    }
                }
            }
            //Diagonal right
            if (CurrentX != 7 && CurrentY != 7)
            {
                c = BoardManager.Instance.Chessmen[CurrentX + 1, CurrentY + 1];
                if (c != null && !c.isWhite)
                {
                    PawnCheck(CurrentX + 1, CurrentY + 1, ref r);
                }
                else
                {
                    r[CurrentX + 1, CurrentY + 1] = false;
                    if (enPassantRight)
                    {
                        PawnCheck(CurrentX + 1, CurrentY + 1, ref r);
                    }
                }
            }
            //Front move
            if(CurrentY != 7)
            {
                c = BoardManager.Instance.Chessmen[CurrentX, CurrentY + 1];
                if(c == null)
                {
                    PawnCheck(CurrentX, CurrentY + 1, ref r);
                }
            }
            //Double front Move
            if(CurrentY == 1)
            {
                c = BoardManager.Instance.Chessmen[CurrentX, CurrentY + 1];
                c2 = BoardManager.Instance.Chessmen[CurrentX, CurrentY + 2];
                if(c == null && c2 == null)
                {
                    PawnCheck(CurrentX, CurrentY + 2, ref r);
                }
            }
        }
        //Black
        else
        {
            //Diagonal left
            if (CurrentX != 0 && CurrentY != 0)
            {
                c = BoardManager.Instance.Chessmen[CurrentX - 1, CurrentY - 1];
                if (c != null && c.isWhite)
                {
                    PawnCheck(CurrentX - 1, CurrentY - 1, ref r);
                }
                else
                {
                    r[CurrentX - 1, CurrentY - 1] = false;
                    if (enPassantLeft)
                    {
                        PawnCheck(CurrentX - 1, CurrentY - 1, ref r);
                    }
                }
            }
            //Diagonal right
            if (CurrentX != 7 && CurrentY != 0)
            {
                c = BoardManager.Instance.Chessmen[CurrentX + 1, CurrentY - 1];
                if (c != null && c.isWhite)
                {
                    PawnCheck(CurrentX + 1, CurrentY - 1, ref r);
                }
                else
                {
                    r[CurrentX + 1, CurrentY - 1] = false;
                    if (enPassantRight)
                    {
                        PawnCheck(CurrentX + 1, CurrentY - 1, ref r);
                    }
                }
            }
            //Front move
            if (CurrentY != 0)
            {
                c = BoardManager.Instance.Chessmen[CurrentX, CurrentY - 1];
                if (c == null)
                {
                    PawnCheck(CurrentX, CurrentY - 1, ref r);
                }
            }
            //Double front Move
            if (CurrentY == 6)
            {
                c = BoardManager.Instance.Chessmen[CurrentX, CurrentY - 1];
                c2 = BoardManager.Instance.Chessmen[CurrentX, CurrentY - 2];
                if (c == null && c2 == null)
                {
                    PawnCheck(CurrentX, CurrentY - 2, ref r);
                }
            }
        }
        r[CurrentX, CurrentY] = false;
        return r;

    }

    private void PawnCheck(int x, int y, ref bool[,] r)
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
