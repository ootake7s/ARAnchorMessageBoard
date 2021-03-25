using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;

public class DeleteDialogOkCancel : MonoBehaviour
{
    public enum DialogResult
    {
        OK,
        Cancel,
    }

    public Action<DialogResult> FixDialog { get; set;}

    public void OnOK()
    {
        this.FixDialog?.Invoke(DialogResult.OK);
        //Debug.Log("##### OK CLICK");

        AnchorsMsgBoard.SetDelete();
        AnchorsMsgBoard.DialogClose(true);
        Destroy(this.gameObject);
    }

    public void OnCancel()
    {
        //MainManager.FormDeActive();

        this.FixDialog?.Invoke(DialogResult.Cancel);
        AnchorsMsgBoard.DialogClose(false);
        Destroy(this.gameObject);
    }

    public void OnContinue()
    {
        Debug.Log("##### 何もしない");
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
