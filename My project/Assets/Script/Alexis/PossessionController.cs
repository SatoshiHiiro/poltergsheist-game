using UnityEngine;

public class PossessionController : MonoBehaviour
{
    float posSpeed;
    float posMaxSpeed;
    float posStopSpeed;

    public enum PossessionType
    {
        None = 0,
        Dresser = 1
    }

    public PossessionType _possessionType;

    PlayerController input;
    Rigidbody2D rigid2D;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = FindFirstObjectByType<PlayerController>();
        rigid2D = gameObject.GetComponent<Rigidbody2D>();

        switch (((int)_possessionType))
        {
            case 1:
                posSpeed = 0.1f;
                posMaxSpeed = 1f;
                posStopSpeed = 2f;
                return;

            default:
                return;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Effectue le mouvement horizontal
        rigid2D.AddForceX(input.horizontalDirection * posSpeed, ForceMode2D.Impulse);

        //Limite la vitesse horizontale en fonction du maxSpeed
        if (Mathf.Abs(rigid2D.linearVelocityX) > posMaxSpeed)
        {
            if (rigid2D.linearVelocityX < 0)
                rigid2D.linearVelocityX = -posMaxSpeed;
            else
                rigid2D.linearVelocityX = posMaxSpeed;
        }

        //Effectue le ralentissement quand aucun input
        if (input.horizontalDirection == 0)
            rigid2D.linearVelocityX = rigid2D.linearVelocityX / posStopSpeed;
    }
}
