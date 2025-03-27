using UnityEngine;

public class BreakableObject : SoundDetection
{
    [SerializeField] private GameObject brokenObject;
    private GameObject hiddenGameObject;
    private SpriteRenderer spriteRenderer;
    private Collider2D objCollider;
    private Rigidbody2D rb;
    private LayerMask floorLayer;

    protected void Awake()
    {

    }
    protected override void Start()
    {
        base.Start();
        objectType = SoundEmittingObject.BreakableObject;
        floorLayer = LayerMask.NameToLayer("Floor");
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        objCollider = gameObject.GetComponent<Collider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        hiddenGameObject = gameObject.transform.GetChild(0).gameObject;
        hiddenGameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == floorLayer)
        {
            BreakObject();
        }
    }
    private void BreakObject()
    {
        audioSource.Play();
        // If there is an hidden object show it
        if (hiddenGameObject != null)
        {
            hiddenGameObject.transform.SetParent(null);
            hiddenGameObject.SetActive(true);
            hiddenGameObject.transform.rotation = Quaternion.identity;
        }
        // Instantiate the broken object
        if (brokenObject != null)
        {
            Instantiate(brokenObject, transform.position, Quaternion.identity);            
        }
        spriteRenderer.sprite = null;
        objCollider.isTrigger = true;
        rb.bodyType = RigidbodyType2D.Static;
        NotifyNearbyEnemies(this);
    }
}
