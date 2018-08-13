using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TilePlacer : MonoBehaviour {

    public enum TileType { Plains, Forest };
    enum DirectionOfGrowth { None, UpAndLeft, UpAndRight, Right, DownAndRight, DownAndLeft, Left };
    DirectionOfGrowth directionOfGrowth;

    TilePlacerUI tilePlacerUI;

    [Range(1, 1920)]
    public int tileWidth = 120;
    [Range(1, 1200)]
    public int tileHeight = 140;
    public GameObject[] availableTiles;

    GameObject tileMap;
    GameObject newTile;
    HexTile tileToGrowFrom;
    List<int[]> availableTileLocations = new List<int[]>();
    List<HexTile> tilesToGrowFrom = new List<HexTile>();
    [SerializeField]
    List<int[]> locationsToGrowTo = new List<int[]>();
    List<int[]> growthSpotsClaimed = new List<int[]>();

    bool runGenerator = false;
    bool randomPlacement = true;

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
    int plainsRate;
    int forestWeight;
    int forestRate;
    int growthCount;

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

                if (randomPlacement) {
                    //ReduceSearchArea(hexTiles);
                    //Debug.Log("Placing random tiles between column " + minColumn + " and " + (maxColumn - 1));
                    //Debug.Log("Placing random tiles between row " + minRow + " and " + (maxRow - 1));
                    //int randColumn = Random.Range(minColumn, maxColumn);
                    //int randRow = Random.Range(minRow, maxRow);

                    //Find a random tile location and set that location for the new seed.
                    int rand = Random.Range(0, availableTileLocations.Count);
                    columnToPlaceIn = availableTileLocations[rand][0];
                    rowToPlaceIn = availableTileLocations[rand][1];

                } else if (locationsToGrowTo.Count > 0) {
                    Debug.Log("Locations to grow to contains items.");
                    int rand = Random.Range(0, locationsToGrowTo.Count);
                    columnToPlaceIn = locationsToGrowTo[rand][0];
                    rowToPlaceIn = locationsToGrowTo[rand][1];

                //} else {
                    //Debug.Log("Attempting to grow from tile at column " + tileToGrowFrom.tileColumn + " row " + tileToGrowFrom.tileRow);

                    //Grow from the seed.
                }

                Debug.Log("Attempting to place hex tile at Column " + columnToPlaceIn + ", Row " + rowToPlaceIn);
                
                //Check if the tile has already been placed or attempted to place
                //bool tilePlacedHere = TilePlacedHere(hexTiles, columnToPlaceIn, rowToPlaceIn);

                //if ( ! tilePlacedHere) {

                //Try to create or grow a tile.
                GameObject pickedTile = PickTile();
                if (randomPlacement) {
                    newTile = SpawnNewTile(pickedTile, columnToPlaceIn, rowToPlaceIn);
                    //tilesToGrowFrom.Add(newTile.GetComponent<HexTile>());
                    //RemoveTileLocation();

                    //Set this new tile as the seed
                    tileToGrowFrom = newTile.GetComponent<HexTile>();
                    Debug.Log("Tile at " + tileToGrowFrom.tileColumn + ", " + tileToGrowFrom.tileRow + " has been set as the tile to grow from.");
                    GrowFromTile(tileToGrowFrom);
                    //growthCount = 6;
                    randomPlacement = false;

                } else if (CanGrow(pickedTile.GetComponent<HexTile>().currentTile) && CanGrowHere(columnToPlaceIn, rowToPlaceIn)) {
                    newTile = SpawnNewTile(pickedTile, columnToPlaceIn, rowToPlaceIn);
                    
                    //tilesToGrowFrom.Add(newTile.GetComponent<HexTile>());
                    //RemoveTileLocation();
                }
                //}
                Debug.Log("Removing tile locations " + columnToPlaceIn + ", " + rowToPlaceIn);
                locationsToGrowTo = RemoveTileLocation(locationsToGrowTo);
                //growthCount--;
                growthSpotsClaimed.Add(new int[] { columnToPlaceIn, rowToPlaceIn });
                Debug.Log("Number of tiles left to grow is " + locationsToGrowTo.Count);
                Debug.Log("Checking if more tiles need to be grown.");
                Debug.Log("Number of tiles that need to be grown is " + locationsToGrowTo.Count);
                if (locationsToGrowTo.Count == 0) {
                    Debug.Log("There are no more locations to grow to.");
                    tilesToGrowFrom.Remove(tileToGrowFrom);
                    tilesToGrowFrom.TrimExcess();
                    Debug.Log("Checking to see if there are more tiles to grow from.");
                    if (tilesToGrowFrom.Count > 0) {
                        int rand = Random.Range(0, tilesToGrowFrom.Count);
                        tileToGrowFrom = tilesToGrowFrom[rand].GetComponent<HexTile>();
                        Debug.Log("Setting new tile to grow from as tile at " + tileToGrowFrom.tileColumn + ", " + tileToGrowFrom.tileRow);
                        GrowFromTile(tileToGrowFrom);
                        //growthCount = 6;
                    } else {
                        Debug.Log("Switching back to Random Placement.");
                        randomPlacement = true;
                    }
                //} else {
                }
            } else {
                //Debug.Log("Ending generation.");
                availableTileLocations.Clear();
                growthSpotsClaimed.Clear();
                runGenerator = false;
            }
        }
    }

    public void PlaceTiles () {

        if ( ! runGenerator) {

            HexTile[] hexTiles = FindObjectsOfType<HexTile>();
            foreach (HexTile hexTile in hexTiles) {
                Destroy(hexTile.gameObject);
            }

            newTile = null;
            tileToGrowFrom = null;
            tilesToGrowFrom.Clear();
            growthSpotsClaimed.Clear();
            directionOfGrowth = DirectionOfGrowth.UpAndLeft;
            randomPlacement = true;
            columns = tilePlacerUI.Columns;
            rows = tilePlacerUI.Rows;
            mapSize = columns * rows;
            minColumn = 0;
            minRow = 0;
            maxColumn = columns;
            maxRow = rows;
            plainsWeight = tilePlacerUI.PlainsWeight;
            plainsRate = tilePlacerUI.PlainsRate;
            forestWeight = tilePlacerUI.ForesWeight;
            forestRate = tilePlacerUI.ForestRate;
            growthCount = 0;
            CreateTileLocations();

            StartCoroutine(RunGenerator());
        }
    }

    void CreateTileLocations () {
        for (int x = 0; x < columns; x++ ) {
            for (int y = 0; y < rows; y++) {
                availableTileLocations.Add(new int[] { x, y });
                //Debug.Log("Addin tile location at " + x + ", " + y + " to available tile locations.");
            }
        }
    }

    List<int[]> RemoveTileLocation (List<int[]> tileLocations) {
        int currentIndex = 0;
        for (int i = 0; i < tileLocations.Count; i++) {
            if (tileLocations[i][0] == columnToPlaceIn && tileLocations[i][1] == rowToPlaceIn) {
                currentIndex = i;
                tileLocations.RemoveAt(currentIndex);
                tileLocations.TrimExcess();
            }
        }
        return tileLocations;
    }

    //void RandomizeTileLocations () {
    //    for (int i = 0; i < availableTileLocations.Count; i++) {
    //        int rand = Random.Range(i, availableTileLocations.Count);
    //        int[] temp = availableTileLocations[i];
    //        availableTileLocations[i] = availableTileLocations[rand];
    //        availableTileLocations[rand] = temp;
    //    }
    //}

    IEnumerator RunGenerator() {
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

    void GrowFromTile (HexTile tile) {

        if (tile.tileRow % 2 == 0) {
            SetTilesToGrowTo(tile.tileColumn - 1, tile.tileRow + 1);
            SetTilesToGrowTo(tile.tileColumn, tile.tileRow + 1);
            SetTilesToGrowTo(tile.tileColumn, tile.tileRow - 1);
            SetTilesToGrowTo(tile.tileColumn - 1, tile.tileRow - 1);
        } else {
            SetTilesToGrowTo(tile.tileColumn, tile.tileRow + 1);
            SetTilesToGrowTo(tile.tileColumn + 1, tile.tileRow + 1);
            SetTilesToGrowTo(tile.tileColumn + 1, tile.tileRow - 1);
            SetTilesToGrowTo(tile.tileColumn, tile.tileRow - 1);
        }
        SetTilesToGrowTo(tile.tileColumn + 1, tile.tileRow);
        SetTilesToGrowTo(tile.tileColumn - 1, tile.tileRow);

        //switch (directionOfGrowth) {
        //    case DirectionOfGrowth.UpAndLeft:
        //        if (tile.tileRow % 2 == 0) {
        //            columnToPlaceIn = tile.tileColumn - 1;
        //        } else {
        //            columnToPlaceIn = tile.tileColumn;
        //        }
        //        rowToPlaceIn = tile.tileRow + 1;
        //        directionOfGrowth = DirectionOfGrowth.UpAndRight;
        //        break;
        //    case DirectionOfGrowth.UpAndRight:
        //        if (tile.tileRow % 2 != 0) {
        //            columnToPlaceIn = tile.tileColumn + 1;
        //        } else {
        //            columnToPlaceIn = tile.tileColumn;
        //        }
        //        rowToPlaceIn = tile.tileRow + 1;
        //        directionOfGrowth = DirectionOfGrowth.Right;
        //        break;
        //    case DirectionOfGrowth.Right:
        //        columnToPlaceIn = tile.tileColumn + 1;
        //        rowToPlaceIn = tile.tileRow;
        //        directionOfGrowth = DirectionOfGrowth.DownAndRight;
        //        break;
        //    case DirectionOfGrowth.DownAndRight:
        //        if (tile.tileRow % 2 != 0) {
        //            columnToPlaceIn = tile.tileColumn + 1;
        //        } else {
        //            columnToPlaceIn = tile.tileColumn;
        //        }
        //        rowToPlaceIn = tile.tileRow - 1;
        //        directionOfGrowth = DirectionOfGrowth.DownAndLeft;
        //        break;
        //    case DirectionOfGrowth.DownAndLeft:
        //        if (tile.tileRow % 2 == 0) {
        //            columnToPlaceIn = tile.tileColumn - 1;
        //        } else {
        //            columnToPlaceIn = tile.tileColumn;
        //        }
        //        rowToPlaceIn = tile.tileRow - 1;
        //        directionOfGrowth = DirectionOfGrowth.Left;
        //        break;
        //    case DirectionOfGrowth.Left:
        //        columnToPlaceIn = tile.tileColumn - 1;
        //        rowToPlaceIn = tile.tileRow;
        //        directionOfGrowth = DirectionOfGrowth.UpAndLeft;
        //        break;
        //    default:
        //        randomPlacement = true;
        //        break;
        //}
    }

    void SetTilesToGrowTo (int column, int row) {
        Debug.Log("Attempting to add column " + column + ", row " + row + " to locations to grow to.");
        foreach (int[] columnRow in availableTileLocations) {
            if (column == columnRow[0] && row == columnRow[1]) {
                locationsToGrowTo.Add(new int[] { column, row });
                Debug.Log("Column " + column + ", row " + row + " added to locations to grow to.");
            }
        }
    }

    bool TilePlacedHere (HexTile[] hexTiles, int column, int row) {

        bool tilePlacedHere = false;
        foreach (HexTile hexTile in hexTiles) {
            if (hexTile.tileColumn == column && hexTile.tileRow == row) {
                tilePlacedHere = true;
            }
        }
        return tilePlacedHere;
    }

    bool CanGrowHere (int column, int row) {
        bool canGrowHere = true;
        if (0 > column || column >= columns ||
            0 > row || row >= rows) {
            canGrowHere = false;
        } else if (growthSpotsClaimed.Count > 0) {
            foreach (int[] columnRow in growthSpotsClaimed) {
                if (column == columnRow[0] && row == columnRow[1]) {
                    canGrowHere = false;
                }
            }
        }
        return canGrowHere;
    }

    GameObject PickTile () {
        GameObject newTile;

        if (randomPlacement) {
            int randTileType = Random.Range(0, (plainsWeight + forestWeight));
            if (randTileType >= plainsWeight) {
                newTile = availableTiles[1];
            } else {
                newTile = availableTiles[0];
            }
        } else {
            switch (tileToGrowFrom.currentTile) {
                case TileType.Forest:
                    newTile = availableTiles[1];
                    break;
                default:
                    newTile = availableTiles[0];
                    break;
            }
        }
            return newTile;        
    }

    GameObject SpawnNewTile (GameObject pickedTile, int column, int row) {
        GameObject tile = Instantiate(pickedTile);

        if (tile) {

            tile.transform.parent = tileMap.transform;
            float tileOffset = (row % 2 == 0) ? 0 : 0.5f;
            tile.transform.position = new Vector3(column + tileOffset, row * (float)(tileWidth + 2) / tileHeight, 0);

            HexTile newHex = tile.GetComponent<HexTile>();

            if (newHex) {
                newHex.tileColumn = column;
                newHex.tileRow = row;
                tilesToGrowFrom.Add(newHex);
                Debug.Log("New Hex Tile created at Column " + newHex.tileColumn + ", Row " + newHex.tileRow);
            }

            availableTileLocations = RemoveTileLocation(availableTileLocations);
        }
        return tile;
    }

    bool CanGrow (TileType tileType) {
        int rand = Random.Range(0, 100);
        switch (tileType) {
            case TileType.Plains:
                if (rand < plainsRate) {
                    return true;
                } else {
                    return false;
                }
            case TileType.Forest:
                if (rand < forestRate) {
                    return true;
                } else {
                    return false;
                }
            default:
                return false;
        }
    }
}
