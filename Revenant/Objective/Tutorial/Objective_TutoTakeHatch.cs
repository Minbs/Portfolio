using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Objective_TutoTakeHatch : Objective
{
	/*
	6번쨰 튜토리얼
	1. 구르기
	*/

	private Player_InputMgr m_InputMgr;

	// Objective Variables
	private int m_Phase = 0;
	private int m_Count = 0;
	private Player m_Player;

	public TutorialHatchObject p_TutorialHatchObject;
	private CameraMgr m_CameraMgr;

	public List<string> p_DroneDialogTextList = new();
	private int m_CurrentDialogTextCount = 0;

	private Vector3 m_InitialRotate = new();
	private bool isHatchActive = false;
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
		p_TutorialHatchObject.Initialize();
		m_ObjMgr.p_TutorialDroneObject.p_TutorialDialog.SetDialogActive(true);
		m_ObjMgr.p_TutorialDroneObject.p_TutorialDialog.SetDialogText(p_DroneDialogTextList[m_CurrentDialogTextCount]);
		m_InitialRotate = m_Player.transform.localScale;
	}

	public override void UpdateObjective()
	{
		m_Player.m_RageGauge.AddGaugeValue(999999f);
		switch (m_Phase)
		{
			case 0:
				DialogPhase();
				break;
			case 1:
				if (!m_CameraMgr.m_IsFollowTarget)
				{
					if(!isHatchActive)
					{
						p_TutorialHatchObject.NextAnimation();
						isHatchActive = true;
					}
				}
				if (m_CameraMgr.m_IsMoveEnd == true)
				{
					m_Phase++;
					if (m_Player.transform.position.x < m_ObjMgr.p_TutorialDroneObject.transform.position.x)
					{
						m_Player.setisRightHeaded(true);
					}
					else
					{
						m_Player.setisRightHeaded(false);
					}
					p_TutorialHatchObject.action += AddCount;
				}
				break;
			case 2:
				if (m_Count > 0)
					m_Phase++;
				break;
			case 3:
				m_ObjMgr.SendObjSuccessInfo(m_ObjIdx, true);
				m_Phase = -1;
				break;
		}
	}

	public override void ExitObjective()
	{
	}

	private void AddCount()
	{
		m_Count++;
	}

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
			m_InputMgr.p_BulletTimeLock = false;
			m_Player.transform.localScale = m_InitialRotate;

			m_CameraMgr.MoveToTarget(p_TutorialHatchObject.transform, 1);
			m_ObjUI.LerpUI(false);
			m_Phase++;
			if (m_Player.transform.position.x < m_ObjMgr.p_TutorialDroneObject.transform.position.x)
			{
				m_Player.setisRightHeaded(true);
			}
			else
			{
				m_Player.setisRightHeaded(false);
			}
		}
	}
}