using UnityEngine;
using System.Collections;

/// But: Poss�der des objets
/// Requiert: PossessionController
/// Requiert Externe: PlayerController(1)
/// Input: Click Gauche = possession/d�possession
/// �tat: Ad�quat(temp)
public class PossessionManager : InteractibleManager, IResetInitialState
{
    [Header("Sound variables")]
    [SerializeField] public AK.Wwise.Event onPossessionSoundEvent;
    [SerializeField] public AK.Wwise.Event possessionOffSoundEvent;

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
    bool isPossessionLocked;

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
        spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
        //if (this.TryGetComponent<SpriteRenderer>(out spriteRenderer)) { }
        //else if (this.transform.GetChild(0).TryGetComponent<SpriteRenderer>(out spriteRenderer)) { }
        //else if (this.transform.GetChild(0).GetChild(0).TryGetComponent<SpriteRenderer>(out spriteRenderer)) { }
        col2D = this.GetComponent<Collider2D>();
        hasPosControl = false;
        if (this.gameObject.TryGetComponent<PossessionController>(out posControl)) { hasPosControl = true; }
        possession = gameObject.GetComponent<IPossessable>();
        //possession.enabled = false;
        isPossessed = false;
        isAnimationFinished = true;
        hasEnoughSpace = true;
        isPossessionLocked = false;
    }

    //Pour l'animation de possession
    IEnumerator AnimationTime()
    {
        player.lastPossession = this;
        manager.gameObject.SetActive(true);
        player.GetComponent<Rigidbody2D>().simulated = false;
        gameObject.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        player.GetComponent<Collider2D>().enabled = false;
        player.canMove = false;
        isAnimationFinished = false;

        player.isPossessionInProgress = true;

        yield return new WaitForSecondsRealtime(.5f);
        manager.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(.5f);
        if(possessedSprite != null)
        {
            spriteRenderer.sprite = possessedSprite;
        }
        isAnimationFinished = true;
        possession.OnPossessed();
        //possession.enabled = true;
        player.isPossessionInProgress = false;
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
        if (isPossessionLocked || player.isPossessionInProgress || (posControl != null && posControl.isClimbing))
        {
            return; // Can't possessed an object if the possession on this object is locked
        }
        if(Vector2.Distance(this.transform.position, player.transform.position) <= possessionDistance)
        {
            if (isAnimationFinished)
            {

                player.GetComponent<Rigidbody2D>().linearVelocityX = 0;

                player.isPossessionInProgress = true;

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
                    onPossessionSoundEvent.Post(gameObject);
                    StartCoroutine(AnimationTime());

                }
                //Si le joueur veut poss�der l'objet
                else if (!player.isPossessing && !isPossessed)
                {
                    isPossessed = true;
                    player.isPossessing = true;
                    onPossessionSoundEvent.Post(gameObject);
                    StartCoroutine(AnimationTime());
                }
                //Si le joueur veut sortir de l'objet
                else if (player.isPossessing && isPossessed && hasEnoughSpace)
                {
                    print("IM IN THE IF!");
                    StopPossession();
                }
            }
        }
    }

    public void LockPossession(bool locked)
    {
        isPossessionLocked = locked;
    }

    //Pour arr�ter la possession
    public void StopPossession()
    {
        if (posControl != null && posControl.isClimbing)
        {
            print("NOT DEPOSSESSING CAUSE CLIMBING");
        }
        print("STOP POSSESSION!");
        possessionOffSoundEvent.Post(gameObject);
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
        manager.gameObject.SetActive(true);
        player.canMove = true;
        player.isPossessionInProgress = false;
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

            if (isPossessed && hasEnoughSpace)
            {
                // If the player leave a restricted small zone we can now allow him to depossessed the object
                player.isPossessionInProgress = false;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("SizeTrigger"))
        {
            hasEnoughSpace = false;
            Debug.Log("Entered Trigger");
        }
    }

    public void ResetInitialState()
    {
        if (isPossessed)
        {
            if(posControl != null)
            {
                posControl.isClimbing = false;
            }
            StopPossession();
        }
        isPossessed = false;
        isAnimationFinished = true;
        hasEnoughSpace = true;
        isPossessionLocked = false;
    }
}
