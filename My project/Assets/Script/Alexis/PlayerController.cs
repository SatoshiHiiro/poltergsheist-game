using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

/// But: Controller un avatar
/// Requiert: Rigidbody2D
/// Input: A = gauche, D = droit, SPACE = saut
/// �tat: Optimal(temp)
public class PlayerController : MonoBehaviour
{
    //Mouvement
    [Header("Mouvement variables")]
    [SerializeField] public float speed;                                //Vitesse du mouvement
    [SerializeField] public float maxSpeed;                             //Vitesse maximale pouvant �tre atteinte
    [SerializeField] public float stopSpeed;                            //Vitesse de ralentissage
    [SerializeField] public float jumpSpeed;                            //Vitesse de saut
    [SerializeField] [HideInInspector] public int horizontalDirection;  //Direction du mouvement
    private bool playerInputEnable;
    private Vector2 moveInput;

    //Conditions
    [Header("Mouvement conditions")]
    public bool canMove;                                                //Pour permettre l'arr�t total des lignes de physiques
    public bool canWalk;                                                //Pour permettre l'arr�t du mouvement physique en X
    public bool canJump;                                                //Pour permettre l'arr�t du mouvement physique en Y
    [HideInInspector] public bool isPossessing;                         //Utilis� par PossessionBehavior pour v�rifier si le Player poss�de un objet
    [HideInInspector] public bool isJumping;                            //Utilis� pour que le Player ne puisse pas sauter s'iel saute

    //Possession
    [Header("Object possession")]
    public string lastPossession;                                       //Pour garder en t�te le dernier objet poss�d� et est utilis� dans le PossessionBehavior pour arr�ter la possession

    //Shortcuts
    Rigidbody2D rigid2D;

    //Input action section, has to be public or can be private with a SerializeField statement
    [Header("Input Section")]
    public InputAction move;
    public InputAction jump;

    private void Awake()
    {
        playerInputEnable = true;
        moveInput = Vector2.zero;

        move.Enable();
        jump.Enable();
    }

    void Start()
    {
        rigid2D = gameObject.transform.GetComponent<Rigidbody2D>();
        canMove = true;
        isJumping = false;
    }

    //Pour enlever de la m�moire le input de saut jusqu'� ce que l'avatar touche le sol
    IEnumerator InputReset()
    {
        yield return new WaitForSecondsRealtime(.2f);
        isJumping = false;
    }

    //Pour la physique
    private void FixedUpdate()
    {
        if (canMove)
        {
            //Effectue le mouvement horizontal
            rigid2D.AddForceX(moveInput.x * speed, ForceMode2D.Impulse);

            ////Limite la vitesse horizontale en fonction du maxSpeed
            rigid2D.linearVelocityX = Mathf.Clamp(rigid2D.linearVelocityX, -maxSpeed, maxSpeed);
            //if (Mathf.Abs(rigid2D.linearVelocityX) > maxSpeed)
            //{
            //    if (rigid2D.linearVelocityX < 0)
            //        rigid2D.linearVelocityX = -maxSpeed;
            //    else
            //        rigid2D.linearVelocityX = maxSpeed;
            //}

            //Effectue le ralentissement quand aucun input
            if (moveInput.x == 0)
                rigid2D.linearVelocityX = rigid2D.linearVelocityX / stopSpeed;

            if (isJumping && (rigid2D.linearVelocityY == 0))
            {
                rigid2D.AddForceY(jumpSpeed, ForceMode2D.Impulse);
                isJumping = false;
            }
        }
    }

    //Pour les inputs
    void Update()
    {
        if (playerInputEnable)
        {
            if(canMove && move.WasPressedThisFrame())
            {
                moveInput = move.ReadValue<Vector2>();
            }
            if (move.WasReleasedThisFrame())
            {
                moveInput = Vector2.zero;
            }
            if (canJump && !isJumping && jump.WasPressedThisFrame())
            {
                isJumping = true;
                StartCoroutine(InputReset());
            }
        }

        ////D�termine la direction du mouvement d�pendamment du input
        //if (canWalk)
        //{
        //    int _horizontalDirection = 0;
        //    if (Input.GetKey(KeyCode.A))
        //        _horizontalDirection--;
        //    if (Input.GetKey(KeyCode.D))
        //        _horizontalDirection++;
        //    horizontalDirection = _horizontalDirection;
        //}

        ////D�termine si les joueurs.es peuvent sauter avec le input
        //if (canJump && !isJumping && Input.GetKeyDown(KeyCode.Space))
        //{
        //    isJumping = true;
        //    StartCoroutine(InputReset());
        //}
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Stair")
        {
            StairController stair = collision.gameObject.GetComponent<StairController>();
            if(moveInput.y != 0)
            {
                StairDirection direction = moveInput.y > 0 ? StairDirection.Upward : StairDirection.Downward;
                stair.ClimbStair(this.gameObject, direction);
            }
            
        }
    }
}
