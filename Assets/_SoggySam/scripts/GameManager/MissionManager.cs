using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class MissionManager : MonoBehaviour
{
    public List<SO_Mission> ActiveMissions;
    public TMP_Text text1;
    public TMP_Text text2;
    public TMP_Text text3;

    public void MissionText()
    {
        if (ActiveMissions.Count > 0)
        {
            text1.text = ActiveMissions[0].ToolTip + GameManager.Instance.stats.inventory.checkInventoryForItemAmount(ActiveMissions[0].Objective) + "/" + ActiveMissions[0].amount;
            if (GameManager.Instance.stats.inventory.checkInventoryForItemAmount(ActiveMissions[0].Objective) >= ActiveMissions[0].amount)
                text1.fontStyle = FontStyles.Strikethrough;

        }
        else text1.text = "";
        if (ActiveMissions.Count > 1)
        {
            text2.text = ActiveMissions[1].ToolTip + GameManager.Instance.stats.inventory.checkInventoryForItemAmount(ActiveMissions[1].Objective) + "/" + ActiveMissions[1].amount;
            if (GameManager.Instance.stats.inventory.checkInventoryForItemAmount(ActiveMissions[1].Objective) >= ActiveMissions[1].amount)
                text1.fontStyle = FontStyles.Strikethrough;
        }
        else text2.text = "";
        if (ActiveMissions.Count > 2)
        {
            text3.text = ActiveMissions[2].ToolTip + GameManager.Instance.stats.inventory.checkInventoryForItemAmount(ActiveMissions[2].Objective) + "/" + ActiveMissions[2].amount;
            if (GameManager.Instance.stats.inventory.checkInventoryForItemAmount(ActiveMissions[2].Objective) >= ActiveMissions[2].amount)
                text1.fontStyle = FontStyles.Strikethrough;
        }
        else text3.text = "No mision";

    }

    public void AddMission(SO_Mission mission)
    {
        if (!ActiveMissions.Contains(mission))
            ActiveMissions.Add(mission);
        else Debug.Log("already have this mission");
    }

    public void RemoveMission(SO_Mission mission)
    {
        ActiveMissions.Remove(mission);
    }

    public void SortMission()
    {
        ActiveMissions.Sort((t2, t1) => t2.amount.CompareTo(t1.amount - GameManager.Instance.stats.inventory.checkInventoryForItemAmount(t1.Objective)) - GameManager.Instance.stats.inventory.checkInventoryForItemAmount(t2.Objective));
    }

    public void FixedUpdate()
    {
        SortMission();
        MissionText();
    }
}
