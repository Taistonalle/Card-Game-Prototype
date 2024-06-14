using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCanvas : MonoBehaviour {
    [SerializeField] float duration;
    [SerializeField] float timeBeforeSecondBlink;


    CanvasGroup canvasGrp;

    void Start() {
        canvasGrp = GetComponent<CanvasGroup>();
    }

    public IEnumerator Fade() {
        float timer = 0f;

        //First darkening blink
        while (timer < duration) {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            yield return canvasGrp.alpha = Mathf.Lerp(0f, 1f, t);
        }
        canvasGrp.alpha = 1f;

        yield return new WaitForSeconds(timeBeforeSecondBlink);

        timer = 0f;
        //Second clearing blink
        while (timer < duration) {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            yield return canvasGrp.alpha = Mathf.Lerp(1f, 0f, t);
        }
        canvasGrp.alpha = 0f;
    }
}
