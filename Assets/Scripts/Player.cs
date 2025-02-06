using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Grid grid;
    public bool smellCheese = false;
    public bool feelBreeze = false;
    public bool smellWumpus = false;

    public TextMeshProUGUI text;

    public AudioSource audioSource;
    public AudioClip walkClip;

    public string WumpusText;
    public string PitText;
    public string TreasureText;
    public string WumpusDeath;
    public string ArrowUsed;
    public string WinText;
    public string LooseText;

    public int[] position = { 0, 0 };
    void Move(int x, int y)
    {

        grid.MoveTile(position[0], position[1], x, y);
        position[0] = Mathf.Clamp(x, 0, grid.size-1);
        position[1] = Mathf.Clamp(y, 0, grid.size-1);
    }

    public void CheckSurroundings()
    {
        smellCheese = false;
        feelBreeze = false;
        smellWumpus = false;
        int[] NORTH = { position[0], position[1] + 1 };
        int[] SOUTH = { position[0], position[1] - 1 };
        int[] EAST = { position[0] + 1, position[1] };
        int[] WEST = { position[0] - 1, position[1] };

        int[] states = {
            grid.CheckTile(NORTH),
            grid.CheckTile(SOUTH),
            grid.CheckTile(EAST),
            grid.CheckTile(WEST)
        };

        if ( isAny(states, Grid.TREASURE) )
        {
            smellCheese = true;
        }
        if (isAny(states, Grid.PIT))
        {
            feelBreeze = true;
        }
        if (isAny(states, Grid.WUMPUS))
        {
            smellWumpus = true;
        }

        string message = "";
        if (grid.killedWumpus)
        {
            message += WumpusDeath + "\n";
        }else if (grid.firedArrow)
        {
            message += ArrowUsed + "\n";
        }
        if (smellCheese)
        {
            message += TreasureText + "\n";
        }
        if (feelBreeze)
        {
            message += PitText + "\n";
        }
        if (smellWumpus)
        {
            message += WumpusText + "\n";
        }
        if (grid.won)
        {
            message += WinText + "\n";
        }
        if (grid.lost){
            message += LooseText + "\n";
        }
        text.text = message;
    }

    bool isAny(int[] states, int target)
    {
        foreach (int state in states)
        {
            if (state == target)
            {
                return true;
            }
        }
        return false;
    }

    void Update()
    {
        if (grid.lost || grid.won)
        {
            return;
        }
        int[] direction = { 0, 0 };
        if (Input.GetKeyDown(KeyCode.W))
        {
            direction[1] = 1;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            direction[1] = -1;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            direction[0] = 1;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            direction[0] = -1;
        }
        CheckSurroundings();
        if (direction[0] != 0 || direction[1] != 0)
        {
            Move(position[0] + direction[0], position[1] + direction[1]);
        }
    }

    public void ResetPlayer()
    {
        position = new int[] { 0, 0 };
        smellCheese = false;
        feelBreeze = false;
        smellWumpus = false;
        text.text = "";
    }
}
