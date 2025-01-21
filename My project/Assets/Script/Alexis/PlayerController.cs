using UnityEngine;
using System.Collections;

/// But: Controller un avatar
/// Requiert: Rigidbody2D
/// Input: A = gauche, D = droit, SPACE = saut
/// État: Optimal(temp)
public class PlayerController : MonoBehaviour
{
    //Mouvement
    [Header("Mouvement variables")]
    [SerializeField] public float speed;                                //Vitesse du mouvement
    [SerializeField] public float maxSpeed;                             //Vitesse maximale pouvant être atteinte
    [SerializeField] public float stopSpeed;                            //Vitesse de ralentissage
    [SerializeField] public float jumpSpeed;                            //Vitesse de saut
    [SerializeField] [HideInInspector] public int horizontalDirection;  //Direction du mouvement

    //Conditions
    [Header("Mouvement conditions")]
    public bool canMove;                                                //Pour permettre l'arrêt total des lignes de physiques
    public bool canWalk;                                                //Pour permettre l'arrêt du mouvement physique en X
    public bool canJump;                                                //Pour permettre l'arrêt du mouvement physique en Y
    [HideInInspector] public bool isPossessing;                         //Utilisé par PossessionBehavior pour vérifier si le Player possède un objet
    [HideInInspector] public bool isJumping;                            //Utilisé pour que le Player ne puisse pas sauter s'iel saute

    //Possession
    [Header("Object possession")]
    public string lastPossession;                                       //Pour garder en tête le dernier objet possédé et est utilisé dans le PossessionBehavior pour arrêter la possession

    //Shortcuts
    Rigidbody2D rigid2D;

    void Start()
    {
        rigid2D = gameObject.transform.GetComponent<Rigidbody2D>();
        canMove = true;
        isJumping = false;
    }

    //Pour enlever de la mémoire le input de saut jusqu'à ce que l'avatar touche le sol
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
}
