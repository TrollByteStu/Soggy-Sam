using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class MissionManager : MonoBehaviour
{
    public List<SO_Mission> ActiveMissions;
    public List<TMP_Text> TextList;

    private void Start()
    {
        foreach (SO_Mission mission in ActiveMissions)
        {
            mission.Achieved = false;
        }
    }

    public void FixedUpdate()
    {
        SortMission();
        MissionAchieved();
        MissionText();
    }

    public void MissionAchieved()
    {
        foreach (SO_Mission mission in ActiveMissions)
        {
            if (mission.Achieved == false && GameManager.Instance.stats.inventory.checkInventoryForItemAmount(mission.Objective) >= mission.Amount)
                mission.Achieved = true;
        }
    }

    public void MissionText()
    {
        for (int i = 0; i < 3; i++)
        {
            if (ActiveMissions.Count > i)
            {
                TextList[i].text = ActiveMissions[i].ToolTip + GameManager.Instance.stats.inventory.checkInventoryForItemAmount(ActiveMissions[i].Objective) + "/" + ActiveMissions[i].Amount;
                if (ActiveMissions[i].Achieved)
                    TextList[i].text += "<color=green> V </color>";
                else
                    TextList[i].text += "<color=red> X </color>";
                //if (ActiveMissions[i].Achieved)
                //    TextList[i].fontStyle = FontStyles.Strikethrough;
                //else
                //    TextList[i].fontStyle = FontStyles.Normal;
            }
            else if (i == TextList.Count - 1) TextList[i].text = "-------------------";
            else TextList[i].text = "";
        }
    }

    public void AddMission(SO_Mission mission)
    {
        mission.Achieved = false;
        if (!ActiveMissions.Contains(mission))
            ActiveMissions.Add(mission);
        else Debug.Log("already have this mission");
    }

    public void RemoveMission(SO_Mission mission)
    {
        ActiveMissions.Remove(mission);
    }

    public void RemoveMission(int mission)
    {
        ActiveMissions.RemoveAt(mission);
    }

    public void SortMission()
    {
        ActiveMissions.Sort((t2, t1) => t2.Amount.CompareTo(t1.Amount - GameManager.Instance.stats.inventory.checkInventoryForItemAmount(t1.Objective)) - GameManager.Instance.stats.inventory.checkInventoryForItemAmount(t2.Objective));
    }

}
