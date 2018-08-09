using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TilePlacer : MonoBehaviour {

    public enum TileType { Plains, Forest };

    TilePlacerUI tilePlacerUI;

    [Range(1, 1920)]
    public int tileWidth = 120;
    [Range(1, 1200)]
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
    int columnToPlaceIn;
    int rowToPlaceIn;
    int plainsWeight;
    int forestWeight;

    void Start() {

        tilePlacerUI = GetComponent<TilePlacerUI>();
        Assert.IsNotNull(tilePlacerUI, "Tile Placer cannot find TilePlacerUI.");

        Assert.IsNotNull(availableTiles, "No tiles have been assigned to the Tile Placer.");

        tileMap = GameObject.Find("Tile Map");
        Assert.IsNotNull(tilePlacerUI, "Tile Placer cannot find Tile Map.");
    }

    void Update() {

        if (runGenerator) {

            HexTile[] hexTiles = FindObjectsOfType<HexTile>();

            if (hexTiles.Length < mapSize) {

                ReduceSearchArea(hexTiles);
                Debug.Log("Placing tiles between column " + minColumn + " and " + (maxColumn - 1));
                Debug.Log("Placing tiles between row " + minRow + " and " + (maxRow - 1));
                int randColumn = Random.Range(minColumn, maxColumn);
                int randRow = Random.Range(minRow, maxRow);
                Debug.Log("Attempting to place hex tile at Column " + randColumn + ", Row " + randRow);

                columnToPlaceIn = randColumn;
                rowToPlaceIn = randRow;

                //Check if the tile has already been placed and try to shrink the area
                bool tilePlaceHere = TilePlacedHere(hexTiles, columnToPlaceIn, rowToPlaceIn);

                //If there isn't a tile at the location, place the tile
                if ( ! tilePlaceHere) {

                    SpawnNewTile(NewTile(), columnToPlaceIn, rowToPlaceIn);
                }
            } else {
                runGenerator = false;
            }
        }
    }

    public void PlaceTiles () {

        if (!runGenerator) {

            columns = tilePlacerUI.Columns;
            rows = tilePlacerUI.Rows;
            mapSize = columns * rows;
            minColumn = 0;
            minRow = 0;
            maxColumn = columns;
            maxRow = rows;
            plainsWeight = tilePlacerUI.PlainsWeight;
            forestWeight = tilePlacerUI.ForesWeight;

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

    void ReduceSearchArea (HexTile[] hexTiles) {
        int maxColumnCount = 0;
        int maxRowCount = 0;
        int minColumnCount = 0;
        int minRowCount = 0;
        foreach (HexTile hexTile in hexTiles) {
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
    }

    bool TilePlacedHere (HexTile[] hexTiles, int column, int row) {

        bool tilePlacedThere = false;
        foreach (HexTile hexTile in hexTiles) {
            if (hexTile.tileColumn == column && hexTile.tileRow == row) {
                tilePlacedThere = true;
            }
        }
        return tilePlacedThere;
    }

    GameObject NewTile () {
        GameObject newTile;
        int randTileType = Random.Range(0, (plainsWeight + forestWeight));
        if (randTileType >= plainsWeight) {
            newTile = availableTiles[1];
        } else {
            newTile = availableTiles[0];
        }
        return newTile;
    }

    void SpawnNewTile (GameObject newTile, int column, int row) {
        GameObject tile = Instantiate(newTile);

        if (tile) {

            tile.transform.parent = tileMap.transform;
            float tileOffset = (row % 2 == 0) ? 0 : 0.5f;
            tile.transform.position = new Vector3(column + tileOffset, row * (float)(tileWidth + 2) / tileHeight, 0);

            HexTile newHex = tile.GetComponent<HexTile>();

            if (newHex) {
                newHex.tileColumn = column;
                newHex.tileRow = row;
                Debug.Log("New Hex Tile created at Column " + newHex.tileColumn + ", Row " + newHex.tileRow);
            }
        }
    }
}
