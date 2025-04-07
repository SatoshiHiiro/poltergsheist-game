using UnityEngine;
using System.Collections;

public class NPCSpriteManager : MonoBehaviour
{
    BasicNPCBehaviour npcBehav;
    Transform fieldOfView;
    bool isStartRelatedFinished;

    float rotationSpeed = 1000f;        //Multiplier for the number of degrees to turn each frame
    Quaternion directionSprite;
    Quaternion directionView;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (this.transform.parent.TryGetComponent<BasicNPCBehaviour>(out npcBehav)) { }
        else if (this.transform.parent.parent.TryGetComponent<BasicNPCBehaviour>(out npcBehav)) { }
        else { Debug.Log("No BasicNPCBehaviour in parent or grandparent for a NPC sprite"); }
        isStartRelatedFinished = false;

        StartCoroutine(StartRelated());
        //fieldOfView = npcBehav.FieldOfView;
    }

    IEnumerator StartRelated()
    {
        yield return new WaitForEndOfFrame();
        fieldOfView = npcBehav.FieldOfView.transform;
        isStartRelatedFinished = true;
    }

    // Update is called once per frame
    void Update()
    {
        bool isFacingRight = npcBehav.FacingRight;
        if (isFacingRight)
        {
            directionSprite = new Quaternion(0, 0, 0, 1);     //Look right
            directionView = new Quaternion(.70711f, .70711f, 0, 0);
        }
        else
        {
            directionSprite = new Quaternion(0, 1, 0, 0);     //Look left
            directionView = new Quaternion(0, 0, .70711f, .70711f);
        }

        if (isStartRelatedFinished) 
        { 
            RotateSprite(directionSprite, directionView);
        }
    }

    void RotateSprite(Quaternion sprite, Quaternion view)
    {
        float step = rotationSpeed * Time.deltaTime;
        Quaternion spriteAngle = Quaternion.RotateTowards(transform.localRotation, sprite, step);
        transform.rotation = spriteAngle;

        Quaternion viewAngle = Quaternion.RotateTowards(fieldOfView.localRotation, view, step);
        fieldOfView.rotation = viewAngle;
    }
}
