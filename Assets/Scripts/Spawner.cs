using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour, IDamageable
{

    public int MaxHitPoints { get { return maxHitPoints; } }

    public int HitPoints { get { return hitPoints; } }

    public int Defense { get { return defense; } }

    public int Resistance { get { return resistance; } }

    public int Luck { get { return luck; } }

    public int Speed { get { return speed; } }

    [SerializeField] private float spawnChance;

    [SerializeField] private int maxHitPoints;
    [SerializeField] private int hitPoints;
    [SerializeField] private int defense;
    [SerializeField] private int resistance;

    private int speed = 0;
    private int luck = 0;

    private static Enemy[] allSpawnableEnemies = { };
    private static DungeonBuilder builder;

    // Start is called before the first frame update
    void Start()
    {
        EventSystem.Subscribe(EventType.ON_ENEMY_TURN_STARTED, SpawnEnemy);
        if(builder == null)
        {
            builder = FindObjectOfType<DungeonBuilder>();
        }
        if(allSpawnableEnemies.Length == 0)
        {
            allSpawnableEnemies = Resources.LoadAll<Enemy>("Enemies");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemy()
    {
        Vector2Int Position = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        if(Random.Range(0, 100) <= spawnChance)
        {
            for(int x = -1; x <= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                {
                    Vector2Int spawnPos = new Vector2Int(x, y) + Position; 
                    if(Mathf.Abs(x) == Mathf.Abs(y))
                    {
                        continue;
                    }
                    if(builder.data.nodes[spawnPos].occupyingElement == null)
                    {
                        List<Enemy> spawnableEnemies = new();
                        Debug.Log(builder.data.CurrentFloor);
                        foreach (Enemy e in allSpawnableEnemies)
                        {
                            Debug.Log("Found an enemy I could spawn, which is: " + e.name);
                            Debug.Log(e.FloorSpawnRange);
                            if (builder.data.CurrentFloor >=  e.FloorSpawnRange.x && builder.data.CurrentFloor <=  e.FloorSpawnRange.y)
                            {
                                spawnableEnemies.Add(e);
                            }
                        }
                        if(spawnableEnemies.Count == 0)
                        {
                            Debug.LogWarning("Couldn't spawn enemies, none are available for this floor!");
                            return;
                        }
                        Enemy enemyToSpawn = Instantiate(spawnableEnemies[Random.Range(0, spawnableEnemies.Count)], new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity);
                        builder.data.nodes[spawnPos].occupyingElement = enemyToSpawn.gameObject;
                        return;
                    }
                }
            }
            Debug.LogWarning("Could not find a valid place to spawn enemy!");
        }
    }

    public void ChangeHealth(int amount)
    {
        hitPoints += amount;
        if(hitPoints > maxHitPoints)
        {
            hitPoints = maxHitPoints;
        }
        if (hitPoints < 0)
        {
            hitPoints = 0;
            Vector2Int position = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            builder.data.nodes[position].occupyingElement = null;
            EventSystem.Unsubscribe(EventType.ON_ENEMY_TURN_STARTED, SpawnEnemy);
            Destroy(this.gameObject);
        }
    }
}
