using System.Collections;
using UnityEngine;

public enum StairDirection
{
    Upward,
    Downward
}
public class StairController : MonoBehaviour
{
    // This class manage stairs depending on the direction desired by the character

    [Header("Stairs")]
    [SerializeField] private Transform startPoint;  // Center of the door on floor level
    [SerializeField] private StairController upperFloor;   // Next upper floor
    [SerializeField] private StairController bottomFloor; // Bottom floor
    //[SerializeField] private Transform nextFloor;   // Next upper floor
    //[SerializeField] private Transform bottomFloor; // Bottom floor
    [SerializeField] private float speed = 5f;      // Speed to go at the center of the door before climbing
    [SerializeField] private float maximumHeight;   // Maximum Height of the object so he can climb the stair
    [SerializeField] private float maximumWidth;    // Maximum Width of the object so he can climb the stair

    private bool isClimbing;
    private bool canCharacterJump;
    private bool canCharacterMove;

    private void Start()
    {
        isClimbing = false;
    }

    public void ClimbStair(GameObject character, StairDirection direction)
    {        
        Renderer characterRenderer = character.GetComponent<Renderer>();
        print("Character size X: " + characterRenderer.bounds.size.x);
        print("Character size Y: " + characterRenderer.bounds.size.y);
        // If the character fit with the stair dimension, then he can climb
        if (characterRenderer.bounds.size.x < maximumWidth && characterRenderer.bounds.size.y < maximumHeight)
        {
            StartCoroutine(HandleClimbingStair(character, direction));
        }
        
    }

    private IEnumerator HandleClimbingStair(GameObject character, StairDirection direction)
    {
        // Which floor does the character want to go to
        StairController targetStair = direction == StairDirection.Upward ? upperFloor : bottomFloor;

        if(targetStair == null)
        {
            yield break;
        }

        // Kepp the player from moving when climbing the stair
        if (character.GetComponent<PlayerController>() != null)
        {
            canCharacterMove = character.GetComponent<PlayerController>().canMove;
            canCharacterJump = character.GetComponent<PlayerController>().canJump;
            character.GetComponent<PlayerController>().canMove = false;
            character.GetComponent<PlayerController>().canJump = false; 
        }

        // Move towards the center of the stairs
        while (Mathf.Abs(character.transform.position.x - startPoint.position.x) > 0.1f)
        {
            // We center the player with the stair before climbing it
            Vector2 targetPosition = new Vector2(startPoint.position.x, character.transform.position.y);
            character.transform.position = Vector2.MoveTowards(character.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);


        // Climbing stair animation

        // Calculate adjusted destination position based on character height
        Renderer characterRenderer = character.GetComponent<Renderer>();
        float characterHeight = characterRenderer.bounds.size.y;
        Vector3 adjustedPosition = targetStair.startPoint.position;

        // Adjust Y position so the character's feet are at the floor level
        adjustedPosition.y += characterHeight / 2f;

        // Teleport the player to the adjusted position
        character.transform.position = adjustedPosition;
        //character.transform.position = destination.position;    // Teleport the player
               

        // Finish climbing stair animation

        // Re-enable player movement
        if (character.GetComponent<PlayerController>() != null)
        {
            character.GetComponent<PlayerController>().canMove = canCharacterMove;
            character.GetComponent<PlayerController>().canJump = canCharacterJump;
        }
    }
}
