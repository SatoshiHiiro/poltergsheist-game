using UnityEngine;


public class StealableBehavior : PickupItemBehavior
{
    EnergySystem energy;
    [SerializeField] public float energyGain;
    public AK.Wwise.Event soundEvent; // Drag & Drop l’event Wwise ici


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    protected override void Awake()
    {
        base.Awake();
    }
    protected void Start()
    {
        energy = FindFirstObjectByType<EnergySystem>();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        PossessionManager possessionManager = collision.GetComponent<PossessionManager>();
        if (collision.GetComponent<PlayerController>() != null || (possessionManager != null && possessionManager.IsPossessing))
        {
            InventorySystem.Instance.AddStolenItemToInventory(this);
            //Debug.Log("Collision détectée avec : " + other.gameObject.name);
            soundEvent.Post(gameObject); // Joue le son
        }
            //energy.ModifyEnergy(energyGain);


  
    }
}
