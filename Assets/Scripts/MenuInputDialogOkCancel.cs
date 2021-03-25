using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;

public class MenuInputDialogOkCancel : MonoBehaviour
{
    private string messageValue = "";

    public enum MenuDialogResult
    {
        OK,
        Cancel,
    }

    public Action<MenuDialogResult> FixDialog { get; set;}

    public void OnOK()
    {
        this.FixDialog?.Invoke(MenuDialogResult.OK);
        Debug.Log("##### MenuInputDialogOkCancel OK CLICK messageValue=" + messageValue);

        AnchorsMsgBoard.SetInputBoardMessage("", messageValue);
        AnchorsMsgBoard.DialogClose(true);
        Destroy(this.gameObject);
    }

    public void OnCancel()
    {
        this.FixDialog?.Invoke(MenuDialogResult.Cancel);
        Debug.Log("##### CANCEL CLICK");
        AnchorsMsgBoard.DialogClose(false);
        Destroy(this.gameObject);
    }

    public void OnContinue()
    {
        Debug.Log("##### 何もしない");
    }

    public void OnMessageEndEdit()
    {
        InputField[] inputMsg = this.GetComponentsInChildren<InputField>();
        for (int i = 0; i < inputMsg.Length; i++)
        {
            //Debug.Log("*** OnMessageEndEdit " + inputMsg[i].name);
            if (inputMsg[i].name == "InputMessage")
            {
                messageValue = inputMsg[i].text;
                i = inputMsg.Length;
            }
        }
    }

    public void OnTemp1()
    {
        messageValue = "<align=center><b>センタリング</b></align>\n"
            + "<color=green>緑色</color>\n"
            + "<size=150%>文字サイズ150%</size>\n"
            + "<u>下線付き</u>\n"
            + "これは<mark=#ffff00aa>マーク</mark>です。";
        InputField inputMsg = this.GetComponentInChildren<InputField>();
        inputMsg.text = messageValue;
    }

    public void OnTemp2()
    {
        messageValue = "<align=center><size=130%><b>本日のランチ</b></size></align>\n"
            + "<color=green>サラダプレート</color>  800円\n"
            + "<color=red>レッドカレー</color>    850円\n"
            + "<color=yellow>オムライス</color>      750円\n"
            + "<align=right><u>ランチはドリンク付き</u></align>";
        InputField inputMsg = this.GetComponentInChildren<InputField>();
        inputMsg.text = messageValue;
    }

    // Start is called before the first frame update
    void Start()
    {
        messageValue = AnchorsMsgBoard.inputBoardMsgValue;        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
