using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mission", menuName = "ScriptableObjects/Mission", order = 3)]
public class SO_Mission : ScriptableObject
{
    public string Mission_name;
    public string Mission_desc;
    [Header("Objective")]
    public SO_Item Objective;
    public string ToolTip;
    public int Amount;
    public bool Achieved = false;
}
