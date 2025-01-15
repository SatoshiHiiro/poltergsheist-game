using UnityEngine;
using System.Collections;

public class PossessionBehavior : MonoBehaviour
{
    [SerializeField] public float lerpSpeed;

    bool isPossessed;
    bool isAnimationFinished;

    

    PlayerController player;
    PossessionController possession;

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        possession = gameObject.GetComponent<PossessionController>();
        possession.enabled = false;
        isPossessed = false;
        isAnimationFinished = true;
    }

    IEnumerator AnimationTime()
    {
        player.lastPossession = gameObject.name;
        player.GetComponent<SpriteRenderer>().enabled = true;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY;
        player.GetComponent<Collider2D>().enabled = false;
        player.canMove = false;
        isAnimationFinished = false;
        yield return new WaitForSecondsRealtime(.5f);
        player.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSecondsRealtime(.5f);
        isAnimationFinished = true;
        gameObject.GetComponent<Collider2D>().isTrigger = false;
        possession.enabled = true;
    }

    void FixedUpdate()
    {
        if (isPossessed)
        {
            Vector3 pos = Vector3.zero;

            pos.y = player.transform.position.y;
            pos.x = Mathf.Lerp(player.transform.position.x, transform.position.x, lerpSpeed * Time.deltaTime);
            player.transform.position = pos;
        }

        if (player.lastPossession != gameObject.name)
        {
            StopPossession();
        }
    }

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
                player.isPossessing = false;
                player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                player.GetComponent<Collider2D>().enabled = true;
                player.GetComponent<SpriteRenderer>().enabled = true;
                player.canMove = true;
            }
        }
    }

    void StopPossession()
    {
        isPossessed = false;
        possession.enabled = false;
        gameObject.GetComponent<Rigidbody2D>().linearVelocityX = 0;
        gameObject.GetComponent<Collider2D>().isTrigger = true;
    }
}
