using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;


public class Objective_TutoAttack : Objective
{
 /*
     두 번쨰로 하는 목표
     1. A키 누르기
     2. D키 누르기
     */
    
    private Player_InputMgr m_InputMgr;

    // Objective Variables
    private int m_Count = 0;
    private int m_Phase = 0;

    private int m_UseLeftClickCount = 0;
    private int m_UseRightClickCount = 0;
	private int m_ReloadCount = 0;

    private Player m_Player;

	public BasicEnemy tutorialEnemy;
	public List<BasicEnemy> tutorialEnemies = new();
    private int enemyCount = 0;

	private CameraMgr m_CameraMgr;

	public List<string> p_DroneDialogTextList = new();
	private int m_CurrentDialogTextCount = 0;

	private Vector3 m_InitialRotate = new();
	public override void InitObjective(ObjectiveMgr _mgr, ObjectiveUI _ui)
    {
        m_ObjMgr = _mgr;
        m_ObjUI = _ui;

		m_CameraMgr = FindObjectOfType<CameraMgr>();
		m_Player = GameMgr.GetInstance().p_PlayerMgr.GetPlayer();
        m_InputMgr = GameMgr.GetInstance().p_PlayerInputMgr;
        
        m_InputMgr.SetAllLockByBool(true);

		m_Phase = 0;
        m_Count = 0;

        m_InputMgr.SetAttackAction(AddLeftClickCount);
		m_Player.AttachActionOnFSM(PlayerStateName.MELEE, () => AddRightClickCount(), true);
		m_CameraMgr.MoveToTarget(tutorialEnemy.transform, 1);
		StartCoroutine(SpawnEnemy());
		enemyCount = tutorialEnemies.Count;
	}

    public override void UpdateObjective()
    {
        switch (m_Phase)
        {
			case 0: // 카메라 이동
				if (m_CameraMgr.m_IsMoveEnd == true)
				{
					m_Phase++;
					m_ObjMgr.p_TutorialDroneObject.p_TutorialDialog.SetDialogActive(true);
					m_ObjMgr.p_TutorialDroneObject.p_TutorialDialog.SetDialogText(p_DroneDialogTextList[m_CurrentDialogTextCount]);
					m_InitialRotate = m_Player.transform.localScale;
					if (m_Player.transform.position.x < m_ObjMgr.p_TutorialDroneObject.transform.position.x)
					{
						m_Player.setisRightHeaded(true);
					}
					else
					{
						m_Player.setisRightHeaded(false);
					}
				}
				break;
			case 1:
				DialogPhase();
				break;
			case 2:
                if(m_UseLeftClickCount == 1)
                {
					m_ObjUI.SetObjectiveProgress(0, 1);
					m_ObjUI.SetObjectiveFontStyle(0, true);
				}
				if (m_UseRightClickCount == 1)
                {
					m_ObjUI.SetObjectiveProgress(1, 1);
					m_ObjUI.SetObjectiveFontStyle(1, true);
				}

				if (m_InputMgr.m_IsPushReloadKey)
				{
					m_ObjUI.SetObjectiveProgress(2, 1f);
					m_ObjUI.SetObjectiveFontStyle(2, true);
					m_ReloadCount++;
				}

				float proceed = 0;
				proceed = Mathf.InverseLerp(enemyCount, 0, GetActiveEnemyCount());

				m_ObjUI.SetObjectiveProgress(3, proceed);
                if(proceed >= 1)
                {
					m_ObjUI.SetObjectiveProgress(3, 1);
					m_ObjUI.SetObjectiveFontStyle(3, true);
                }

                if(proceed >= 1 && m_UseLeftClickCount >= 1 && m_UseRightClickCount >= 1 && m_ReloadCount >= 1)
                {
					m_Phase++;
                }
				break;
            case 3:
                m_ObjMgr.SendObjSuccessInfo(m_ObjIdx, true);
                m_Phase = -1;
                m_InputMgr.ResetAttackAction();
                break;
        }
    }



    public override void ExitObjective()
    {
        
    }

    private int GetActiveEnemyCount()
    {
        int count = 0;
        foreach(var enemy in tutorialEnemies)
        {
            if(enemy.m_CurEnemyStateName != EnemyStateName.DEAD)
                count++;    
        }
        return count;
    }

	private IEnumerator SpawnEnemy()
	{
		foreach (var enemy in tutorialEnemies)
		{
			enemy.gameObject.SetActive(true);
			yield return new WaitForSeconds(0.5f);
		}
	}

    private void AddLeftClickCount() => m_UseLeftClickCount++;
	private void AddRightClickCount() => m_UseRightClickCount++;
	public void DialogPhase()
	{
		if (m_Player.transform.position.x < m_ObjMgr.p_TutorialDroneObject.transform.position.x)
		{
			m_Player.transform.localScale = new Vector3(1, 1, 1);
		}
		else
		{
			m_Player.transform.localScale = new Vector3(-1, 1, 1);
		}

		if (m_CurrentDialogTextCount < p_DroneDialogTextList.Count)
		{
			if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F))
			{
				if (m_ObjMgr.p_TutorialDroneObject.p_TutorialDialog.isTextEnd)
				{
					m_CurrentDialogTextCount++;
					if (m_CurrentDialogTextCount < p_DroneDialogTextList.Count)
						m_ObjMgr.p_TutorialDroneObject.p_TutorialDialog.SetDialogText(p_DroneDialogTextList[m_CurrentDialogTextCount]);
				}
				else
				{
					m_ObjMgr.p_TutorialDroneObject.p_TutorialDialog.SkipEvent?.Invoke();
				}
			}
		}
		else
		{
			m_ObjMgr.p_TutorialDroneObject.p_TutorialDialog.SetDialogActive(false);
			m_InputMgr.p_MousePosLock = false;
			m_InputMgr.p_MoveInputLock = false;
			m_InputMgr.p_FireLock = false;
			m_InputMgr.p_SideAttackLock = false;
			m_InputMgr.p_ReloadLock = false;
			m_InputMgr.p_RollLock = false;
			m_InputMgr.p_HideLock = false;
			m_Player.transform.localScale = m_InitialRotate;
			m_ObjUI.LerpUI(false);
			m_Phase++;
		}
	}
}