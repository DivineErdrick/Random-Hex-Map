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
    List<HexTile> tilesToGrowFrom = new List<HexTile>();
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
                    ReduceSearchArea(hexTiles);
                    Debug.Log("Placing random tiles between column " + minColumn + " and " + (maxColumn - 1));
                    Debug.Log("Placing random tiles between row " + minRow + " and " + (maxRow - 1));
                    int randColumn = Random.Range(minColumn, maxColumn);
                    int randRow = Random.Range(minRow, maxRow);
                    columnToPlaceIn = randColumn;
                    rowToPlaceIn = randRow;
                } else {
                    Debug.Log("Attempting to grow from tile at column " + tileToGrowFrom.tileColumn + " row " + tileToGrowFrom.tileRow);
                    GrowFromTile(tileToGrowFrom);
                }

                Debug.Log("Attempting to place hex tile at Column " + columnToPlaceIn + ", Row " + rowToPlaceIn);
                //Check if the tile has already been placed or attempted to place
                bool tilePlacedHere = TilePlacedHere(hexTiles, columnToPlaceIn, rowToPlaceIn);

                if ( ! tilePlacedHere) {
                    GameObject pickedTile = PickTile();
                    if (randomPlacement) {
                        newTile = SpawnNewTile(pickedTile, columnToPlaceIn, rowToPlaceIn);
                        tileToGrowFrom = newTile.GetComponent<HexTile>();
                        tilesToGrowFrom.Add(newTile.GetComponent<HexTile>());
                        growthCount = 6;
                        Debug.Log("Tile at " + tileToGrowFrom.tileColumn + ", " + tileToGrowFrom.tileRow + " has been set as the tile to grow from.");
                        randomPlacement = false;
                    } else if (CanGrow(pickedTile.GetComponent<HexTile>().currentTile) && CanGrowHere(columnToPlaceIn, rowToPlaceIn)) {
                        newTile = SpawnNewTile(pickedTile, columnToPlaceIn, rowToPlaceIn);
                        tilesToGrowFrom.Add(newTile.GetComponent<HexTile>());
                    }
                }
                if (growthCount < 0) {
                    tilesToGrowFrom.Remove(tileToGrowFrom);
                    tilesToGrowFrom.TrimExcess();
                    if (tilesToGrowFrom.Count > 0) {
                        tileToGrowFrom = tilesToGrowFrom[0].GetComponent<HexTile>();
                        growthCount = 6;
                        Debug.Log("Setting new tile to go from as tile at " + tileToGrowFrom.tileColumn + ", " + tileToGrowFrom.tileRow);
                    } else {
                        Debug.Log("Switching back to Random Placement.");
                        randomPlacement = true;
                    }
                } else {
                    growthCount--;
                    growthSpotsClaimed.Add(new int[] { columnToPlaceIn, rowToPlaceIn });                    
                    Debug.Log("Tiles left to grow is equal to " + growthCount + 1);
                }
            } else {
                Debug.Log("Ending generation.");
                growthSpotsClaimed.Clear();
                runGenerator = false;
            }
        }
    }

    public void PlaceTiles () {

        if (!runGenerator) {

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

            StartCoroutine(RunGenerator());
        }
    }

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
        switch (directionOfGrowth) {
            case DirectionOfGrowth.UpAndLeft:
                if (tile.tileRow % 2 == 0) {
                    columnToPlaceIn = tile.tileColumn - 1;
                } else {
                    columnToPlaceIn = tile.tileColumn;
                }
                rowToPlaceIn = tile.tileRow + 1;
                directionOfGrowth = DirectionOfGrowth.UpAndRight;
                break;
            case DirectionOfGrowth.UpAndRight:
                if (tile.tileRow % 2 != 0) {
                    columnToPlaceIn = tile.tileColumn + 1;
                } else {
                    columnToPlaceIn = tile.tileColumn;
                }
                rowToPlaceIn = tile.tileRow + 1;
                directionOfGrowth = DirectionOfGrowth.Right;
                break;
            case DirectionOfGrowth.Right:
                columnToPlaceIn = tile.tileColumn + 1;
                rowToPlaceIn = tile.tileRow;
                directionOfGrowth = DirectionOfGrowth.DownAndRight;
                break;
            case DirectionOfGrowth.DownAndRight:
                if (tile.tileRow % 2 != 0) {
                    columnToPlaceIn = tile.tileColumn + 1;
                } else {
                    columnToPlaceIn = tile.tileColumn;
                }
                rowToPlaceIn = tile.tileRow - 1;
                directionOfGrowth = DirectionOfGrowth.DownAndLeft;
                break;
            case DirectionOfGrowth.DownAndLeft:
                if (tile.tileRow % 2 == 0) {
                    columnToPlaceIn = tile.tileColumn - 1;
                } else {
                    columnToPlaceIn = tile.tileColumn;
                }
                rowToPlaceIn = tile.tileRow - 1;
                directionOfGrowth = DirectionOfGrowth.Left;
                break;
            case DirectionOfGrowth.Left:
                columnToPlaceIn = tile.tileColumn - 1;
                rowToPlaceIn = tile.tileRow;
                directionOfGrowth = DirectionOfGrowth.UpAndLeft;
                break;
            default:
                randomPlacement = true;
                break;
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
                Debug.Log("New Hex Tile created at Column " + newHex.tileColumn + ", Row " + newHex.tileRow);
            }
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
