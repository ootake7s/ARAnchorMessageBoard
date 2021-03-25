using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;

public class ErrorManager : MonoBehaviour
{
    public int errProcNumber;
    public string errProc;
    public string errProcDetail;
    public string errMessage;

    [SerializeField] private Text textMessage;

    private static ErrorManager instance;
    public static ErrorManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ErrorManager>();
            }
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        errProcNumber = AnchorsMsgBoard.procNumber;
        errProc = AnchorsMsgBoard.errProc;
        errProcDetail = AnchorsMsgBoard.errDetail;
        errMessage = AnchorsMsgBoard.errMessage;

        string procName = "";
        if (errProcNumber == 1)
        {
            procName = "Message Board 作成";
        }
        else {
            procName = "Message Board 検索";
        }
        textMessage.text = procName + "\n" + errProc + "\n" + errProcDetail + "\n" + errMessage + "\n";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
