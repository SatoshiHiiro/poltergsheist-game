using UnityEngine;

public class StealableBehavior : PickupItemBehavior
{
    EnergySystem energy;
    [SerializeField] public float energyGain;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        energy = FindFirstObjectByType<EnergySystem>();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.GetComponent<PlayerController>() != null || collision.GetComponent<PossessionController>() != null)
            energy.ModifyEnergy(energyGain);
    }
}
