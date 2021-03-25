using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;

public class BoardTrigger : MonoBehaviour
{
    public EventTrigger _EventTrigger;
    public Text textAnchorId;
    public Text textBoardId;
    public Text textUserId;
    public Text textPrefabName;

    Coroutine PressCoroutine;
    bool isPressDown = false;
    float PressTime = 1f;

    void Awake()
    {
        EventTrigger.Entry pressdown = new EventTrigger.Entry();
        pressdown.eventID = EventTriggerType.PointerDown;
        pressdown.callback.AddListener((data) => PointerDown());
        _EventTrigger.triggers.Add(pressdown);

        EventTrigger.Entry pressup = new EventTrigger.Entry();
        pressup.eventID = EventTriggerType.PointerUp;
        pressup.callback.AddListener((data) => PointerUp());
        _EventTrigger.triggers.Add(pressup);
    }

    void PointerDown()
    {
        if (PressCoroutine != null)
        {
            StopCoroutine(PressCoroutine);
        }
        PressCoroutine = StartCoroutine(TimeForPointerDown());

    }

    IEnumerator TimeForPointerDown()
    {
        isPressDown = true;

        yield return new WaitForSeconds(PressTime);

        if (isPressDown)
        {
            // 削除可能か判定
            if (AnchorsMsgBoard.IsBoardDelete(textUserId.text))
            {
                AnchorsMsgBoard.DeleteDialogShow(textAnchorId.text, textBoardId.text);
            }
        }

        isPressDown = false;
    }

    void PointerUp()
    {
        if (isPressDown)
        {
            // 伝言板の場合
            if (DataManager.Instance.IsMsgBoardByPrefabName(textPrefabName.text) && AnchorsMsgBoard.IsMsgBoardInput())
            {
                AnchorsMsgBoard.InputDialogShow(textBoardId.text, textPrefabName.text);
            }

            // メニュー、メニューボードの場合
            if (DataManager.Instance.IsMenuBoardByPrefabName(textPrefabName.text))
            {
                if (AnchorsMsgBoard.procNumber == 1)
                {
                    if (AnchorsMsgBoard.IsMsgBoardInput())
                    {
                        AnchorsMsgBoard.InputDialogShow(textBoardId.text, textPrefabName.text);
                    }
                }
                else {
                    AnchorsMsgBoard.HomePageDialogShow(textPrefabName.text);
                }
            }
            isPressDown = false;
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
