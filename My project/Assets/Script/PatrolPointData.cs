using UnityEngine;

public enum PatrolPointType
{
    Normal,
    Room
}
public class PatrolPointData : MonoBehaviour
{
    [SerializeField] private Transform point;
    [SerializeField] private PatrolPointType patrolPointType;
}
