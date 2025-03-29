using UnityEngine;


public class StealableBehavior : PickupItemBehavior
{
    EnergySystem energy;
    [SerializeField] public float energyGain;
    public AK.Wwise.Event soundEvent; // Drag & Drop l�event Wwise ici


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
        if (collision.GetComponent<PlayerController>() != null || collision.GetComponent<PossessionController>() != null)
        {
            InventorySystem.Instance.AddStolenItemToInventory(this);
            //Debug.Log("Collision d�tect�e avec : " + other.gameObject.name);
            soundEvent.Post(gameObject); // Joue le son
        }
            //energy.ModifyEnergy(energyGain);


  
    }
}
