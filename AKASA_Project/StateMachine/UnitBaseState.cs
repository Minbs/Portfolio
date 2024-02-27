using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBaseState
{
    public abstract void Begin(UnitStateMachine stateMachine);
    public abstract void Update(UnitStateMachine stateMachine);
    public abstract void End(UnitStateMachine stateMachine);
}
