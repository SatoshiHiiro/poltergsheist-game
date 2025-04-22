using UnityEngine;
using System.Collections;

public class PlayerManager : RotationManager
{
    //General animation variables
    Animator baseHeight;
    Animator bodyAnim;
    [HideInInspector] public Transform playerFace;
    PossessionManager lastPossession;
    //Coroutine lerpFace;
    private Vector3 facePosIni;
    private Vector3 faceScaleIni;
    private Quaternion faceRotIni;

    private Vector3 scaleIni;

    public Vector3 FacePositionIni { get { return facePosIni; } }
    public Vector3 FaceScaleIni { get { return faceScaleIni; } }
    public Quaternion FaceRotationIni { get { return faceRotIni; } }

    protected override void Start()
    {
        base.Start();
        scaleIni = this.transform.lossyScale;
        lastPossession = null;
    }

    void OnEnable()
    {
        baseHeight = GetComponentInParent<HeightAndSpriteResizeSystem>().GetComponent<Animator>();
        bodyAnim = baseHeight.transform.GetChild(0).Find("Body").GetComponent<Animator>();
        playerFace = bodyAnim.transform.parent.Find("Face");
        facePosIni = playerFace.localPosition;
        faceScaleIni = playerFace.localScale;
        faceRotIni = playerFace.rotation;
        player.onJump += EventParameter;
        player.onLand += EventParameter;
        player.onBonkR += EventParameter;
        player.onBonkL += EventParameter;
        StartCoroutine(OnEnableRelated());
    }

    void OnDisable()
    {
        player.onJump -= EventParameter;
        player.onLand -= EventParameter;
        player.onBonkR -= EventParameter;
        player.onBonkL -= EventParameter;
        if (lastPossession != null) 
        {
            lastPossession.onDepossess -= PossessionParameter;
            lastPossession.onDepossess -= PossessionParameter; 
        }
    }

    protected override void Update()
    {
        base.Update();
        baseHeight.SetBool("inContact", player.isInContact);
        if (lastPossession != player.lastPossession)
        {
            //bodyAnim.SetTrigger("IsDepossessing");
            bodyAnim.SetTrigger("IsPossessing");
            if (lastPossession != null) 
            {
                lastPossession.onPossess -= PossessionParameter;
                lastPossession.onDepossess -= PossessionParameter; 
            }
            lastPossession = player.lastPossession;
            lastPossession.onPossess += PossessionParameter;
            lastPossession.onDepossess += PossessionParameter;
        }
    }

    IEnumerator OnEnableRelated()
    {
        yield return new WaitForEndOfFrame();
        canRotate = true;
    }

    IEnumerator DepossessionDelay()
    {
        yield return new WaitForFixedUpdate();
        //playerFace.SetParent(player.transform, true);
        playerFace.SetParent(bodyAnim.transform.parent, true);
        playerFace.SetAsLastSibling();
        //playerFace.eulerAngles = new Vector3(playerFace.eulerAngles.x, player.transform.eulerAngles.y, playerFace.eulerAngles.z);
        AnimatorClipInfo[] clip = bodyAnim.GetCurrentAnimatorClipInfo(0);
        FaceLerpPosAndRot(facePosIni, faceRotIni, clip[0].clip.length, true);
    }

    IEnumerator FaceLerp(Vector3 posTarget, Quaternion rotTarget, float duration, bool isDepossession)
    {
        float endTime = Time.time + duration;
        float angle = Quaternion.Angle(playerFace.rotation, rotTarget) / duration;
        float distance = Vector3.Distance(playerFace.localPosition, posTarget) / duration;
        float sizeDiff = Vector3.Distance(playerFace.localScale, faceScaleIni) / duration;

        while (Time.time <= endTime)
        {
            float deltaTime = Time.deltaTime;
            playerFace.rotation = Quaternion.RotateTowards(playerFace.rotation, rotTarget, angle * deltaTime);
            playerFace.localPosition = Vector3.MoveTowards(playerFace.localPosition, posTarget, distance * deltaTime);
            playerFace.localScale = Vector3.MoveTowards(playerFace.localScale, faceScaleIni, sizeDiff * deltaTime);
            yield return null;
        }
        playerFace.rotation = rotTarget;
        playerFace.localPosition = posTarget;
        playerFace.localScale = faceScaleIni;

        if (isDepossession) { canRotate = true; }
    }

    public void PossessionParameter(string param)
    {
        if (param == lastPossession.possessParam)
        {
            Debug.Log("Possessing Animation");
            bodyAnim.SetTrigger("IsPossessing");
        }
        else if (param == lastPossession.depossessParam)
        {
            Debug.Log("Depossessing Animation");
            bodyAnim.SetTrigger("IsDepossessing");
            StartCoroutine(DepossessionDelay());
        }
    }

    public void FaceLerpPosAndRot(Vector3 positionTarget, Quaternion rotationTarget, float time, bool isDepossession)
    {
        StopAllCoroutines();
        StartCoroutine(FaceLerp(positionTarget, rotationTarget, time, isDepossession));
    }

    public void EventParameter(string param)
    {
        CollisionConditionsForManager(param, player, baseHeight);
    }

    public void VariablesToDefaultValues()
    {
        this.transform.localScale = scaleIni;
    }
}
