using UnityEngine;

public class FloorNavigation : MonoBehaviour
{
    // Singleton pattern
    private static FloorNavigation instance;
    public static FloorNavigation Instance { get { return instance; } }

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


}
