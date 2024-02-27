using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Objective_TutoRoll : Objective
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

	public List<string> p_DroneDialogTextList = new();
	private int m_CurrentDialogTextCount = 0;

	private Vector3 m_InitialRotate = new();
	public override void InitObjective(ObjectiveMgr _mgr, ObjectiveUI _ui)
    {
        m_ObjMgr = _mgr;
        m_ObjUI = _ui;

		m_Player = GameMgr.GetInstance().p_PlayerMgr.GetPlayer();
        m_InputMgr = GameMgr.GetInstance().p_PlayerInputMgr;
        
        m_InputMgr.SetAllLockByBool(true);
        m_Player.AttachActionOnFSM(PlayerStateName.ROLL,() => AddCount(), true);
        m_Phase = 0;
        m_Count = 0;
		m_ObjMgr.p_TutorialDroneObject.p_TutorialDialog.SetDialogActive(true);
		m_ObjMgr.p_TutorialDroneObject.p_TutorialDialog.SetDialogText(p_DroneDialogTextList[m_CurrentDialogTextCount]);
		m_InitialRotate = m_Player.transform.localScale;
	}

    public override void UpdateObjective()
    {
        switch (m_Phase)
        {
			case 0:
				DialogPhase();
				break;
			case 1:
                if (m_Count >= 3)
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
       m_Player.RemoveActionOnFSM(PlayerStateName.ROLL);
    }

    private void AddCount()
    {
        m_Count++;
        m_ObjUI.SetObjectiveProgress(0, m_Count * 0.33f);
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
		Debug.Log(m_Player.transform.localScale);

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
			m_InputMgr.p_RollLock = false;
			m_InputMgr.p_HideLock = false;
			m_Player.transform.localScale = m_InitialRotate;
			m_ObjUI.LerpUI(false);
			m_Phase++;
		}
	}
}