using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int capacity =5; // Max nukes to hold
    private int nukeCount; // number of nukes

    void Start()
    {
        nukeCount = 0;
        GameManager.GetInstance().uiManager.UpdateNukeCount(nukeCount);   
    }

    // Called when a nuke is picked up
    public bool AddNuke()
    {
        // If inventory is full, do not add
        if (nukeCount >= capacity)
        {
            Debug.Log("Nuke inventory is full.");
            return false;
        }

        nukeCount++; // Increment count
        Debug.Log($"Nuke added. Count = {nukeCount}");

        // Update the UI with new nuke count
        GameManager.GetInstance().uiManager.UpdateNukeCount(nukeCount);
        return true;
    }

    // Called when a nuke is used
    public bool ConsumeNuke()
    {
        // If none left, do nothing
        if (nukeCount <= 0)
        {
            Debug.Log("No nukes left.");
            return false;
        }

        nukeCount--; // Decrement count
        Debug.Log($"Scene nuked. Nukes left: {nukeCount}");

        // Update the UI with new nuke count
        GameManager.GetInstance().uiManager.UpdateNukeCount(nukeCount);
        return true;
    }

    // Return the current number of nukes to UI Manager
    public int GetNukeCount()
    {
        return nukeCount;
    }
}
