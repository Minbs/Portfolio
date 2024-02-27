using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class ObjectiveMgr : MonoBehaviour
{
    // Visible Member Variables
    public float p_ObjectiveSuccessWaitTime = 2f;
    public ObjectiveUI m_ObjectiveUI = null;
    public List<Objective> p_ObjectiveList = new List<Objective>();
    public TutorialDroneObject p_TutorialDroneObject = null;

    // Member Variables
    public Objective m_CurObjective { get; private set; } = null;
    private Coroutine m_ObjectiveSuccessWaitCoroutine = null;


    // Constructors
    private void Start()
    {
        for (int i = 0; i < p_ObjectiveList.Count; i++)
        {
            p_ObjectiveList[i].SetObjectiveIdx(i);
        }
        
        m_CurObjective = p_ObjectiveList[0];
        m_ObjectiveUI.InputObjectiveToUI(m_CurObjective);
        m_CurObjective.InitObjective(this, m_ObjectiveUI);
    }


    // Updates
    private void Update()
    {
        m_CurObjective.UpdateObjective();
    }


    // Functions
    [Button]
    public void ChangeObjective(int _idx)
    {
        Debug.Log(_idx);
        if (!ReferenceEquals(m_ObjectiveSuccessWaitCoroutine, null))
        {
            StopCoroutine(m_ObjectiveSuccessWaitCoroutine);
            m_ObjectiveSuccessWaitCoroutine = null;
        }
        
        if (_idx < 0 || _idx >= p_ObjectiveList.Count)
        {
            Debug.Log("Info : ObjectiveMgr에서 ObjectiveList OOR");
            return;
        }

        if (!ReferenceEquals(m_CurObjective, null))
            m_CurObjective.ExitObjective();

        m_CurObjective = p_ObjectiveList[_idx];
        m_ObjectiveUI.ResetObjectiveUI();
        m_ObjectiveUI.InputObjectiveToUI(m_CurObjective);
        
        m_CurObjective.InitObjective(this, m_ObjectiveUI);
    }
    
    
    // Functions
    
    
    public void SelfExit(bool _playNext = false)
    {
        m_ObjectiveUI.ResetObjectiveUI();
        if (_playNext)
        {
            m_CurObjective.ExitObjective();
            m_CurObjective = null;
        }
        else
        {
            m_CurObjective.ExitObjective();
            m_CurObjective = null;
        }
    }

    public void SendObjSuccessInfo(int _idx, bool _playNext = false)
    {
        if (!ReferenceEquals(m_ObjectiveSuccessWaitCoroutine, null))
        {
            StopCoroutine(m_ObjectiveSuccessWaitCoroutine);
            m_ObjectiveSuccessWaitCoroutine = null;
        }

        m_ObjectiveSuccessWaitCoroutine = StartCoroutine(ObjSuccessWait(_idx, _playNext));
    }

    private IEnumerator ObjSuccessWait(int _idx, bool _playNext = false)
    {
        yield return new WaitForSecondsRealtime(p_ObjectiveSuccessWaitTime);

        if (_playNext)
        {
            m_ObjectiveUI.LerpUI(true, () => ChangeObjective(_idx + 1));
        }
        else
        {
            m_ObjectiveUI.LerpUI(true);
        }

        yield break;
    }
}