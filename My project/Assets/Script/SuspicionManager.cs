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
    protected float npcFactor;

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
        UpdateSuspicion();
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

    private void UpdateSuspicion()
    {
        if(paranormalObserverCount > 0)
        {
            // Double the increase of suspicion when multiple NPC see the object moves
            npcFactor = paranormalObserverCount > 1 ? 2f : 1f;
            currentSuspicion += suspicionRate * npcFactor * Time.deltaTime;
            currentSuspicion = Mathf.Clamp(currentSuspicion, 0f, maxSuspicion);
            OnSuspicionChanged?.Invoke(currentSuspicion / maxSuspicion);
        }
        
    }
}
