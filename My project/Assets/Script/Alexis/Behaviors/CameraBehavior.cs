using UnityEngine;

/// But: Suivre un GameObject (le Player) tout en limitant le mouvement de la cam�ra
/// Requiert: Camera
/// �tat: Ad�quat(temp)
public class CameraBehavior : MonoBehaviour
{
    // Singleton
    public static CameraBehavior Instance { get; private set; }



    //Gameobject autres
    [Header("Other GameObjects")]
    [SerializeField] private Transform limiteHG;              //Pour limiter la cam�ra � un point haut-gauche
    [SerializeField] private Transform limiteBD;              //Pour limiter la cam�ra � un point bas-droit
    [SerializeField] private Transform focus;                 //Pour centrer sur le joueur

    //Camera Limits
    float minX;
    float maxX;
    float halfSizeX;
    float minY;
    float maxY;
    float halfSizeY;

    //Variables
    [Header("Variables")]
    [SerializeField] public float camSpeed;                 //Vitesse de la cam�ra vers le Focus
    Vector3 posSelf;                                        //Pour modifier la position de la cam�ra

    //Shortcut
    Camera cam;

    [SerializeField] LayerMask onClickLayers;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
        minX = limiteHG.position.x;
        maxX = limiteBD.position.x;
        minY = limiteBD.position.y;
        maxY = limiteHG.position.y;
        posSelf = transform.position;
        cam.eventMask = onClickLayers;
    }

    //Pour le mouvement de la cam�ra
    void FixedUpdate()
    {
        //Gestion des limites et du mouvement de la cam�ra sur x
        halfSizeX = cam.orthographicSize * cam.aspect;
        posSelf.x = Mathf.Lerp(transform.position.x, focus.position.x, camSpeed * Time.deltaTime);
        posSelf.x = Mathf.Clamp(posSelf.x, minX + halfSizeX, maxX - halfSizeX);

        //Gestion des limites et du mouvement de la cam�ra sur y
        halfSizeY = cam.orthographicSize;
        posSelf.y = Mathf.Lerp(transform.position.y, focus.position.y, camSpeed * Time.deltaTime);
        posSelf.y = Mathf.Clamp(posSelf.y, minY + halfSizeY, maxY - halfSizeY);

        transform.position = posSelf;
    }

    // Update Camera bounds to fit next room
    public void UpdateCameraLimits(Transform limitTopLeft, Transform limitBottomRight)
    {
        if(limitBottomRight != null && limitTopLeft != null)
        {
            limiteHG = limitTopLeft;
            limiteBD = limitBottomRight;

            minX = limiteHG.position.x;
            maxX = limiteBD.position.x;
            minY = limiteBD.position.y;
            maxY = limiteHG.position.y;
        }
    }
}
