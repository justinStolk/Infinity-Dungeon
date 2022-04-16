using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject cursorPrefab;
    public static GameManager instance;
    public Navigator selectedUnit;

    private Cursor cursor;
    private DungeonGeneratorV2 dungeonGenerator;

    private void Awake()
    {
        instance = this;
        dungeonGenerator = FindObjectOfType<DungeonGeneratorV2>();
        cursor = Instantiate(cursorPrefab, Vector3.zero, Quaternion.identity).GetComponent<Cursor>();
    }
    void Update()
    {
        Vector3 mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int roundedPosition = new Vector2Int(Mathf.RoundToInt(mouseLocation.x), Mathf.RoundToInt(mouseLocation.y));
        cursor.transform.position = new Vector3(roundedPosition.x, roundedPosition.y, 0);
        //if (Input.GetMouseButtonDown(0))
        //{
        //    if(selectedUnit != null)
        //    {
        //        if(dungeonGenerator.nodes.ContainsKey(roundedPosition))
        //        {
        //            selectedUnit.SetDestination(roundedPosition);
        //        }
        //    }
        //    else
        //    {
        //        cursor.OnCursorClick();
        //    } 
        //}
    }
}
