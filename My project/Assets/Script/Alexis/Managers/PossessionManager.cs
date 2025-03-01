using UnityEngine;
using System.Collections;

/// But: Posséder des objets
/// Requiert: PossessionController
/// Requiert Externe: PlayerController(1)
/// Input: Click Gauche = possession/dépossession
/// État: Adéquat(temp)
public class PossessionManager : MonoBehaviour
{
    //Variables
    [Header("Variables")]
    [SerializeField] public float lerpSpeed;        //Vitesse du lerp
    [SerializeField] public float initialEnergyLoss;
    [SerializeField] public float continuousEnergyLoss;

    //Conditions
    bool isPossessed;                               //Pour savoir si l'objet possessible est possédé
    bool isAnimationFinished;                       //Pour savoir si l'animaation de possession est fini

    //Shortcuts
    PlayerController player;
    IPossessable possession;

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        possession = gameObject.GetComponent<IPossessable>();
        //possession.enabled = false;
        isPossessed = false;
        isAnimationFinished = true;
    }

    //Pour l'animation de possession
    IEnumerator AnimationTime()
    {
        FindFirstObjectByType<EnergySystem>().ModifyEnergy(-initialEnergyLoss);
        player.lastPossession = gameObject.name;
        player.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        player.GetComponent<Rigidbody2D>().simulated = false;
        gameObject.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        player.GetComponent<Collider2D>().enabled = false;
        player.canMove = false;
        isAnimationFinished = false;
        yield return new WaitForSecondsRealtime(.5f);
        player.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSecondsRealtime(.5f);
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
                FindFirstObjectByType<EnergySystem>().ModifyEnergy(-continuousEnergyLoss);
            }
            
            player.transform.position = pos;
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

            //Si le joueur veut posséder l'objet en possédant déjà un autre
            if (player.isPossessing && !isPossessed)
            {
                isPossessed = true;
                StartCoroutine(AnimationTime());
            }
            //Si le joueur veut posséder l'objet
            else if (!player.isPossessing && !isPossessed)
            {
                isPossessed = true;
                player.isPossessing = true;
                StartCoroutine(AnimationTime());
            }
            //Si le joueur veut sortir de l'objet
            else if (player.isPossessing && isPossessed)
            {
                StopPossession();
                //player.isPossessing = false;
                //player.GetComponent<Collider2D>().enabled = true;
                //player.GetComponent<Rigidbody2D>().simulated = true;
                //player.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                //player.canMove = true;
            }
        }
    }

    //Pour arrêter la possession
    public void StopPossession()
    {
        isPossessed = false;
        possession.OnDepossessed();
        //possession.enabled = false;
        gameObject.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        gameObject.GetComponent<Rigidbody2D>().linearVelocityX = 0;

        player.isPossessing = false;
        player.GetComponent<Collider2D>().enabled = true;
        player.GetComponent<Rigidbody2D>().simulated = true;
        player.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        player.canMove = true;
    }
}
