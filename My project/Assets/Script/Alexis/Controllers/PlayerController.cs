using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

/// But: Player controller
/// Requiert: Rigidbody2D
/// Input: A = gauche, D = droit, SPACE = saut
/// État: Optimal(temp)
public class PlayerController : MovementController
{
    //Possession
    [Header("Object possession")]
    public PossessionManager lastPossession;

    // TODO FAIRE DE LA BONNE MANIÈRE!!!
    // Conditions
    [HideInInspector] public bool isPossessing; //Utilisé par PossessionBehavior pour vérifier si le Player possède un objet
    [HideInInspector] public Vector2 sizeofPlayer;

    protected override void Start()
    {
        base.Start();
        canMove = true;
        sizeofPlayer = GetComponent<Collider2D>().bounds.size;
    }
}
