using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    //Gameobject autres
    [SerializeField] public Transform limiteHG;              //Pour limiter la cam�ra � un point haut-gauche
    [SerializeField] public Transform limiteBD;              //Pour limiter la cam�ra � un point bas-droit
    [SerializeField] public Transform focus;                 //Pour centrer sur le joueur

    //Camera Limits
    float minX;
    float maxX;
    float halfSizeX;
    float minY;
    float maxY;
    float halfSizeY;

    //Variables
    Vector3 posSelf;                                        //Pour modifier la position de la cam�ra
    [SerializeField] public float camSpeed;                 //Vitesse de la cam�ra vers le Focus

    //Shortcut
    Camera cam;

    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
        minX = limiteHG.position.x;
        maxX = limiteBD.position.x;
        minY = limiteBD.position.y;
        maxY = limiteHG.position.y;
        posSelf = transform.position;
    }

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
}
