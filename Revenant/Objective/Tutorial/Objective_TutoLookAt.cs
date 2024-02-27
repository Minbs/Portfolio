using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;



public class Objective_TutoLookAt : Objective
{
    /*
     첫 번쨰로 하는 목표
     1. 왼쪽 보기
     2. 오른쪽 보기
     */
    
    private Player_InputMgr m_InputMgr;

    // Objective Variables
    private float m_Timer = 0f;
    private int m_Phase = 0;
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
        m_Timer = 0f;
        m_Phase = 0;

        m_Player.transform.localScale = new Vector3(1, 1, 1);
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
                m_ObjUI.LerpUI(false);
				m_InputMgr.p_MousePosLock = false;
                m_Phase++;
                break;
            
            case 2:
                if (!m_Player.m_IsRightHeaded)
                {
                    m_Timer += Time.deltaTime * 0.5f;
                    m_ObjUI.SetObjectiveProgress(0, m_Timer);

                    if (m_Timer >= 1f)
                    {
                        m_Timer = 0f;
                        m_ObjUI.SetObjectiveFontStyle(0, true);
                        m_Phase++;
                    }
                }
                break;
            
            case 3:
                if (m_Player.m_IsRightHeaded)
                {
                    m_Timer += Time.deltaTime * 0.5f;
                    m_ObjUI.SetObjectiveProgress(1, m_Timer);
                    
                    if (m_Timer >= 1f)
                    {
                        m_Timer = 0f;
                        m_ObjUI.SetObjectiveFontStyle(1, true);
                        m_Phase++;
                    }
                }
                break;
            
            case 4:
                //m_ObjMgr.SelfExit();
                m_ObjMgr.SendObjSuccessInfo(m_ObjIdx, true);
                m_Phase = -1;
                break;
        }
    }

    public override void ExitObjective()
    {
        
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
			m_Player.transform.localScale = m_InitialRotate;
			m_ObjMgr.p_TutorialDroneObject.p_TutorialDialog.SetDialogActive(false);
			m_Phase++;
		}
	}
}