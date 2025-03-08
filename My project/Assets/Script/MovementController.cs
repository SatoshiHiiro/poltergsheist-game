using UnityEngine;
using System;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.Events;

/// But: Control the player or an object movement
/// Requiert: Rigidbody2D
/// Input: A = gauche, D = droit, SPACE = saut
/// �tat: Optimal(temp)
[RequireComponent(typeof(Rigidbody2D))]
public abstract class MovementController : MonoBehaviour
{
    //Delegates
    public delegate void Callback(string parameter);
    [HideInInspector] public string jumpParam = "jump";
    [HideInInspector] public string landParam = "land";

    //Events
    public event Callback onJump;
    public event Callback onLand;

    //Mouvement
    [Header("Mouvement variables")]
    [SerializeField] public float speed;                                //Vitesse du mouvement
    [SerializeField] public float maxSpeed;                             //Vitesse maximale pouvant �tre atteinte
    [SerializeField] public float stopSpeed;                            //Vitesse de ralentissage
    [SerializeField] public float jumpSpeed;                            //Vitesse de saut
    [SerializeField][HideInInspector] public int horizontalDirection;   //Direction du mouvement
    private bool playerInputEnable;
    protected Vector2 moveInput;
    Vector2 lastInput;
    
    //Contacts
    [Header("GameObjets in contact")]
    public List<Collision2D> curObject = new List<Collision2D>();       //Pour stocker tous les GameObjets en contact avec l'objet
    [HideInInspector] public float halfSizeOfObject;
    float slideDownWall;

    //Conditions
    [Header("Mouvement conditions")]
    public bool canMove;                                                //Pour permettre l'arr�t total des lignes de physiques
    public bool canWalk;                                                //Pour permettre l'arr�t du mouvement physique en X
    public bool canJump;                                                //Pour permettre l'arr�t du mouvement physique en Y
    public bool isInContact;                                            //Utiliser pour savoir si l'objet est en contact avec quelque chose lui permettant de sauter
    [HideInInspector] public bool isJumping;                            //Utilis� pour que le Player ne puisse pas sauter s'iel saute
    private bool canClimbAgain;

    //Shortcuts
    protected Rigidbody2D rigid2D;
    Collider2D col2D;

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
        halfSizeOfObject = (transform.lossyScale.y * gameObject.GetComponent<Collider2D>().bounds.size.y) / 2;
        slideDownWall = 0;

        move.Enable();
        jump.Enable();
    }

    protected virtual void Start()
    {
        rigid2D = gameObject.transform.GetComponent<Rigidbody2D>();
        col2D = gameObject.GetComponent<Collider2D>();
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
                if (onJump != null) { onJump(jumpParam); };
                rigid2D.AddForceY(jumpSpeed, ForceMode2D.Impulse);
                isJumping = false;
            }

            lastInput = moveInput;
        }
    }

    //Pour les inputs
    protected virtual void Update()
    {
        if (playerInputEnable)
        {
            if (canMove && move.IsPressed())
            {
                moveInput = move.ReadValue<Vector2>();
                moveInput.x = Mathf.Round(moveInput.x);
                moveInput.y = Mathf.Round(moveInput.y);

                if (moveInput.y != 0)
                    canClimbAgain = true;
            }
            else
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

    //Pour enlever de la m�moire le input de saut jusqu'� ce que l'avatar touche le sol
    IEnumerator InputReset()
    {
        yield return new WaitForSecondsRealtime(.2f);
        isJumping = false;
    }

    //Stock les GameObjets en contact avec l'objet
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        curObject.Add(collision);
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y >= .8f && !isInContact)
            {
                if (onLand != null) { onLand(landParam); };
                break;
            }
        }
    }

    //Enl�ve les GameObjets en contact avec l'objet
    void OnCollisionExit2D(Collision2D collision)
    {
        curObject.Remove(collision);
        isInContact = false;
    }

    //Checks if the GameObjects in contact are below the controller
    private void OnCollisionStay2D(Collision2D collision)
    {
        for (int i = 0; i < curObject.Count; i++)
        {
            if (collision.gameObject == curObject[i].gameObject)
            {
                for (int ii = 0; ii < collision.contactCount; ii++)
                {
                    if (collision.GetContact(ii).normal.y >= .8f)
                    {
                        isInContact = true;
                        slideDownWall = 0;
                        break;
                    }
                }
                break;
            }
        }
        //Makes the controller slide down the sides of rigidbodies
        if (!isInContact && rigid2D.linearVelocityY <= 0)
        {
            rigid2D.linearVelocityY = slideDownWall;
            slideDownWall -= .133f;
        }
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
