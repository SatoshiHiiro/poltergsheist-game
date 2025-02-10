using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// But: Control the player or an object movement
/// Requiert: Rigidbody2D
/// Input: A = gauche, D = droit, SPACE = saut
/// �tat: Optimal(temp)
public abstract class MovementController : MonoBehaviour
{
    //Mouvement
    [Header("Mouvement variables")]
    [SerializeField] public float speed;                                //Vitesse du mouvement
    [SerializeField] public float maxSpeed;                             //Vitesse maximale pouvant �tre atteinte
    [SerializeField] public float stopSpeed;                            //Vitesse de ralentissage
    [SerializeField] public float jumpSpeed;                            //Vitesse de saut
    [SerializeField][HideInInspector] public int horizontalDirection;  //Direction du mouvement
    private bool playerInputEnable;
    protected Vector2 moveInput;
    
    //Contacts
    [Header("GameObjets in contact")]
    public List<GameObject> curObject = new List<GameObject>();     //Pour stocker tous les GameObjets en contact avec l'objet

    //Conditions
    [Header("Mouvement conditions")]
    public bool canMove;                                                //Pour permettre l'arr�t total des lignes de physiques
    public bool canWalk;                                                //Pour permettre l'arr�t du mouvement physique en X
    public bool canJump;                                                //Pour permettre l'arr�t du mouvement physique en Y
    public bool isInContact;                                        //Utiliser pour savoir si l'objet est en contact avec quelque chose lui permettant de sauter
    [HideInInspector] public bool isJumping;                            //Utilis� pour que le Player ne puisse pas sauter s'iel saute

    //Shortcuts
    Rigidbody2D rigid2D;

    //Input action section, has to be public or can be private with a SerializeField statement
    [Header("Input Section")]
    public InputAction move;
    public InputAction jump;

    protected virtual void Awake()
    {
        playerInputEnable = true;
        moveInput = Vector2.zero;

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
            }
            if (move.WasReleasedThisFrame() && !move.WasPressedThisFrame())
            {
                moveInput = Vector2.zero;
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
    void OnCollisionEnter2D(Collision2D collision)
    {
        curObject.Add(collision.gameObject);
        if (!collision.gameObject.CompareTag("Wall"))
            isInContact = true;
    }

    //Enl�ve les GameObjets en contact avec l'objet
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
}
