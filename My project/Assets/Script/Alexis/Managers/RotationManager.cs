using UnityEngine;

public class RotationManager : SpriteManager
{
    [Header("Rotation Animation")]
    [SerializeField] public float rotationSpeed;        //Multiplier for the number of degrees to turn each frame
    Quaternion direction = new Quaternion(0, 0, 0, 1);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //To change target rotation depending on the last position
        if (goesLeft)
            direction = new Quaternion(0, 1, 0, 0);     //Look left
        else if (goesRight)
            direction = new Quaternion(0, 0, 0, 1);     //Look right

        RotateSprite(direction);
    }

    //The actual rotation on the y axis
    protected void RotateSprite(Quaternion _direction)
    {
        float step = rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.localRotation, _direction, step);
    }
}
