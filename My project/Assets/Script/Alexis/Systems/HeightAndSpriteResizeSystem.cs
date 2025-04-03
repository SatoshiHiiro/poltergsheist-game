using UnityEngine;

[ExecuteInEditMode]
public class HeightAndSpriteResizeSystem : MonoBehaviour
{
    Collider2D col2D;
    Transform height;
    Transform sprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        height = this.transform;
        col2D = height.parent.GetComponent<Collider2D>();
        sprite = height.GetChild(0).transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            Vector3 temp = new Vector3(0, -col2D.bounds.extents.y/col2D.transform.localScale.y, 0);
            height.localPosition = temp;
            sprite.localPosition = -temp;
        }
    }
}
