using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveState : UnitBaseState
{
    public override void Begin(UnitStateMachine stateMachine)
    {

    }

    public override void Update(UnitStateMachine stateMachine)
    {
        if (stateMachine.gameObject.GetComponent<Enemy>())
            stateMachine.MoveToDirection(stateMachine.unit.direction);
        else if(stateMachine.gameObject.GetComponent<Minion>() && stateMachine.unit.target == null)
            stateMachine.ReturnToTilePosition();


        
        if (stateMachine.unit.spineAnimation.skeletonAnimation.AnimationName != stateMachine.unit.skinName + "/run"
            && stateMachine.gameObject.GetComponent<Enemy>())
            stateMachine.unit.spineAnimation.PlayAnimation(stateMachine.unit.skinName + "/run", true, GameManager.Instance.gameSpeed);
        else if (stateMachine.unit.spineAnimation.skeletonAnimation.AnimationName != stateMachine.unit.skinName + "/run"
            && stateMachine.gameObject.GetComponent<Minion>())
            stateMachine.unit.spineAnimation.PlayAnimation(stateMachine.unit.skinName + "/run", true, GameManager.Instance.gameSpeed);

        if (GameManager.Instance.state == State.BATTLE)
            BattleMove(stateMachine);
        else if (GameManager.Instance.state == State.WAVE_END)
            stateMachine.ReturnToTilePosition();
    }

    public override void End(UnitStateMachine stateMachine)
    {

    }

    public void BattleMove(UnitStateMachine stateMachine)
    {
        if (stateMachine.gameObject.GetComponent<Minion>())
        {
                stateMachine.SetTargetInCognitiveRange();

            if (stateMachine.unit.target)
                stateMachine.ChangeState(stateMachine.approachingState);
        }
        else if (stateMachine.gameObject.GetComponent<Enemy>())
        {

            stateMachine.SetTargetInCognitiveRange();

            if (stateMachine.unit.target)
                stateMachine.ChangeState(stateMachine.approachingState);
        }
    }
}
