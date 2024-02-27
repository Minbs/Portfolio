using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Linq;
using System;

public class EnemySpawner : MonoBehaviour
{
	[Serializable]
	public struct EnemySpawnData
	{
		public string name;
		public Node node;
		public int wave;
	}

	public List<EnemySpawnData> enemySpawnDatas = new List<EnemySpawnData>();

	public float spawnTimer;

	public void ReadEnemySpawnData()
	{
		TextAsset textFile = Resources.Load("Datas/EnemySpawnInfo/EnemySpawnInfo_Stage1") as TextAsset;
		StringReader stringReader = new StringReader(textFile.text);
		string line = stringReader.ReadLine();

		while (stringReader != null)
		{
			line = stringReader.ReadLine();
			if (line == null)
			{
				break;
			}

			EnemySpawnData enemyData = new EnemySpawnData();

			enemyData.name = line.Split(',')[0];
			enemyData.node.row = int.Parse(line.Split(',')[1]);
			enemyData.node.column = int.Parse(line.Split(',')[2]);
			enemyData.wave = int.Parse(line.Split(',')[3]);

			enemySpawnDatas.Add(enemyData);
		}
	}

	void Start()
	{
		spawnTimer = 0;
		ReadEnemySpawnData();
	}

	public IEnumerator Spawn(int wave)
	{
		if (enemySpawnDatas.Count <= 0
			|| enemySpawnDatas[0].wave != wave)
			yield break;

		yield return null;
		GameObject enemy = ObjectPool.Instance.PopFromPool(enemySpawnDatas[0].name);
		enemy.GetComponent<Unit>().onTile = BoardManager.Instance.GetEnemyTile(enemySpawnDatas[0].node);
		enemy.GetComponent<Unit>().SetPositionOnTile();
		GameManager.Instance.enemiesList.Add(enemy);
		enemy.SetActive(true);
		enemySpawnDatas.RemoveAt(0);

	}
}
