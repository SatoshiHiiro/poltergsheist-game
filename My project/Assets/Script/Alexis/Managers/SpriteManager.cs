using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class SpriteManager : MonoBehaviour
{
    [Header("Rotation Animation")]
    [SerializeField] public float rotationSpeed;        //Multiplier for the number of degrees to turn each frame
    Quaternion direction = new Quaternion(0, 0, 0, 1);
    protected Vector2 lastPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        lastPos = transform.position;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        float posDiff = lastPos.x - transform.position.x;
        Vector2 input;

        if (transform.parent.GetComponent<PlayerController>() != null)
        {
            input = transform.parent.GetComponent<PlayerController>().move.ReadValue<Vector2>();
        }
        else
            input = new Vector2(-posDiff, 0);

        //To change target rotation depending on the last position
        if (input.x < 0)
            direction = new Quaternion(0, 1, 0, 0);     //Look left
        else if (input.x > 0)
            direction = new Quaternion(0, 0, 0, 1);     //Look right

        RotateSprite(direction);
    }

    private void LateUpdate()
    {
        lastPos = transform.position;
    }

    //The actual rotation on the y axis
    protected void RotateSprite(Quaternion _direction)
    {
        var step = rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.localRotation, _direction, step);
    }
}
