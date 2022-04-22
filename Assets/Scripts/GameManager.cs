using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private FSM stateMachine;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        instance = this;
        stateMachine = new FSM(typeof(PlayerTurnState), GetComponents<BaseState>());

    }
    void Update()
    {
        stateMachine.OnStateMachineUpdate();
    }
}
