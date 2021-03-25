using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;

public class MenuBtnClick : MonoBehaviour
{
    [SerializeField] private Text textDebug;

    // Start is called before the first frame update
    void Start()
    {
        textDebug.text = PlayerPrefs.GetString("MailAddress")
                    + "\n" + PlayerPrefs.GetString("HandleName")
                    + "\n作成数：" + PlayerPrefs.GetInt("MessageBoardCount");
                    //+ "\n" + PlayerPrefs.GetString("UserID");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCreateClick()
    {
        Sound.Instance.SoundBtnClick();

        AnchorsMsgBoard.procNumber = 1;
        SceneManager.LoadScene("MsgBoardScene");
    }

    public void OnSearchClick()
    {
        Sound.Instance.SoundBtnClick();

        AnchorsMsgBoard.procNumber = 2;
        SceneManager.LoadScene("MsgBoardScene");
    }
}
