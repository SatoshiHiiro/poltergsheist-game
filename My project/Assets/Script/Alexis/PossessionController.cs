using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// But: Controller un objet possédé
/// Requiert: Rigidbody2D
/// Requiert Externe: tag "Wall"
/// Input: A = droite, D = gauche, SPACE = saut
/// État: Adéquat(temp)
public class PossessionController : MonoBehaviour
{
    //Variables
    [Header("Variables")]
    public float speed;                                             //Vitesse horizontale de l'objet
    public float maxSpeed;                                          //Vitesse horizontale maximale de l'objet
    public float stopSpeed;                                         //Vitesse horizontale d'arrêt de l'objet
    public float jumpSpeed;                                         //Vitesse de saut de l'objet
    int horizontalDirection;                                        //Direction horizontale

    //Conditions
    [Header("Conditions")]
    public bool canMove;                                            //Pour permettre l'arrêt total des lignes de physiques
    public bool canWalk;                                            //Pour permettre l'arrêt du mouvement physique en X
    public bool canJump;                                            //Pour permettre l'arrêt du mouvement physique en Y
    public bool isInContact;                                        //Utiliser pour savoir si l'objet est en contact avec quelque chose lui permettant de sauter
    bool isJumping;                                                 //Utiliser pour savoir si l'objet est en saut

    //Contacts
    [Header("GameObjets in contact")]
    public List<GameObject> curObject = new List<GameObject>();     //Pour stocker tous les GameObjets en contact avec l'objet

    //Types de possession
    public enum PossessionType
    {
        None = 0,
        Dresser = 1,
        Test = 2
    }
    [Header("Possession type")]
    public PossessionType _possessionType;

    //Shortcuts
    Rigidbody2D rigid2D;

    void Start()
    {
        rigid2D = gameObject.GetComponent<Rigidbody2D>();
        canMove = true;
        canWalk = true;
        canJump = true;
        isJumping = false;

        //Met en place les variables de mouvement dépendamment du type de possession
        switch (((int)_possessionType))
        {
            case 1:
                speed = 0.2f;
                maxSpeed = 1f;
                stopSpeed = 2f;
                canJump = false;
                return;

            case 2:
                speed = 0.3f;
                maxSpeed = 5f;
                stopSpeed = 2f;
                jumpSpeed = 5f;
                return;

            default:
                return;
        }
    }

    //Pour enlever de la mémoire le input de saut jusqu'à ce que l'avatar touche le sol
    IEnumerator InputReset()
    {
        yield return new WaitForSecondsRealtime(.2f);
        isJumping = false;
    }

    //Pour la physique
    void FixedUpdate()
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

            //Effectue le saut
            if (isJumping && isInContact)
            {
                rigid2D.AddForceY(jumpSpeed, ForceMode2D.Impulse);
                isJumping = false;
            }
        }
    }

    //Pour les inputs
    void Update()
    {
        //Détermine la direction du mouvement dépendamment du input
        if (canWalk)
        {
            int _horizontalDirection = 0;
            if (Input.GetKey(KeyCode.A))
                _horizontalDirection--;
            if (Input.GetKey(KeyCode.D))
                _horizontalDirection++;
            horizontalDirection = _horizontalDirection;
        }

        //Détermine si les joueurs.es peuvent sauter avec le input
        if (canJump && !isJumping && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            StartCoroutine(InputReset());
        }
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
}
