using UnityEngine;

public enum StairDirection
{
    Upward,
    Downward
}
public class StairController : MonoBehaviour
{
    [Header("Stair next level")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    public void ClimbStair(GameObject character, StairDirection direction)
    {

    }
}
