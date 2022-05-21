using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnStartState : BaseState
{
    public override void OnStateEnter()
    {
        EventSystem.CallEvent(EventType.ON_ENEMY_TURN_STARTED);
        //Unfreeze all enemy units.
        ////foreach(Enemy e in GameManager.instance.allEnemies)
        ////{

        ////}
        //Possibly apply status effects or re-evaluate them, to see if they still apply
        //Spawn units here? Or some other moment?
        owner.SwitchState(typeof(EnemyTurnState));
    }

    public override void OnStateExit()
    {
    }

    public override void OnStateUpdate()
    {
    }
}
