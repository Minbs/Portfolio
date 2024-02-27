using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

[Serializable]
public enum State
{
	WAIT, //전투 시작 전
	BATTLE, //전투 진행
	SKILL_PERFORM, //스킬 시전
	WAVE_END, // 웨이브 종료
	END //전투 종료
}

public enum DeployState
{
	NONE,
	POSITIONING, // 배치 위치 결정
	Deploying // 배치 방향 결정
}

[Serializable]
public struct IncomeUpgradeData
{
	public int upgradeCost;
	public int income;
}

public class GameManager : Singleton<GameManager>
{

	public GameObject UImanager;
	public GameObject StageFailUi;
	public GameObject StageVictoryUI;
	public Image[] Victory;
	public Image[] Fail;

	public float cost = 20; // 초기 보유 코스트
	public float costTime = 10; // 초기 코스트 획득량

	public float waitTime = 30; // 대기 시간
	public float clearTimeTerm = 30;
	public float currentWaitTimer { get; set; }
	public GameObject WaveUI;

	public State state { get; set; }
	public DeployState deployState { get; set; } // 배치 상태

	public EnemySpawner spawner;

	public Camera tileCamera;
	public Camera characterCamera;

	public float gameSpeed { get; set; }

	public int currentWave { get; set; }

	public List<int> waveClearRewards;

	Ray ray;

	public Vector3 minionSetPosition;

	public List<GameObject> enemiesList = new List<GameObject>();
	public List<GameObject> minionsList = new List<GameObject>();

	public int minionsListIndex { get; set; }

	private GameObject unitSetTile;

	public GameObject settingCharacter { get; set; }

	public bool isChangePosition { get; set; }

	public int totalIncome { get; set; }
	public List<IncomeUpgradeData> incomeUpgradeDatas;
	public int incomeUpgradeCount { get; set; }

	public GameObject turret;


