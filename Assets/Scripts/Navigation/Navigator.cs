using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator : MonoBehaviour, ISelectable
{
    public float moveSpeed;

    private AStar aStar;
    private List<Vector2Int> path = new();
    private DungeonGeneratorV2 generatorV2;

    // Start is called before the first frame update
    void Start()
    {
        generatorV2 = FindObjectOfType<DungeonGeneratorV2>();
        aStar = new AStar(ref generatorV2.nodes, false);    
        
    }

    // Update is called once per frame
    void Update()
    {
        if(path != null && path.Count > 0)
        {
            if (transform.position != Vector2IntToVector3(path[0]))
            {
                //float targetAngle = Mathf.Atan2(transform.position.x - path[0].x, transform.position.y - path[0].y);
                //targetAngle *= Mathf.Rad2Deg;
                //transform.rotation = Quaternion.Euler(0, targetAngle - 180, 0);
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
        Debug.Log("Selected: " + this.name);
    }

    public void SetDestination(Vector2Int target)
    {
        if (path.Count > 0)
            return;
        Vector2Int currentPosition = new(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        path = aStar.FindPathToTarget(currentPosition, target);
        generatorV2.nodes[currentPosition].occupyingElement = null;
        generatorV2.nodes[target].occupyingElement = this.gameObject;
    }

    private Vector3 Vector2IntToVector3(Vector2Int vector2Int)
    {
        return new Vector3(vector2Int.x, vector2Int.y, 0);
    }
}
