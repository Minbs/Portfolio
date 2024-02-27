using UnityEngine;



public class Objective_TutoMove : Objective
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
    private Player m_Player;
    
    private float m_LTimer = 0f;
    private float m_RTimer = 0f;

    public override void InitObjective(ObjectiveMgr _mgr, ObjectiveUI _ui)
    {
        Debug.Log("move");
        m_ObjMgr = _mgr;
        m_ObjUI = _ui;
        
        m_Player = GameMgr.GetInstance().p_PlayerMgr.GetPlayer();
        m_InputMgr = GameMgr.GetInstance().p_PlayerInputMgr;

		m_InputMgr.SetAllLockByBool(true);
        m_InputMgr.p_MousePosLock = false;
        m_InputMgr.p_MoveInputLock = false;
        m_LTimer = 0f;
        m_RTimer = 0f;
        m_Phase = 0;
        m_Count = 0;
		m_ObjUI.LerpUI(false);
	}

    public override void UpdateObjective()
    {
        switch (m_Phase)
        {
            case 0:
                if (m_InputMgr.m_IsPushLeftKey && m_LTimer < 1f)
                {
                    m_LTimer += Time.deltaTime * 0.5f;
                    m_ObjUI.SetObjectiveProgress(0, m_LTimer);
                    if (m_LTimer >= 1f)
                    {
                        m_ObjUI.SetObjectiveFontStyle(0, true);
                        m_Count++;

					}
                }

                if (m_InputMgr.m_IsPushRightKey && m_RTimer < 1f)
                {
                    m_RTimer += Time.deltaTime * 0.5f;
                    m_ObjUI.SetObjectiveProgress(1, m_RTimer);
                    if (m_RTimer >= 1f)
                    {
                        m_ObjUI.SetObjectiveFontStyle(1, true);
                        m_Count++;
                    }
                }

                if (m_Count >= 2)
                {
                    m_Count = 0;
                    m_Phase = 1;
                }
                break;
            
            case 1:
                m_ObjMgr.SendObjSuccessInfo(m_ObjIdx, true);

				m_Phase = -1;
                break;
        }
    }

    public override void ExitObjective()
    {
        
    }
}