using Unity.VisualScripting;
using UnityEngine;


public class Objective_TutoEnterDoor : Objective
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
	public TutorialDoorObject p_TutorialDoor;
    public TutorialObject p_TutorialNextDoor;
	public TutorialStairObject p_tutorialStair;

	public override void InitObjective(ObjectiveMgr _mgr, ObjectiveUI _ui)
    {
        m_ObjMgr = _mgr;
        m_ObjUI = _ui;
        
        m_Player = GameMgr.GetInstance().p_PlayerMgr.GetPlayer();
        m_InputMgr = GameMgr.GetInstance().p_PlayerInputMgr;
        
        m_InputMgr.SetAllLockByBool(true);

		m_InputMgr.p_MousePosLock = false;
		m_InputMgr.p_MoveInputLock = false;
		m_InputMgr.p_StairLock = false;

		m_ObjUI.LerpUI(false);
		p_TutorialDoor.action += AddCount;
        m_Phase = 0;
        m_Count = 0;
    }

    public override void UpdateObjective()
    {
        switch (m_Phase)
        {
            case 0:
                if (m_Count == 0)
                {
    				float proceed = 0;
	    			proceed = Mathf.InverseLerp(p_tutorialStair.p_StairTopTransform.position.y, p_tutorialStair.p_StairBottomTransform.position.y, m_Player.transform.position.y); // 계단 오르기 진행도
		    		if (proceed >= 0.99f)
			    	{
				    	proceed = 1;
					    m_ObjUI.SetObjectiveFontStyle(0, true);
                        m_Count++;
				    }
					m_ObjUI.SetObjectiveProgress(0, proceed);
				}

				if (m_Count >= 2)
                {
					m_ObjUI.SetObjectiveProgress(1, 1f);
                    m_ObjUI.SetObjectiveFontStyle(1, true);
                    m_ObjMgr.SendObjSuccessInfo(m_ObjIdx, true);
					p_TutorialNextDoor.NextAnimation();
					m_Phase = -1;
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
}