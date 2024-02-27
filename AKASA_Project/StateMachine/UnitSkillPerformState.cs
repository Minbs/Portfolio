using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSkillPerformState : UnitBaseState
{
    public override void Begin(UnitStateMachine stateMachine)
    {
        stateMachine.agent.isStopped = true;
    }

    public override void Update(UnitStateMachine stateMachine)
    {
    }

    public override void End(UnitStateMachine stateMachine)
    {
        stateMachine.agent.isStopped = false;
    }
}
