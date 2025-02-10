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
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform nextFloor;
    [SerializeField] private Transform bottomFloor;
    [SerializeField] private float speed = 5f;

    private bool isClimbing;

    private void Start()
    {
        isClimbing = false;
    }

    public void ClimbStair(GameObject character, StairDirection direction)
    {        
        StartCoroutine(HandleClimbingStair(character, direction));
    }

    private IEnumerator HandleClimbingStair(GameObject character, StairDirection direction)
    {
        // Which floor does the character want to go to
        Transform destination = direction == StairDirection.Upward ? nextFloor : bottomFloor;

        if(destination == null)
        {
            yield break;
        }

        // Kepp the player from moving when climbing the stair
        if (character.GetComponent<PlayerController>() != null)
        {
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

        
        character.transform.position = destination.position;    // Teleport the player
               

        // Finish climbing stair animation

        // Re-enable player movement
        if (character.GetComponent<PlayerController>() != null)
        {
            character.GetComponent<PlayerController>().canMove = true;
            character.GetComponent<PlayerController>().canJump = true;
        }
    }
}
