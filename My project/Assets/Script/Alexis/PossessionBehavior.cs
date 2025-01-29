using UnityEngine;
using System.Collections;

/// But: Posséder des objets
/// Requiert: PossessionController
/// Requiert Externe: PlayerController(1)
/// Input: Click Gauche = possession/dépossession
/// État: Adéquat(temp)
public class PossessionBehavior : MonoBehaviour
{
    //Variables
    [Header("Variables")]
    [SerializeField] public float lerpSpeed;        //Vitesse du lerp
    public float energyDrainSpeed;

    //Conditions
    bool isPossessed;                               //Pour savoir si l'objet possessible est possédé
    bool isAnimationFinished;                       //Pour savoir si l'animaation de possession est fini

    //Shortcuts
    PlayerController player;
    PossessionController possession;
    EnergySystem energy;

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        possession = gameObject.GetComponent<PossessionController>();
        energy = FindFirstObjectByType<EnergySystem>();
        possession.enabled = false;
        isPossessed = false;
        isAnimationFinished = true;
        energyDrainSpeed = 0.001f;
    }

    //Pour l'animation de possession
    IEnumerator AnimationTime()
    {
        player.lastPossession = gameObject.name;
        player.GetComponent<SpriteRenderer>().enabled = true;
        player.GetComponent<Rigidbody2D>().simulated = false;
        gameObject.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        player.GetComponent<Collider2D>().enabled = false;
        player.canMove = false;
        isAnimationFinished = false;
        yield return new WaitForSecondsRealtime(.5f);
        player.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSecondsRealtime(.5f);
        isAnimationFinished = true;
        possession.enabled = true;
    }

    void FixedUpdate()
    {
        //Pour le mouvement de l'animation
        if (isPossessed)
        {
            Vector3 pos = Vector3.zero;

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
            energy.ModifyEnergy(-energyDrainSpeed);

            if (energy.CurrentEnergy() == 0)
            {
                GetOutOfPossession();
            }
        }

        //Pour enlever la possession sur l'objet si le Player possède un autre objet avant de déposséder
        if (player.lastPossession != gameObject.name && isPossessed)
            StopPossession();
    }

    //Input de possession
    private void OnMouseDown()
    {
        if (isAnimationFinished)
        {
            player.GetComponent<Rigidbody2D>().linearVelocityX = 0;

            if (!isPossessed && energy.CurrentEnergy() > 0)
            {
                //Si le joueur veut posséder l'objet en possédant déjà un autre
                if (player.isPossessing)
                {
                    isPossessed = true;
                    StartCoroutine(AnimationTime());
                }
                //Si le joueur veut posséder l'objet
                else
                {
                    isPossessed = true;
                    player.isPossessing = true;
                    StartCoroutine(AnimationTime());
                }
            }
            
            //Si le joueur veut sortir de l'objet
            else if (player.isPossessing && isPossessed)
            {
                GetOutOfPossession();
            }
        }
    }

    //Pour arrêter la possession
    void StopPossession()
    {
        isPossessed = false;
        possession.enabled = false;
        gameObject.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        gameObject.GetComponent<Rigidbody2D>().linearVelocityX = 0;
    }

    void GetOutOfPossession()
    {
        StopPossession();
        player.isPossessing = false;
        player.GetComponent<Collider2D>().enabled = true;
        player.GetComponent<Rigidbody2D>().simulated = true;
        player.GetComponent<SpriteRenderer>().enabled = true;
        player.canMove = true;
    }
}
