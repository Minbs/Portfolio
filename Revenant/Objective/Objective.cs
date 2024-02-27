using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public abstract class Objective : MonoBehaviour
{
    // Member Variables
    public int m_ObjIdx;
    protected ObjectiveMgr m_ObjMgr;
    protected ObjectiveUI m_ObjUI;
    
    public string m_TitleTxt;
    public string[] m_ObjectiveTxtArr = new string[5];


    public void SetObjectiveIdx(int _idx)
    {
        m_ObjIdx = _idx;
    }
    public abstract void InitObjective(ObjectiveMgr _mgr, ObjectiveUI _ui);

    public abstract void UpdateObjective();

    public abstract void ExitObjective();
}