using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject cursorPrefab;
    public static GameManager instance;
    public Navigator selectedUnit;

    private GameObject cursor;
    private DungeonGeneratorV2 dungeonGenerator;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        instance = this;
        dungeonGenerator = FindObjectOfType<DungeonGeneratorV2>();
        cursor = Instantiate(cursorPrefab, Vector3.zero, Quaternion.identity);
    }
    void Update()
    {
        Vector3 mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int roundedPosition = new Vector2Int(Mathf.RoundToInt(mouseLocation.x), Mathf.RoundToInt(mouseLocation.y));
        cursor.transform.position = new Vector3(roundedPosition.x, roundedPosition.y, 0);
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedUnit == null)
            {
                if (dungeonGenerator.nodes.ContainsKey(roundedPosition))
                {
                    GameObject obj = dungeonGenerator.nodes[roundedPosition].occupyingElement;
                    selectedUnit = obj?.GetComponent<Navigator>();
                    selectedUnit?.OnSelected();

                }
                return;
            }
            selectedUnit.SetDestination(roundedPosition);
            selectedUnit = null;
        }
    }
}
