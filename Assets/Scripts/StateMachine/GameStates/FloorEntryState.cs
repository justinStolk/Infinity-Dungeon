using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorEntryState : BaseState
{
    [SerializeField] private DungeonGeneratorV2 dungeonGenerator;
    [SerializeField] private DungeonBuilder dungeonBuilder;

    public override void OnStateEnter()
    {
        //dungeonGenerator.GenerateDungeon();
        //dungeonBuilder.BuildDungeon(dungeonGenerator.dungeonData);
        owner.SwitchState(typeof(PlayerTurnStartState));
    }

    public override void OnStateExit()
    {
        
    }

    public override void OnStateUpdate()
    {
        
    }

}
