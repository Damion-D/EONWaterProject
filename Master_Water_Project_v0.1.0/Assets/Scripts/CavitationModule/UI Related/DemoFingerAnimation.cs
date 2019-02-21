//Writers: Levin & Xingrong

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DemoFingerAnimation : MonoBehaviour {

    [Header("References")]
    public Sprite tappingFinger;
    public Sprite movingFinger;

    [Header("Settings")]
    public float speed;

    [HideInInspector] public bool complete = false;

    private RectTransform trans;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
        trans = GetComponent<RectTransform>();
    }

    //Contols the entire animation and sprite changes for the finger that demos the slider in normal operation
    public IEnumerator MovingAnimation()
    {
        //Only loops if the finger still exists
        while (!complete)
        {
            yield return new WaitForSeconds(0.5f);
            if (complete) yield break; //Is used to exit the function if the finger is destroyed mid animation
            trans.anchorMin = new Vector2(0.62f, trans.anchorMin.y); //Moves the finger side to side
            trans.anchorMax = new Vector2(1.02f, trans.anchorMax.y);
            image.sprite = tappingFinger; //Updates the sprite

            yield return new WaitForSeconds(0.5f); //Similar to block above
            if (complete) yield break;
            trans.anchorMin = new Vector2(0.6f, trans.anchorMin.y);
            trans.anchorMax = new Vector2(1.0f, trans.anchorMax.y);
            image.sprite = movingFinger;

            yield return new WaitForSeconds(0.5f);
            if (complete) yield break;
            trans.anchorMin = new Vector2(0.62f, trans.anchorMin.y);
            trans.anchorMax = new Vector2(1.02f, trans.anchorMax.y);
            image.sprite = tappingFinger;

            yield return new WaitForSeconds(0.4f);
            if (complete) yield break;
            image.sprite = movingFinger;

            //Moves the finger towards the top of the screen until it reaches a certain point and as long as the finger hasn't been deleted mid animation cycle
            while (!complete && trans.anchorMin.y < 0.65f && trans.anchorMax.y < 1.05f)
            {
                trans.anchorMin = new Vector2(trans.anchorMin.x, trans.anchorMin.y + Time.deltaTime * speed); //Affects up and down movement of the finger
                trans.anchorMax = new Vector2(trans.anchorMax.x, trans.anchorMax.y + Time.deltaTime * speed);
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
            if (complete) yield break;
            trans.anchorMin = new Vector2(0.6f, trans.anchorMin.y);
            trans.anchorMax = new Vector2(1.0f, trans.anchorMax.y);
            image.sprite = movingFinger;

            yield return new WaitForSeconds(0.4f);
            if (complete) yield break;
            trans.anchorMin = new Vector2(trans.anchorMin.x, -0.1f);
            trans.anchorMax = new Vector2(trans.anchorMax.x, 0.3f);
            yield return null;
        }
    }
}