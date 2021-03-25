using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;

public class SelectCell : MonoBehaviour
{
    //[SerializeField] private GameObject SelectCellPanel;

    private int _itemNo;

    public int ItemNo {
        set
        {
            _itemNo = value;
        }
        get
        {
            return _itemNo;
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

    public void OnClick()
    {
        DataManager.Instance.PanelColorClear();
        DataManager.Instance.ChangePanelColor(_itemNo);
//        SelectCellPanel.GetComponent<Image>().color = new Color32(75,255,225,255);
        AnchorsMsgBoard.SetAnchorItemNo(_itemNo);
        Debug.Log("_itemNo=" + _itemNo);
    }
}
