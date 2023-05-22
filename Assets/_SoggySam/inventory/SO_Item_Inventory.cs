using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class SO_Item_Inventory
{
    [System.Serializable]
    public class Resource
    {
        public SO_Item item;
        public int amount;
        public int randomMin;
        public int randomMax;

        public Resource set(SO_Item newitem, int newamount)
        {
            var newResource = new Resource();
            item = newitem;
            amount = newamount;
            return newResource;
        }
    }
    public List<Resource> Inventory;

    private float massCalc;

    //constructor
    public SO_Item_Inventory()
    {
        Inventory = new List<Resource>();
    }

    public void RandomizeLoot()
    {
        if (Inventory.Count > 0)
        {// if there is things on the list, we can do a foreach
            foreach (Resource res in Inventory)
            {
                if (res.randomMax > 0) res.amount = Random.Range(res.randomMin, res.randomMax);
            }
        }
    }

    public int countItem( string countName )
    {
        if (Inventory.Count > 0)
        {// if there is things on the list, we can do a foreach
            foreach (Resource res in Inventory)
            {
                if (res.item.itemName == countName)
                {
                    return res.amount;
                }
            }
        }
        return 0;
    }

    public string InventoryToString()
    {
        string output = "";
        bool moreThanOne = false;
        if (Inventory.Count > 0)
        {// if there is things on the list, we can do a foreach
            foreach (Resource res in Inventory)
            {
                if (moreThanOne) output += "\n";
                output += res.item.itemName + " : " + res.amount;
                moreThanOne = true;
            }
        }
        return output;
    }

    public SO_Item removeRandom()
    { // remove one random item and return it
        if (Inventory.Count < 1) return null;
        int rand = Random.Range(0, Inventory.Count);
        if (Inventory[rand].amount < 1) return null;
        Inventory[rand].amount--;
        return Inventory[rand].item;
    }

    public void addItem(string item, int num)
    { // this one can easily crash the whole thing
        if (item == "") return;
        if (Inventory.Count > 0)
        {// if there is things on the list, we can do a foreach
            foreach (Resource res in Inventory)
            {
                if (res.item.itemName == item)
                {
                    res.amount += num;
                    return;
                }
            }
        }  // there is no list or items is not found, so we add it
        Debug.Log("Tried to add new item " + item + " amount " + num.ToString());
    }
    public void addItem( SO_Item item, int num )
    { // this one can easily crash the whole thing
        if (item == null) return;
        if ( Inventory.Count> 0)
        {// if there is things on the list, we can do a foreach
            Debug.Log("newname "+item.itemName+" id "+item.GetInstanceID().ToString());
            foreach (Resource res in Inventory)
            {
                Debug.Log("trying "+res.item.itemName+" id "+res.item.GetInstanceID().ToString());
                if ( res.item == item )
                {
                    res.amount += num;
                    return;
                }
            }
        }  // there is no list or items is not found, so we add it
        var newItem = new Resource();
        newItem.set(item, num);
        Inventory.Add(newItem);
    }
    public void addItem(SO_Item item)
    { // just add one
        addItem(item, 1);
    }

    public void removeItem( SO_Item item, int num )
    {
        if (item == null) return;
        if (Inventory.Count > 0)
        {// if there is things on the list, we can do a foreach
            foreach (Resource res in Inventory)
            {
                if (res.item == item)
                {
                    res.amount -= num;
                    return;
                }
            }
        }  // there is no list or items is not found, bugger!
        Debug.Log("Trying to remove item that isn't there: " + item.itemName);
    }

    public void removeItem(string removeName, int num)
    {
        if (Inventory.Count > 0)
        {// if there is things on the list, we can do a foreach
            foreach (Resource res in Inventory)
            {
                if (res.item.itemName == removeName)
                {
                    res.amount -= num;
                    return;
                }
            }
        }  // there is no list or items is not found, bugger!
        Debug.Log("Trying to remove item that isn't there(string): " + removeName);
    }

    public void cleanInventory()
    {// removes everything
        if (Inventory.Count > 0)
        {// if there is things on the list, we can do a foreach
            foreach (Resource res in Inventory)
            {
                res.amount = 0;
            }
        }  // there is no list or items is not found, bugger!
    }

    public string getJSON()
    {// create custom data string from inventory 
        string thelist = "";
        if ( Inventory.Count > 0)
        { // we dont' do foreach unless we have some items
            for ( int i = 0; i < Inventory.Count; i++)
            {
                thelist += "#%"+Inventory[i].item.itemName+"#"+Inventory[i].amount;
            }
        }
        return thelist;
    }

    public void addJSON(string newItems)
    { // get a JSON string, and adds that to inventory
        string[] temp = newItems.Split("%");
        for(int i = 0; i < temp.Length; i++)
        { // keep adding and removing until none are left
            int amount = 0;
            if (temp[i].Split("#").Length > 1)
                int.TryParse(temp[i].Split("#")[1], out amount);
            addItem(temp[i].Split("#")[0], amount);
        }
    }

    public float getCombinedMass()
    {
        massCalc = 0f;
        if (Inventory.Count > 0)
        {
            foreach (Resource eachItem in Inventory)
            {
                massCalc += (float)eachItem.amount * eachItem.item.itemMass;
            }
        }
        return massCalc;
    }

    public bool checkInventoryForItem(SO_Item item, int amount)
    {
        if ( Inventory.Count > 0 )
        {
            foreach(Resource checkThis in Inventory)
            {
                if (item.itemName == checkThis.item.itemName && checkThis.amount >= amount) return true;
            }
        }
        return false;
    }

    public bool checkInventoryForRecipe(Resource[] ingredients)
    {
        if ( ingredients.Length > 0)
        {
            for (int i = 0; i < ingredients.Length; i++)
            {
                if (!checkInventoryForItem(ingredients[i].item,ingredients[i].amount)) return false;
            }
        }
        return true;
    }

}
