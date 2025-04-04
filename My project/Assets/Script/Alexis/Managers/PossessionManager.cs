using UnityEngine;
using System.Collections;

/// But: Poss�der des objets
/// Requiert: PossessionController
/// Requiert Externe: PlayerController(1)
/// Input: Click Gauche = possession/d�possession
/// �tat: Ad�quat(temp)
public class PossessionManager : InteractibleManager, IResetInitialState
{
    //Variables
    [Header("Variables")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite possessedSprite;
    [SerializeField] public float lerpSpeed;            //Vitesse du lerp
    [SerializeField] private float possessionDistance;  // Distance between the player and the object so he can possessed it

    //Conditions
    bool isPossessed;                               //Pour savoir si l'objet possessible est poss�d�
    bool isAnimationFinished;                       //Pour savoir si l'animation de possession est fini
    bool hasEnoughSpace;
    bool hasPosControl;

    //Shortcuts
    PlayerController player;
    PlayerManager manager;
    IPossessable possession;
    SpriteRenderer spriteRenderer;
    Collider2D col2D;
    PossessionController posControl;

    // Getters
    public bool IsPossessing => isPossessed;

    protected override void Start()
    {
        base.Start();
        player = FindFirstObjectByType<PlayerController>();
        manager = FindFirstObjectByType<PlayerManager>();
        if (this.TryGetComponent<SpriteRenderer>(out spriteRenderer)) { }
        else if (this.transform.GetChild(0).TryGetComponent<SpriteRenderer>(out spriteRenderer)) { }
        else if (this.transform.GetChild(0).GetChild(0).TryGetComponent<SpriteRenderer>(out spriteRenderer)) { }
        col2D = this.GetComponent<Collider2D>();
        hasPosControl = false;
        if (this.gameObject.TryGetComponent<PossessionController>(out posControl)) { hasPosControl = true; }
        possession = gameObject.GetComponent<IPossessable>();
        //possession.enabled = false;
        isPossessed = false;
        isAnimationFinished = true;
        hasEnoughSpace = true;
    }

    //Pour l'animation de possession
    IEnumerator AnimationTime()
    {
        player.lastPossession = this;
        manager.GetComponent<SpriteRenderer>().enabled = true;
        player.GetComponent<Rigidbody2D>().simulated = false;
        gameObject.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        player.GetComponent<Collider2D>().enabled = false;
        player.canMove = false;
        isAnimationFinished = false;
        yield return new WaitForSecondsRealtime(.5f);
        manager.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSecondsRealtime(.5f);
        if(possessedSprite != null)
        {
            spriteRenderer.sprite = possessedSprite;
        }
        isAnimationFinished = true;
        possession.OnPossessed();
        //possession.enabled = true;
    }

    void FixedUpdate()
    {
        //Pour le mouvement de l'animation
        if (isPossessed)
        {
            Vector3 pos = new Vector3(0,0,player.transform.position.z);

            if (!isAnimationFinished)
            {
                pos.y = Mathf.Lerp(player.transform.position.y, transform.position.y, lerpSpeed * Time.deltaTime);
                pos.x = Mathf.Lerp(player.transform.position.x, transform.position.x, lerpSpeed * Time.deltaTime);
            }
            else
            {
                pos.y = transform.position.y;
                pos.x = transform.position.x;
            }

            player.transform.position = pos;
        }
    }

    //Input de possession
    private void OnMouseDown()
    {
        if(Vector2.Distance(this.transform.position, player.transform.position) <= possessionDistance)
        {
            if (isAnimationFinished)
            {

                player.GetComponent<Rigidbody2D>().linearVelocityX = 0;

                //Si le joueur veut poss�der l'objet en poss�dant d�j� un autre
                if (player.isPossessing && !isPossessed)
                {
                    print("PLAYER WANTS TO POSSESSED ANOTHER BOJECT");
                    if (player.lastPossession != null)
                    {
                        player.lastPossession.StopPossession();
                    }

                    isPossessed = true;
                    player.isPossessing = true;
                    StartCoroutine(AnimationTime());

                }
                //Si le joueur veut poss�der l'objet
                else if (!player.isPossessing && !isPossessed)
                {
                    isPossessed = true;
                    player.isPossessing = true;
                    StartCoroutine(AnimationTime());
                }
                //Si le joueur veut sortir de l'objet
                else if (player.isPossessing && isPossessed && hasEnoughSpace)
                {
                    StopPossession();
                }
            }
        }
    }

    //Pour arr�ter la possession
    public void StopPossession()
    {
        print("STOP POSSESSION!");
        isPossessed = false;

        if(normalSprite != null)
            spriteRenderer.sprite = normalSprite;

        possession.OnDepossessed();
        //possession.enabled = false;
        gameObject.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        gameObject.GetComponent<Rigidbody2D>().linearVelocityX = 0;
        player.isPossessing = false;
        player.GetComponent<Collider2D>().enabled = true;
        player.GetComponent<Rigidbody2D>().simulated = true;

        if (hasPosControl)
        {
            if (posControl.isInContact)
                player.transform.position += new Vector3(0, player.GetComponent<Collider2D>().bounds.extents.y - col2D.bounds.extents.y, 0);
        }
        else
            player.transform.position += new Vector3(0, player.GetComponent<Collider2D>().bounds.extents.y - col2D.bounds.extents.y + 0.1f, 0);

        manager.GetComponent<PlayerManager>().VariablesToDefaultValues();
        manager.GetComponent<SpriteRenderer>().enabled = true;
        player.canMove = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SizeTrigger"))
        {
            hasEnoughSpace = false;
            Debug.Log("Entered Trigger");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("SizeTrigger"))
        {
            hasEnoughSpace = true;
            Debug.Log("Entered Trigger");
        }
    }

    public void ResetInitialState()
    {
        if (isPossessed)
        {
            StopPossession();
        }
        isPossessed = false;
        isAnimationFinished = true;
        hasEnoughSpace = true;
    }
}