	void Start()
	{
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (var e in enemies)
			enemiesList.Add(e);

		currentWave = 0;


		totalIncome = incomeUpgradeDatas[0].income;
		incomeUpgradeCount = 0;
		isChangePosition = false;
		StartCoroutine(WaitState());
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.T))
		{
			StageVictoryEvent();

		}
		ray = tileCamera.ScreenPointToRay(Input.mousePosition);

		if (!settingCharacter)
		{
			if (Physics.Raycast(ray, out RaycastHit raycastHit))
			{
				if (raycastHit.collider.transform.tag == "Tower"
					&& Input.GetMouseButtonUp(1))
				{
					BattleUIManager.Instance.SetIncomeUpgradeButtonActive(true);
				}
				else if ((raycastHit.collider.transform.tag != "Tower"
					&& !raycastHit.collider.gameObject.Equals(BattleUIManager.Instance.incomeUpgradeButton))
					&& (Input.GetMouseButtonUp(1)
					|| Input.GetMouseButtonUp(0)))
				{
					BattleUIManager.Instance.SetIncomeUpgradeButtonActive(false);
				}
			}
			else
			{
				if (Input.GetMouseButtonUp(1)
					|| Input.GetMouseButtonUp(0))
				{
					BattleUIManager.Instance.SetIncomeUpgradeButtonActive(false);
				}
			}
		}
	}

	/// <summary>
	/// 웨이브 시작 전 대기 상태
	/// </summary>
	/// <returns></returns>
	IEnumerator WaitState()
	{
		// 초기화
		WaitStateInit();

		// 대기 시간 종료까지
		while (currentWaitTimer > 0)
		{
			StartCoroutine(spawner.Spawn(currentWave));
			currentWaitTimer -= Time.deltaTime;
			WaveUI.GetComponent<Wave_UI_Script>().TimerText(currentWaitTimer, waitTime);
			WaitStateUpdate();
			yield return null;
		}

		deployState = DeployState.NONE;

		// 종료
		WaitStateEnd();

		StartCoroutine(BattleState());
	}

	void WaitStateInit()
	{
		state = State.WAIT;
		deployState = DeployState.NONE;
		SetGameSpeed(1);
		currentWaitTimer = waitTime;
		WaveUI.GetComponent<Wave_UI_Script>().StageText_Next();
		WaveUI.GetComponent<Wave_UI_Script>().Wave_Logo_ColorChange(currentWave, "Yellow");
		WaveUI.GetComponent<Wave_UI_Script>().TimerText(currentWaitTimer, waitTime);
		currentWave++;
	}

	void WaitStateUpdate()
	{
		if (BattleUIManager.Instance.minionUpgradeUI.activeSelf)
		{
			if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Object")))
			{
				if (!raycastHit.collider.transform.parent.GetComponent<Minion>()
					&& Input.GetMouseButtonUp(1))

					BattleUIManager.Instance.minionUpgradeUI.SetActive(false);
			}
			else
			{
				if (Input.GetMouseButtonUp(1))
				{
					BattleUIManager.Instance.minionUpgradeUI.SetActive(false);
				}
			}
		}
		else
		{
			BattleUIManager.Instance.circleAttackRangeUI.SetActive(false);
			BattleUIManager.Instance.rectangleAttackRangeUI.SetActive(false);
		}

		switch (deployState)
		{
			case DeployState.POSITIONING:
				PositioningMinion();
				break;
			case DeployState.Deploying:
				DeployingMinion();
				break;
			default:
				break;
		}
	}

	private void WaitStateEnd()
	{
		if (settingCharacter)
		{
			if (isChangePosition)
			{
				Vector3 pos = settingCharacter.GetComponent<DefenceMinion>().onTile.gameObject.transform.position;
				pos += minionSetPosition;
				settingCharacter.transform.position = pos;
				settingCharacter.GetComponent<DefenceMinion>().onTile.isOnUnit = true;
				BattleUIManager.Instance.sellPanel.SetActive(false);
			}
			else
			{
				Destroy(settingCharacter);
			}
		}

		BattleUIManager.Instance.minionUpgradeUI.SetActive(false);
		BattleUIManager.Instance.circleAttackRangeUI.SetActive(false);
		BattleUIManager.Instance.rectangleAttackRangeUI.SetActive(false);

		settingCharacter = null;
		StartCoroutine(BattleState());
	}
	IEnumerator BattleState()
	{
		state = State.BATTLE;
		SetGameSpeed(1);

		foreach (var minion in minionsList)
		{
			minion.transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;
			minion.GetComponent<UnitStateMachine>().agent.enabled = true;
		}

		//1. 스킬 타입 2. 이름 순 오름차순 정렬
		var tempList = minionsList.OrderBy(x => x.GetComponent<DefenceMinion>().skillType).ThenBy(x => x.GetComponent<DefenceMinion>().Unitname);
		minionsList = tempList.ToList();

		foreach (var e in enemiesList)
		{
			e.GetComponent<UnitStateMachine>().ChangeState(e.GetComponent<UnitStateMachine>().moveState);
		}

		foreach (var tile in BoardManager.Instance.minionDeployTilesList)
		{
			tile.ShowDeployableTile(false);
		}



		while (enemiesList.Count > 0)
		{
			BattleStateUpdate();
			yield return null;
		}

		StartCoroutine(WaveEndState());
	}

	void BattleStateUpdate()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			SkillManager.Instance.UseCharacterSkill(0);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			SkillManager.Instance.UseCharacterSkill(1);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			SkillManager.Instance.UseCharacterSkill(2);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			SkillManager.Instance.UseCharacterSkill(3);
		}
	}


	IEnumerator WaveEndState()
	{
		state = State.WAVE_END;
		SetGameSpeed(1);

		if (spawner.enemySpawnDatas.Count <= 0)
			StageVictoryEvent();

		int count = 0;

		while (count < minionsList.Count)
		{
			if (minionsList[count].activeSelf
	   && minionsList[count].GetComponent<Unit>().currentHp > 0)
			{
				minionsList[count].GetComponent<UnitStateMachine>().ChangeState(minionsList[count].GetComponent<UnitStateMachine>().moveState);
				minionsList[count].GetComponent<Unit>().currentHp = minionsList[count].GetComponent<Unit>().maxHp;
				minionsList[count].GetComponent<Unit>().target = null;
				count++;
				WaveUI.GetComponent<Wave_UI_Script>().Wave_Logo_ColorChange(currentWave - 1, "Blue");   //현재 스테이지 로고 Blue로 변경
			}
			else
			{
				WaveUI.GetComponent<Wave_UI_Script>().Wave_Logo_ColorChange(currentWave - 1, "Red");    //현재 스테이지 로고 Red로 변경
				minionsList[count].GetComponent<Unit>().onTile.isOnUnit = false;
				minionsList.RemoveAt(count);

			}
		}

		while (true)
		{
			bool isAllMinionReturn = true;
			foreach (var m in minionsList)
			{
				if (!m.GetComponent<UnitStateMachine>().currentState.Equals(m.GetComponent<UnitStateMachine>().idleState) && m.activeSelf)
					isAllMinionReturn = false;
			}
			WaveUI.GetComponent<Wave_UI_Script>().TimerText(currentWaitTimer, waitTime);
			if (isAllMinionReturn)
				break;

			yield return null;
		}

		cost += waveClearRewards[currentWave - 1];
		WaveUI.GetComponent<Wave_UI_Script>().CostUpUI((waveClearRewards[currentWave - 1]), "temp");
		//클리어 코스트 증가
		BattleUIManager.Instance.costText.text = cost.ToString();

		StartCoroutine(WaitState());
	}

	/// <summary>
	/// 유닛 배치 타일 결정
	/// </summary>
	private void PositioningMinion()
	{
		BattleUIManager.Instance.minionUpgradeUI.SetActive(false);
		BattleUIManager.Instance.circleAttackRangeUI.SetActive(false);
		BattleUIManager.Instance.rectangleAttackRangeUI.SetActive(false);

		foreach (var minion in minionsList)
		{
			if (minion.Equals(settingCharacter))
				continue;
			minion.transform.GetChild(0).GetComponent<BoxCollider>().enabled = false;
			minion.GetComponent<UnitStateMachine>().agent.enabled = false;


		}

		if (settingCharacter)
		{
			Vector3 mousePosition
		   = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 22);
			settingCharacter.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
		}

		if (Physics.Raycast(ray, out RaycastHit raycastHit))
		{
			if (raycastHit.collider.transform.tag == "Tile" && raycastHit.collider.GetComponent<Tile>().IsDeployableMinionTile())
			{

				if (Input.GetMouseButtonDown(0))
				{
					deployState = DeployState.Deploying;
					unitSetTile = raycastHit.collider.gameObject;
					//Debug.Log("소환성공");

				}
			}
			else if (raycastHit.collider.gameObject.Equals(BattleUIManager.Instance.sellPanel) && isChangePosition)
			{
				if (Input.GetMouseButtonUp(0))
				{

					cost += settingCharacter.GetComponent<DefenceMinion>().sellCost;
					BattleUIManager.Instance.costText.text = cost.ToString();
					minionsList.Remove(settingCharacter);
					//판매
					UImanager.GetComponent<Unit_Select_UI>().Display_Unit_Button(settingCharacter.GetComponent<DefenceMinion>().Unitname);
					Destroy(settingCharacter);
					settingCharacter = null;
					isChangePosition = false;
					BattleUIManager.Instance.sellPanel.SetActive(false);

					foreach (var minion in minionsList)
					{
						SynergyManager.Instance.CheckClassSynergy(minion);
						minion.transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;
						minion.GetComponent<UnitStateMachine>().agent.enabled = true;
					}

					foreach (var tile in BoardManager.Instance.minionDeployTilesList)
					{
						tile.ShowDeployableTile(false);
					}
					deployState = DeployState.NONE;
				}
			}
		}
		else
		{

		}
	}

	/// <summary>
	/// 유닛 배치
	/// </summary>
	public void DeployingMinion()
	{
		Vector3 pos = unitSetTile.transform.position;
		pos += minionSetPosition;

		Direction direction = Direction.RIGHT;

		BattleUIManager.Instance.isCheck = true;

		settingCharacter.transform.position = pos;
		unitSetTile.GetComponent<Tile>().isOnUnit = true;
		settingCharacter.GetComponent<DefenceMinion>().onTile = unitSetTile.GetComponent<Tile>();
		deployState = DeployState.NONE;
		settingCharacter.GetComponent<Unit>().SetDirection(direction);

		foreach (var tile in BoardManager.Instance.minionDeployTilesList)
		{
			tile.ShowDeployableTile(false);
		}

		settingCharacter.GetComponent<Unit>().Init();
		unitSetTile = null;

		if (!isChangePosition)
		{
			minionsList.Add(settingCharacter);
			BattleUIManager.Instance.UseCost(settingCharacter.GetComponent<DefenceMinion>().cost);
		}
		if (settingCharacter.GetComponent<DefenceMinion>().OneTimeSummon == false)
		{
			settingCharacter.GetComponent<DefenceMinion>().OneTimeSummon = true;
			UImanager.GetComponent<Unit_Select_UI>().Hide_Unit_Button(settingCharacter.GetComponent<DefenceMinion>().Unitname);
		}

		settingCharacter.GetComponent<UnitStateMachine>().isDeploying = false;
		settingCharacter = null;
		isChangePosition = false;
		BattleUIManager.Instance.sellPanel.SetActive(false);




		foreach (var m in minionsList)
		{
			SynergyManager.Instance.CheckClassSynergy(m);
			m.transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;
			m.GetComponent<UnitStateMachine>().agent.enabled = true;
		}

	}

	/// <summary>
	/// 버튼 클릭 콜백 이벤트 함수 유닛배치모드로 전환
	/// </summary>
	public void ChangeMinionPositioningState()
	{
		deployState = DeployState.POSITIONING;

		foreach (var tile in BoardManager.Instance.minionDeployTilesList)
		{
			tile.ShowDeployableTile(true);
		}
	}

	public void minionChangePos(GameObject minion)
	{
		if (settingCharacter)
		{
			return;
		}

		isChangePosition = true;
		settingCharacter = minion;
		settingCharacter.GetComponent<Unit>().onTile.isOnUnit = false;
		settingCharacter.transform.GetChild(0).GetComponent<BoxCollider>().enabled = false;
		BattleUIManager.Instance.SetSellCostText(settingCharacter.GetComponent<DefenceMinion>().sellCost);
		BattleUIManager.Instance.sellPanel.SetActive(true);
		deployState = DeployState.POSITIONING;



		foreach (var tile in BoardManager.Instance.minionDeployTilesList)
		{
			tile.ShowDeployableTile(true);
		}
	}


	public void SetGameSpeed(float speed)
	{
		if (gameSpeed.Equals(speed))
			return;

		gameSpeed = speed;

		foreach (var e in enemiesList)
		{
			e.GetComponent<Unit>().spineAnimation.skeletonAnimation.AnimationState.TimeScale = gameSpeed;
			e.GetComponent<UnitStateMachine>().agent.velocity = e.GetComponent<UnitStateMachine>().agent.velocity * speed;
		}

		foreach (var m in minionsList)
		{
			m.GetComponent<Unit>().spineAnimation.skeletonAnimation.AnimationState.TimeScale = gameSpeed;
			m.GetComponent<UnitStateMachine>().agent.velocity = m.GetComponent<UnitStateMachine>().agent.velocity * speed;
		}
	}

	public void StageVictoryEvent()
	{
		StageVictoryUI.SetActive(true);
		for (int i = 0; i < 4; i++)
		{
			Victory[i].GetComponent<Image>().DOFade(1, 1f);
		}

	}

	public void StageFailEvent()
	{
		StageFailUi.SetActive(true);
		for (int i = 0; i < 4; i++)
		{
			Fail[i].GetComponent<Image>().DOFade(1, 1f);
		}
	}

}