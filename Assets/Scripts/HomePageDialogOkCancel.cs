using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;

public class HomePageDialogOkCancel : MonoBehaviour
{
    public enum DialogResult
    {
        OK,
        Cancel,
    }

    public Action<DialogResult> FixDialog { get; set;}

    public string prefabName = "";

    public void OnOK()
    {
        this.FixDialog?.Invoke(DialogResult.OK);

        string url = DataManager.Instance.GetURL(prefabName);
        Application.OpenURL(url);
        AnchorsMsgBoard.DialogClose(true);
        Destroy(this.gameObject);
    }

    public void OnCancel()
    {
        this.FixDialog?.Invoke(DialogResult.Cancel);
        AnchorsMsgBoard.DialogClose(false);
        Destroy(this.gameObject);
    }

    public void OnContinue()
    {
        //Debug.Log("##### 何もしない");
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
