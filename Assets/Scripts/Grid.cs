using System;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.InputSystem.Editor;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    public int wumpusCount = 1;
    public int pitCount = 3;

    bool hideMap = false;
    public GameObject curtain;

    public TMP_InputField wumpusField;
    public TMP_InputField pitField;
    public TMP_InputField sizeField;

    public GameObject backDrop;
    public Color backgroundEven;
    public Color backgroundOdd;
    public GameObject tile;

    public AudioSource audioSource;
    public AudioClip walkClip;
    public AudioClip shootClip;
    public AudioClip killClip;
    public AudioClip changeClip;
    public AudioClip winClip;
    public AudioClip loseClip;

    public int size = 5;
    private int[,] gridState;
    private Tile[,] gridObjects;

    public static int EMPTY = 0;
    public static int WUMPUS = 1;
    public static int PIT = 2;
    public static int TREASURE = 3;
    public static int PLAYER = 4;

    public bool lost = false;
    public bool won = false;
    public bool killedWumpus = false;
    public bool firedArrow = false;

    public int MAX_TRIES = 20;

    public Player player;
    public void MoveTile(int x1, int y1, int x2, int y2)
    {
        if ( x2 < 0 || x2 >= size || y2 < 0 || y2 >= size || lost || won)
        {
            return;
        }
        int from = gridState[x1, y1];
        int to = gridState[x2, y2];
        if (from == PLAYER && to == WUMPUS)
        {
            lost = true;
            audioSource.PlayOneShot(loseClip);
            player.CheckSurroundings();
        }
        else if (from == PLAYER && to == PIT)
        {
            lost = true;
            audioSource.PlayOneShot(loseClip);
            player.CheckSurroundings();
        }
        else if (from == PLAYER && to == TREASURE)
        {
            won = true;
            audioSource.PlayOneShot(winClip);
            player.CheckSurroundings();
        }
        else if (from == WUMPUS && to == EMPTY)
        {
            audioSource.PlayOneShot(killClip);
        }
        else
        {
            audioSource.PlayOneShot(walkClip);
        }
        gridState[x1, y1] = EMPTY;
        gridState[x2, y2] = from;
        UpdateGrid();
    }
    public int CheckTile(int x, int y)
    {
        if (x < 0 || x >= size || y < 0 || y >= size)
        {
            return 0;
        }
        return gridState[x, y];
    }

    public int CheckTile(int[] pos)
    {
        return CheckTile(pos[0], pos[1]);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridState = new int[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                gridState[i, j] = EMPTY;
            }
        }
        gridState[0, 0] = PLAYER;
        for (int i = 0; i < wumpusCount; i++)
        {
            SetThing(WUMPUS);
        }
        for (int i = 0; i < pitCount; i++)
        {
            SetThing(PIT);
        }
        SetThing(TREASURE);
        gridObjects = new Tile[size, size];
        InitiateGrid();
        player = FindFirstObjectByType<Player>();
        UpdateGrid();
    }

    public void SetThing(int thing, int x = -1, int y = -1, int tries = 0)
    {
        if (tries > MAX_TRIES)
        {
            Debug.LogWarning(string.Format("Reached spawning limit, could not place {0} after {1} tries.", thing, tries));
            return;
        }
        if (x == -1)
        {
            x = Random.Range(0, size);
        }
        if (y == -1)
        {
            y = Random.Range(0, size);
        }
        if (gridState[x, y] == EMPTY && (x > 1 || y > 1))
        {
            gridState[x, y] = thing;
            return;
        }
        else
        {
            SetThing(thing, -1, -1, tries+1);
        }
    }

    public void InitiateGrid()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                gridObjects[i, j] = Instantiate(tile, new Vector3(i, j, 0), Quaternion.identity).GetComponent<Tile>();
                GameObject tile1 = gridObjects[i, j].gameObject;
                tile1.transform.parent = transform;
                tile1.transform.localPosition = new Vector3(i-(size/2f), j-(size/2f), 0);
                gridObjects[i, j].grid = this;
                gridObjects[i, j].x = i;
                gridObjects[i, j].y = j;
                GameObject backtile = Instantiate(backDrop, new Vector3(i, j, 0), Quaternion.identity);
                backtile.transform.parent = transform;
                backtile.transform.localPosition = new Vector3(i - (size / 2f), j - (size / 2f), 10);
                backtile.transform.localScale = new Vector3(1, 1, 1);
                backtile.GetComponent<SpriteRenderer>().color = (i + j) % 2 == 0 ? backgroundEven : backgroundOdd;
            }
        }
        float locscale = 6f / size;
        transform.localScale = new Vector3(locscale, locscale, 1);
    }

    public void UpdateGrid()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                gridObjects[i, j].SetType(gridState[i, j]);
                gridObjects[i, j].UpdateTile();
            }
        }
    }

    public void UpdateOneTile(int i, int j, int type)
    {
        gridState[i, j] = type;
    }

    private void ResetGrid()
    {
        if (int.TryParse(wumpusField.text, out int wumpusCountParsed))
        {
            wumpusCount = wumpusCountParsed;
        }
        if (int.TryParse(pitField.text, out int pitCountParsed))
        {
            pitCount = pitCountParsed;
        }
        if (int.TryParse(sizeField.text.Trim(), out int sizeParsed))
        {
            size = sizeParsed;
        }
        // delete all the children
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        gridState = new int[size, size];
        gridObjects = new Tile[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                gridState[i, j] = EMPTY;
            }
        }
        gridState[0, 0] = PLAYER;
        for (int i = 0; i < wumpusCount; i++)
        {
            SetThing(WUMPUS);
        }
        for (int i = 0; i < pitCount; i++)
        {
            SetThing(PIT);
        }
        SetThing(TREASURE);
        lost = false;
        won = false;
        killedWumpus = false;
        firedArrow = false;
        InitiateGrid();
        UpdateGrid();
    }

    private void ShootArrow(int[] dir)
    {
        firedArrow = true;
        int[] pos = { player.position[0], player.position[1] };
        for (int i = 0; i < size; i++)
        {
            pos[0] += dir[0];
            pos[1] += dir[1];
            if (pos[0] < 0 || pos[0] >= size || pos[1] < 0 || pos[1] >= size)
            {
                break;
            }
            if (gridState[pos[0], pos[1]] == WUMPUS)
            {
                gridState[pos[0], pos[1]] = EMPTY;
                killedWumpus = true;
                UpdateGrid();
                audioSource.PlayOneShot(killClip);
                return;
            }
        }
        audioSource.PlayOneShot(shootClip);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            ResetGrid();
            player.ResetPlayer();
        }
        if(!firedArrow)
        {
            int[] direction = { 0, 0 };
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                direction[1] = 1;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                direction[1] = -1;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                direction[0] = 1;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                direction[0] = -1;
            }
            if (direction[0] != 0 || direction[1] != 0)
            {
                ShootArrow(direction);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hideMap = !hideMap;
            curtain.SetActive(hideMap);
        }
    }
}
