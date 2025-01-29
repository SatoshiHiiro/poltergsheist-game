using UnityEngine;

public class StealableBehavior : MonoBehaviour
{
    EnergySystem energy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        energy = FindFirstObjectByType<EnergySystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Possess"))
        {
            energy.ModifyEnergy(1f);
            Destroy(gameObject);
        }
    }
}
