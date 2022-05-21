using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    public List<Enemy> allEnemies { get; set; } = new();

    private FSM stateMachine;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        instance = this;
        stateMachine = new FSM(typeof(FloorEntryState), GetComponents<BaseState>());

    }
    void Update()
    {
        stateMachine.OnStateMachineUpdate();
    }
}
