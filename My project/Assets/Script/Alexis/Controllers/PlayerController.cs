using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

/// But: Player controller
/// Requiert: Rigidbody2D
/// Input: A = gauche, D = droit, SPACE = saut
/// �tat: Optimal(temp)
public class PlayerController : MovementController
{
    [Header ("Sound variables")]
    [SerializeField] protected AK.Wwise.Event hoveringSoundEvent;
    protected bool isHovering;

    //Possession
    [Header("Object possession")]
    public PossessionManager lastPossession;

    // TODO FAIRE DE LA BONNE MANI�RE!!!
    // Conditions
    [HideInInspector] public bool isPossessing; //Utilis� par PossessionBehavior pour v�rifier si le Player poss�de un objet
    [HideInInspector] public Vector2 sizeofPlayer;
    [HideInInspector] public bool isPossessionInProgress = false;

    protected override void Start()
    {
        base.Start();
        canMove = true;
        sizeofPlayer = GetComponent<Collider2D>().bounds.size;
    }

    protected override void Update()
    {
        base.Update();
        if (isPossessing)
        {
            hoveringSoundEvent.Stop(gameObject);
            isHovering = false;
        }
        else if(!isPossessing && !isHovering)
        {
            isHovering = true;
            hoveringSoundEvent.Post(gameObject);
        }
    }
}
