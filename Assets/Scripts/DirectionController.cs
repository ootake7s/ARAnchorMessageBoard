using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;

public class DirectionController : MonoBehaviour
{
    private RectTransform rectTran;

    // Start is called before the first frame update
    void Start()
    {
        rectTran = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rectTran.LookAt(Camera.main.transform);        
    }
}
