using UnityEngine;


public class Objective_TutoReload : Objective
{
/*
     네 번쨰로 하는 목표
     1. 장전하기
     */
    
    private Player_InputMgr m_InputMgr;

    // Objective Variables
    private int m_Phase = 0;
    private Player m_Player;


    public override void InitObjective(ObjectiveMgr _mgr, ObjectiveUI _ui)
    {
        m_ObjMgr = _mgr;
        m_ObjUI = _ui;
        
        m_Player = GameMgr.GetInstance().p_PlayerMgr.GetPlayer();
        m_InputMgr = GameMgr.GetInstance().p_PlayerInputMgr;
        
        m_InputMgr.SetAllLockByBool(true);
        
        m_InputMgr.p_MousePosLock = false;
        m_InputMgr.p_MoveInputLock = false;
        m_InputMgr.p_FireLock = false;
        m_InputMgr.p_ReloadLock = false;

        m_Phase = 0;
    }

    public override void UpdateObjective()
    {
        switch (m_Phase)
        {
            case 0:
                if (m_InputMgr.m_IsPushReloadKey)
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

    }
}