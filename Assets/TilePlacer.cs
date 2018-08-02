using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TilePlacer : MonoBehaviour {

    public enum TileType { Plains, Forest };

    public GameObject[] availableTiles;

    void Start () {

        Assert.IsNotNull(availableTiles, "No tiles have been assigned to the Tile Placer.");
    }

    public void PlaceTiles () {

    }
}
