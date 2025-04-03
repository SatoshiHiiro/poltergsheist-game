using UnityEngine;

public class StretchFailAnimation : MonoBehaviour
{
    public float shakeMagnitude;
    public float timeToComplete;
    public bool isLocal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnEnable()
    {
        Vector3 amount = new Vector3(shakeMagnitude, 1 , 1);
        iTween.ShakePosition(this.gameObject, iTween.Hash("amount", amount, "islocal", isLocal, "time", timeToComplete));
    }
}
