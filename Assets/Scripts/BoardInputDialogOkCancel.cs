using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;

public class BoardInputDialogOkCancel : MonoBehaviour
{
    private string boardNameValue = "";
    private string messageValue = "";

    public enum DialogResult
    {
        OK,
        Cancel,
    }

    public Action<DialogResult> FixDialog { get; set;}

    public void OnOK()
    {
        this.FixDialog?.Invoke(DialogResult.OK);
        Debug.Log("##### BoardInputDialogOkCancel OK CLICK boardNameValue=" + boardNameValue + ",messageValue=" + messageValue);

        AnchorsMsgBoard.SetInputBoardMessage(boardNameValue, messageValue);
        AnchorsMsgBoard.DialogClose(true);
        Destroy(this.gameObject);
    }

    public void OnCancel()
    {
        this.FixDialog?.Invoke(DialogResult.Cancel);
        Debug.Log("##### CANCEL CLICK");
        AnchorsMsgBoard.DialogClose(false);
        Destroy(this.gameObject);
    }

    public void OnContinue()
    {
        Debug.Log("##### 何もしない");
    }

    public void OnBoardNameEndEdit()
    {
        InputField[] inputMsg = this.GetComponentsInChildren<InputField>();
        for (int i = 0; i < inputMsg.Length; i++)
        {
            Debug.Log("*** OnBoardNameEndEdit " + inputMsg[i].name);
            if (inputMsg[i].name == "InputBoardName")
            {
                boardNameValue = inputMsg[i].text;
                i = inputMsg.Length;
            }
        }
    }

    public void OnMessageEndEdit()
    {
        InputField[] inputMsg = this.GetComponentsInChildren<InputField>();
        for (int i = 0; i < inputMsg.Length; i++)
        {
            Debug.Log("*** OnMessageEndEdit " + inputMsg[i].name);
            if (inputMsg[i].name == "InputMessage")
            {
                messageValue = inputMsg[i].text;
                i = inputMsg.Length;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {        
        boardNameValue = AnchorsMsgBoard.inputBoardNameValue;
        messageValue = AnchorsMsgBoard.inputBoardMsgValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
