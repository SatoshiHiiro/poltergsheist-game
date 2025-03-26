using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public abstract class PickupItemBehavior : MonoBehaviour
{
    // This class manage the behavior of items that can be picked up by the player
    protected SpriteRenderer itemSprite;
    protected Collider2D itemCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        itemSprite = GetComponentInChildren<SpriteRenderer>();
        itemCollider = GetComponentInChildren<Collider2D>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // We hide the item sprite when the player collect it
        if (collision.GetComponent<PlayerController>() != null || (collision.GetComponent<PossessionController>() != null && collision.GetComponent<KeyController>() == null))
            HideItem();
    }

    protected void HideItem()
    {
        itemSprite.enabled = false;
        itemCollider.enabled = false;
        //gameObject.transform.position += new Vector3(0, -1000, 0);
    }
}
