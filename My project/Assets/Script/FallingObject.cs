using UnityEngine;

public enum ObjectState
{
    Normal,
    Falling,
    BeingReplaced
}


public class FallingObject : SoundDetection, IPossessable
{
    // Behaviour of the falling object when he's possessed

    public Vector2 initialPosition;    // Initial Position before Falling
    private Vector2 floorY;            // Position of the floor
    private Rigidbody2D rb;
    private Collider2D objectCollider;
    public ObjectState State { get; private set; }

    // Sound
    private AudioSource audioSource;
    
    void Start()
    {
        initialPosition = transform.position;
        rb= GetComponent<Rigidbody2D>();
        objectCollider= GetComponent<Collider2D>();
        audioSource= GetComponent<AudioSource>();
        State = ObjectState.Normal;
    }
    public void OnPossessed()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;  // To make the object fall down
    }
    public void OnDepossessed()
    {
        //throw new System.NotImplementedException();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // When the object collide with the floor it makes a sound that alert nearby enemies.
        if (collision.gameObject.CompareTag("Floor"))
        {
            audioSource.Play();
            NotifyNearbyEnemies(collision.gameObject.transform.position.y, this.gameObject);
        }
    }
    // Reset the object state and physics
    public void ReplaceObject()
    {
        State = ObjectState.BeingReplaced;
        rb.bodyType= RigidbodyType2D.Kinematic;
    }
    // Reset the object position and rotation
    public void FinishReplacement()
    {
        State = ObjectState.Normal;
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity;
    }




}
