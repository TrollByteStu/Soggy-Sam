using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Item", order = 1)]
public class SO_Item : ScriptableObject
{
    public string itemName;

    public string itemDescription;

    [Tooltip("The Icon to describe this item")]
    public Texture itemIcon;

    public float itemMass;

    public float itemBaseValue;

    //[Tooltip("Keep at zero until we get this to work!!! ;)")]
    //public int Amount = 0;
}
