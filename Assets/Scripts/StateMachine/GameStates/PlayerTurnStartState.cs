using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnStartState : BaseState
{
    public override void OnStateEnter()
    {
        //EventSystem.CallEvent(EventType.ON_PLAYER_TURN_STARTED);
        //Unfreeze all player units.
        //Possibly apply status effects or re-evaluate them, to see if they still apply
        owner.SwitchState(typeof(PlayerTurnState));
    }

    public override void OnStateExit()
    {

    }

    public override void OnStateUpdate()
    {

    }

}
