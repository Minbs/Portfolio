using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitApproachingState : UnitBaseState
{
    public override void Begin(UnitStateMachine stateMachine)
    {

    }

    public override void Update(UnitStateMachine stateMachine) 
    {
        if (stateMachine.unit.spineAnimation.skeletonAnimation.AnimationName != stateMachine.unit.skinName + "/run"
            && stateMachine.gameObject.GetComponent<Enemy>()) 
            stateMachine.unit.spineAnimation.PlayAnimation(stateMachine.unit.skinName + "/run", true, GameManager.Instance.gameSpeed);
        else if (stateMachine.unit.spineAnimation.skeletonAnimation.AnimationName != stateMachine.unit.skinName + "/run"
            && stateMachine.gameObject.GetComponent<Minion>())
            stateMachine.unit.spineAnimation.PlayAnimation(stateMachine.unit.skinName + "/run", true, GameManager.Instance.gameSpeed);

        if (stateMachine.gameObject.GetComponent<Minion>())
        {
            if (stateMachine.gameObject.GetComponent<Minion>().minionClass == MinionClass.Rescue)
                stateMachine.SetTargetInCognitiveRange();
            else
                stateMachine.SetTargetInCognitiveRange();
        }
        else if (stateMachine.gameObject.GetComponent<Enemy>())
            stateMachine.SetTargetInCognitiveRange();


        if (stateMachine.unit.target == null || !stateMachine.unit.target.activeSelf)
            stateMachine.ChangeState(stateMachine.moveState);
        else
        {
            stateMachine.agent.SetDestination(stateMachine.unit.target.transform.position);
            stateMachine.LookAtTarget(stateMachine.unit.target.transform.position);

            if (stateMachine.IsTargetInAttackRange())
                stateMachine.ChangeState(stateMachine.AttackState);
        }
    }

    public override void End(UnitStateMachine stateMachine)
    {

    }
}
