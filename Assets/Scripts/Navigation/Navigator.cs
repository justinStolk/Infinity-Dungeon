using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator : MonoBehaviour, ISelectable
{
    public float moveSpeed;

    private AStar aStar;
    private List<Vector2Int> path = new();


    // Start is called before the first frame update
    void Start()
    {
        aStar = new AStar(ref FindObjectOfType<DungeonGeneratorV2>().nodes, false);    
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
    public void OnSelected()
    {
        GameManager.instance.selectedUnit = this;
    }

    public void SetDestination(Vector2Int target)
    {
        Vector2Int currentPosition = new(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        path = aStar.FindPathToTarget(currentPosition, target);
    }

    private Vector3 Vector2IntToVector3(Vector2Int vector2Int)
    {
        return new Vector3(vector2Int.x, vector2Int.y, 0);
    }
}
