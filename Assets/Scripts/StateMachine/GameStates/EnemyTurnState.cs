using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnState : BaseState
{
    public override void OnStateEnter()
    {
    }

    public override void OnStateExit()
    {
    }

    public override void OnStateUpdate()
    {
        //Let the computer handle movement and strategies for all enemies in a list of unfrozen enemies.



        //At the end, end the turn and let the player move again
        owner.SwitchState(typeof(PlayerTurnStartState));
    }

}
