using UnityEngine;
using UnityEngine.UI;

public class EnergySystem : MonoBehaviour
{
    public float energyValue;
    float maxEnergy;
    float minEnergy;

    Slider slider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = gameObject.GetComponent<Slider>();
        maxEnergy = slider.maxValue;
        minEnergy = slider.minValue;
        energyValue = slider.value;
    }

    public float CurrentEnergy()
    {
        return energyValue;
    }

    public void ModifyEnergy(float addedValue)
    {
        float newValue = energyValue + addedValue;
        if (!(newValue > maxEnergy || newValue < minEnergy))
        {
            energyValue = newValue;
            slider.value = energyValue;
        }
        else
        {
            if (newValue > maxEnergy)
                energyValue = maxEnergy;
            else
                energyValue = minEnergy;
        }
    }
}
