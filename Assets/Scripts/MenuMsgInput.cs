using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;

public class MenuMsgInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMessgaeEndEdit()
    {
        Text[] inputMsg = this.GetComponentsInChildren<Text>();
        for (int i = 0; i < inputMsg.Length; i++)
        {
            Debug.Log("*** OnMessgaeEndEdit " + inputMsg[i].name);
            if (inputMsg[i].name == "TextMessage")
            {
                AnchorsMsgBoard.SetInputBoardMessage("", inputMsg[i].text);
                i = inputMsg.Length;
            }
        }

    }
}
