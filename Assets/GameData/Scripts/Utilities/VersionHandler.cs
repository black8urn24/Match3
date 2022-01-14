using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VersionHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"Applicaiton Version - {Application.version}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
