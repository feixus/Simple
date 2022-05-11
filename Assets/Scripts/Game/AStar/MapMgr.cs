using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class MapMgr : MonoBehaviour
{
    private Tilemap tilemap;
    void Start()
    {
        Dictionary<Vector3Int, Tile> tiles = new Dictionary<Vector3Int, Tile>();
        tilemap = GameObject.FindObjectOfType(typeof(Tilemap)) as Tilemap;
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            if (tilemap.HasTile(localPlace))
            {
                Tile tile = tilemap.GetTile<Tile>(localPlace);
                tiles.Add(localPlace, tile);
            }
        }

        List<Vector3Int> obstacles = new List<Vector3Int>();
        Vector3Int start = Vector3Int.zero, end = Vector3Int.zero;
        foreach (var item in tiles)
        {
            if (item.Value.name == "red")
                end = item.Key;
            else if (item.Value.name == "green")
                start = item.Key;
            else if (item.Value.name == "blue")
                obstacles.Add(item.Key);
        }

        Tile yellow = AssetDatabase.LoadAssetAtPath<Tile>("assets/arts/map/tiles/yellow.asset");
        Tile purple = AssetDatabase.LoadAssetAtPath<Tile>("assets/arts/map/tiles/purple.asset");

        List<Vector3Int> list = AStar.AStarAlgorithm(start, end, obstacles, (Vector3Int point) => {
            if (point != end)
            {
                tilemap.SetTile(point, null);
                tilemap.SetTile(point, purple);
            }
        });

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != start && list[i] != end)
            {
                tilemap.SetTile(list[i], null);
                tilemap.SetTile(list[i], yellow);
            }
        }
    }


}
