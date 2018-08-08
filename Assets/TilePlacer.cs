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
    int minColumn;
    int minRow;
    int maxColumn;
    int maxRow;
    int mapSize;

    void Start () {

        tilePlacerUI = GetComponent<TilePlacerUI>();
        Assert.IsNotNull(tilePlacerUI, "Tile Placer cannot find TilePlacerUI.");

        Assert.IsNotNull(availableTiles, "No tiles have been assigned to the Tile Placer.");

        tileMap = GameObject.Find("Tile Map");
        Assert.IsNotNull(tilePlacerUI, "Tile Placer cannot find Tile Map.");
    }

    void Update () {

        if (runGenerator) {

            HexTile[] hexTiles = FindObjectsOfType<HexTile>();

            if (hexTiles.Length < mapSize) {

                Debug.Log("Placing tiles between column " + minColumn + " and " + (maxColumn - 1));
                Debug.Log("Placing tiles between row " + minRow + " and " + (maxRow - 1));
                int randColumn = Random.Range(minColumn, maxColumn);
                int randRow = Random.Range(minRow, maxRow);
                Debug.Log("Attempting to place hex tile at Column " + randColumn + ", Row " + randRow);

                //Check if the tile has already been placed and try to shrink the area
                bool tilePlacedThere = false;
                int maxColumnCount = 0;
                int maxRowCount = 0;
                int minColumnCount = 0;
                int minRowCount = 0;
                HexTile[] placedHexTiles = FindObjectsOfType<HexTile>();
                foreach (HexTile hexTile in hexTiles) {
                    if (hexTile.tileColumn == randColumn && hexTile.tileRow == randRow) {
                        tilePlacedThere = true;
                    }
                    if (hexTile.tileColumn == maxColumn - 1) {
                        maxColumnCount++;
                    }
                    if (hexTile.tileRow == maxRow - 1) {
                        maxRowCount++;
                    }
                    if (hexTile.tileColumn == minColumn) {
                        minColumnCount++;
                    }
                    if (hexTile.tileRow == minRow) {
                        minRowCount++;
                    }
                }
                if (maxColumnCount == rows && minColumn < maxColumn - 1) {
                    maxColumn--;
                }
                if (maxRowCount == columns && minRow < maxRow - 1) {
                    maxRow--;
                }
                if (minColumnCount == rows && minColumn < maxColumn - 1) {
                    minColumn++;
                }
                if (minRowCount == columns && minRow < maxRow - 1) {
                    minRow++;
                }

                //If there isn't a tile at the location, place the tile
                if ( ! tilePlacedThere) {

                    GameObject newTile = Instantiate(availableTiles[0]);

                    if (newTile) {

                        newTile.transform.parent = tileMap.transform;
                        float tileOffset = (randRow % 2 == 0) ? 0 : 0.5f;
                        newTile.transform.position = new Vector3(randColumn + tileOffset, randRow * (float)(tileWidth + 2) / tileHeight, 0);

                        HexTile newHex = newTile.GetComponent<HexTile>();

                        if (newHex) {
                            newHex.tileColumn = randColumn;
                            newHex.tileRow = randRow;
                            Debug.Log("New Hex Tile created at Column " + newHex.tileColumn + ", Row " + newHex.tileRow);
                        }
                    }
                }
            } else {
                runGenerator = false;
            }
        }
    }

    public void PlaceTiles () {

        if ( ! runGenerator) {

            columns = tilePlacerUI.Columns;
            rows = tilePlacerUI.Rows;
            mapSize = columns * rows;
            minColumn = 0;
            minRow = 0;
            maxColumn = columns;
            maxRow = rows;

            HexTile[] hexTiles = FindObjectsOfType<HexTile>();
            foreach (HexTile hexTile in hexTiles) {
                Destroy(hexTile.gameObject);
            }
            StartCoroutine(RunGenerator());
        }
    }

    IEnumerator RunGenerator () {
        yield return new WaitForSeconds(0.5f);
        runGenerator = true;
    }
}
