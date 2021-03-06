using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Navigator : MonoBehaviour, ISelectable
{
    public static float moveSpeed = 5;
    public int moveRange = 5;
    public bool hasMoved { get; private set; }

    private AStar aStar;
    private List<Vector2Int> path = new();
    private DungeonBuilder builder;

    private List<Vector2Int> rangeTilePositions;

    void Awake()
    {
        builder = FindObjectOfType<DungeonBuilder>();
        aStar = new AStar(builder.data.nodes, false);
        EventSystem.Subscribe(EventType.ON_PLAYER_TURN_STARTED, UnFreezeUnit);
    }

    // Update is called once per frame
    void Update()
    {
        if(path != null && path.Count > 0)
        {
            if (transform.position != Vector2IntToVector3(path[0]))
            {
                transform.position = Vector3.MoveTowards(transform.position, Vector2IntToVector3(path[0]), moveSpeed * Time.deltaTime);
            }
            else
            {
                path.RemoveAt(0);
            }
        }
    }
    public Task MoveToTarget()
    {
        while(path != null && path.Count > 0)
        {
            if (transform.position != Vector2IntToVector3(path[0]))
            {
                transform.position = Vector3.MoveTowards(transform.position, Vector2IntToVector3(path[0]), moveSpeed * Time.deltaTime);
            }
            else
            {
                path.RemoveAt(0);
            }
        }
        return Task.CompletedTask;
    }
    public void OnSelected()
    {
        if (path.Count > 0 || hasMoved)
            return;
        rangeTilePositions = aStar.GetNodesInRange(Vector3ToVector2Int(transform.position), moveRange);
        foreach(Vector2Int v in rangeTilePositions)
        {
            builder.data.moveRangeTiles[v].SetActive(true);
        }
        Debug.Log("Selected: " + this.name);
    }

    public void SetDestination(Vector2Int target)
    {
        foreach(Vector2Int v in rangeTilePositions)
        {
            builder.data.moveRangeTiles[v].SetActive(false);
        }
        if (path.Count > 0 || !rangeTilePositions.Contains(target))
            return;
        Vector2Int currentPosition = new(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        path = aStar.FindPathToTarget(currentPosition, target);
        builder.data.nodes[currentPosition].occupyingElement = null;
        builder.data.nodes[target].occupyingElement = this.gameObject;
        //FreezeUnit();
    }
    public void FreezeUnit()
    {
        hasMoved = true;
        //Possibly grey out the character;
    }
    public void UnFreezeUnit()
    {
        hasMoved = false;
        //Set unit colour back to default.
    }
    private Vector3 Vector2IntToVector3(Vector2Int vector2Int)
    {
        return new Vector3(vector2Int.x, vector2Int.y, 0);
    }
    private Vector2Int Vector3ToVector2Int(Vector3 vector3)
    {
        return new Vector2Int(Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y));
    }
}
