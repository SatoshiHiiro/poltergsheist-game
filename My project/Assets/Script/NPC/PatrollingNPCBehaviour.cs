using UnityEngine;

public class PatrollingNPCBehaviour : HumanNPCBehaviour
{
    // Enemy patrol variables
    [SerializeField] protected Transform[] patrolPoints;    // Points were the NPC patrol
    [SerializeField] protected float movementSpeed = 6f;
    protected int indexPatrolPoints;    // Next index patrol point
    SpriteRenderer spriteRenderer;

    [SerializeField] protected float waitingTime;
    [SerializeField] protected float roomWaitingTime;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        indexPatrolPoints = 0;
    }
    protected override void Update()
    {
        // À CHANGER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        base.Update();
    }

    protected void Patrol()
    {
        // Get movement direction
        Vector2 destination = new Vector2(patrolPoints[indexPatrolPoints].position.x, transform.position.y);
        Vector2 direction = (destination - (Vector2)transform.position).normalized;

        // Flip sprite based on direction

        spriteRenderer.flipX = direction.x < 0;
        facingRight = !spriteRenderer.flipX;

        // Move towards destination
        transform.position = Vector2.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);

        // If the enemy reaches his destination, he is given a new destination to patrol
        if (Mathf.Abs(patrolPoints[indexPatrolPoints].position.x - transform.position.x) <= 0.2f)
        {
            indexPatrolPoints++;
            if (indexPatrolPoints >= patrolPoints.Length)
            {
                indexPatrolPoints = 0;
            }
        }
    }
}
