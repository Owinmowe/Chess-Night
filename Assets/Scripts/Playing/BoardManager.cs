using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class BoardManager : MonoBehaviour {

    public delegate void KingUnderAttack(bool isKingWhite);
    public event KingUnderAttack kingUnderAttack;

    public delegate void KingOK(bool isKingWhite);
    public event KingOK kingOK;

    public static BoardManager Instance { set; get; }
    private bool[,] AllowedMoves { set; get; }
    private bool[,] WhiteInCheck { set; get; }
    private bool[,] WhiteInCheckMate { set; get; }
    private bool[,] BlackInCheck { set; get; }
    private bool[,] BlackInCheckMate { set; get; }
    private bool[,] WhiteStalemate { set; get; }
    private bool[,] BlackStalemate { set; get; }

    public bool promoting = false;
    private Vector2Int pawnToPromote;
    private Chessman ePLW;
    private Chessman ePRW;
    private Chessman ePLB;
    private Chessman ePRB;

    private bool whiteCheck;
    private bool blackCheck;
    private bool whiteCheckMate;
    private bool blackCheckMate;
    [SerializeField] GameObject whiteBrokenKing;
    [SerializeField] GameObject blackBrokenKing;
    [SerializeField] float animSpeed;
    private bool draw;
    private GameObject lastPieceEaten;

    public Chessman[,] Chessmen { set; get; }
    public Chessman selectedChessman;

    private Vector2Int whiteKing = new Vector2Int(4, 0);
    private Vector2Int blackKing = new Vector2Int(4, 7);
    private Vector2Int previousLocation = new Vector2Int(0, 0);

    const float TILE_SIZE = 1.0f;
    const float TILE_OFFSET = 0.5f;
    const string letters = "abcdefgh";
    const string MODEL_DETAILS = "ModelDetails";
    const string WHITE_CONTROL = "WhiteControl";
    const string BLACK_CONTROL = "BlackControl";
    const int HUMAN = 0;
    const int AI = 1;

    private int selectionX = -1;
    private int selectionY = -1;
    private float raycastLenght = 100f;
    private string promPiece;
    private bool movingAnimation;

    public List<GameObject> lowPolyChessmanPrefabs;
    public List<GameObject> highPolyChessmanPrefabs;
    public List<GameObject> brokenPiecesPrefabs;
    private List<GameObject> activeChessman;
    private List<GameObject> checkListChessman;

    public bool isWhiteTurn = true;

    [SerializeField] GameManager gameManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnAllChessman();
        if (isWhiteTurn && PlayerPrefs.GetInt(WHITE_CONTROL) == AI)
        {
            UCIToGUI.instance.SearchForMove();
        }
    }

    private void Update()
    {
        UpdateSelection();
        if (Input.GetMouseButtonDown(0))
        {
            if (selectionX >= 0 && selectionY >= 0)
            {
                if (selectedChessman == null)
                {
                    SelectChessman(selectionX, selectionY);
                }
                else
                {
                    MoveChessman(selectionX, selectionY);
                }
            }
        }

    }

    private void SelectChessman(int x, int y)
    {
        if (Chessmen[x, y] == null)
        {
            return;
        }

        if (Chessmen[x, y].isWhite != isWhiteTurn)
        {
            return;
        }

        bool hasAtLeastOneMove = false;
        AllowedMoves = Chessmen[x, y].PossibleMove();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (AllowedMoves[i, j])
                {
                    hasAtLeastOneMove = true;
                }
            }
        }
        if (!hasAtLeastOneMove)
        {
            return;
        }
        selectedChessman = Chessmen[x, y];
        Chessmen[x, y].GetComponent<Outline>().enabled = true;
        BoardHighlight.Instance.HighlightAllowedMoves(AllowedMoves);
    }

    private void MoveChessman(int x, int y)
    {
        if (AllowedMoves[x, y])
        {
            SendMoveToUCI(x, y);
            SpecialMovesChecks(x, y);
            ChangePiecePosition(x, y);
            if (promoting)
            {
                return;
            }
            else
            {
                NextTurn();
            }
        }
        Deselect();
    }

    private void Deselect()
    {
        if (selectedChessman != null)
        {
            BoardHighlight.Instance.HideHighlights();
            selectedChessman.GetComponent<Outline>().enabled = false;
            selectedChessman = null;
        }
    }

    private void NextTurn()
    {
        selectedChessman.GetComponent<Outline>().enabled = false;
        isWhiteTurn = !isWhiteTurn;
        gameManager.ChangeTurn();
        BlackCheckTest();
        if (blackCheck == true)
        {
            BlackCheckMateTest();
        }
        WhiteCheckTest();
        if (whiteCheck == true)
        {
            WhiteCheckMateTest();
        }
        if (!blackCheck && !whiteCheck)
        {
            DrawTestWhite();
            DrawTestBlack();
        }
        if (isWhiteTurn && PlayerPrefs.GetInt(WHITE_CONTROL) == AI)
        {
            UCIToGUI.instance.SearchForMove();
        }
        else if (!isWhiteTurn && PlayerPrefs.GetInt(BLACK_CONTROL) == AI)
        {
            UCIToGUI.instance.SearchForMove();
        }
    }

    private void ChangePiecePosition(int x, int y)
    {
        previousLocation = new Vector2Int(selectedChessman.CurrentX, selectedChessman.CurrentY);
        Chessman c = Chessmen[x, y];
        if (c != null && c.isWhite != isWhiteTurn)
        {
            activeChessman.Remove(c.gameObject);
            c.gameObject.SetActive(false);
            var k = c.gameObject.transform.GetChild(0).gameObject;
            k.transform.parent = transform;
            k.SetActive(true);
            SoundManager.Instance.ShatterSound();
            Destroy(k.gameObject, 2f);
        }
        Chessmen[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
        //selectedChessman.transform.position = GetTileCenter(x, y);
        StartCoroutine(CoroutineAnimationChangePiecePosition(x, y, selectedChessman));
        selectedChessman.SetPosition(x, y);
        Chessmen[x, y] = selectedChessman;
        if (selectedChessman.GetType() == typeof(King))
        {
            if (isWhiteTurn)
            {
                whiteKing = new Vector2Int(x, y);
            }
            else
            {
                blackKing = new Vector2Int(x, y);
            }
        }
    }

    IEnumerator CoroutineAnimationChangePiecePosition(int x, int y, Chessman selectedChessman)
    {
        movingAnimation = true;
        Chessman sc = selectedChessman;
        Vector3 endPlace = GetTileCenter(x, y);
        var xEnd = GetTileCenter(sc.CurrentX, y); //for knight square move
        if (sc.GetType() == typeof(Knight))
        {
            while (sc.transform.position != xEnd)
            {
                sc.transform.position = Vector3.MoveTowards(sc.transform.position, xEnd, Time.deltaTime * animSpeed);
                yield return new WaitForEndOfFrame();
            }
            while (sc.transform.position != endPlace)
            {
                sc.transform.position = Vector3.MoveTowards(sc.transform.position, endPlace, Time.deltaTime * animSpeed);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
                while (sc.transform.position != endPlace)
                {
                    sc.transform.position = Vector3.MoveTowards(sc.transform.position, endPlace, Time.deltaTime * animSpeed);
                    yield return new WaitForEndOfFrame();
                }
        }
        movingAnimation = false;
        yield return new WaitForEndOfFrame();
    }

    private void WhiteCheckTest()
    {
        WhiteInCheck = Chessmen[whiteKing.x, whiteKing.y].GameCheckTest();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (WhiteInCheck[i, j])
                {
                    whiteCheck = true;
                    BoardHighlight.Instance.HighlightWhiteCheckMoves(WhiteInCheck);
                    gameManager.Check(true, true);
                    return;
                }
                else
                {
                    whiteCheck = false;
                }
            }
        }
        if (!whiteCheck)
        {
            kingOK(true);
            BoardHighlight.Instance.HideWhiteCheckHighlights();
            gameManager.Check(false, true);
        }
    }

    private void WhiteCheckMateTest()
    {
        whiteCheckMate = true;
        checkListChessman = new List<GameObject>(activeChessman);
        foreach (GameObject go in checkListChessman)
        {
            Chessman c = go.GetComponent<Chessman>();
            if (c.isWhite)
            {
                WhiteInCheckMate = c.PossibleMove();
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (WhiteInCheckMate[i, j])
                        {
                            whiteCheckMate = false;
                            return;
                        }
                    }
                }
            }
        }
        if (whiteCheckMate)
        {
            BlackTeamWin();
        }
    }

    private void BlackCheckTest()
    {
        BlackInCheck = Chessmen[blackKing.x, blackKing.y].GameCheckTest();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (BlackInCheck[i, j])
                {
                    blackCheck = true;
                    BoardHighlight.Instance.HighlightBlackCheckMoves(BlackInCheck);
                    gameManager.Check(true, false);
                    return;
                }
                else
                {
                    blackCheck = false;
                }
            }
        }
        if (!blackCheck)
        {
            kingOK(false);
            BoardHighlight.Instance.HideBlackCheckHighlights();
            gameManager.Check(false, false);
        }
    }

    private void BlackCheckMateTest()
    {
        blackCheckMate = true;
        checkListChessman = new List<GameObject>(activeChessman);
        foreach (GameObject go in checkListChessman)
        {
            Chessman c = go.GetComponent<Chessman>();
            if (!c.isWhite)
            {
                BlackInCheckMate = c.PossibleMove();
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (BlackInCheckMate[i, j])
                        {
                            blackCheckMate = false;
                            return;
                        }
                    }
                }
            }
        }
        if (blackCheckMate)
        {
            WhiteTeamWin();
        }
    }

    private void DrawTestWhite()
    {
        if (isWhiteTurn)
        {
            draw = true;
            checkListChessman = new List<GameObject>(activeChessman);
            foreach (GameObject go in checkListChessman)
            {
                Chessman c = go.GetComponent<Chessman>();
                if (c.isWhite)
                {
                    WhiteStalemate = c.PossibleMove();
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (WhiteStalemate[i, j])
                            {
                                draw = false;
                                return;
                            }
                        }
                    }
                }
            }
        }
        if (draw)
        {
            Draw();
        }
    }

    private void DrawTestBlack()
    {
        if (!isWhiteTurn)
        {
            draw = true;
            checkListChessman = new List<GameObject>(activeChessman);
            foreach (GameObject go in checkListChessman)
            {
                Chessman c = go.GetComponent<Chessman>();
                if (!c.isWhite)
                {
                    BlackStalemate = c.PossibleMove();
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (BlackStalemate[i, j])
                            {
                                draw = false;
                                return;
                            }
                        }
                    }
                }
            }
        }
        if (draw)
        {
            Draw();
        }
    }

    private void SpecialMovesChecks(int x, int y)
    {
        if (isWhiteTurn)
        {
            if (ePLB != null)
            {
                ePLB.enPassantLeft = false;
            }
            if (ePRB != null)
            {
                ePRB.enPassantRight = false;
            }
        }
        if (!isWhiteTurn)
        {
            if (ePLW != null)
            {
                ePLW.enPassantLeft = false;
            }
            if (ePRW != null)
            {
                ePRW.enPassantRight = false;
            }
        }
        if (selectedChessman.GetType() == typeof(Pawn))
        {
            if (y == 7 || y == 0)
            {
                pawnToPromote = new Vector2Int(x, y);
                StartCoroutine(CoroutinePromotion());
            }
            else if (selectedChessman.CurrentY == 1 && y == 3)
            {
                if (x + 1 < 8)
                {
                    ePLB = Chessmen[x + 1, y];
                    if (ePLB != null && ePLB.isWhite != selectedChessman.isWhite && ePLB.GetType() == typeof(Pawn))
                    {
                        ePLB.enPassantLeft = true;
                    }
                }
                if (x - 1 >= 0)
                {
                    ePRB = Chessmen[x - 1, y];
                    if (ePRB != null && ePRB.isWhite != selectedChessman.isWhite && ePRB.GetType() == typeof(Pawn))
                    {
                        ePRB.enPassantRight = true;
                    }
                }
            }
            else if (selectedChessman.CurrentY == 6 && y == 4)
            {
                if (x + 1 < 8)
                {
                    ePLW = Chessmen[x + 1, y];
                    if (ePLW != null && ePLW.isWhite != selectedChessman.isWhite && ePLW.GetType() == typeof(Pawn))
                    {
                        ePLW.enPassantLeft = true;
                    }
                }
                if (x - 1 >= 0)
                {
                    ePRW = Chessmen[x - 1, y];
                    if (ePRW != null && ePRW.isWhite != selectedChessman.isWhite && ePRW.GetType() == typeof(Pawn))
                    {
                        ePRW.enPassantRight = true;
                    }
                }
            }
            else if (selectedChessman.enPassantRight && selectedChessman.CurrentX + 1 == x)
            {
                Chessman c = Chessmen[selectedChessman.CurrentX + 1, selectedChessman.CurrentY];
                activeChessman.Remove(c.gameObject);
                c.gameObject.SetActive(false);
                var k = c.gameObject.transform.GetChild(0).gameObject;
                k.transform.parent = transform;
                k.SetActive(true);
                SoundManager.Instance.ShatterSound();
                Destroy(k.gameObject, 2f);
            }
            else if (selectedChessman.enPassantLeft && selectedChessman.CurrentX - 1 == x)
            {
                Chessman c = Chessmen[selectedChessman.CurrentX - 1, selectedChessman.CurrentY];
                activeChessman.Remove(c.gameObject);
                c.gameObject.SetActive(false);
                var k = c.gameObject.transform.GetChild(0).gameObject;
                k.transform.parent = transform;
                k.SetActive(true);
                SoundManager.Instance.ShatterSound();
                Destroy(k.gameObject, 2f);
            }
        }
        else if (selectedChessman.GetType() == typeof(Rook))
        {
            selectedChessman.firstTurn = false;
        }
        else if (selectedChessman.GetType() == typeof(King))
        {
            selectedChessman.firstTurn = false;
            CastlingRookMove(x);
        }
    }

    private void UpdateSelection()
    {
        if (!Camera.main)
        {
            return;
        }
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, raycastLenght, LayerMask.GetMask("ChessPlane"))
            && !movingAnimation)
        {
            if ((isWhiteTurn && PlayerPrefs.GetInt(WHITE_CONTROL) == HUMAN) || (!isWhiteTurn && PlayerPrefs.GetInt(BLACK_CONTROL) == HUMAN))
            {
                selectionX = (int)hitInfo.point.x;
                selectionY = (int)hitInfo.point.z;
            }
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }

    private void SpawnAllChessman()
    {
        activeChessman = new List<GameObject>();
        Chessmen = new Chessman[8, 8];

        //              White

        //King
        SpawnChessman(0, 4, 0);

        //Queen
        SpawnChessman(1, 3, 0);

        //Rooks
        SpawnChessman(2, 0, 0);
        SpawnChessman(2, 7, 0);

        //Bishops
        SpawnChessman(3, 2, 0);
        SpawnChessman(3, 5, 0);

        //Knights
        SpawnChessman(4, 1, 0);
        SpawnChessman(4, 6, 0);

        //Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(5, i, 1);
        }

        //              Black

        //King
        SpawnChessman(6, 4, 7);

        //Queen
        SpawnChessman(7, 3, 7);

        //Rooks
        SpawnChessman(8, 0, 7);
        SpawnChessman(8, 7, 7);

        //Bishops
        SpawnChessman(9, 2, 7);
        SpawnChessman(9, 5, 7);

        //Knights
        SpawnChessman(10, 1, 7);
        SpawnChessman(10, 6, 7);

        //Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(11, i, 6);
        }
    }

    private void SpawnChessman(int index, int x, int y)
    {
        if (PlayerPrefs.GetInt(MODEL_DETAILS) == 0)
        {
            GameObject go = Instantiate(lowPolyChessmanPrefabs[index], GetTileCenter(x, y), lowPolyChessmanPrefabs[index].transform.rotation) as GameObject;
            go.transform.SetParent(transform);
            Chessmen[x, y] = go.GetComponent<Chessman>();
            Chessmen[x, y].SetPosition(x, y);
            activeChessman.Add(go);
        }
        else if (PlayerPrefs.GetInt(MODEL_DETAILS) == 1)
        {
            GameObject go = Instantiate(highPolyChessmanPrefabs[index], GetTileCenter(x, y), highPolyChessmanPrefabs[index].transform.rotation) as GameObject;
            go.transform.SetParent(transform);
            Chessmen[x, y] = go.GetComponent<Chessman>();
            Chessmen[x, y].SetPosition(x, y);
            activeChessman.Add(go);
        }
        GameObject bp = Instantiate(brokenPiecesPrefabs[index], GetTileCenter(x, y), transform.rotation) as GameObject;
        bp.transform.SetParent(Chessmen[x, y].transform);
        bp.SetActive(false);
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }

    IEnumerator CoroutinePromotion()
    {
        promoting = true;
        while (movingAnimation)
        {
            yield return new WaitForEndOfFrame();
        }
        if (isWhiteTurn && PlayerPrefs.GetInt(WHITE_CONTROL) == AI ||
            !isWhiteTurn && PlayerPrefs.GetInt(BLACK_CONTROL) == AI)
        {
            if(promPiece == "q")
            {
                activeChessman.Remove(selectedChessman.gameObject);
                Destroy(selectedChessman.gameObject);
                var p = pawnToPromote;
                if (p.y == 7)
                {
                    SpawnChessman(1, p.x, p.y);
                    SelectChessman(p.x, p.y);
                }
                else if (p.y == 0)
                {
                    SpawnChessman(7, p.x, p.y);
                    SelectChessman(p.x, p.y);
                }
                promoting = false;
            }
            else if (promPiece == "b")
            {
                activeChessman.Remove(selectedChessman.gameObject);
                Destroy(selectedChessman.gameObject);
                var p = pawnToPromote;
                if (p.y == 7)
                {
                    SpawnChessman(3, p.x, p.y);
                    SelectChessman(p.x, p.y);
                }
                else if (p.y == 0)
                {
                    SpawnChessman(9, p.x, p.y);
                    SelectChessman(p.x, p.y);
                }
                promoting = false;
            }
            else if (promPiece == "n")
            {
                activeChessman.Remove(selectedChessman.gameObject);
                Destroy(selectedChessman.gameObject);
                var p = pawnToPromote;
                if (p.y == 7)
                {
                    SpawnChessman(4, p.x, p.y);
                    SelectChessman(p.x, p.y);
                }
                else if (p.y == 0)
                {
                    SpawnChessman(10, p.x, p.y);
                    SelectChessman(p.x, p.y);
                }
                promoting = false;
            }
            else if (promPiece == "r")
            {
                activeChessman.Remove(selectedChessman.gameObject);
                Destroy(selectedChessman.gameObject);
                var p = pawnToPromote;
                if (p.y == 7)
                {
                    SpawnChessman(2, p.x, p.y);
                    SelectChessman(p.x, p.y);
                }
                else if (p.y == 0)
                {
                    SpawnChessman(8, p.x, p.y);
                    SelectChessman(p.x, p.y);
                }
                promoting = false;
            }
        }
        else
        {
            gameManager.OpenOrClosePromotionScreen();
            raycastLenght = 1f;
            while (promoting)
            {
                selectionX = -1;
                selectionY = -1;
                yield return new WaitForEndOfFrame();
            }
        }
        promPiece = null;
        NextTurn();
    }

    public void PromoteToQueen()
    {
        activeChessman.Remove(selectedChessman.gameObject);
        Destroy(selectedChessman.gameObject);
        var p = pawnToPromote;
        if (p.y == 7)
        {
            SpawnChessman(1, p.x, p.y);
            SelectChessman(p.x, p.y);
        }
        else if (p.y == 0)
        {
            SpawnChessman(7, p.x, p.y);
            SelectChessman(p.x, p.y);
        }
        gameManager.OpenOrClosePromotionScreen();
        raycastLenght = 100f;
        promoting = false;
    }

    public void PromoteToKnight()
    {
        activeChessman.Remove(selectedChessman.gameObject);
        Destroy(selectedChessman.gameObject);
        var p = pawnToPromote;
        if (p.y == 7)
        {
            SpawnChessman(4, p.x, p.y);
            SelectChessman(p.x, p.y);
        }
        else if (p.y == 0)
        {
            SpawnChessman(10, p.x, p.y);
            SelectChessman(p.x, p.y);
        }
        gameManager.OpenOrClosePromotionScreen();
        raycastLenght = 100f;
        promoting = false;
    }

    public void PromoteToRook()
    {
        activeChessman.Remove(selectedChessman.gameObject);
        Destroy(selectedChessman.gameObject);
        var p = pawnToPromote;
        if (p.y == 7)
        {
            SpawnChessman(2, p.x, p.y);
            SelectChessman(p.x, p.y);
        }
        else if (p.y == 0)
        {
            SpawnChessman(8, p.x, p.y);
            SelectChessman(p.x, p.y);
        }
        gameManager.OpenOrClosePromotionScreen();
        raycastLenght = 100f;
        promoting = false;
    }

    public void PromoteToBishop()
    {
        activeChessman.Remove(selectedChessman.gameObject);
        Destroy(selectedChessman.gameObject);
        var p = pawnToPromote;
        if (p.y == 7)
        {
            SpawnChessman(3, p.x, p.y);
            SelectChessman(p.x, p.y);
        }
        else if (p.y == 0)
        {
            SpawnChessman(9, p.x, p.y);
            SelectChessman(p.x, p.y);
        }
        gameManager.OpenOrClosePromotionScreen();
        raycastLenght = 100f;
        promoting = false;
    }

    private void CastlingRookMove(int x)
    {
        if (selectedChessman.isWhite && (selectedChessman.CurrentX + 2) == x)
        {
            selectedChessman = Chessmen[7, 0];
            ChangePiecePosition(5, 0);
            selectedChessman = Chessmen[whiteKing.x, whiteKing.y];
            selectedChessman.GetComponent<Outline>().enabled = false;
        }
        if (selectedChessman.isWhite && (selectedChessman.CurrentX - 2) == x)
        {
            selectedChessman = Chessmen[0, 0];
            ChangePiecePosition(3, 0);
            selectedChessman = Chessmen[whiteKing.x, whiteKing.y];
            selectedChessman.GetComponent<Outline>().enabled = false;
        }
        if (!selectedChessman.isWhite && (selectedChessman.CurrentX + 2) == x)
        {
            selectedChessman = Chessmen[7, 7];
            ChangePiecePosition(5, 7);
            selectedChessman = Chessmen[blackKing.x, blackKing.y];
            selectedChessman.GetComponent<Outline>().enabled = false;
        }
        if (!selectedChessman.isWhite && (selectedChessman.CurrentX - 2) == x)
        {
            selectedChessman = Chessmen[0, 7];
            ChangePiecePosition(3, 7);
            selectedChessman = Chessmen[blackKing.x, blackKing.y];
            selectedChessman.GetComponent<Outline>().enabled = false;
        }
    }

    public void KingUnderAttackBoardManager(bool isKingWhite)
    {
        kingUnderAttack(isKingWhite);
    }

    public void PieceKingIsOk(bool isWhite)
    {
        if (isWhite)
        {
            kingOK(true);
        }
        else
        {
            kingOK(false);
        }
    }

    public void PieceCheckTest(bool isWhite)
    {
        if (isWhite)
        {
            Chessmen[whiteKing.x, whiteKing.y].CheckTest();
        }
        else
        {
            Chessmen[blackKing.x, blackKing.y].CheckTest();
        }
    }

    public void PieceCheckChangePosition(int x, int y)
    {
        previousLocation = new Vector2Int(selectedChessman.CurrentX, selectedChessman.CurrentY);
        Chessman c = Chessmen[x, y];
        if (c != null && c.isWhite != isWhiteTurn)
        {
            lastPieceEaten = c.gameObject;
            activeChessman.Remove(c.gameObject);
            //Destroy(c.gameObject);
        }
        Chessmen[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
        selectedChessman.SetPosition(x, y);
        Chessmen[x, y] = selectedChessman;
        if (selectedChessman.GetType() == typeof(King))
        {
            if (isWhiteTurn)
            {
                whiteKing = new Vector2Int(x, y);
            }
            else
            {
                blackKing = new Vector2Int(x, y);
            }
        }
    }

    public void PieceCheckUndoLastMove(int x, int y)
    {
        Chessmen[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
        selectedChessman.SetPosition(previousLocation.x, previousLocation.y);
        Chessmen[previousLocation.x, previousLocation.y] = selectedChessman;
        if (selectedChessman.GetType() == typeof(King))
        {
            if (isWhiteTurn)
            {
                whiteKing = new Vector2Int(previousLocation.x, previousLocation.y);
            }
            else
            {
                blackKing = new Vector2Int(previousLocation.x, previousLocation.y);
            }
        }
        if (lastPieceEaten != null)
        {
            Chessmen[x, y] = lastPieceEaten.GetComponent<Chessman>();
            Chessmen[x, y].SetPosition(x, y);
            activeChessman.Add(lastPieceEaten);
            lastPieceEaten = null;
        }
        Deselect();
    }

    public void BlackTeamWin()
    {
        raycastLenght = 1f;
        StartCoroutine(CoroutineBlowWhiteTeamUp());
        gameManager.BlackWin();
    }

    public void WhiteTeamWin()
    {
        raycastLenght = 1f;
        StartCoroutine(CoroutineBlowBlackTimeUp());
        gameManager.WhiteWin();
    }

    private void Draw()
    {
        raycastLenght = 1f;
        gameManager.Draw();
    }

    IEnumerator CoroutineBlowWhiteTeamUp()
    {
        foreach (GameObject c in activeChessman)
        {
            Chessman ch = c.GetComponent<Chessman>();
            if (ch.isWhite)
            {
                var cb = Chessmen[ch.CurrentX, ch.CurrentY].gameObject;
                cb.SetActive(false);
                var k = cb.transform.GetChild(0).gameObject;
                k.transform.parent = transform;
                k.SetActive(true);
                SoundManager.Instance.ShatterSound();
                yield return new WaitForSeconds(.5f);
            }
        }
    }

    IEnumerator CoroutineBlowBlackTimeUp()
    {
        foreach (GameObject c in activeChessman)
        {
            Chessman ch = c.GetComponent<Chessman>();
            if (!ch.isWhite)
            {
                var cb = Chessmen[ch.CurrentX, ch.CurrentY].gameObject;
                cb.SetActive(false);
                var k = cb.transform.GetChild(0).gameObject;
                k.transform.parent = transform;
                k.SetActive(true);
                SoundManager.Instance.ShatterSound();
                Destroy(k.gameObject, 2f);
                yield return new WaitForSeconds(.5f);
            }
        }
    }

    private void SendMoveToUCI(int x, int y)
    {
        var a = letters[selectedChessman.CurrentX];
        var b = (selectedChessman.CurrentY + 1).ToString();
        var c = letters[x];
        var d = (y + 1).ToString();
        if (promPiece != null)
        {
            var e = promPiece;
            UCIToGUI.instance.piecesPosition = UCIToGUI.instance.piecesPosition + " " + a + b + c + d + e;
        }
        else
        {
            UCIToGUI.instance.piecesPosition = UCIToGUI.instance.piecesPosition + " " + a + b + c + d;
        }
    }

    public void RecieveMoveFromUCI(string move)
    {
        var s = Convert.ToInt32(move[0]) - 97;
        var s2 = Convert.ToInt32(move[1]) - 49;
        var m = Convert.ToInt32(move[2]) - 97;
        var m2 = Convert.ToInt32(move[3]) - 49;
        var p = move[4].ToString();
        if(p != " ")
        {
            promPiece = p;
        }
        SelectChessman(s, s2);
        MoveChessman(m , m2);
    }
}
