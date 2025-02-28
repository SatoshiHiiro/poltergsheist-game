using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] KeyItemBehavior[] keyItems;
    [SerializeField] bool[] isConditionsMet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        keyItems = FindObjectsByType<KeyItemBehavior>(FindObjectsSortMode.InstanceID);
        isConditionsMet = new bool[keyItems.Length];
        for (int i = 0; i < keyItems.Length; i++)
            isConditionsMet[i] = false;
    }

    public void CreateUIItem(Sprite image)
    {
        GameObject obj = new GameObject("KeyObj");
        obj.AddComponent<Image>();
        obj.GetComponent<Image>().sprite = image;
        obj.transform.SetParent(gameObject.transform);
    }

    public void StockItem(KeyItemBehavior keyItem, bool condition)
    {
        for (int i = 0; i < keyItems.Length; i++)
        {
            if (keyItems[i] == keyItem)
                isConditionsMet[i] = condition;
        }
    }

    public bool[] ReadCondition(KeyItemBehavior keyItem)
    {
        bool[] condition = {false, false};

        for (int i = 0; i < keyItems.Length; i++)
        {
            if (keyItems[i] == keyItem)
            {
                condition[0] = isConditionsMet[i];
                condition[1] = true;
            }
        }
        return condition;
    }
}