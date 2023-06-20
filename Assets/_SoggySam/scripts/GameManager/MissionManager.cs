using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using _SoggySam.scripts.Spawner;


public class MissionManager : MonoBehaviour
{
    // reference to gamemanager
    private GameManager myGM;

    public List<SO_Mission> FinishMissions;
    public List<SO_Mission> ActiveMissions;
    public List<TMP_Text> TextList;
    public Animator panelAnimator;

    private bool _updateText = false;
    private static readonly int Trigger = Animator.StringToHash("Trigger");

    private void Start()
    {
        // get refence to gamemanager
        myGM = GetComponent<GameManager>();

        foreach (SO_Mission mission in ActiveMissions)
        {
            mission.Achieved = false;
            mission.currentProgress = 0;
            mission.lastProgress = 0;
        }

        _updateText = true;
    }

    public void FixedUpdate()
    {
        if (ActiveMissions.Count != 0) MissionProgress();
        MissionText();
    }

    public void MissionProgress()
    {
        for (int i = ActiveMissions.Count - 1; i > -1; i--)
        {
            if (!ActiveMissions[i].BossFight)
            { 
                ActiveMissions[i].currentProgress = GameManager.Instance.stats.inventory.checkInventoryForItemAmount(ActiveMissions[i].Objective);
                if (ActiveMissions[i].currentProgress > ActiveMissions[i].lastProgress)
                {
                    ActiveMissions[i].lastProgress = ActiveMissions[i].currentProgress;
                    _updateText = true;
                }
                if (ActiveMissions[i].Achieved == false && ActiveMissions[i].currentProgress >= ActiveMissions[i].Amount)
                {
                    ActiveMissions[i].Achieved = true;
                    FinishMissions.Add(ActiveMissions[i]);
                    ActiveMissions.Remove(ActiveMissions[i]);
                    // How many missions are left now?
                    if (ActiveMissions.Count == 0)
                    { // you have won popup
                        myGM.callUiPopup(1);
                    } else { // mission completed popup
                        myGM.callUiPopup(3);
                    }
                    _updateText = true;
                }
            }
        }
        
        if (ActiveMissions.Count == 1)
        { // is on last mission
            Debug.Log("last mission update");
            if (ActiveMissions[0].BossFight && !ActiveMissions[0].alreadySpawned)
            {
                GameObject.Find(ActiveMissions[0].activateSpawn).GetComponent<Spawner>().Spawn();
                ActiveMissions[0].alreadySpawned = true;
                Debug.Log("Spawning boss");
            }
        }
    }

    public void MissionText() 
    {
        if (!_updateText) return;
        _updateText = false;
        panelAnimator.SetTrigger(Trigger);
        if (ActiveMissions.Count > 1)
        SortMission();
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
            TextList[1].text = "No missions left";

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
        //ActiveMissions.Sort((t2, t1) => t2.Amount.CompareTo(t1.Amount - GameManager.Instance.stats.inventory.checkInventoryForItemAmount(t1.Objective)) - GameManager.Instance.stats.inventory.checkInventoryForItemAmount(t2.Objective));
    }

}
