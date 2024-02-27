using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class MapCreator
{
    public void GenerateMinionTileMap(Vector3 tileStartPos, int width, int height)
    {
        InitTiles(tileStartPos, width, height);
    }

    public void GenerateEnemyTileMap(Vector3 tileStartPos, int width, int height)
    {
        InitEnemyTiles(tileStartPos, width, height);
    }

    /// <summary>
    /// 아군 배치 타일 생성
    /// </summary>
    void InitTiles(Vector3 tileStartPos, int width, int height)
    {
        float w = BattleUIManager.Instance.DeployableTileImage.GetComponent<RectTransform>().rect.width;
        float h = BattleUIManager.Instance.DeployableTileImage.GetComponent<RectTransform>().rect.height;

        for (int col = 0; col < height; col++)
            for(int row = 0; row < width; row++)
            {
                GameObject tileImage = GameObject.Instantiate(BattleUIManager.Instance.DeployableTileImage, BattleUIManager.Instance.worldCanvas.transform);
                tileImage.transform.position = new Vector3(tileStartPos.x + row * w , tileStartPos.y, tileStartPos.z - col * h);
                tileImage.transform.eulerAngles = new Vector3(90, 0, 0);
                tileImage.AddComponent<Tile>();
                tileImage.GetComponent<Tile>().node = new Node(row , col);
                tileImage.SetActive(false);
                BoardManager.Instance.minionDeployTilesList.Add(tileImage.GetComponent<Tile>());
            }
    }

    /// <summary>
    /// 적 배치 타일 생성
    /// </summary>
    void InitEnemyTiles(Vector3 tileStartPos, int width, int height)
    {
        float w = BattleUIManager.Instance.DeployableTileImage.GetComponent<RectTransform>().rect.width;
        float h = BattleUIManager.Instance.DeployableTileImage.GetComponent<RectTransform>().rect.height;

        for (int col = 0; col < height; col++)
            for (int row = 0; row < width; row++)
            {
                GameObject tileImage = GameObject.Instantiate(BattleUIManager.Instance.DeployableTileImage, BattleUIManager.Instance.worldCanvas.transform);
                tileImage.transform.position = new Vector3(tileStartPos.x + row * w, tileStartPos.y, tileStartPos.z - col * h);
                tileImage.transform.eulerAngles = new Vector3(90, 0, 0);
                tileImage.AddComponent<Tile>();
                tileImage.GetComponent<Tile>().node = new Node(row, col);
                tileImage.SetActive(false);
                BoardManager.Instance.enemyDeployTilesList.Add(tileImage.GetComponent<Tile>());
            }
    }


}
