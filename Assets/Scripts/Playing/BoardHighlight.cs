using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHighlight : MonoBehaviour
{

    public static BoardHighlight Instance { set; get; }

    [SerializeField] GameObject highlightPrefab;
    [SerializeField] GameObject checkHighlightPrefab;
    private List<GameObject> highlights;
    private List<GameObject> whiteCheckHighlights;
    private List<GameObject> blackCheckHighlights;

    private void Start()
    {
        Instance = this;
        highlights = new List<GameObject>();
        whiteCheckHighlights = new List<GameObject>();
        blackCheckHighlights = new List<GameObject>();
    }

    private GameObject GetHighlightObject()
    {
        GameObject go = highlights.Find(g => !g.activeSelf);
        if(go == null)
        {
            go = Instantiate(highlightPrefab);
            highlights.Add(go);
        }

        return go;
    }

    private GameObject GetWhiteCheckHighlightObject()
    {
        GameObject cwo = whiteCheckHighlights.Find(c => c.activeSelf);
        if (cwo == null)
        {
            cwo = Instantiate(checkHighlightPrefab);
            whiteCheckHighlights.Add(cwo);
        }
        if(whiteCheckHighlights.Count == 3)
        {
            GameObject r = whiteCheckHighlights.Find(c => !c.activeSelf);
            whiteCheckHighlights.Remove(r);
            Destroy(r);
        }

        return cwo;
    }


    private GameObject GetBlackCheckHighlightObject()
    {
        GameObject cbo = blackCheckHighlights.Find(c => c.activeSelf);
        if (cbo == null)
        {
            cbo = Instantiate(checkHighlightPrefab);
            blackCheckHighlights.Add(cbo);
        }
        if (blackCheckHighlights.Count == 3)
        {
            GameObject r = blackCheckHighlights.Find(c => !c.activeSelf);
            blackCheckHighlights.Remove(r);
            Destroy(r);
        }

        return cbo;
    }

    public void HighlightAllowedMoves(bool[,] moves)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if(moves[i,j] == true)
                {
                    GameObject go = GetHighlightObject();
                    go.SetActive(true);
                    go.transform.position = new Vector3(i + 0.5f, Mathf.Epsilon, j+ 0.5f);
                }
            }
        }
    }

    public void HighlightWhiteCheckMoves(bool[,] moves)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (moves[i, j] == true)
                {
                    GameObject co = GetWhiteCheckHighlightObject();
                    co.SetActive(true);
                    co.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
                }
            }
        }
    }

    public void HighlightBlackCheckMoves(bool[,] moves)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (moves[i, j] == true)
                {
                    GameObject co = GetBlackCheckHighlightObject();
                    co.SetActive(true);
                    co.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
                }
            }
        }
    }

    public void HideHighlights()
    {
        foreach(GameObject go in highlights)
        {
            go.SetActive(false);
        }
    }

    public void HideWhiteCheckHighlights()
    {
        foreach (GameObject go in whiteCheckHighlights)
        {
            go.SetActive(false);
        }
    }

    public void HideBlackCheckHighlights()
    {
        foreach (GameObject go in blackCheckHighlights)
        {
            go.SetActive(false);
        }
    }
}
