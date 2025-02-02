using System;
using UnityEngine;

public class SuspicionManager : MonoBehaviour
{
    // Singleton
    public static SuspicionManager Instance { get; private set; }

    private int paranormalObserverCount;
    private float currentSuspicion;
    private float maxSuspicion;

    [SerializeField] protected float suspicionRate;
    [SerializeField] protected float sizeFactor;
    [SerializeField] protected float npcFactor;
    [SerializeField] protected float displacementFactor;

    // Position suspicion change variables
    [SerializeField] protected float maxPositionChange;
    [SerializeField] protected float maxPositionFactor;
    [SerializeField] protected float minPositionChange;
    [SerializeField] protected float minPositionFactor;

    // Rotation suspicion change variables
    [SerializeField] protected float maxRotationChange;
    [SerializeField] protected float maxRotationFactor;
    [SerializeField] protected float minRotationChange;
    [SerializeField] protected float minRotationFactor;
    



    public event Action<float> OnSuspicionChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        paranormalObserverCount = 0;
        currentSuspicion = 0f;
        maxSuspicion = 100f;
    }

    private void Update()
    {
        //UpdateSuspicion();
    }

    public void AddParanormalObserver()
    {
        paranormalObserverCount++;
    }

    public void RemoveParanormalObserver()
    {
        
        paranormalObserverCount--;        
        paranormalObserverCount = Mathf.Max(0, paranormalObserverCount);
    }

    public void UpdateMovementSuspicion(float objectSize)
    {
        // Caculate suspicion when an object is moving
        if (paranormalObserverCount > 0)
        {
            // Double the increase of suspicion when multiple NPC see the object moves
            npcFactor = paranormalObserverCount > 1 ? npcFactor : 1f;
            currentSuspicion += suspicionRate * npcFactor * (sizeFactor * objectSize) * Time.deltaTime;
            currentSuspicion = Mathf.Clamp(currentSuspicion, 0f, maxSuspicion);
            OnSuspicionChanged?.Invoke(currentSuspicion / maxSuspicion);
        }
    }

    public void UpdateDisplacementSuspicion(float objectSize, float rotationChange, float positionChange)
    {
        print("ALLO");
        print("PositionChange: " + positionChange);
        float positionFactor = 1f;
        if(positionChange > maxPositionChange)
        {
            positionFactor = maxPositionFactor;
        }
        else if(positionChange > minPositionChange)
        {
            positionFactor = minPositionFactor;
        }

        float rotationFactor = 1f;
        if(rotationChange > maxRotationChange)
        {
            rotationFactor = maxRotationFactor;
        }
        else if (rotationChange > minRotationChange)
        {
            rotationFactor = minRotationFactor;
        }


        //float positionFactor = Mathf.Clamp(positionChange / maxPositionChange, 1f, 5f);
        //float rotationFactor = Mathf.Clamp(rotationChange / maxRotationChange, 1f, 3f);
        
        float changeFactor = Mathf.Max(positionFactor, rotationFactor);
        print("Change factor : " + changeFactor);
        print("Current suspicion: " + currentSuspicion);
        print("size " + (sizeFactor * objectSize));
        currentSuspicion += displacementFactor * (sizeFactor * objectSize) * changeFactor;
        print(currentSuspicion);
        OnSuspicionChanged?.Invoke(currentSuspicion / maxSuspicion);
    }
}
