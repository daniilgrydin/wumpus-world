using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    // enumartion for the tile type
    public enum TileType
    {
        Empty,
        Wumpus,
        Pit,
        Treasure,
        Player
    }

    public Grid grid;

    public int x;
    public int y;
    public AudioSource audioSource;

    // sprite for each type of tile
    public Sprite emptySprite;
    public Sprite wumpusSprite;
    public Sprite pitSprite;
    public Sprite treasureSprite;
    public Sprite player;

    public TileType type = TileType.Empty;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // set the tile type to empty
        type = TileType.Empty;
    }

    private void Update()
    {
        if (type == TileType.Player)
        {
            float scale = 1 + Mathf.Sin(Time.time * 6) * 0.02f;
            transform.localScale = new Vector3(scale, scale, 1);
        }
    }

    public void UpdateTile()
    {
        // set the sprite of the tile based on the type
        switch (type)
        {
            case TileType.Empty:
                GetComponent<SpriteRenderer>().sprite = emptySprite;
                break;
            case TileType.Wumpus:
                GetComponent<SpriteRenderer>().sprite = wumpusSprite;
                break;
            case TileType.Pit:
                GetComponent<SpriteRenderer>().sprite = pitSprite;
                break;
            case TileType.Treasure:
                GetComponent<SpriteRenderer>().sprite = treasureSprite;
                break;
            case TileType.Player:
                GetComponent<SpriteRenderer>().sprite = player;
                break;
        }
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void SetType(int type)
    {
        switch (type)
        {
            case 0:
                this.type = TileType.Empty;
                break;
            case 1:
                this.type = TileType.Wumpus;
                break;
            case 2:
                this.type = TileType.Pit;
                break;
            case 3:
                this.type = TileType.Treasure;
                break;
            case 4:
                this.type = TileType.Player;
                break;
        }
    }

    private void OnMouseDown()
    {
        audioSource.Play();
        // if the tile is empty
        if (type == TileType.Empty)
        {
            // set the tile type to treasure
            type = TileType.Treasure;
            // update the tile
            UpdateTile();
            grid.UpdateOneTile(x, y, 3);
        }
        // if the tile is treasure
        else if (type == TileType.Treasure)
        {
            // set the tile type to empty
            type = TileType.Wumpus;
            // update the tile
            UpdateTile();
            grid.UpdateOneTile(x, y, 1);
        }
        else if (type == TileType.Wumpus)
        {
            // set the tile type to pit
            type = TileType.Pit;
            // update the tile
            UpdateTile();
            grid.UpdateOneTile(x, y, 2);
        }
        else if (type == TileType.Pit)
        {
            // set the tile type to wumpus
            type = TileType.Empty;
            // update the tile
            UpdateTile();
            grid.UpdateOneTile(x, y, 0);
        }
    }
}
