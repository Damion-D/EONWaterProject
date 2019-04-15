using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDropScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(5, 5, 5);
        Destroy(gameObject, 1);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0.5f, -1, 0) / 50, Space.World);
    }
}