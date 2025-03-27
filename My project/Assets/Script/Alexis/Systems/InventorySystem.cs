using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;
using System.Linq;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    List<StealableBehavior> stolenItemList = new List<StealableBehavior>();
    List<KeyItemBehavior> keyItemsList = new List<KeyItemBehavior>();


    //List<KeyItemBehavior> items = new List<KeyItemBehavior>();
    //private Dictionary<KeyItemBehavior, ItemData> inventoryItems = new Dictionary<KeyItemBehavior, ItemData>();


    //List<ItemData> itemsDataList = new List<ItemData>();

    //[SerializeField] KeyItemBehavior[] keyItems;
    //[SerializeField] bool[] isConditionsMet;
    //private Dictionary<KeyItemBehavior, GameObject> itemUI = new Dictionary<KeyItemBehavior, GameObject>();
    //private Dictionary<KeyItemBehavior, bool> inventoryItems = new Dictionary<KeyItemBehavior, bool>();
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

    // The player stole an item
    public void AddStolenItemToInventory(StealableBehavior stolenItem)
    {
        stolenItemList.Add(stolenItem);
        InventoryUI.Instance.UpdateStealableBarUI(stolenItem, true);
    }

    // The player found a key
    public void AddKeyToInventory(KeyItemBehavior keyItem)
    {
        keyItemsList.Add(keyItem);
        InventoryUI.Instance.UpdateCollectedItemBarUI(keyItem, true);
    }

    // Did the player picked up a particular key item
    public bool IsKeyPickedUp(KeyItemBehavior keyItem)
    {
        if (keyItemsList.Contains(keyItem))
        {
            return true;
        }
        return false;
    }

    // Remove a specefic object from the inventory
    public void RemoveObject(PickupItemBehavior pickupItem)
    {
        // Is the object we want to remove stealable
        if (stolenItemList.Contains(pickupItem))
        {
            stolenItemList.Remove((StealableBehavior)pickupItem);
            InventoryUI.Instance.UpdateStealableBarUI((StealableBehavior)pickupItem, false);
        }
        // Is the object we want to remove a key
        else if (keyItemsList.Contains(pickupItem))
        {
            keyItemsList.Remove((KeyItemBehavior)pickupItem);
            InventoryUI.Instance.UpdateCollectedItemBarUI((KeyItemBehavior)pickupItem, false);
        }
    }

    // Add object UI in inventory
    //public void SetUpInventory()
    //{
    //    foreach(KeyItemBehavior item in items)
    //    {
    //        // Add to the inventory the shadow of the picture to find
    //        GameObject obj = new GameObject("InventoryObj");
    //        obj.AddComponent<Image>();
    //        Image objectImage= obj.GetComponent<Image>();
    //        objectImage.sprite = item.ItemSpriteRenderer.sprite;
    //        objectImage.color = Color.black;
    //        objectImage.preserveAspect = true;
    //        obj.transform.SetParent(gameObject.transform);

    //        ItemData itemData = new ItemData(obj, false);

    //        inventoryItems.Add(item, itemData);
    //        // Stock reference of image and key
    //        //itemUI[item] = obj;
    //    }

    //}

    //// The player collected one item
    //public void UpdateInventory(KeyItemBehavior pickedUpItem)
    //{
    //    // Make sure the item is recorded in the inventory
    //    if (inventoryItems.ContainsKey(pickedUpItem))
    //    {
    //        inventoryItems[pickedUpItem].isPickedUp = true;
    //        inventoryItems[pickedUpItem].itemGameObject.GetComponent<Image>().color = Color.white;
    //    }
    //    //foreach(ItemData item in itemsDataList)
    //    //{
    //    //    if(item.pickedUpItem == pickedUpItem)
    //    //    {
    //    //        item.isPickedUp = true;
    //    //        item.itemGameObject.GetComponent<Image>().color = Color.white;
    //    //    }
    //    //}
    //    //for (int i = 0; i < keyItems.Length; i++)
    //    //{
    //    //    if (keyItems[i] == item)
    //    //    {
    //    //        isConditionsMet[i] = true;
    //    //        // When the player pick up the object the object replace the shadow
    //    //        itemUI[item].GetComponent<Image>().color = Color.white;
    //    //    }

    //    //}
    //}

    //// Is the item collected by the player
    //public bool IsItemPickedUp(KeyItemBehavior keyItem)
    //{
    //    if (inventoryItems.ContainsKey(keyItem))
    //    {
    //        if (inventoryItems[keyItem].isPickedUp)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //// Remove an object from the inventory and UI
    //public void RemoveObject(PickupItemBehavior item)
    //{
    //    //if (inventoryItems.ContainsKey(item))
    //    //{
    //    //    inventoryItems[item].isPickedUp = false;
    //    //    inventoryItems[item].itemGameObject.GetComponent<Image>().color = Color.black;
    //    //    OnResetKey?.Invoke(item);
    //    //}



    //    //for(int i = 0; i < keyItems.Length; i++)
    //    //{
    //    //    if(keyItems[i] == keyItem)
    //    //    {
    //    //        isConditionsMet[i] = false;
    //    //        GameObject keyImage = itemUI[keyItem];
    //    //        Destroy(keyImage);
    //    //        OnResetKey?.Invoke(keyItem);
    //    //    }
    //    //}
    //}
}

//[System.Serializable]
//public class ItemData
//{
//    public GameObject itemGameObject;
//    public bool isPickedUp;

//    public ItemData(GameObject itemImage, bool pickedUp)
//    {
//        this.itemGameObject = itemImage;
//        this.isPickedUp = pickedUp;
//    }
//}