using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Pickupable
{
    public static int MAXWEIGHT = 100; // MAXWEIGHT is a maximum-weight as kg of the chest
    public float currentWeight; // Currentweight of the current Chest object
    
    public Dictionary<string, float> items = new Dictionary<string, float>();
    /// <summary>
    /// Sums weight of the chosen ItemType with stored ItemType weight
    /// </summary>
    /// <param name="itemtype">ItemType is a unique parameter of Resource like "woodTree"</param>
    /// <param name="weight">Weight as Kg for chosen ItemType like 2kg,woodTree </param>
    public void ItemAdd(string itemtype, float weight)
    {
        currentWeight += weight;
        if (items.ContainsKey(itemtype))
        {

            items[itemtype] += weight;

        }
        else
        {
            items.Add(itemtype, weight);
        }
        Printall();
    }
    public bool isfull()
    {
        return currentWeight > MAXWEIGHT;
    }
    /// <summary>
    /// Subtracts weight of the chosen ItemType with stored ItemType weight
    /// </summary>
    /// <param name="itemtype">ItemType is a unique parameter of Resource like "woodTree"</param>
    /// <param name="weight">Weight as Kg for chosen ItemType like 2kg,woodTree</param>
    /// <returns></returns>
    public KeyValuePair<string, float> ItemRemove(string itemtype, float weight)
    {
        currentWeight -= weight;
        KeyValuePair<string, float> Returner = new KeyValuePair<string, float>(itemtype, weight);
        if (items[itemtype] != 0 && weight <= items[itemtype])
        {
            items[itemtype] -= weight;
            Printall();
            return Returner;
        }
        else
        {
            Printall();
            return new KeyValuePair<string, float>("no_item", -1);
        }
        
    }
    /// <summary>
    /// Compares our given ItemType and Weight with stored ones in the chest
    /// </summary>
    /// <param name="itemtype">ItemType is a unique parameter of Resource like "woodTree"</param>
    /// <param name="weight">Weight as Kg for chosen ItemType like 2kg,woodTree</param>
    /// <returns> if chest has equal or more ItemType amount it will return us true otherwise false</returns>
    public bool ItemFind(string itemtype, float weight)
    {

        if (weight <= items[itemtype])
        {
            return true;
        }
        else
            return false;
    }
    /// <summary>
    /// Returns amount of given ItemType
    /// </summary>
    /// <param name="itemtype">ItemType is a unique parameter of Resource like "woodTree"</param>
    /// <returns>Returns amount of given ItemType</returns>
    public float ItemFind(string itemtype)
    {
        return items[itemtype];
    }
    /// <summary>
    /// Prints all resources in the Chest
    /// </summary>
    private void Printall()
    {
        foreach (var key in items)
        {
            Debug.Log(key.Key + " " + key.Value);
        }
    }


}
