using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

/// But: Player controller
/// Requiert: Rigidbody2D
/// Input: A = gauche, D = droit, SPACE = saut
/// �tat: Optimal(temp)
public class PlayerController : MovementController
{
    //Possession
    [Header("Object possession")]
    public PossessionManager lastPossession;
    protected override float velocityXForSquash { get => base.velocityXForSquash; set => base.velocityXForSquash = value; }
    protected override float posDiffForSquash { get => base.posDiffForSquash; set => base.posDiffForSquash = value; }

    // TODO FAIRE DE LA BONNE MANI�RE!!!
    // Conditions
    [HideInInspector] public bool isPossessing; //Utilis� par PossessionBehavior pour v�rifier si le Player poss�de un objet
    [HideInInspector] public Vector2 sizeofPlayer;

    protected override void Start()
    {
        base.Start();
        canMove = true;
        velocityXForSquash = .2f;
        posDiffForSquash = .2f;
        sizeofPlayer = GetComponent<Collider2D>().bounds.size;
    }
}
