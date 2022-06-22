using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private float levelUpDisplayTime;
    [SerializeField] private GameObject levelUpInterface;
    [SerializeField] private Text statText;
    [SerializeField] private Text statIncreaseText;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLevelUp(int[] oldStats, int[] updatedStats)
    {
        statText.text = $"Hit Points: {oldStats[0]}/{oldStats[1]}\nStrength: {oldStats[2]}\nDefense: " +
            $"{oldStats[3]}\nMagic: {oldStats[4]}\nResistance: {oldStats[5]}\nSpeed: {oldStats[6]}\nLuck: {oldStats[7]}";

        int[] statIncreases = new int[7];
        statIncreases[0] = updatedStats[0] - oldStats[0];
        for(int i = 2; i < 8; i++)
        {
            statIncreases[i - 1] = updatedStats[i] - oldStats[i]; 
        }
        //statIncreases[1] = updatedStats.Strength - oldStats.Strength;
        //statIncreases[2] = updatedStats.Defense - oldStats.Defense;
        //statIncreases[3] = updatedStats.Magic - oldStats.Magic;
        //statIncreases[4] = updatedStats.Resistance - oldStats.Resistance;
        //statIncreases[5] = updatedStats.Speed - oldStats.Speed;
        //statIncreases[6] = updatedStats.Luck - oldStats.Luck;

        statIncreaseText.text = "";
        foreach (int incr in statIncreases)
        {
            //Debug.Log(incr);
            if (incr > 0)
            {
                statIncreaseText.text += $"+{incr}";
            }
            statIncreaseText.text += "\n";
        }
        StartCoroutine(HandleLevelUpMenu(updatedStats));
    }

    private IEnumerator HandleLevelUpMenu(int[] newStats)
    {
        levelUpInterface.SetActive(true);
        yield return new WaitForSeconds(1f);
        statText.text = $"Hit Points: {newStats[0]}/{newStats[1]}\nStrength: {newStats[2]}\nDefense: {newStats[3]}\nMagic: {newStats[4]}\nResistance: {newStats[5]}\nSpeed: {newStats[6]}\nLuck: {newStats[7]}";
        yield return new WaitForSeconds(levelUpDisplayTime);
        levelUpInterface.SetActive(false);
    }


}
