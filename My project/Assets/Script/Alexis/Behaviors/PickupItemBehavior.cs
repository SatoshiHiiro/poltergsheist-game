using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public abstract class PickupItemBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void LateUpdate()
    {

    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null || collision.GetComponent<PossessionController>() != null)
            HideItem();
    }

    protected void HideItem()
    {
        gameObject.transform.position += new Vector3(0, -1000, 0);
    }
}
