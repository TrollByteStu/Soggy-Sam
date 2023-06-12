using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class MissionManager : MonoBehaviour
{
    public List<SO_Mission> FinishMissions;
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
            {
                mission.Achieved = true;
                FinishMissions.Add(mission);
                ActiveMissions.Remove(mission);
            }
        }
    }

    public void MissionText() 
    {
        if (FinishMissions.Count > 0)
        {
            TextList[0].text = FinishMissions[FinishMissions.Count - 1].ToolTip + GameManager.Instance.stats.inventory.checkInventoryForItemAmount(FinishMissions[FinishMissions.Count - 1].Objective) + "/" + FinishMissions[FinishMissions.Count - 1].Amount;
            TextList[0].text += " <color=green> V </color>";
        }
        else
            TextList[0].text = "-------------------";

        if (ActiveMissions.Count > 0)
        {
            TextList[1].text = ActiveMissions[0].ToolTip + GameManager.Instance.stats.inventory.checkInventoryForItemAmount(ActiveMissions[0].Objective) + "/" + ActiveMissions[0].Amount;
            TextList[1].text += "<color=red> X </color>";
        }
        else
            TextList[2].text = "No missions left";

        if (ActiveMissions.Count > 1)
        {
            TextList[2].text = ActiveMissions[1].ToolTip + GameManager.Instance.stats.inventory.checkInventoryForItemAmount(ActiveMissions[1].Objective) + "/" + ActiveMissions[1].Amount;
            TextList[2].text += "<color=red> X </color>";
        }
        else
            TextList[2].text = "-------------------";
    }

    //public void MissionText()
    //{
    //    for (int i = 0; i < 3; i++)
    //    {
    //        if (ActiveMissions.Count > i)
    //        {
    //            TextList[i].text = ActiveMissions[i].ToolTip + GameManager.Instance.stats.inventory.checkInventoryForItemAmount(ActiveMissions[i].Objective) + "/" + ActiveMissions[i].Amount;
    //            if (ActiveMissions[i].Achieved)
    //                TextList[i].text += " <color=green> V </color>";
    //            else
    //                TextList[i].text += "<color=red> X </color>";
    //            //if (ActiveMissions[i].Achieved)
    //            //    TextList[i].fontStyle = FontStyles.Strikethrough;
    //            //else
    //            //    TextList[i].fontStyle = FontStyles.Normal;
    //        }
    //        else if (i == TextList.Count - 1) TextList[i].text = "-------------------";
    //        else TextList[i].text = "";
    //    }
    //}

    public void AddMission(SO_Mission mission)
    {
        if (!ActiveMissions.Contains(mission) && !FinishMissions.Contains(mission))
        {
            mission.Achieved = false;
            ActiveMissions.Add(mission);
        }
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
