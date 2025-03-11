using UnityEngine;
using System.Collections;

/// But: Poss�der des objets
/// Requiert: PossessionController
/// Requiert Externe: PlayerController(1)
/// Input: Click Gauche = possession/d�possession
/// �tat: Ad�quat(temp)
public class PossessionManager : InteractibleManager
{
    //Variables
    [Header("Variables")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite possessedSprite;
    [SerializeField] public float lerpSpeed;        //Vitesse du lerp
    [SerializeField] public float initialEnergyLoss;
    [SerializeField] public float continuousEnergyLoss;
    [SerializeField] private float possessionDistance;  // Distance between the player and the object so he can possessed it
    private Vector2 sizeOfObject;
    private float ySizeOfObject;

    //Conditions
    bool isPossessed;                               //Pour savoir si l'objet possessible est poss�d�
    bool isAnimationFinished;                       //Pour savoir si l'animaation de possession est fini
    bool hasEnoughSpace;

    //Shortcuts
    EnergySystem energy;
    PlayerController player;
    IPossessable possession;
    SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();
        player = FindFirstObjectByType<PlayerController>();
        energy = FindFirstObjectByType<EnergySystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        possession = gameObject.GetComponent<IPossessable>();
        //possession.enabled = false;
        isPossessed = false;
        isAnimationFinished = true;
        sizeOfObject = this.gameObject.GetComponent<Collider2D>().bounds.size;
        hasEnoughSpace = true;
    }

    //Pour l'animation de possession
    IEnumerator AnimationTime()
    {
        energy.ModifyEnergy(-initialEnergyLoss);
        energy.StopResumeRegen(true);
        float temp = continuousEnergyLoss;
        continuousEnergyLoss = 0;
        player.lastPossession = this;
        player.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        player.GetComponent<Rigidbody2D>().simulated = false;
        gameObject.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        player.GetComponent<Collider2D>().enabled = false;
        player.canMove = false;
        isAnimationFinished = false;
        yield return new WaitForSecondsRealtime(.5f);
        player.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSecondsRealtime(.5f);
        if(possessedSprite != null)
        {
            spriteRenderer.sprite = possessedSprite;
        }
        isAnimationFinished = true;
        continuousEnergyLoss = temp;
        energy.StopResumeRegen(false);
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

            energy.ModifyEnergy(-continuousEnergyLoss);
            player.transform.position = pos;
        }

        // There is no more energy to possessed the object
        if (energy.CurrentEnergy() == 0 && isAnimationFinished && hasEnoughSpace)
            StopPossession();

    }

    void Update()
    {
        /*if (isPossessed)
        {
            float distanceY = player.sizeofPlayer.y - sizeOfObject.y;
            float distanceX = player.sizeofPlayer.x - sizeOfObject.x;

            if (distanceY > 0 && distanceX > 0)
            {
                RaycastHit2D[] raycastHits1;
                RaycastHit2D[] raycastHits2;
                raycastHits1 = Physics2D.RaycastAll(this.gameObject.transform.position + new Vector3(player.sizeofPlayer.x / 2, ySizeOfObject / 2, 0), Vector2.up, distanceY);
                raycastHits2 = Physics2D.RaycastAll(this.gameObject.transform.position + new Vector3(-(player.sizeofPlayer.x / 2), ySizeOfObject / 2, 0), Vector2.up, distanceY);

                //Debug.Log("Ray1: " + raycastHits1.Length + "   Ray2: " + raycastHits2.Length);

                if (raycastHits1.Length > 0 || raycastHits2.Length > 0)
                    hasEnoughSpace = false;
                else
                    hasEnoughSpace = true;
            }
        }*/
    }

    //Input de possession
    private void OnMouseDown()
    {
        if(Vector2.Distance(this.transform.position, player.transform.position) <= possessionDistance && energy.CurrentEnergy() > initialEnergyLoss)
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
        {
            spriteRenderer.sprite = normalSprite;
        }
        possession.OnDepossessed();
        //possession.enabled = false;
        gameObject.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        gameObject.GetComponent<Rigidbody2D>().linearVelocityX = 0;
        player.isPossessing = false;
        player.GetComponent<Collider2D>().enabled = true;
        player.GetComponent<Rigidbody2D>().simulated = true;
        player.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
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
}
