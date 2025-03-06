using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// But: Control the player or an object movement
/// Requiert: Rigidbody2D
/// Input: A = gauche, D = droit, SPACE = saut
/// État: Optimal(temp)
public abstract class MovementController : MonoBehaviour
{
    //Mouvement
    [Header("Mouvement variables")]
    [SerializeField] public float speed;                                //Vitesse du mouvement
    [SerializeField] public float maxSpeed;                             //Vitesse maximale pouvant être atteinte
    [SerializeField] public float stopSpeed;                            //Vitesse de ralentissage
    [SerializeField] public float jumpSpeed;                            //Vitesse de saut
    [SerializeField][HideInInspector] public int horizontalDirection;  //Direction du mouvement
    private bool playerInputEnable;
    protected Vector2 moveInput;
    Vector2 lastInput;
    
    //Contacts
    [Header("GameObjets in contact")]
    public List<GameObject> curObject = new List<GameObject>();     //Pour stocker tous les GameObjets en contact avec l'objet

    //Conditions
    [Header("Mouvement conditions")]
    public bool canMove;                                                //Pour permettre l'arrêt total des lignes de physiques
    public bool canWalk;                                                //Pour permettre l'arrêt du mouvement physique en X
    public bool canJump;                                                //Pour permettre l'arrêt du mouvement physique en Y
    public bool isInContact;                                        //Utiliser pour savoir si l'objet est en contact avec quelque chose lui permettant de sauter
    [HideInInspector] public bool isJumping;                            //Utilisé pour que le Player ne puisse pas sauter s'iel saute
    private bool canClimbAgain;

    //Shortcuts
    protected Rigidbody2D rigid2D;

    //Input action section, has to be public or can be private with a SerializeField statement
    [Header("Input Section")]
    public InputAction move;
    public InputAction jump;

    protected virtual void Awake()
    {
        playerInputEnable = true;
        moveInput = Vector2.zero;
        lastInput = Vector2.zero;
        canClimbAgain = true;

        move.Enable();
        jump.Enable();
    }

    protected virtual void Start()
    {
        rigid2D = gameObject.transform.GetComponent<Rigidbody2D>();
        //canMove = true;
        //isJumping = false;
    }

    //Pour la physique
    protected virtual void FixedUpdate()
    {
        if (canMove)
        {
            if (lastInput.x != moveInput.x && moveInput.x != 0)
            {
                rigid2D.linearVelocityX = 0;
            }

            //Effectue le mouvement horizontal
            rigid2D.AddForceX(moveInput.x * speed, ForceMode2D.Impulse);

            ////Limite la vitesse horizontale en fonction du maxSpeed
            rigid2D.linearVelocityX = Mathf.Clamp(rigid2D.linearVelocityX, -maxSpeed, maxSpeed);
            
            //Effectue le ralentissement quand aucun input
            if (moveInput.x == 0)
                rigid2D.linearVelocityX = rigid2D.linearVelocityX / stopSpeed;

            if (isJumping && isInContact)
            {
                rigid2D.AddForceY(jumpSpeed, ForceMode2D.Impulse);
                isJumping = false;
            }
            //if (isJumping && (rigid2D.linearVelocityY == 0))
            //{
            //    rigid2D.AddForceY(jumpSpeed, ForceMode2D.Impulse);
            //    isJumping = false;
            //}
            lastInput = moveInput;
        }
    }

    //Pour les inputs
    protected virtual void Update()
    {
        if (playerInputEnable)
        {
            if (canMove && move.WasPressedThisFrame())
            {
                moveInput = move.ReadValue<Vector2>();
                if(moveInput.y != 0)
                {
                    canClimbAgain = true;
                }
            }
            if (move.WasReleasedThisFrame() && !move.WasPressedThisFrame())
            {
                moveInput = Vector2.zero;
                canClimbAgain = true;
            }
            if (canJump && !isJumping && jump.WasPressedThisFrame())
            {
                isJumping = true;
                StartCoroutine(InputReset());
            }
        }
    }

    //Pour enlever de la mémoire le input de saut jusqu'à ce que l'avatar touche le sol
    IEnumerator InputReset()
    {
        yield return new WaitForSecondsRealtime(.2f);
        isJumping = false;
    }

    //Stock les GameObjets en contact avec l'objet
    void OnCollisionEnter2D(Collision2D collision)
    {
        curObject.Add(collision.gameObject);
        if (!collision.gameObject.CompareTag("Wall"))
            isInContact = true;
    }

    //Enlève les GameObjets en contact avec l'objet
    void OnCollisionExit2D(Collision2D collision)
    {
        curObject.Remove(collision.gameObject);
        bool temp = false;
        if (curObject.Count > 0)
        {
            for (int i = 0; i < curObject.Count; i++)
            {
                if (!curObject[i].gameObject.CompareTag("Wall"))
                {
                    temp = true;
                    break;
                }
            }
        }
        isInContact = temp;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Detection of the stairs
        if (collision.gameObject.tag == "Stair")
        {
            HandleStairClimbing(collision);
        }
    }

    // Manage the climbing of the stair for the player
    private void HandleStairClimbing(Collider2D collider)
    {
        StairController stair = collider.gameObject.GetComponent<StairController>();
        if (moveInput.y != 0 && canClimbAgain)
        {
            canClimbAgain = false;
            StairDirection direction = moveInput.y > 0 ? StairDirection.Upward : StairDirection.Downward;
            stair.ClimbStair(this.gameObject, direction);
        }
    }
}
