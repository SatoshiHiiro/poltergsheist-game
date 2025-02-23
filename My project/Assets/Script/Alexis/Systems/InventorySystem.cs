using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] bool[] isConditionsMet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int length = FindObjectsByType<KeyItemBehavior>(FindObjectsSortMode.None).Length;
        isConditionsMet = new bool[length];
        for (int i = 0; i < length; i++)
            isConditionsMet[i] = false;
    }

    public void CreateUIItem(Sprite image)
    {
        GameObject obj = new GameObject("KeyObj");
        obj.AddComponent<Image>();
        obj.GetComponent<Image>().sprite = image;
        obj.transform.SetParent(gameObject.transform);
    }

    public void StockItem(int index, bool condition)
    {
        isConditionsMet[index] = condition;
    }

    public bool ReadCondition(int index)
    {
        return isConditionsMet[index];
    }
}

public enum KeyObjectType
{
    None = -1,
    DoorKey = 0,
    ChestKey = 1
}