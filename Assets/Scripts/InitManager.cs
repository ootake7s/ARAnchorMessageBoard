using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InitManager : MonoBehaviour
{
    private string url_nm = "https://shinbashi.sevenseas.tokyo/anchor/db/userinfo/chkadd/";
    private string url_ip = "https://160.16.137.115/anchor/db/userinfo/chkadd/";

    private string chkurl = "https://shinbashi.sevenseas.tokyo/anchor/db/userinfo/findmail/";

    [SerializeField] private GameObject inputHandleNameObj;
    [SerializeField] private InputField inputHandleName;
    [SerializeField] private InputField inputMailAddress;
    [SerializeField] private Text textMessage;
    [SerializeField] private GameObject panelPre;
    [SerializeField] private GameObject panelInput;
    [SerializeField] private GameObject check;

    void Awake()
    {
        if(PlayerPrefs.HasKey("MailAddress"))
        {
            string handleName = PlayerPrefs.GetString("HandleName");
            string mailAddress = PlayerPrefs.GetString("MailAddress");
            StartCoroutine(UserCreate(handleName, mailAddress));
        }
        else {
            panelPre.SetActive(false);
            panelInput.SetActive(true);
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

    public void OnMailAddressEnd()
    {
        //if (inputMailAddress.touchScreenKeyboard.status == TouchScreenKeyboard.Status.Canceled)
        if (inputMailAddress.touchScreenKeyboard.status == TouchScreenKeyboard.Status.Done)
        {
            StartCoroutine(CheckMailAddress());
        }
    }

    public void OnUserCreateClick()
    {
        Sound.Instance.SoundBtnClick();

        if (!InputCheck())
        {
            return;
        }

        StartCoroutine(UserCreate(inputHandleName.text, inputMailAddress.text));
    }

    private bool InputCheck()
    {
        textMessage.text = "";

        if (inputHandleName.text == "")
        {
            textMessage.text = "ハンドル名を入力してください。";
            return false;
        }
        if (inputMailAddress.text == "")
        {
            textMessage.text = "メールアドレスを入力してください。";
            return false;
        }
        return true;
    }

    private IEnumerator UserCreate(string handleName, string mailaddress) {
        var url = "";
    #if UNITY_ANDROID || UNITY_IOS
        url = url_nm;
    #else
        url = url_ip;
    #endif

        //Debug.Log("UserCreate name=" + handleName + ",mail=" + mailaddress);
        WWWForm form = new WWWForm();
        form.AddField ("handle_name", handleName);
        form.AddField ("mail_address", mailaddress);

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            textMessage.text = request.error;
            //Debug.Log("*** request ERROR " + request.error);
        }
        else {
            if (request.responseCode == 200)
            {
                string jsonText = request.downloadHandler.text;
                //Debug.Log("data=" + jsonText);

                UserInfoData userInfoData = new UserInfoData();
                var json = "{" + $"\"root\": {jsonText}" + "}";
                //Debug.Log("json=" + json);
                JsonUtility.FromJsonOverwrite(json, userInfoData);

                //Debug.Log("userInfoData=" + userInfoData.root._id + "," + userInfoData.root.handle_name + "," + userInfoData.root.mail_address + "," + userInfoData.root.message_board_count);

                PlayerPrefs.SetString("HandleName", userInfoData.root.handle_name);
                PlayerPrefs.SetString("MailAddress", userInfoData.root.mail_address);
                PlayerPrefs.SetInt("MessageBoardCount", userInfoData.root.message_board_count);
                PlayerPrefs.SetString("UserID", userInfoData.root._id);

                SceneManager.LoadScene("MenuScene");
            }
            else {
                textMessage.text = "request.responseCode: " + request.responseCode;
                //Debug.Log("*** responseCode NOT 200 " + request.error);
            }
        }
    }

    private IEnumerator CheckMailAddress() {
        inputHandleNameObj.SetActive(false);
        check.SetActive(false);
        textMessage.text = "";

        var url = chkurl;

        WWWForm form = new WWWForm();
        form.AddField ("mail_address", inputMailAddress.text);

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            textMessage.text = request.error;
        }
        else {
            if (request.responseCode == 200)
            {
                string jsonText = request.downloadHandler.text;
                //Debug.Log("jsonText=" + jsonText);
                if (jsonText != null)
                {
                    UserInfoData userInfoData = new UserInfoData();
                    var json = "{" + $"\"root\": {jsonText}" + "}";
                    //Debug.Log("json=" + json);
                    JsonUtility.FromJsonOverwrite(json, userInfoData);
                    inputHandleName.text = userInfoData.root.handle_name;
                    inputHandleNameObj.SetActive(true);

                    if (inputHandleName.text != "")
                    {
                        inputHandleName.readOnly = false;
                        inputHandleName.interactable = false;
                    }
                    else {
                        inputHandleName.readOnly = false;
                        inputHandleName.interactable = true;
                    }
                    check.SetActive(true);
                }
            }
            else {
                textMessage.text = "request.responseCode: " + request.responseCode;
            }
        }
    }

}

[System.Serializable]
public class UserInfoModel
{
    public string _id;
    public string handle_name;
    public string mail_address;
    public int message_board_count;
    public string create_date;
    public string update_date;
    public int __v;
}

[System.Serializable]
public class UserInfoData
{
    public UserInfoModel root;
}
