using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Objective_TutoHide : Objective
{
     /*
     5번쨰 튜토리얼
     1. 구르기
     */
    
    private Player_InputMgr m_InputMgr;

    // Objective Variables
    private int m_Phase = 0;
    private int m_Count = 0;
    private Player m_Player;

    public TutorialHideObject p_TutorialHideObject;
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

		m_CameraMgr.MoveToTarget(p_TutorialHideObject.transform, 1);
		p_TutorialHideObject.Initialize();
		p_TutorialHideObject.action?.Invoke();

        m_Phase = 0;
        m_Count = 0;
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
				if (m_Count >= 1)
                {
                    m_ObjUI.SetObjectiveProgress(0, 1f);
                    m_ObjUI.SetObjectiveFontStyle(0, true);
                    m_ObjMgr.SendObjSuccessInfo(m_ObjIdx, true);
					m_Phase = -1;
                }
                break;
        }
    }

    public override void ExitObjective()
    {
       m_Player.RemoveActionOnFSM(PlayerStateName.HIDDEN);
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
			m_InputMgr.p_StairLock = false;
			m_InputMgr.p_HideLock = false;
			m_Player.AttachActionOnFSM(PlayerStateName.HIDDEN, () => AddCount(), true);
			m_Player.transform.localScale = m_InitialRotate;
			m_ObjUI.LerpUI(false);
			m_Phase++;
		}
	}
}