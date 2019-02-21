//Writer: Alec

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flow : MonoBehaviour {


    public float scrollSpeed = 0.5F;
    public float currentSpeed = 1f;
    public Renderer rend;
    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    void Update()
    {
        float offset = Time.time * scrollSpeed * (currentSpeed+.1f);
        rend.material.mainTextureOffset = new Vector2(0, offset);
    }
}
