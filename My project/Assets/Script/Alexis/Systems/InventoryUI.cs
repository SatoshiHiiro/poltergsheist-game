using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance {  get; private set; }

    [SerializeField] private GameObject collectedItemBar;
    [SerializeField] private GameObject stealableBar;

    List<StealableBehavior> stealableItemList = new List<StealableBehavior>();  // List of every stealable items in the scene
    Dictionary<Sprite, GameObject> stealableUI = new Dictionary<Sprite, GameObject>();  // Each treasure has it's own sprite
    Dictionary<KeyItemBehavior, GameObject> keyUI = new Dictionary<KeyItemBehavior, GameObject>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }

        stealableItemList = FindObjectsByType<StealableBehavior>(FindObjectsSortMode.InstanceID).OrderBy(obj => obj.name).ToList<StealableBehavior>();
    }

    private void Start()
    {
        SetupStealableBarUI();
    }

    private void SetupStealableBarUI()
    {
        if(stealableItemList.Count == 0)
        {
            Debug.LogWarning("Stealable list is empty");
        }
        foreach(StealableBehavior stealableItem in stealableItemList)
        {
            Sprite sprite = stealableItem.ItemSpriteRenderer.sprite;

            if (!stealableUI.ContainsKey(sprite))
            {
                // Add to the inventory the shadow of the picture to find
                GameObject obj = new GameObject("StealableObj");
                obj.AddComponent<Image>();
                Image objectImage = obj.GetComponent<Image>();
                objectImage.sprite = stealableItem.ItemSpriteRenderer.sprite;
                objectImage.color = Color.black;
                objectImage.preserveAspect = true;
                obj.transform.SetParent(stealableBar.transform);

                stealableUI[sprite] = obj;
            }
            

            //stealableUI[stealableItem] = obj;
        }
    }

    // Update the stealable bar UI when an item is stolen or when we reset
    public void UpdateStealableBarUI(StealableBehavior itemStolen, bool isPickedUp)
    {
        Sprite sprite = itemStolen.ItemSpriteRenderer.sprite;
        if (stealableUI.ContainsKey(sprite))
        {
            Color colorItem = isPickedUp == true ? Color.white : Color.black;
            stealableUI[sprite].GetComponent<Image>().color = colorItem;
        }
    }

    public void UpdateCollectedItemBarUI(KeyItemBehavior keyitem, bool isPickedUp)
    {
        if (isPickedUp)
        {
            GameObject obj = new GameObject("StealableObj");
            obj.AddComponent<Image>();
            Image objectImage = obj.GetComponent<Image>();
            objectImage.sprite = keyitem.ItemSpriteRenderer.sprite;
            objectImage.preserveAspect = true;
            obj.transform.SetParent(collectedItemBar.transform);

            keyUI[keyitem] = obj;
        }
        else
        {
            Destroy(keyUI[keyitem]);
        }
       
    }

}
