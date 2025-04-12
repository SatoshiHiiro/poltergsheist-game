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
    Coroutine jumpReset;
    Coroutine jumpBuff;
    public float inputBuffer;
    protected Vector2 moveInput;
    Vector2 lastInput;

    //Contacts
    [Header("GameObjets in contact")]
    [HideInInspector] public List<GameObject> curObject = new List<GameObject>();
    [HideInInspector] public List<ContactPoint2D> curContact = new List<ContactPoint2D>();
    [HideInInspector] public float halfSizeOfObject;
    [HideInInspector] public float lastVelocityX;
    [HideInInspector] public float lastPosY;
    ContactFilter2D filter = new ContactFilter2D();

    //Conditions
    [Header("Mouvement conditions")]
    public bool canMove;                                                //Can stop all lines that use physics
    public bool canWalk;                                                //Can stop the physics using the x axis
    public bool canJump;                                                //Can stop the physics using the y axis
    public bool isInContact;                                            //To know if the object is in contact with another at the bottom
    public bool isJumping;                            //To stop multijump.
    private bool isPerformingJump = false;
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
        inputBuffer = .3f;
        halfSizeOfObject = (transform.lossyScale.y * gameObject.GetComponent<Collider2D>().bounds.size.y) / 2;

        move.Enable();
        jump.Enable();
    }

    protected virtual void Start()
    {
        rigid2D = gameObject.transform.GetComponent<Rigidbody2D>();
        col2D = this.GetComponent<Collider2D>();
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

            if (isJumping && isInContact && !isPerformingJump)
            {
                isPerformingJump = true;
                if (onJump != null) { onJump(jumpParam); };
                rigid2D.AddForceY(jumpSpeed, ForceMode2D.Impulse);
                //isJumping = false;
                StartCoroutine(InputReset());
                //isInContact = false;
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
                    if (canMove)
                    {
                        if (canJump)
                        {
                            isJumping = true;
                            //if (jumpReset != null) { StopCoroutine(jumpReset); }
                            //jumpReset = StartCoroutine(InputReset());
                        }
                        else
                            if (onJump != null) { onJump(jumpParam); };
                    }
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
        // Wait a few physics frames to ensure the jump force has been applied
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        isInContact = false;
        isPerformingJump = false;
        //yield return new WaitForSecondsRealtime(.3f);
        //isJumping = false;
    }

    //Stock les GameObjets en contact avec l'objet
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        curContact.Clear();
        Physics2D.GetContacts(col2D, collision.collider, filter.NoFilter(), curContact);

        for (int i = 0; i < curContact.Count; i++)
        {
            if (curContact[i].normal.y <= -.9f)
            {
                isInContact = true;
                isJumping = false;
                if (lastPosY - this.transform.position.y > .2f)
                {
                    if (onLand != null) { onLand(landParam); };
                }
                break;
            }
            if (canMove && lastVelocityX >= maxSpeed - .1f)
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

        /*for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y >= .9f)
            {
                isInContact = true;
                isJumping = false;
                if (lastPosY - this.transform.position.y > .2f)
                {
                    if (onLand != null) { onLand(landParam); };
                }
                break;
            }
        }

        curObject.Add(collision.gameObject);
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
        }*/
    }

    IEnumerator JumpBuffer()
    {
        yield return new WaitForSecondsRealtime(.2f);
        isInContact = false;
    }

    //Enl�ve les GameObjets en contact avec l'objet
    void OnCollisionExit2D(Collision2D collision)
    {
        //curObject.Remove(collision.gameObject);
        if (jumpBuff != null) { StopCoroutine(JumpBuffer()); }
        jumpBuff = StartCoroutine(JumpBuffer());
    }

    //Checks if the GameObjects in contact are below the controller
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        curContact.Clear();
        Physics2D.GetContacts(col2D, collision.collider, filter.NoFilter(), curContact);

        for (int i = 0; i < curContact.Count; i++)
        {
            if (curContact[i].normal.y <= -.9f)
            {
                isInContact = true;
                //isJumping = false;
                if (jumpBuff != null) { StopCoroutine(JumpBuffer()); }
                break;
            }
        }

        /*for (int i = 0; i < curObject.Count; i++)
        {
            if (collision.gameObject == curObject[i])
            {
                for (int ii = 0; ii < collision.contactCount; ii++)
                {
                    if (collision.GetContact(ii).normal.y >= .9f)
                    {
                        isInContact = true;
                        isJumping = false;
                        if (jumpBuff != null) { StopCoroutine(JumpBuffer()); }
                        break;
                    }
                }
                if (isInContact)
                    break;
            }
        }*/

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
