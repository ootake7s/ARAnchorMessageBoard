using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;

public class DataManager : MonoBehaviour
{
    private Sprite [] sprites;
    private List<string> textList = new List<string>();
    private List<GameObject> cellPanelList = new List<GameObject>();

    [SerializeField] private RectTransform CellPrefab;
    [SerializeField] private GameObject Content;

    private PrefabInfoData prefabInfoData;

    private static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataManager>();
            }
            return instance;
        }
    }

    void Awake()
    {
        StartCoroutine(GetPrefabInfoData());
    }

    // Start is called before the first frame update
    void Start()
    {
        sprites = Resources.LoadAll<Sprite>("Textures");
        foreach (var sprite in sprites)
        {
            textList.Add(sprite.name);
        }

        SetCanvasList();
    }
    private void SetCanvasList()
    {
        int ix = 0;
        while (ix < sprites.Length)
        {
            var cell = Instantiate(CellPrefab) as RectTransform;
            cell.SetParent(Content.transform, false);

            var buttonCell = cell.GetComponentInChildren<Button>();
            buttonCell.GetComponent<Image>().sprite = sprites[ix];

            SelectCell sc = buttonCell.GetComponent<SelectCell>();
            sc.ItemNo = ix;

            var textCell = cell.GetComponentInChildren<Text>();
            textCell.text = textList[ix];

            cellPanelList.Add(cell.gameObject);

            ix++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PanelColorClear()
    {
        foreach (GameObject cell in cellPanelList)
        {
            cell.GetComponentInChildren<Image>().color = new Color32(255,255,225,100);
        }        
    }

    public void ChangePanelColor(int ix)
    {
        cellPanelList[ix].GetComponentInChildren<Image>().color = new Color32(75,255,225,255);
    }

    public string GetPrefabName(int ix)
    {
        return prefabInfoData.root[ix].name;
    }

    public string GetDisplay3dModel(int ix)
    {
        return prefabInfoData.root[ix].display_3dmodel;
    }

    public string GetPrefabNameByModelName(string modelName)
    {
        string prefabName = "";
        for (int i = 0; i < prefabInfoData.root.Length; i++)
        {
            if (prefabInfoData.root[i].display_3dmodel == modelName)
            {
                prefabName = prefabInfoData.root[i].name;
                i = prefabInfoData.root.Length;
            }
        }
        return prefabName;
    }

    public bool IsMsgBoardByPrefabName(string prefabName)
    {
        bool res = false;
        for (int i = 0; i < prefabInfoData.root.Length; i++)
        {
            if (prefabInfoData.root[i].name == prefabName && prefabInfoData.root[i].name == "CanvasAnchor")
            {
                res = true;
                i = prefabInfoData.root.Length;
            }
        }
        return res;
    }

    public bool IsMenuBoardByPrefabName(string prefabName)
    {
        bool res = false;
        for (int i = 0; i < prefabInfoData.root.Length; i++)
        {
            if (prefabInfoData.root[i].name == prefabName && (prefabInfoData.root[i].name == "CafeMenu" || prefabInfoData.root[i].name == "MenuBoard") )
            {
                res = true;
                i = prefabInfoData.root.Length;
            }
        }
        return res;
    }

    public string GetURL(string prefabName)
    {
        string url = "";
        for (int i = 0; i < prefabInfoData.root.Length; i++)
        {
            if (prefabInfoData.root[i].name == prefabName)
            {
                url = prefabInfoData.root[i].homepage_url;
                i = prefabInfoData.root.Length;
            }
        }
        return url;
    }

    public IEnumerator GetPrefabInfoData()
    {
        var url = "";
    #if UNITY_ANDROID || UNITY_IOS
        url = "https://shinbashi.sevenseas.tokyo/anchor/db/prefabinfo/";
    #else
        url = "https://160.16.137.115/anchor/db/prefabinfo/";
    #endif

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
            GoToErrorScene("API:" + url, "PrefabInfo Data request error", request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                string jsonText = request.downloadHandler.text;
                prefabInfoData = new PrefabInfoData();
                var json = "{" + $"\"root\": {jsonText}" + "}";
                //Debug.Log("json=" + json);
                JsonUtility.FromJsonOverwrite(json, prefabInfoData);
                //Debug.Log("*** data prefabInfoData[0]=" + prefabInfoData.root[0].name);
            }
            else {
                GoToErrorScene("API:" + url, "PrefabInfo Data request error", "responseCode:" + request.responseCode);
            }
        }
    }

    private void GoToErrorScene(string paraErrProc, string paraErrDetail, string paraErrMessage)
    {
        AnchorsMsgBoard.errProc = paraErrProc;
        AnchorsMsgBoard.errDetail = paraErrDetail;
        AnchorsMsgBoard.errMessage = paraErrMessage;
        SceneManager.LoadScene("ErrorScene");            
    }

}

[System.Serializable]
public class PrefabInfoModel
{
    public string _id;
    public string display_3dmodel;
    public string name;
    public string homepage_url;
    public string create_date;
    public string update_date;
    public int __v;
}

[System.Serializable]
public class PrefabInfoData
{
    public PrefabInfoModel[] root;
}
