using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;

public class InputDialogOkCancel : MonoBehaviour
{
    private string inputValue = "";

    public enum DialogResult
    {
        OK,
        Cancel,
    }

    public Action<DialogResult> FixDialog { get; set;}

    public void OnOK()
    {
        if (inputValue == "")
        {
            return;
        }

        this.FixDialog?.Invoke(DialogResult.OK);
        Debug.Log("##### OK CLICK");

        AnchorsMsgBoard.SetInputMessage(inputValue);
        AnchorsMsgBoard.DialogClose(true);
        Destroy(this.gameObject);
    }

    public void OnCancel()
    {
        //MainManager.FormDeActive();

        this.FixDialog?.Invoke(DialogResult.Cancel);
        Debug.Log("##### CANCEL CLICK");
        //Destroy(inputDialogContainer);
        AnchorsMsgBoard.DialogClose(false);
        Destroy(this.gameObject);
    }

    public void OnContinue()
    {
        Debug.Log("##### 何もしない");
    }

    public void OnEndEdit()
    {
        InputField[] inputMsg = this.GetComponentsInChildren<InputField>();
        for (int i = 0; i < inputMsg.Length; i++)
        {
            Debug.Log(inputMsg[i].name);
            if (inputMsg[i].name == "InputMessage")
            {
                inputValue = inputMsg[i].text;
                i = inputMsg.Length;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
