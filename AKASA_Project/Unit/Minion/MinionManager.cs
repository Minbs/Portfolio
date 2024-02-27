using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionManager : Singleton<MinionManager>
{
	public List<GameObject> minionPrefabs = new List<GameObject>();
	public List<DefenceMinion> minionQueue = new List<DefenceMinion>();

	void Start()
	{
		foreach (var hero in minionPrefabs)
		{
			minionQueue.Add(hero.GetComponent<Minion>());
		}
	}
}
