using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class HeightAndSpriteResizeSystem : MonoBehaviour
{
    Collider2D col2D;
    Transform height;
    Transform sprite;

    [HideInInspector] public Vector3 heightPos;

    void OnEnable()
    {
        StartCoroutine(OnEnableRelated());
    }

    IEnumerator OnEnableRelated()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        heightPos = new Vector3(0, -col2D.bounds.extents.y / col2D.transform.localScale.y, 0);
    }

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
            heightPos = new Vector3(0, -col2D.bounds.extents.y/col2D.transform.localScale.y, 0);
            height.localPosition = heightPos;
            sprite.localPosition = -heightPos;
        }
    }
}
