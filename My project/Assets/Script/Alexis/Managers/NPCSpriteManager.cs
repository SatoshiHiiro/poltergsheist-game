using UnityEngine;
using System.Collections;

public class NPCSpriteManager : MonoBehaviour
{
    BasicNPCBehaviour npcBehav;
    Transform fieldOfView;
    Transform pivot;
    Transform npcTrans;
    Animator npcAnim;
    bool isStartRelatedFinished;

    float rotationSpeed = 1000f;        //Multiplier for the number of degrees to turn each frame
    Quaternion directionSprite;
    Quaternion directionView;
    Vector3 lastNPCPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (this.transform.parent.TryGetComponent<BasicNPCBehaviour>(out npcBehav)) { }
        else if (this.transform.parent.parent.TryGetComponent<BasicNPCBehaviour>(out npcBehav)) { }
        else { Debug.Log("No BasicNPCBehaviour in parent or grandparent for a NPC sprite"); }
        npcTrans = npcBehav.transform;
        isStartRelatedFinished = false;
        npcAnim = this.GetComponentInParent<Animator>();
        pivot = npcAnim.transform;

        lastNPCPos = npcTrans.position;
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
        /*if (npcBehav.transform.name != "Cat")
        {
            Vector3 currentPos = npcTrans.position;
            if (lastNPCPos.x != currentPos.x)
            {
                npcAnim.SetBool("InMovement", true);
                if (currentPos.x - lastNPCPos.x > 0)
                {
                    npcAnim.SetFloat("Direction", 1);
                }
                else
                {
                    npcAnim.SetFloat("Direction", -1);
                }
            }
            else
            {
                npcAnim.SetBool("InMovement", false);
            }
        }*/
        
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

        lastNPCPos = npcTrans.position;
    }

    void RotateSprite(Quaternion sprite, Quaternion view)
    {
        float step = rotationSpeed * Time.deltaTime;
        Quaternion spriteAngle = Quaternion.RotateTowards(pivot.localRotation, sprite, step);
        pivot.rotation = spriteAngle;

        Quaternion viewAngle = Quaternion.RotateTowards(fieldOfView.localRotation, view, step);
        fieldOfView.rotation = viewAngle;
    }
}
