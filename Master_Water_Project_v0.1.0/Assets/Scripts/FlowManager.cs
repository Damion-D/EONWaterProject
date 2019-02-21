//Writer: Alec

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowManager : MonoBehaviour {

    [SerializeField]
    public float currentBlendVal;
    SkinnedMeshRenderer skinnedMeshRenderer;
    ParticleSystem.EmissionModule emissionModule;

    public ParticleSystem bubbles;

    public float scrollSpeed = 0.5F;
    public float currentSpeed = 1f;
    public Renderer rend;
    public bool flashing;
    public bool trend;

    // Use this for initialization
    void Start () {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        emissionModule = bubbles.emission;
        rend = GetComponent<Renderer>();

    }
	
	// Update is called once per frame
	void Update () {
        currentBlendVal = currentSpeed * 100;
        emissionModule.rateOverTime = currentBlendVal*20;

        if (trend)skinnedMeshRenderer.SetBlendShapeWeight(0, currentBlendVal);

        float offset = Time.time * scrollSpeed * (currentSpeed + .1f);
        rend.material.mainTextureOffset = new Vector2(0, offset);

        if (!bubbles.isPlaying && flashing)
        {
            bubbles.Play();
        } else if (!flashing)
        {
            bubbles.Stop();
        }

		
	}
}
