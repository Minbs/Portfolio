using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Objective_TutoPressButton : Objective
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

	public TutorialStairObject p_tutorialStair;
	public TutorialButtonObject p_tutorialButton;
    public TutorialDoorObject p_tutorialDoor;
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
		p_tutorialButton.action?.Invoke();
        p_tutorialButton.action += AddCount;

		p_tutorialDoor.Initialize();

		p_tutorialStair.action?.Invoke();
		m_CameraMgr.MoveToTarget(p_tutorialStair.transform, 1);
	}
	

	public override void UpdateObjective()
    {
		float proceed = 0;
		proceed = Mathf.InverseLerp(p_tutorialStair.p_StairBottomTransform.position.y, p_tutorialStair.p_StairTopTransform.position.y, m_Player.transform.position.y); // 계단 오르기 진행도
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
			case 2: // 계단 오르기
				m_ObjUI.SetObjectiveProgress(0, proceed);
				if (proceed >= 0.99f)
				{
                    m_Phase++;
					m_ObjUI.SetObjectiveFontStyle(0, true);
				}
				break;

            case 3: // 버튼 누르기
                if (m_Count >= 1)
                {
					m_ObjUI.SetObjectiveProgress(1, 1);
					m_ObjUI.SetObjectiveFontStyle(1, true);
					m_CameraMgr.MoveToTarget(p_tutorialDoor.transform, 1);
					m_InputMgr.SetAllLockByBool(true);
					m_Phase++;
				}
				break;
			case 4: // 문쪽으로 카메라 이동
                if(!m_CameraMgr.m_IsFollowTarget)
                {
                    p_tutorialDoor.action?.Invoke();
				}

                if(m_CameraMgr.m_IsMoveEnd)
                {
                    m_Phase = -1;
					m_ObjMgr.SendObjSuccessInfo(m_ObjIdx, true);
					m_InputMgr.p_MousePosLock = false;
					m_InputMgr.p_MoveInputLock = false;
					m_InputMgr.p_StairLock = false;
				}
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
			m_InputMgr.p_StairLock = false;
			m_Player.transform.localScale = m_InitialRotate;
			m_ObjUI.LerpUI(false);
			m_Phase++;
		}
	}
}