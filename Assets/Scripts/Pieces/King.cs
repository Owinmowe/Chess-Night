using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Chessman
{
    bool underCheck = false;

    private void Start()
    {
        BoardManager.Instance.kingUnderAttack += Instance_kingUnderAttack;
        BoardManager.Instance.kingOK += Instance_kingOK;
    }

    private void Instance_kingOK(bool isKingWhite)
    {
        if (isWhite == isKingWhite)
        {
            underCheck = false;
        }
    }

    private void Instance_kingUnderAttack(bool isKingWhite)
    {
        if (isWhite == isKingWhite)
        {
            underCheck = true;
        }
    }

    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];

        KingMove(CurrentX + 1, CurrentY, ref r); // right
        KingMove(CurrentX - 1, CurrentY, ref r); // left
        KingMove(CurrentX, CurrentY - 1, ref r); // down
        KingMove(CurrentX, CurrentY + 1, ref r); // up
        KingMove(CurrentX + 1, CurrentY - 1, ref r); // right down
        KingMove(CurrentX - 1, CurrentY - 1, ref r); // left down
        KingMove(CurrentX + 1, CurrentY + 1, ref r); // right up
        KingMove(CurrentX - 1, CurrentY + 1, ref r); // left up

        if (firstTurn)
        {
            CheckForShortCastling(CurrentX, CurrentY, ref r);
            CheckForLongCastling(CurrentX, CurrentY, ref r);
        }

        return r;
    }

    public void KingMove(int x, int y, ref bool[,] r)
    {
        Chessman c;
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
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



    public override bool[,] CheckTest()
    {
        bool[,] r = new bool[8, 8];

        Chessman c;
        int i, j;

        KnightCheck(CurrentX - 1, CurrentY + 2, ref r); //UpLeft        
        KnightCheck(CurrentX + 1, CurrentY + 2, ref r); //UpRight       
        KnightCheck(CurrentX + 2, CurrentY + 1, ref r); //RightUp     
        KnightCheck(CurrentX + 2, CurrentY - 1, ref r); //RightDown      
        KnightCheck(CurrentX - 1, CurrentY - 2, ref r); //DownLeft        
        KnightCheck(CurrentX + 1, CurrentY - 2, ref r); //DownRight      
        KnightCheck(CurrentX - 2, CurrentY + 1, ref r); //LeftUp       
        KnightCheck(CurrentX - 2, CurrentY - 1, ref r); //LeftDown

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
                r[i, CurrentY] = false;
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    if (c.GetType() == typeof(Rook) || c.GetType() == typeof(Queen))
                    {
                        r[i, CurrentY] = true;
                        RaiseKingUnderAttack(isWhite);
                    }
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
                r[i, CurrentY] = false;
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    if (c.GetType() == typeof(Rook) || c.GetType() == typeof(Queen))
                    {
                        r[i, CurrentY] = true;
                        RaiseKingUnderAttack(isWhite);
                    }
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
                r[CurrentX, i] = false;
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    if (c.GetType() == typeof(Rook) || c.GetType() == typeof(Queen))
                    {
                        r[CurrentX, i] = true;
                        RaiseKingUnderAttack(isWhite);
                    }
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
                r[CurrentX, i] = false;
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    if (c.GetType() == typeof(Rook) || c.GetType() == typeof(Queen))
                    {
                        r[CurrentX, i] = true;
                        RaiseKingUnderAttack(isWhite);
                    }
                }
                break;
            }
        }

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
                r[i, j] = false;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    if (c.GetType() == typeof(Bishop) || c.GetType() == typeof(Queen))
                    {
                        r[i, j] = true;
                        RaiseKingUnderAttack(isWhite);
                    }
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
                r[i, j] = false;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    if (c.GetType() == typeof(Bishop) || c.GetType() == typeof(Queen))
                    {
                        r[i, j] = true;
                        RaiseKingUnderAttack(isWhite);
                    }
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
                r[i, j] = false;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    if (c.GetType() == typeof(Bishop) || c.GetType() == typeof(Queen))
                    {
                        r[i, j] = true;
                        RaiseKingUnderAttack(isWhite);
                    }
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
                r[i, j] = false;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    if (c.GetType() == typeof(Bishop) || c.GetType() == typeof(Queen))
                    {
                        r[i, j] = true;
                        RaiseKingUnderAttack(isWhite);
                    }
                }
                break;
            }
        }

        KingCheck(CurrentX + 1, CurrentY, ref r); // up
        KingCheck(CurrentX - 1, CurrentY, ref r); // down
        KingCheck(CurrentX, CurrentY - 1, ref r); // left
        KingCheck(CurrentX, CurrentY + 1, ref r); // right
        KingCheck(CurrentX + 1, CurrentY - 1, ref r); // up left
        KingCheck(CurrentX - 1, CurrentY - 1, ref r); // down left
        KingCheck(CurrentX + 1, CurrentY + 1, ref r); // up right
        KingCheck(CurrentX - 1, CurrentY + 1, ref r); // down right

        if (!isWhite)
        {
            PawnCheck(CurrentX - 1, CurrentY - 1, ref r);
            PawnCheck(CurrentX + 1, CurrentY - 1, ref r);
        }
        else if (isWhite)
        {
            PawnCheck(CurrentX - 1, CurrentY + 1, ref r);
            PawnCheck(CurrentX + 1, CurrentY + 1, ref r);
        }

        return r;
    }

    private void KingCheck(int x, int y, ref bool[,] r)
    {
        Chessman c;
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            c = BoardManager.Instance.Chessmen[x, y];
            if (c == null)
            {
                r[x, y] = false;
            }
            else if (isWhite != c.isWhite)
            {
                if (c.GetType() == typeof(King))
                {
                    r[x, y] = true;
                    RaiseKingUnderAttack(isWhite);
                }
            }
        }
    }

    private void KnightCheck(int x, int y, ref bool[,] r)
    {
        Chessman c;
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            c = BoardManager.Instance.Chessmen[x, y];
            if (c == null)
            {
                r[x, y] = false;
            }
            else if (isWhite != c.isWhite)
            {
                if (c.GetType() == typeof(Knight))
                {
                    r[x, y] = true;
                    RaiseKingUnderAttack(isWhite);
                }
            }
        }
    }

    private void PawnCheck(int x, int y, ref bool[,] r)
    {
        Chessman c;
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            c = BoardManager.Instance.Chessmen[x, y];
            if (c == null)
            {
                r[x, y] = false;
            }
            else if (isWhite != c.isWhite)
            {
                if (c.GetType() == typeof(Pawn))
                {
                    r[x, y] = true;
                    RaiseKingUnderAttack(isWhite);
                }
            }
        }
    }

    public override bool[,] GameCheckTest()
    {
        bool[,] r = new bool[8, 8];

        Chessman c;
        int i, j;

        GameKnightCheck(CurrentX - 1, CurrentY + 2, ref r); //UpLeft        
        GameKnightCheck(CurrentX + 1, CurrentY + 2, ref r); //UpRight       
        GameKnightCheck(CurrentX + 2, CurrentY + 1, ref r); //RightUp     
        GameKnightCheck(CurrentX + 2, CurrentY - 1, ref r); //RightDown      
        GameKnightCheck(CurrentX - 1, CurrentY - 2, ref r); //DownLeft        
        GameKnightCheck(CurrentX + 1, CurrentY - 2, ref r); //DownRight      
        GameKnightCheck(CurrentX - 2, CurrentY + 1, ref r); //LeftUp       
        GameKnightCheck(CurrentX - 2, CurrentY - 1, ref r); //LeftDown

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
                r[i, CurrentY] = false;
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    if (c.GetType() == typeof(Rook) || c.GetType() == typeof(Queen))
                    {
                        r[i, CurrentY] = true;
                    }
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
                r[i, CurrentY] = false;
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    if (c.GetType() == typeof(Rook) || c.GetType() == typeof(Queen))
                    {
                        r[i, CurrentY] = true;
                    }
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
                r[CurrentX, i] = false;
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    if (c.GetType() == typeof(Rook) || c.GetType() == typeof(Queen))
                    {
                        r[CurrentX, i] = true;
                    }
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
                r[CurrentX, i] = false;
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    if (c.GetType() == typeof(Rook) || c.GetType() == typeof(Queen))
                    {
                        r[CurrentX, i] = true;
                    }
                }
                break;
            }
        }

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
                r[i, j] = false;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    if (c.GetType() == typeof(Bishop) || c.GetType() == typeof(Queen))
                    {
                        r[i, j] = true;
                    }
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
                r[i, j] = false;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    if (c.GetType() == typeof(Bishop) || c.GetType() == typeof(Queen))
                    {
                        r[i, j] = true;
                    }
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
                r[i, j] = false;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    if (c.GetType() == typeof(Bishop) || c.GetType() == typeof(Queen))
                    {
                        r[i, j] = true;
                    }
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
                r[i, j] = false;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    if (c.GetType() == typeof(Bishop) || c.GetType() == typeof(Queen))
                    {
                        r[i, j] = true;
                    }
                }
                break;
            }
        }

        GameKingCheck(CurrentX + 1, CurrentY, ref r); // up
        GameKingCheck(CurrentX - 1, CurrentY, ref r); // down
        GameKingCheck(CurrentX, CurrentY - 1, ref r); // left
        GameKingCheck(CurrentX, CurrentY + 1, ref r); // right
        GameKingCheck(CurrentX + 1, CurrentY - 1, ref r); // up left
        GameKingCheck(CurrentX - 1, CurrentY - 1, ref r); // down left
        GameKingCheck(CurrentX + 1, CurrentY + 1, ref r); // up right
        GameKingCheck(CurrentX - 1, CurrentY + 1, ref r); // down right

        if (!isWhite)
        {
            GamePawnCheck(CurrentX - 1, CurrentY - 1, ref r);
            GamePawnCheck(CurrentX + 1, CurrentY - 1, ref r);
        }
        else if (isWhite)
        {
            GamePawnCheck(CurrentX - 1, CurrentY + 1, ref r);
            GamePawnCheck(CurrentX + 1, CurrentY + 1, ref r);
        }

        return r;
    }

    private void GameKingCheck(int x, int y, ref bool[,] r)
    {
        Chessman c;
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            c = BoardManager.Instance.Chessmen[x, y];
            if (c == null)
            {
                r[x, y] = false;
            }
            else if (isWhite != c.isWhite)
            {
                if (c.GetType() == typeof(King))
                {
                    r[x, y] = true;
                }
            }
        }
    }

    private void GameKnightCheck(int x, int y, ref bool[,] r)
    {
        Chessman c;
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            c = BoardManager.Instance.Chessmen[x, y];
            if (c == null)
            {
                r[x, y] = false;
            }
            else if (isWhite != c.isWhite)
            {
                if (c.GetType() == typeof(Knight))
                {
                    r[x, y] = true;
                }
            }
        }
    }

    private void GamePawnCheck(int x, int y, ref bool[,] r)
    {
        Chessman c;
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            c = BoardManager.Instance.Chessmen[x, y];
            if (c == null)
            {
                r[x, y] = false;
            }
            else if (isWhite != c.isWhite)
            {
                if (c.GetType() == typeof(Pawn))
                {
                    r[x, y] = true;
                }
            }
        }
    }

    private void CheckForLongCastling(int x, int y, ref bool[,] r)
    {
        if (underCheck)
        {
            return;
        }
        else
        {
            Chessman c;
            c = BoardManager.Instance.Chessmen[x - 1, y];
            if (c == null)
            {
                BoardManager.Instance.selectedChessman = BoardManager.Instance.Chessmen[this.CurrentX, this.CurrentY];
                BoardManager.Instance.PieceCheckChangePosition(x - 1, y);
                BoardManager.Instance.PieceCheckTest(isWhite);
                if (underCheck)
                {
                    BoardManager.Instance.PieceKingIsOk(isWhite);
                    BoardManager.Instance.PieceCheckUndoLastMove(x, y);
                    return;
                }
                else
                {
                    BoardManager.Instance.PieceKingIsOk(isWhite);
                    BoardManager.Instance.PieceCheckUndoLastMove(x, y);
                    c = BoardManager.Instance.Chessmen[x - 2, y];
                    if (c == null)
                    {
                        BoardManager.Instance.selectedChessman = BoardManager.Instance.Chessmen[this.CurrentX, this.CurrentY];
                        BoardManager.Instance.PieceCheckChangePosition(x - 2, y);
                        BoardManager.Instance.PieceCheckTest(isWhite);
                        if (underCheck)
                        {
                            BoardManager.Instance.PieceKingIsOk(isWhite);
                            BoardManager.Instance.PieceCheckUndoLastMove(x, y);
                            return;
                        }
                        else
                        {
                            c = BoardManager.Instance.Chessmen[x - 3, y];
                            {
                                if(c == null)
                                {
                                    c = BoardManager.Instance.Chessmen[x - 4, y];
                                    if(c.GetType() == typeof(Rook) && c.firstTurn)
                                    {
                                        r[x - 2, y] = true;
                                    }
                                }
                            }
                            BoardManager.Instance.PieceKingIsOk(isWhite);
                            BoardManager.Instance.PieceCheckUndoLastMove(x, y);
                        }
                    }
                }
            }
        }
    }

    private void CheckForShortCastling(int x, int y, ref bool[,] r)
    {
        if (underCheck)
        {
            return;
        }
        else
        {
            Chessman c;
            c = BoardManager.Instance.Chessmen[x + 1, y];
            if (c == null)
            {
                BoardManager.Instance.selectedChessman = BoardManager.Instance.Chessmen[this.CurrentX, this.CurrentY];
                BoardManager.Instance.PieceCheckChangePosition(x + 1, y);
                BoardManager.Instance.PieceCheckTest(isWhite);
                if (underCheck)
                {
                    BoardManager.Instance.PieceKingIsOk(isWhite);
                    BoardManager.Instance.PieceCheckUndoLastMove(x, y);
                    return;
                }
                else
                {
                    BoardManager.Instance.PieceKingIsOk(isWhite);
                    BoardManager.Instance.PieceCheckUndoLastMove(x, y);
                    c = BoardManager.Instance.Chessmen[x + 2, y];
                    if (c == null)
                    {
                        BoardManager.Instance.selectedChessman = BoardManager.Instance.Chessmen[this.CurrentX, this.CurrentY];
                        BoardManager.Instance.PieceCheckChangePosition(x + 2, y);
                        BoardManager.Instance.PieceCheckTest(isWhite);
                        if (underCheck)
                        {
                            BoardManager.Instance.PieceKingIsOk(isWhite);
                            BoardManager.Instance.PieceCheckUndoLastMove(x, y);
                            return;
                        }
                        else
                        {
                            c = BoardManager.Instance.Chessmen[x + 3, y];
                            if(c.GetType() == typeof(Rook) && c.firstTurn)
                            {
                                r[x + 2, y] = true;
                            }
                        }
                        BoardManager.Instance.PieceKingIsOk(isWhite);
                        BoardManager.Instance.PieceCheckUndoLastMove(x, y);
                    }
                }
            }
        }
    }

}