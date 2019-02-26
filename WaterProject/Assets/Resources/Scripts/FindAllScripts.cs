using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindAllScripts : MonoBehaviour
{
    [SerializeField] private List<Transform> allObjects;
    [Space]
    [Space]
    [Space]
    [Space]
    [SerializeField]
    private List<MonoBehaviour> allScripts;

    // Start is called before the first frame update
    void Start()
    {
        Transform[] tempObjects = FindObjectsOfType<Transform>();
        MonoBehaviour[] tempBehaviors;
        for (int i = 0; i < tempObjects.Length; i++)
        {
            if (tempObjects[i].gameObject.activeInHierarchy)
                allObjects.Add(tempObjects[i]);
        }

        for (int i = 0; i < allObjects.Count; i++)
        {
            tempBehaviors = allObjects[i].GetComponents<MonoBehaviour>();
            for (int k = 0; k < tempBehaviors.Length; k++)
            {
                allScripts.Add(tempBehaviors[k]);
            }
        }
    }
}
