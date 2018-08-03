using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TilePlacer : MonoBehaviour {

    public enum TileType { Plains, Forest };

    TilePlacerUI tilePlacerUI;

    [Range (1,1920)]
    public int tileWidth = 120;
    [Range (1,1200)]
    public int tileHeight = 140;
    public GameObject[] availableTiles;

    GameObject tileMap;
    bool runGenerator = false;
    int columns;
    int rows;

    void Start () {

        tilePlacerUI = GetComponent<TilePlacerUI>();
        Assert.IsNotNull(tilePlacerUI, "Tile Placer cannot find TilePlacerUI.");

        Assert.IsNotNull(availableTiles, "No tiles have been assigned to the Tile Placer.");

        tileMap = GameObject.Find("Tile Map");
        Assert.IsNotNull(tilePlacerUI, "Tile Placer cannot find Tile Map.");
    }

    void Update () {

        if (runGenerator) {

            List<HexTile> hexTiles;

            int randColumn = Random.Range(0, columns);
            int randRow = Random.Range(0, rows);
            Debug.Log("Attempting to place hex tile at Column " + randColumn + ", Row " + randRow);

            GameObject newTile = Instantiate(availableTiles[0]);
            if (newTile) {
                newTile.transform.parent = tileMap.transform;
                float tileOffset = (randRow % 2 == 0) ? 0 : 0.5f;
                newTile.transform.position = new Vector3(randColumn + tileOffset, randRow * (float)(tileWidth+2)/tileHeight, 0);

                HexTile newHex = newTile.GetComponent<HexTile>();
                if (newHex) {
                    newHex.tileColumn = randColumn;
                    newHex.tileRow = randRow;
                    Debug.Log("New Hex Tile created at Column " + newHex.tileColumn + ", Row " + newHex.tileRow);
                }
            }

            runGenerator = false;
        }
    }

    public void PlaceTiles () {

        if ( ! runGenerator) {

            columns = tilePlacerUI.Columns;
            rows = tilePlacerUI.Rows;
            runGenerator = true;
        }
    }
}
