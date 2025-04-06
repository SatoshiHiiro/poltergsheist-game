using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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
    [HideInInspector] public string bonkLeftParam = "bonkL";
    [HideInInspector] public string bonkRightParam = "bonkR";
    [HideInInspector] public string cantWalkLeftParam = "shakeL";
    [HideInInspector] public string cantWalkRightParam = "shakeR";

    //Events
    public event Callback onJump;
    public event Callback onLand;
    public event Callback onBonkL;
    public event Callback onBonkR;
    public event Callback onShakeL;
    public event Callback onShakeR;

    //Mouvement
    [Header("Mouvement variables")]
    [SerializeField] public float speed;                                //Mouvement speed
    [SerializeField] public float maxSpeed;                             
    [SerializeField] public float stopSpeed;                            
    [SerializeField] public float jumpSpeed;                            //Jump impulsion force
    [SerializeField][HideInInspector] public int horizontalDirection;
    private bool playerInputEnable;
    protected Vector2 moveInput;
    Vector2 lastInput;
    
    //Contacts
    [Header("GameObjets in contact")]
    public List<Collision2D> curObject = new List<Collision2D>();       //To stock object with which it is currently in collision
    [HideInInspector] public float halfSizeOfObject;
    [HideInInspector] public float lastVelocityX;
    [HideInInspector] public float lastPosY;

    //Conditions
    [Header("Mouvement conditions")]
    public bool canMove;                                                //Can stop all lines that use physics
    public bool canWalk;                                                //Can stop the physics using the x axis
    public bool canJump;                                                //Can stop the physics using the y axis
    public bool isInContact;                                            //To know if the object is in contact with another at the bottom
    [HideInInspector] public bool isJumping;                            //To stop multijump.
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
        halfSizeOfObject = (transform.lossyScale.y * gameObject.GetComponent<Collider2D>().bounds.size.y) / 2;

        move.Enable();
        jump.Enable();
    }

    protected virtual void Start()
    {
        rigid2D = gameObject.transform.GetComponent<Rigidbody2D>();
    }

    //Physics
    protected virtual void FixedUpdate()
    {
        if (canMove)
        {
            if (lastInput.x != moveInput.x && moveInput.x != 0)
            {
                rigid2D.linearVelocityX = 0;
            }

            //Mouvement on the x axis
            if (canWalk)
                rigid2D.AddForceX(moveInput.x * speed, ForceMode2D.Impulse);

            //Caps the speed according to maxspeed
            rigid2D.linearVelocityX = Mathf.Clamp(rigid2D.linearVelocityX, -maxSpeed, maxSpeed);
            
            //Slows the x mouvement if no x input.
            if (moveInput.x == 0)
                rigid2D.linearVelocityX = rigid2D.linearVelocityX / stopSpeed;

            if (isJumping && isInContact)
            {
                if (onJump != null) { onJump(jumpParam); };
                rigid2D.AddForceY(jumpSpeed, ForceMode2D.Impulse);
                isJumping = false;
            }

            rigid2D.linearVelocityY = Mathf.Clamp(rigid2D.linearVelocityY, -8.5f, 8.5f);

            lastInput = moveInput;
        }
        // Keep it from moving
        else
        {
            rigid2D.linearVelocityX = 0;
        }
    }

    //Pour les inputs
    protected virtual void Update()
    {
        if (playerInputEnable)
        {
            // The player has to release the W or S key to climb stairs again
            if (Keyboard.current.wKey.wasReleasedThisFrame || Keyboard.current.sKey.wasReleasedThisFrame)
            {
                canClimbAgain = true;
            }

            if (move.IsPressed())
            {
                moveInput = move.ReadValue<Vector2>();

                if (canWalk)
                {
                    moveInput.x = Mathf.Round(moveInput.x);
                    moveInput.y = Mathf.Round(moveInput.y);
                }
                else if (canMove && move.WasPressedThisFrame())
                {
                    if (moveInput.x > 0)
                    {
                        if (onShakeR != null) { onShakeL(cantWalkRightParam); }
                    }
                    else if (moveInput.x < 0)
                    {
                        if (onShakeL != null) { onShakeR(cantWalkLeftParam); }
                    }
                }
            }
            else
            {
                moveInput = Vector2.zero;
            }

            if (jump.WasPressedThisFrame())
            {
                if (!isJumping)
                {
                    if (canJump)
                    {
                        isJumping = true;
                        StartCoroutine(InputReset());
                    }
                    else if (canMove)
                        if (onJump != null) { onJump(jumpParam); };
                }
            }
        }

        if (rigid2D.linearVelocityY <= .1f && rigid2D.linearVelocityY >= -.1f)
            lastPosY = this.transform.position.y;

        lastVelocityX = Mathf.Abs(rigid2D.linearVelocityX);
    }

    //Pour enlever de la m�moire le input de saut jusqu'� ce que l'avatar touche le sol
    IEnumerator InputReset()
    {
        yield return new WaitForSecondsRealtime(.3f);
        isJumping = false;
    }

    //Stock les GameObjets en contact avec l'objet
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y >= .9f)
            {
                isInContact = true;
                if (lastPosY - this.transform.position.y > .2f)
                {
                    if (onLand != null) { onLand(landParam); };
                }
                break;
            }
        }

        curObject.Add(collision);
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (canMove && lastVelocityX > 0)
            {
                if (collision.GetContact(i).normal.x <= -.9f)
                {
                    if (onBonkR != null) { onBonkR(bonkRightParam); }
                    break;
                }
                else if (collision.GetContact(i).normal.x >= .9f)
                {
                    if (onBonkL != null) { onBonkL(bonkLeftParam); }
                    break;
                }
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
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        for (int i = 0; i < curObject.Count; i++)
        {
            if (collision.gameObject == curObject[i].gameObject)
            {
                for (int ii = 0; ii < collision.contactCount; ii++)
                {
                    if (collision.GetContact(ii).normal.y >= .9f)
                    {
                        isInContact = true;
                        break;
                    }
                }
                if (isInContact)
                    break;
            }
        }

        if (collision.gameObject.CompareTag("TrapDoor"))
        {
            TrapDoor trapDoor = collision.gameObject.GetComponent<TrapDoor>();
            // If the player is on the trap door and press S
            if (trapDoor != null && moveInput.y < 0 && !trapDoor.IsTrapDoorBlocked())
            {
                trapDoor.OpenDoor();
            }
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
            moveInput.x = 0f;
            rigid2D.linearVelocityX = 0f;
            canClimbAgain = false;
            //StairDirection direction = moveInput.y > 0 ? StairDirection.Upward : StairDirection.Downward;
            StairDirection direction;
            if (stair.UpperFloor != null)
            {
                direction = StairDirection.Upward;
            }
            else
            {
                direction = StairDirection.Downward;
            }
            //StairDirection direction = StairDirection.Upward;
            StairController nextStair = direction == StairDirection.Upward ? stair.UpperFloor : stair.BottomFloor;
            if (nextStair != null)
            {
                // If the stairs are not blocked
                if (!stair.IsStairBlocked() && !nextStair.IsStairBlocked())
                {
                    stair.ClimbStair(this.gameObject, direction);
                }
            }            
                      
        }
    }
}
