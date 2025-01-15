using UnityEngine;

public class PushableBehavior : MonoBehaviour
{
    [SerializeField] public float distanceFromPlayer;
    Vector3 positionPlayer;
    bool isPushed;
    float stockMaxSpeed;

    void Update()
    {
        if (isPushed)
        {
            Vector3 _position = transform.position;
            _position.x = positionPlayer.x + distanceFromPlayer;
            transform.position = _position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            distanceFromPlayer = Mathf.Abs(distanceFromPlayer);
            if (transform.position.x < collision.transform.position.x)
            {
                distanceFromPlayer = -distanceFromPlayer;
            }
            stockMaxSpeed = collision.GetComponent<PlayerController>().maxSpeed;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            positionPlayer = collision.transform.position;
            if (Input.GetKey(KeyCode.E) && !collision.GetComponent<PlayerController>().isPossessing)
            {
                collision.GetComponent<PlayerController>().maxSpeed = stockMaxSpeed / 2f;
                isPushed = true;
            }
            else
            {
                collision.GetComponent<PlayerController>().maxSpeed = stockMaxSpeed;
                isPushed = false;
            }
        }
    }
}
