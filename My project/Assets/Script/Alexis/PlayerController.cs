using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Mouvement
    [SerializeField] [HideInInspector] public int horizontalDirection;            //Direction du mouvement
    [SerializeField] public float speed;                                          //Vitesse du mouvement
    [SerializeField] public float maxSpeed;                                       //Vitesse maximale pouvant être atteinte
    [SerializeField] public float stopSpeed;                                      //Vitesse de ralentissage
    public string lastPossession;

    //Conditions
    [HideInInspector] public bool canMove;
    [HideInInspector] public bool isPossessing;

    //Shortcuts
    Rigidbody2D rigid2D;

    void Start()
    {
        rigid2D = gameObject.transform.GetComponent<Rigidbody2D>();
        canMove = true;
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            //Effectue le mouvement horizontal
            rigid2D.AddForceX(horizontalDirection * speed, ForceMode2D.Impulse);

            //Limite la vitesse horizontale en fonction du maxSpeed
            if (Mathf.Abs(rigid2D.linearVelocityX) > maxSpeed)
            {
                if (rigid2D.linearVelocityX < 0)
                    rigid2D.linearVelocityX = -maxSpeed;
                else
                    rigid2D.linearVelocityX = maxSpeed;
            }

            //Effectue le ralentissement quand aucun input
            if (horizontalDirection == 0)
                rigid2D.linearVelocityX = rigid2D.linearVelocityX / stopSpeed;
        }
    }

    void Update()
    {
        //Détermine la direction du mouvement dépendamment du input
        int _horizontalDirection = 0;
        if (Input.GetKey(KeyCode.A))
            _horizontalDirection--;
        if (Input.GetKey(KeyCode.D))
            _horizontalDirection++;
        horizontalDirection = _horizontalDirection;
    }
}
