using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }


    [SerializeField] KeyItemBehavior[] keyItems;
    [SerializeField] bool[] isConditionsMet;
    private Dictionary<KeyItemBehavior, GameObject> itemUI = new Dictionary<KeyItemBehavior, GameObject>();

    public event Action<KeyItemBehavior> OnResetKey;

    EnergySystem energy;

    public InputAction Ctrl;
    public InputAction FullEnergy;
    public InputAction MinEnergy;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        energy = FindFirstObjectByType<EnergySystem>();
        keyItems = FindObjectsByType<KeyItemBehavior>(FindObjectsSortMode.InstanceID);
        isConditionsMet = new bool[keyItems.Length];
        for (int i = 0; i < keyItems.Length; i++)
            isConditionsMet[i] = false;

        Ctrl.AddBinding("<Keyboard>/ctrl");
        FullEnergy.AddBinding("<Keyboard>/r");
        MinEnergy.AddBinding("<Keyboard>/e");
        Ctrl.Enable();
        FullEnergy.Enable();
        MinEnergy.Enable();
    }

    private void Update()
    {
        if (Ctrl.IsPressed())
        {
            if (FullEnergy.WasPressedThisFrame())
            {
                Debug.Log("Full energy");
                energy.ModifyEnergy(energy.maxEnergy);
            }
            if (MinEnergy.WasPressedThisFrame())
            {
                Debug.Log("Min energy");
                energy.ModifyEnergy(-energy.maxEnergy);
            }
        }
    }

    // Add object UI in inventory
    public void CreateUIItem(Sprite image, KeyItemBehavior keyItem)
    {
        GameObject obj = new GameObject("KeyObj");
        obj.AddComponent<Image>();
        obj.GetComponent<Image>().sprite = image;
        obj.GetComponent<Image>().preserveAspect = true;
        obj.transform.SetParent(gameObject.transform);

        // Stock reference of image and key
        itemUI[keyItem] = obj;
    }

    // The player collected one key
    public void StockItem(KeyItemBehavior keyItem, bool condition)
    {
        for (int i = 0; i < keyItems.Length; i++)
        {
            if (keyItems[i] == keyItem)
                isConditionsMet[i] = condition;
        }
    }

    // Is the item collected by the player
    public bool ReadCondition(KeyItemBehavior keyItem)
    {
        bool condition = false;

        for (int i = 0; i < keyItems.Length; i++)
        {
            if (keyItems[i] == keyItem)
            {
                condition = isConditionsMet[i];
            }
        }
        return condition;
    }

    // Remove an object from the inventory and UI
    public void RemoveObject(KeyItemBehavior keyItem)
    {
        for(int i = 0; i < keyItems.Length; i++)
        {
            if(keyItems[i] == keyItem)
            {
                isConditionsMet[i] = false;
                GameObject keyImage = itemUI[keyItem];
                Destroy(keyImage);
                OnResetKey?.Invoke(keyItem);
            }
        }
    }
}