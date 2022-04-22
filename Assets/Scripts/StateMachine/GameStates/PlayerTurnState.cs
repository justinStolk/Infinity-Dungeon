using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnState : BaseState
{
    public GameObject cursorPrefab;

    [SerializeField] private float maxCamNodeDistance = 8;
    [SerializeField] private float camMoveSpeed;

    private Navigator selectedUnit;
    private GameObject cursor;
    private DungeonGeneratorV2 dungeonGenerator;
    private List<Navigator> playerCharacters = new();
    private Camera gameCam;
    private Vector2Int roundedMousePosition;

    private void Start()
    {
        gameCam = Camera.main;
        dungeonGenerator = FindObjectOfType<DungeonGeneratorV2>();
        cursor = Instantiate(cursorPrefab, Vector3.zero, Quaternion.identity);
    }
    public override void OnStateEnter()
    {
        foreach (Navigator pc in playerCharacters)
        {
            pc.UnFreezeUnit();
            //Unfreeze all characters, so they can move again.
        }
    }

    public override void OnStateUpdate()
    {
        Vector3 mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        roundedMousePosition = new Vector2Int(Mathf.RoundToInt(mouseLocation.x), Mathf.RoundToInt(mouseLocation.y));
        cursor.transform.position = new Vector3(roundedMousePosition.x, roundedMousePosition.y, 0);
        HandleInput();
        HandleCamMovement();

    }
    public override void OnStateExit()
    {
        
    }

    private void HandleCamMovement()
    {
        if (dungeonGenerator.nodes.ContainsKey(roundedMousePosition) && Vector3.Distance(new Vector3(roundedMousePosition.x, roundedMousePosition.y, transform.position.z), gameCam.transform.position) > maxCamNodeDistance)
        {
            gameCam.transform.position = Vector3.MoveTowards(gameCam.transform.position, new Vector3(roundedMousePosition.x, roundedMousePosition.y, gameCam.transform.position.z), camMoveSpeed * Time.deltaTime);
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedUnit == null)
            {
                if (dungeonGenerator.nodes.ContainsKey(roundedMousePosition))
                {
                    GameObject obj = dungeonGenerator.nodes[roundedMousePosition].occupyingElement;
                    selectedUnit = obj?.GetComponent<Navigator>();
                    selectedUnit?.OnSelected();

                }
                return;
            }
            selectedUnit.SetDestination(roundedMousePosition);
            selectedUnit = null;
        }
    }

}