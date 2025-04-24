using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class FireAnimationBehavior : MonoBehaviour
{
    SpriteRenderer sprite;
    Light2D fire; 
    Coroutine fireAnim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sprite = this.GetComponent<SpriteRenderer>();
        fire = this.GetComponent<Light2D>();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        fireAnim = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (fireAnim == null) { fireAnim = StartCoroutine(FireAnim()); }
    }

    IEnumerator FireAnim()
    {
        float animTime = Random.Range(.25f, .5f);
        float endTime = Time.time + animTime;
        float targetFalloff = Random.Range(.5f, .8f);
        float sizeDiff = Mathf.Abs(fire.shapeLightFalloffSize - targetFalloff) / animTime;

        while (Time.time <= endTime)
        {
            float value = Mathf.MoveTowards(fire.shapeLightFalloffSize, targetFalloff, sizeDiff * Time.deltaTime);
            fire.shapeLightFalloffSize = value;
            fire.intensity = value + 3.5f;
            yield return null;
        }

        //yield return new WaitForSecondsRealtime(animTime);
        sprite.flipX = !sprite.flipX;
        fireAnim = null;
    }
}
