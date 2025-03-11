using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    BoxCollider2D boxCollider;
    Animator animator;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Possess"))
        {
            animator.SetBool("ClearFog",true);
            boxCollider.enabled = false;
        }
    }
}
