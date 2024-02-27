using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoardManager : Singleton<BoardManager>
{
    [SerializeField]
    public List<Tile> minionDeployTilesList = new List<Tile>();

    public Vector3 tileStartPos;

    public int sizeX;
    public int sizeY;

    [SerializeField]
    public List<Tile> enemyDeployTilesList = new List<Tile>();

    public Vector3 enemyTileStartPos;

    public int enemyTilesSizeX;
    public int enemyTilesSizeY;
    public bool isTileSet = false;

    private MapCreator mapCreator;

    void Awake()
    {
        mapCreator = new MapCreator();

        if (minionDeployTilesList == null)
            Debug.Log("n");
        else
        {
            mapCreator.GenerateMinionTileMap(tileStartPos, sizeX, sizeY);
            mapCreator.GenerateEnemyTileMap(enemyTileStartPos, enemyTilesSizeX, enemyTilesSizeY);
            isTileSet = true;
        }
    }

    /// <summary>
    /// 해당 노드 위치에 있는 타일 반환
    /// </summary>
    /// <param name="x"> row </param>
    /// <param name="y"> column </param>
    /// <returns> Tile </returns>
    public Tile GetTile(int x, int y)
    {
        Node n = new Node(x, y);
        var tile = minionDeployTilesList.Where(t => t.node == n);

        Tile returnVal = tile.SingleOrDefault(); //1개 데이터만 허용

        return returnVal; 
    }

    /// <summary>
    ///  해당 노드 위치에 있는 타일 반환
    /// </summary>
    /// <param name="node"></param>
    /// <returns>Tile</returns>
    public Tile GetTile(Node node)
    {
        Node n = node;
        var tile = minionDeployTilesList.Where(t => t.node == n);

        Tile returnVal = tile.SingleOrDefault(); //1개 데이터만 허용

        return returnVal;
    }

    /// <summary>
    /// 해당 노드 위치에 있는 타일 반환
    /// </summary>
    /// <param name="x"> row </param>
    /// <param name="y"> column </param>
    /// <returns> Tile </returns>
    public Tile GetEnemyTile(int x, int y)
    {
        Node n = new Node(x, y);
        var tile = enemyDeployTilesList.Where(t => t.node == n);

        Tile returnVal = tile.SingleOrDefault(); //1개 데이터만 허용

        return returnVal;
    }

    /// <summary>
    ///  해당 노드 위치에 있는 타일 반환
    /// </summary>
    /// <param name="node"></param>
    /// <returns>Tile</returns>
    public Tile GetEnemyTile(Node node)
    {
        Node n = node;
        var tile = enemyDeployTilesList.Where(t => t.node == n);

        Tile returnVal = tile.SingleOrDefault(); //1개 데이터만 허용

        return returnVal;
    }
}
