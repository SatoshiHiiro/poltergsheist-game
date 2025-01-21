using UnityEngine;
using System.Collections;

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
        //D�termine la direction du mouvement d�pendamment du input
        if (canWalk)
        {
            int _horizontalDirection = 0;
            if (Input.GetKey(KeyCode.A))
                _horizontalDirection--;
            if (Input.GetKey(KeyCode.D))
                _horizontalDirection++;
            horizontalDirection = _horizontalDirection;
        }

        //D�termine si les joueurs.es peuvent sauter avec le input
        if (canJump && !isJumping && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            StartCoroutine(InputReset());
        }
    }
}
