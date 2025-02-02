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

    private void UpdateSuspicion()
    {
        // Caculate suspicion when an object is moving
        if(paranormalObserverCount > 0)
        {
            // Double the increase of suspicion when multiple NPC see the object moves
            npcFactor = paranormalObserverCount > 1 ? npcFactor : 1f;
            currentSuspicion += suspicionRate * npcFactor * Time.deltaTime;
            currentSuspicion = Mathf.Clamp(currentSuspicion, 0f, maxSuspicion);
            OnSuspicionChanged?.Invoke(currentSuspicion / maxSuspicion);
        }
        
    }
}
