using UnityEngine;
using System.Collections;

public class PlayerManager : RotationManager
{
    //General animation variables
    Animator baseHeight;
    Animator bodyAnim;
    Animator mustacheAnim;
    [HideInInspector] public Transform playerFace;
    SpriteRenderer[] faceSpriteArray;
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
        faceSpriteArray = playerFace.GetComponentsInChildren<SpriteRenderer>(true);
    }

    void OnEnable()
    {
        baseHeight = GetComponentInParent<HeightAndSpriteResizeSystem>().GetComponent<Animator>();
        bodyAnim = baseHeight.transform.GetChild(0).Find("Body").GetComponent<Animator>();
        mustacheAnim = bodyAnim.transform.parent.Find("Face").Find("Mustache").GetComponent<Animator>();
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
        FaceLerping(facePosIni, faceScaleIni, clip[0].clip.length, true);
    }

    IEnumerator FaceLerp(Vector3 posTarget, Vector3 scalTarget, float duration, bool isDepossession)
    {
        for (int i = 0; i < faceSpriteArray.Length; i++) { faceSpriteArray[i].sortingLayerName = "Player"; }

        float endTime = Time.time + duration;
        float angle = Quaternion.Angle(playerFace.rotation, faceRotIni) / duration;
        float distance = Vector3.Distance(playerFace.localPosition, posTarget) / duration;
        float sizeDiff = Vector3.Distance(playerFace.localScale, scalTarget) / duration;

        while (Time.time <= endTime)
        {
            float deltaTime = Time.deltaTime;
            playerFace.rotation = Quaternion.RotateTowards(playerFace.rotation, faceRotIni, angle * deltaTime);
            playerFace.localPosition = Vector3.MoveTowards(playerFace.localPosition, posTarget, distance * deltaTime);
            playerFace.localScale = Vector3.MoveTowards(playerFace.localScale, scalTarget, sizeDiff * deltaTime);
            yield return null;
        }
        playerFace.rotation = faceRotIni;
        playerFace.localPosition = posTarget;
        playerFace.localScale = scalTarget;

        if (isDepossession) 
        {
            mustacheAnim.SetBool("IsPossessing", false);
            canRotate = true;
        }
        else
        {
            for (int i = 0; i < faceSpriteArray.Length; i++)
            {
                faceSpriteArray[i].sortingLayerName = playerFace.parent.GetComponent<SpriteRenderer>().sortingLayerName;
            }
        }
    }

    public void PossessionParameter(string param)
    {
        if (param == lastPossession.possessParam)
        {
            //Debug.Log("Possessing Animation");
            bodyAnim.SetTrigger("IsPossessing");
        }
        else if (param == lastPossession.depossessParam)
        {
            //Debug.Log("Depossessing Animation");
            bodyAnim.SetTrigger("IsDepossessing");
            StartCoroutine(DepossessionDelay());
        }
    }

    public void FaceLerping(Vector3 positionTarget, Vector3 scaleTarget, float time, bool isDepossession)
    {
        StopAllCoroutines();
        StartCoroutine(FaceLerp(positionTarget, scaleTarget, time, isDepossession));
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
