using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

namespace Microsoft.Azure.SpatialAnchors.Unity.Examples
{

    public class AnchorsMsgBoard : DemoScriptBase
    {
        public static int procNumber = 0;   // 1:Create 2:Search

        internal enum AppState
        {
            DemoStepCreateSession = 0,
            DemoStepConfigSession,
            DemoStepStartSession,
            DemoStepCreateLocationProvider,
            DemoStepConfigureSensors,
            DemoStepCreateLocalAnchor,
            DemoStepSaveCloudAnchor,
            DemoStepSavingCloudAnchor,
            DemoStepStopSession,
            DemoStepDestroySession,
            DemoStepCreateSessionForQuery,
            DemoStepStartSessionForQuery,
            DemoStepLookForAnchorsNearDevice,
            DemoStepLookingForAnchorsNearDevice,
            DemoStepStopWatcher,
            DemoStepStopSessionForQuery,
            DemoStepComplete,
            DemoStepCreateStart,
            DemoStepCreateExpiration,
//            DemoStepCreateInput,  //DemoStepCreateAnchor,
//            DemoStepCreateEndInput,
            DemoStepCreateSave,
            DemoStepCreateExit,
//            DemoStepCreatePicture,
            DemoStepSeachStart,
            DemoStepSeachStop
        }

        private readonly Dictionary<AppState, DemoStepParams> stateParams = new Dictionary<AppState, DemoStepParams>
        {
            { AppState.DemoStepCreateSession,new DemoStepParams() { StepMessage = "Next: Create Azure Spatial Anchors Session", StepColor = Color.clear }},
            { AppState.DemoStepConfigSession,new DemoStepParams() { StepMessage = "Next: Configure Azure Spatial Anchors Session", StepColor = Color.clear }},
            { AppState.DemoStepStartSession,new DemoStepParams() { StepMessage = "Next: Start Azure Spatial Anchors Session", StepColor = Color.clear }},
            { AppState.DemoStepCreateLocationProvider,new DemoStepParams() { StepMessage = "Next: Create Location Provider", StepColor = Color.clear }},
            { AppState.DemoStepConfigureSensors,new DemoStepParams() { StepMessage = "Next: Configure Sensors", StepColor = Color.clear }},
            { AppState.DemoStepCreateLocalAnchor,new DemoStepParams() { StepMessage = "物を置く場所にタップしてください。", StepColor = Color.blue }},
            { AppState.DemoStepSaveCloudAnchor,new DemoStepParams() { StepMessage = "Next: Save Local Anchor to cloud", StepColor = Color.yellow }},
            { AppState.DemoStepSavingCloudAnchor,new DemoStepParams() { StepMessage = "ローカルアンカーをクラウドに保存します...", StepColor = Color.yellow }},
            { AppState.DemoStepStopSession,new DemoStepParams() { StepMessage = "Next: Stop Azure Spatial Anchors Session", StepColor = Color.green }},
            { AppState.DemoStepCreateSessionForQuery,new DemoStepParams() { StepMessage = "Next: Create Azure Spatial Anchors Session for query", StepColor = Color.clear }},
            { AppState.DemoStepStartSessionForQuery,new DemoStepParams() { StepMessage = "Next: Start Azure Spatial Anchors Session for query", StepColor = Color.clear }},
            { AppState.DemoStepLookForAnchorsNearDevice,new DemoStepParams() { StepMessage = "OKボタン押下で、デバイスの近くのアンカーを探します。", StepColor = Color.clear }},
            { AppState.DemoStepLookingForAnchorsNearDevice,new DemoStepParams() { StepMessage = "メッセージボードを探しています...", StepColor = Color.yellow }},
            { AppState.DemoStepStopWatcher,new DemoStepParams() { StepMessage = "Next: Stop Watcher", StepColor = Color.yellow }},
            { AppState.DemoStepStopSessionForQuery,new DemoStepParams() { StepMessage = "Next: Stop Azure Spatial Anchors Session for query", StepColor = Color.grey }},
            { AppState.DemoStepComplete,new DemoStepParams() { StepMessage = "Next: Restart demo", StepColor = Color.clear }},
            { AppState.DemoStepCreateStart,new DemoStepParams() { StepMessage = "作る物を選択したらチェックしてください。", StepColor = Color.clear }},
            { AppState.DemoStepCreateExpiration,new DemoStepParams() { StepMessage = "有効期限日数を入力したらチェックしてください。", StepColor = Color.clear }},
            //{ AppState.DemoStepCreateAnchor,new DemoStepParams() { StepMessage = "平面をタッチしてアンカーを置きます。", StepColor = Color.blue }},
            //{ AppState.DemoStepCreateInput,new DemoStepParams() { StepMessage = "伝言板のメッセージを入力してください。", StepColor = Color.blue }},
            //{ AppState.DemoStepCreateEndInput,new DemoStepParams() { StepMessage = "入力内容を伝言板に反映します。", StepColor = Color.blue }},
            { AppState.DemoStepCreateSave,new DemoStepParams() { StepMessage = "をタップすると入力できます。\n登録する時はチェックしてください。", StepColor = Color.yellow }},
            { AppState.DemoStepCreateExit,new DemoStepParams() { StepMessage = "登録が完了しました\n写真を撮りましょう", StepColor = Color.green }},
            //{ AppState.DemoStepCreatePicture,new DemoStepParams() { StepMessage = "写真のアップロードが完了しました。", StepColor = Color.green }},
            { AppState.DemoStepSeachStart,new DemoStepParams() { StepMessage = "検索を開始します。\nチェックしてください。", StepColor = Color.clear }},
            { AppState.DemoStepSeachStop,new DemoStepParams() { StepMessage = "検索が完了しました", StepColor = Color.yellow }}
        };

//        private AppState _currentAppState = AppState.DemoStepCreateSession;
        private AppState _currentAppState = 0;

        AppState currentAppState
        {
            get
            {
                return _currentAppState;
            }
            set
            {
                if (_currentAppState != value)
                {
                    Debug.LogFormat("State from {0} to {1}", _currentAppState, value);
                    _currentAppState = value;
                    if (spawnedObjectMat != null)
                    {
                        spawnedObjectMat.color = stateParams[_currentAppState].StepColor;
                    }

                    if (!isErrorActive)
                    {
                        string msg = stateParams[_currentAppState].StepMessage;
                        if (_currentAppState == AppState.DemoStepCreateSave)
                        {
                            if (anchorItemNo == 0)
                            {
                                msg = "チョーク" + msg;
                            }
                            else {
                                msg = "ボード" + msg;
                            }
                        }
                        feedbackBox.text = msg;     //stateParams[_currentAppState].StepMessage;
                    }
                    EnableCorrectUIControls();
                }
            }
        }

        private PlatformLocationProvider locationProvider;
        private List<GameObject> allDiscoveredAnchors = new List<GameObject>();
        private Dictionary<string, CloudSpatialAnchor> listCloudSpatialAnchor = new Dictionary<string, CloudSpatialAnchor>();
        
        private void EnableCorrectUIControls()
        {
            int nextButtonIndex = 0;
            int enumerateButtonIndex = 2;

            switch (currentAppState)
            {
                case AppState.DemoStepCreateLocalAnchor:
                case AppState.DemoStepSavingCloudAnchor:
                case AppState.DemoStepLookingForAnchorsNearDevice:
                #if WINDOWS_UWP || UNITY_WSA
                    // Sample disables "Next step" button on Hololens, so it doesn't overlay with placing the anchor and async operations, 
                    // which are not affected by user input.
                    // This is also part of a workaround for placing anchor interaction, which doesn't receive callback when air tapping for placement
                    // This is not applicable to Android/iOS versions.
                    XRUXPicker.Instance.GetDemoButtons()[nextButtonIndex].gameObject.SetActive(false);
                #else
                    if (currentAppState == AppState.DemoStepCreateLocalAnchor
                        || currentAppState == AppState.DemoStepLookingForAnchorsNearDevice)
                    {
                        XRUXPicker.Instance.GetDemoButtons()[nextButtonIndex].gameObject.SetActive(false);
                    }
                #endif
                    break;
                case AppState.DemoStepStopSessionForQuery:
                case AppState.DemoStepSeachStop:
                //case AppState.DemoStepCreatePicture:
                    //XRUXPicker.Instance.GetDemoButtons()[enumerateButtonIndex].gameObject.SetActive(true);
                    XRUXPicker.Instance.GetDemoButtons()[nextButtonIndex].gameObject.SetActive(false);
                    XRUXPicker.Instance.GetDemoButtons()[enumerateButtonIndex].gameObject.SetActive(false);
                    break;
                case AppState.DemoStepCreateExit:
                    XRUXPicker.Instance.GetDemoButtons()[nextButtonIndex].gameObject.GetComponent<Image>().sprite = ImageCamera;
                    XRUXPicker.Instance.GetDemoButtons()[nextButtonIndex].gameObject.SetActive(true);       
                    XRUXPicker.Instance.GetDemoButtons()[enumerateButtonIndex].gameObject.SetActive(false);
                    break;
                default:
                    XRUXPicker.Instance.GetDemoButtons()[nextButtonIndex].gameObject.SetActive(true);
                    XRUXPicker.Instance.GetDemoButtons()[enumerateButtonIndex].gameObject.SetActive(false);
                    break;
            }
        }

        public SensorStatus GeoLocationStatus
        {
            get
            {
                if (locationProvider == null)
                    return SensorStatus.MissingSensorFingerprintProvider;
                if (!locationProvider.Sensors.GeoLocationEnabled)
                    return SensorStatus.DisabledCapability;
                switch (locationProvider.GeoLocationStatus)
                {
                    case GeoLocationStatusResult.Available:
                        return SensorStatus.Available;
                    case GeoLocationStatusResult.DisabledCapability:
                        return SensorStatus.DisabledCapability;
                    case GeoLocationStatusResult.MissingSensorFingerprintProvider:
                        return SensorStatus.MissingSensorFingerprintProvider;
                    case GeoLocationStatusResult.NoGPSData:
                        return SensorStatus.NoData;
                    default:
                        return SensorStatus.MissingSensorFingerprintProvider;
                }
            }
        }

        public SensorStatus WifiStatus
        {
            get
            {
                if (locationProvider == null)
                    return SensorStatus.MissingSensorFingerprintProvider;
                if (!locationProvider.Sensors.WifiEnabled)
                    return SensorStatus.DisabledCapability;
                switch (locationProvider.WifiStatus)
                {
                    case WifiStatusResult.Available:
                        return SensorStatus.Available;
                    case WifiStatusResult.DisabledCapability:
                        return SensorStatus.DisabledCapability;
                    case WifiStatusResult.MissingSensorFingerprintProvider:
                        return SensorStatus.MissingSensorFingerprintProvider;
                    case WifiStatusResult.NoAccessPointsFound:
                        return SensorStatus.NoData;
                    default:
                        return SensorStatus.MissingSensorFingerprintProvider;
                }
            }
        }

        public SensorStatus BluetoothStatus
        {
            get
            {
                if (locationProvider == null)
                    return SensorStatus.MissingSensorFingerprintProvider;
                if (!locationProvider.Sensors.BluetoothEnabled)
                    return SensorStatus.DisabledCapability;
                switch (locationProvider.BluetoothStatus)
                {
                    case BluetoothStatusResult.Available:
                        return SensorStatus.Available;
                    case BluetoothStatusResult.DisabledCapability:
                        return SensorStatus.DisabledCapability;
                    case BluetoothStatusResult.MissingSensorFingerprintProvider:
                        return SensorStatus.MissingSensorFingerprintProvider;
                    case BluetoothStatusResult.NoBeaconsFound:
                        return SensorStatus.NoData;
                    default:
                        return SensorStatus.MissingSensorFingerprintProvider;
                }
            }
        }

        [SerializeField] private CloudNativeAnchor cloudNativeAnchor;
        public static string inputBoardNameValue = "";
        public static string inputBoardMsgValue = "";
        private static string currentAnchorId = "";
        private static string currentMsgBoardId = "";
        private static string currentPrefabName = "";
        [SerializeField] private RectTransform cellPrefab;
        [SerializeField] private GameObject scrollView; // アンカー選択
        private static int anchorItemNo = 0;    // 選択したアンカー物体のNo
        [SerializeField] private GameObject panelInput; // 有効期限

        [SerializeField] private Canvas parent = default;
        [SerializeField] private BoardInputDialogOkCancel boardinput_dialog = default;
        [SerializeField] private MenuInputDialogOkCancel menuinput_dialog = default;
        [SerializeField] private InputDialogOkCancel input_dialog = default;
        [SerializeField] private DeleteDialogOkCancel delete_dialog = default;
        [SerializeField] private HomePageDialogOkCancel homepage_dialog = default;
        private static int dialogShow = 0;  // 1:input 2:delete 3:homepage
        private static int msgReflect = 0; // 1:ダイアログで入力した伝言板名と伝言メッセージを伝言板に反映する 
                                           // 2:ダイアログで入力した伝言メッセージを伝言板に反映する
                                           // 3:伝言板を削除する
        public static bool dialogdisp = false;  // true:ダイアログ表示中（伝言板が移動しないようにする為）
        private static bool boardEnable = false;    // true:伝言板の入力可能 false:伝言板の入力不可

        private MsgBoardData msgBoardData;
        private MessageData messageData;
        private UserInfoData userInfoData;
        private string anchorinfo_id = "";  // 伝言板登録時に返却されたDBアンカー情報のID
        [SerializeField] private Sprite ImageCamera;

        [SerializeField] private GameObject destoryEffectObject;
        private bool initFlg = true;    // アンカー検索で使用
        [SerializeField] private GameObject buttnCamera;    // アンカー検索で表示するボタン(カメラ)

        public static string errProc = "";      // ErrorSceneで表示
        public static string errDetail = "";    // ErrorSceneで表示
        public static string errMessage = "";   // ErrorSceneで表示

        void Awake()
        {
            //Debug.Log("### Awake start procNumber=" + procNumber);
            if (procNumber == 1)
            {
                _currentAppState = AppState.DemoStepCreateStart;
                scrollView.SetActive(true);
            } else {
                _currentAppState = AppState.DemoStepSeachStart;
                scrollView.SetActive(false);
                StartCoroutine(GetDbData());
            }
            //Debug.Log("### Awake end _currentAppState=" + _currentAppState);
        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("*** Azure Spatial Anchors Main Script Start  procNumber=" + procNumber + ",currentAppState=" + currentAppState);

            dialogShow = 0;  // 1:input 2:delete 3:homepage
            msgReflect = 0; // 1:ダイアログで入力した伝言板名と伝言メッセージを伝言板に反映する 
                            // 2:ダイアログで入力した伝言メッセージを伝言板に反映する
                            // 3:伝言板を削除する
            dialogdisp = false;  // true:ダイアログ表示中（伝言板が移動しないようにする為）
            inputBoardNameValue = "";
            inputBoardMsgValue = "";
            currentAnchorId = "";
            currentMsgBoardId = "";
            currentPrefabName = "";
            anchorinfo_id = "";

            base.Start();

            if (!SanityCheckAccessConfiguration())
            {
                return;
            }
            feedbackBox.text = stateParams[currentAppState].StepMessage;

            Debug.Log("*** Azure Spatial Anchors Demo script started");

            enableAdvancingOnSelect = false;

            EnableCorrectUIControls();

        }

        protected override void OnCloudAnchorLocated(AnchorLocatedEventArgs args)
        {
            //Debug.Log("*** OnCloudAnchorLocated Start args.Status=" + args.Status);

            base.OnCloudAnchorLocated(args);

            //Debug.Log("*** OnCloudAnchorLocated (1)");

            if (args.Status == LocateAnchorStatus.Located)
            {
                //Debug.Log("*** OnCloudAnchorLocated (2)");

                CloudSpatialAnchor cloudAnchor = args.Anchor;
                //Debug.Log("*** OnCloudAnchorLocated (3)");

                UnityDispatcher.InvokeOnAppThread(() =>
                {
//                    currentAppState = AppState.DemoStepStopWatcher;
                    currentAppState = AppState.DemoStepSeachStop;
                    Pose anchorPose = Pose.identity;

                    //Debug.Log("*** OnCloudAnchorLocated (5)");

    #if UNITY_ANDROID || UNITY_IOS
                    anchorPose = currentCloudAnchor.GetPose();
    #endif

                    //Debug.Log("***** 検索(0) id=" + cloudAnchor.Identifier);
                    // 該当データのINDEXED取得
                    var ix = GetIndexedMsgBoard(cloudAnchor.Identifier);
                    string modelName = DataManager.Instance.GetPrefabNameByModelName(msgBoardData.root[ix].anchorinfo_id[0].display_3dmodel);
                    GameObject gobj = (GameObject)Resources.Load(modelName);
                    AnchoredObjectPrefab = gobj;

                    // HoloLens: The position will be set based on the unityARUserAnchor that was located.
                    GameObject spawnedObject = SpawnNewAnchoredObject(anchorPose.position, anchorPose.rotation, cloudAnchor);
                    Debug.Log("***** 検索 id=" + cloudAnchor.Identifier + ",modelName=" + modelName);

                    string inval = cloudAnchor.Identifier;
                    listCloudSpatialAnchor.Add(inval, cloudAnchor);
                    // 該当データの設定
                    SetModelData(ix, spawnedObject, inval, modelName);

                    if (initFlg)
                    {
                        initFlg = false;
                        StartCoroutine(feedbackBoxClean());
                    }
                });
            }
        }

        private void SetModelData(int ix, GameObject spawnedObject, string inval, string modelName)
        {
            Text[] outText = spawnedObject.GetComponentsInChildren<Text>(true);
            if (ix < 0 || outText == null)
            {
                allDiscoveredAnchors.Add(spawnedObject);
                return;
            }

            foreach (Text oText in outText)
            {
                if (oText.name == "TextAnchorID")
                {
                    oText.text = inval;     // アンカーID
                }
                else if (ix >= 0 && oText.name == "TextUserID")
                {
                    oText.text = msgBoardData.root[ix].userinfo_id[0]._id; // ユーザーID
                }
                else if (ix >= 0 && oText.name == "TextBoardID")
                {
                    oText.text = msgBoardData.root[ix]._id;     // 伝言板のID
                }
                else if (oText.name == "TextPrefabName")
                {
                    oText.text = modelName;  // プレハブ名
                }
                else if (ix >= 0 && oText.name == "TextBoardName")
                {
                    oText.text = msgBoardData.root[ix].board_name;  // 伝言板名
                }
            }

            //if (modelName == "CanvasAnchor")
            if (DataManager.Instance.IsMsgBoardByPrefabName(modelName))
            {
                Transform tran_content = spawnedObject.transform.Find("Panel/Scroll View/Viewport/Content");
                GameObject content = tran_content.gameObject;
                int msgCount = 0;
                for (int i = 0; i < messageData.root.Length; i++)
                {
                    if (messageData.root[i].msgboard_id[0]._id == msgBoardData.root[ix]._id)
                    {
                        RectTransform cell = Instantiate(cellPrefab) as RectTransform;
                        cell.SetParent(content.transform, false);
                        var textCell = cell.GetComponentInChildren<Text>();
                        string handleName = messageData.root[i].userinfo_id[0].handle_name;
                        textCell.text = messageData.root[i].message + "\n" + "@" + handleName + "\n" + EditMessgaeDate(messageData.root[i].create_date);
                        msgCount++;
                    }
                }

                if (msgCount > 2)
                {
                    content.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
                }                
            }
            else {
                string msg = "";
                for (int i = 0; i < messageData.root.Length; i++)
                {
                    if (messageData.root[i].msgboard_id[0]._id == msgBoardData.root[ix]._id)
                    {
                        msg = messageData.root[i].message;
                        i = messageData.root.Length;
                    }
                }

                if (DataManager.Instance.IsMenuBoardByPrefabName(modelName))
                {
                    TextMeshProUGUI msgText = spawnedObject.GetComponentInChildren<TextMeshProUGUI>();
                    msgText.text = msg;
                }
                /*
                else {
                    InputField inField = spawnedObject.GetComponentInChildren<InputField>();
                    GameObject inFieldGobj = inField.gameObject;
                    inFieldGobj.SetActive(false);

                    foreach (Text oText in outText)
                    {
                        if (oText.name == "TextOutMsg")
                        {
                            oText.text = msg;
                            GameObject oTextGobj = oText.gameObject;
                            oTextGobj.SetActive(true);
                        }
                    }
                }
                */
            }

            allDiscoveredAnchors.Add(spawnedObject);
        }

        private IEnumerator feedbackBoxClean()
        {
            yield return new WaitForSeconds(5f);
            feedbackPanel.SetActive(false);
            feedbackBox.text = "";
        }

        public void OnApplicationFocus(bool focusStatus)
        {
    #if UNITY_ANDROID
            // We may get additional permissions at runtime. Enable the sensors once app is resumed
            if (focusStatus && locationProvider != null)
            {
                ConfigureSensors();
            }
    #endif
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (spawnedObjectMat != null)
            {
                float rat = 0.1f;
                float createProgress = 0f;
                if (CloudManager.SessionStatus != null)
                {
                    createProgress = CloudManager.SessionStatus.RecommendedForCreateProgress;
                }
                rat += (Mathf.Min(createProgress, 1) * 0.9f);
//                spawnedObjectMat.color = GetStepColor() * rat;
            }

            if (dialogShow == 1)
            {
                dialogShow = 0;
                XRUXPicker.Instance.MobileAndEditorUXTree.SetActive(false);

                if (procNumber == 1){
                    string prefabName = GetPrefabNameBySpawnedObject();
                    if (DataManager.Instance.IsMsgBoardByPrefabName(prefabName))
                    {
                        var _dialog = Instantiate(boardinput_dialog);
                        _dialog.transform.SetParent(parent.transform, false);
                        _dialog.FixDialog = result => Debug.Log("**** boardinput res=" + result);
                        InputField[] inputMsg = _dialog.GetComponentsInChildren<InputField>();
                        for (int i = 0; i < inputMsg.Length; i++)
                        {
                            if (inputMsg[i].name == "InputBoardName")
                            {
                                inputMsg[i].text = inputBoardNameValue;
                            } else if (inputMsg[i].name == "InputMessage")
                            {
                                inputMsg[i].text = inputBoardMsgValue;
                            }
                        }
                    }
                    else {
                        var _dialog = Instantiate(menuinput_dialog);
                        _dialog.transform.SetParent(parent.transform, false);
                        _dialog.FixDialog = result => Debug.Log("**** menuinput res=" + result);
                        InputField[] inputMsg = _dialog.GetComponentsInChildren<InputField>();
                        for (int i = 0; i < inputMsg.Length; i++)
                        {
                            if (inputMsg[i].name == "InputMessage")
                            {
                                inputMsg[i].text = inputBoardMsgValue;
                            }
                        }
                    }
                } else
                {
                    var _dialog = Instantiate(input_dialog);
                    _dialog.transform.SetParent(parent.transform, false);
                    _dialog.FixDialog = result => Debug.Log("**** input res=" + result);

                    string boardName = GetBoardName(currentMsgBoardId);
                    Text textBoardName = _dialog.GetComponentInChildren<Text>();
                    textBoardName.text = boardName;
                }
            } else if (dialogShow == 2)
            {
                dialogShow = 0;
                XRUXPicker.Instance.MobileAndEditorUXTree.SetActive(false);

                var _dialog = Instantiate(delete_dialog);
                _dialog.transform.SetParent(parent.transform, false);
                _dialog.FixDialog = result => Debug.Log("**** delete res=" + result);
            } else if (dialogShow == 3)
            {
                dialogShow = 0;
                XRUXPicker.Instance.MobileAndEditorUXTree.SetActive(false);

                var _dialog = Instantiate(homepage_dialog);
                _dialog.transform.SetParent(parent.transform, false);
                _dialog.prefabName = currentPrefabName;
                _dialog.FixDialog = result => Debug.Log("**** homepage res=" + result);
            } else if (dialogShow == 9)
            {
                dialogdisp = false;
                dialogShow = 0;
            }

            if (msgReflect == 1)
            {
                string prefabName = GetPrefabNameBySpawnedObject();
                Debug.Log("*** msgReflect=1 prefabName=" + prefabName + ",inputBoardNameValue=" + inputBoardNameValue + ",inputBoardMsgValue=" + inputBoardMsgValue);
                if (DataManager.Instance.IsMsgBoardByPrefabName(prefabName))
                {
                    msgReflect = 0;
                    Text name = spawnedObject.GetComponentInChildren<Text>();
                    name.text = inputBoardNameValue;

                    Transform tran_content = spawnedObject.transform.Find("Panel/Scroll View/Viewport/Content");
                    GameObject content = tran_content.gameObject;

                    var textAddCell = content.GetComponentInChildren<Text>();
                    if (textAddCell == null)
                    {
                        RectTransform cell = Instantiate(cellPrefab) as RectTransform;
                        cell.SetParent(content.transform, false);
                        var textCell = cell.GetComponentInChildren<Text>();
                        textCell.text = inputBoardMsgValue;
                        dialogdisp = false;
                    }
                    else {
                        textAddCell.text = inputBoardMsgValue;
                        dialogdisp = false;
                    }
                }
                else {
                    msgReflect = 0;
                    TextMeshProUGUI textMsg = spawnedObject.GetComponentInChildren<TextMeshProUGUI>();
                    textMsg.text = inputBoardMsgValue;
                    dialogdisp = false;
                }
            }
            else if (msgReflect == 2)
            {
                msgReflect = 0;
                // 伝言メッセージのDB登録
                StartCoroutine(MessageCreate(CallbackMessageCreate));
                dialogdisp = false;
            }
            else if (msgReflect == 3)
            {
                msgReflect = 0;
                feedbackBox.text = "アンカーの削除中...";
                feedbackPanel.SetActive(true);
                // アンカーの削除
                DeleteAzureAnchor();
                // 伝言板のDB削除
                StartCoroutine(BoardDelete());
                dialogdisp = false;
            }
        }

        private void CallbackMessageCreate(string createDate)
        {
            string dispDate = EditMessgaeDate(createDate);

            // 該当する伝言板にメッセージを追加
            foreach (GameObject anchorGameObject in allDiscoveredAnchors)
            {
                Text[] outText = anchorGameObject.GetComponentsInChildren<Text>(true);
                if (outText != null)
                {
                    foreach (Text oText in outText)
                    {
                        if (oText.name == "TextBoardID")
                        {
                            if (oText.text == currentMsgBoardId)
                            {
                                Transform content = anchorGameObject.transform.Find("Panel/Scroll View/Viewport/Content");
                                RectTransform cell = Instantiate(cellPrefab) as RectTransform;
                                cell.SetParent(content.transform, false);
                                var textCell = cell.GetComponentInChildren<Text>();
                                textCell.text = inputBoardMsgValue  + "\n" + "@" + PlayerPrefs.GetString("HandleName") + "\n" + dispDate;

                                Text[] textList = content.GetComponentsInChildren<Text>();
                                if (textList.Length > 2)
                                {
                                    content.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
                                }
                                feedbackBox.text = "伝言板にメッセージを追加しました。";
                                feedbackPanel.SetActive(true);
                                StartCoroutine(feedbackBoxClean());
                            }
                        }
                    }
                }
            }
        }

        private async void DeleteAzureAnchor()
        {
            // Azureのアンカー削除
            CloudSpatialAnchor deleteAnchor = listCloudSpatialAnchor[currentAnchorId];
            await CloudManager.DeleteAnchorAsync(deleteAnchor);

            // 表示している伝言板の削除
            foreach (GameObject anchorGameObject in allDiscoveredAnchors)
            {
                if (anchorGameObject != null)
                {
                    Text[] outText = anchorGameObject.GetComponentsInChildren<Text>(true);
                    if (outText != null)
                    {
                        foreach (Text oText in outText)
                        {
                            if (oText.name == "TextAnchorID")
                            {
                                //Debug.Log("*** DeleteAzureAnchor text=" + oText.text + ",currentAnchorId=" + currentAnchorId);
                                if (oText.text == currentAnchorId)
                                {
                                    //Debug.LogFormat("*** DeleteAzureAnchor anchor {0}", oText.text);
                                    Destroy(anchorGameObject);

                                    Vector3 pos = anchorGameObject.transform.position;
                                    float deleteTime = 5f;
                                    var instantiateEffect = GameObject.Instantiate (destoryEffectObject, pos, Quaternion.identity) as GameObject;
                                    Destroy (instantiateEffect, deleteTime);

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override bool IsPlacingObject()
        {
//            return currentAppState == AppState.DemoStepCreateLocalAnchor;

            if (currentAppState == AppState.DemoStepCreateLocalAnchor
                && dialogdisp == false)
            {
                return true;
            } else
            {
                return false;
            }
        }

        protected override Color GetStepColor()
        {
            return stateParams[currentAppState].StepColor;
        }

        protected override async Task OnSaveCloudAnchorSuccessfulAsync()
        {
            await base.OnSaveCloudAnchorSuccessfulAsync();

            Debug.Log("Anchor created, yay!");

            // Sanity check that the object is still where we expect
            Pose anchorPose = Pose.identity;

    #if UNITY_ANDROID || UNITY_IOS
            anchorPose = currentCloudAnchor.GetPose();
    #endif
            // HoloLens: The position will be set based on the unityARUserAnchor that was located.

            SpawnOrMoveCurrentAnchoredObject(anchorPose.position, anchorPose.rotation);

            // DB登録・更新
            Debug.Log("currentCloudAnchor.Identifier=" + base.currentCloudAnchor.Identifier);
            if (base.currentCloudAnchor.Identifier != null && base.currentCloudAnchor.Identifier != "")
            {
                StartCoroutine(MsgBoardCreate(base.currentCloudAnchor.Identifier));
            }

            //currentAppState = AppState.DemoStepCreateExit;    // MsgBoardCreateに移動した
        }

        protected override void OnSaveCloudAnchorFailed(Exception exception)
        {
            base.OnSaveCloudAnchorFailed(exception);
        }



        public async override Task AdvanceDemoAsync()
        {
/*
            switch (currentAppState)
            {
                case AppState.DemoStepCreateSession:
                    if (CloudManager.Session == null)
                    {
                        await CloudManager.CreateSessionAsync();
                    }
                    currentCloudAnchor = null;
                    currentAppState = AppState.DemoStepConfigSession;
                    break;
                case AppState.DemoStepConfigSession:
                    ConfigureSession();
                    currentAppState = AppState.DemoStepStartSession;
                    break;
                case AppState.DemoStepStartSession:
                    await CloudManager.StartSessionAsync();
                    currentAppState = AppState.DemoStepCreateLocationProvider;
                    break;
                case AppState.DemoStepCreateLocationProvider:
                    locationProvider = new PlatformLocationProvider();
                    CloudManager.Session.LocationProvider = locationProvider;
                    currentAppState = AppState.DemoStepConfigureSensors;
                    break;
                case AppState.DemoStepConfigureSensors:
                    SensorPermissionHelper.RequestSensorPermissions();
                    ConfigureSensors();
                    currentAppState = AppState.DemoStepCreateLocalAnchor;
                    // Enable advancing to next step on Air Tap, which is an easier interaction for placing the anchor.
                    // (placing the anchor with Air tap automatically advances the demo).
                    enableAdvancingOnSelect = true;
                    break;
                case AppState.DemoStepCreateLocalAnchor:
                    if (spawnedObject != null)
                    {
                        currentAppState = AppState.DemoStepSaveCloudAnchor;
                    }
                    enableAdvancingOnSelect = false;
                    break;
                case AppState.DemoStepSaveCloudAnchor:
                    currentAppState = AppState.DemoStepSavingCloudAnchor;
                    await SaveCurrentObjectAnchorToCloudAsync();
                    break;
                case AppState.DemoStepStopSession:
                    CloudManager.StopSession();
                    CleanupSpawnedObjects();
                    await CloudManager.ResetSessionAsync();
                    locationProvider = null;
                    currentAppState = AppState.DemoStepCreateSessionForQuery;
                    break;
                case AppState.DemoStepCreateSessionForQuery:
                    ConfigureSession();
                    locationProvider = new PlatformLocationProvider();
                    CloudManager.Session.LocationProvider = locationProvider;
                    ConfigureSensors();
                    currentAppState = AppState.DemoStepStartSessionForQuery;
                    break;
                case AppState.DemoStepStartSessionForQuery:
                    await CloudManager.StartSessionAsync();
                    currentAppState = AppState.DemoStepLookForAnchorsNearDevice;
                    break;
                case AppState.DemoStepLookForAnchorsNearDevice:
                    currentAppState = AppState.DemoStepLookingForAnchorsNearDevice;
                    currentWatcher = CreateWatcher();
                    break;
                case AppState.DemoStepLookingForAnchorsNearDevice:
                    break;
                case AppState.DemoStepStopWatcher:
                    if (currentWatcher != null)
                    {
                        currentWatcher.Stop();
                        currentWatcher = null;
                    }
                    currentAppState = AppState.DemoStepStopSessionForQuery;
                    break;
                case AppState.DemoStepStopSessionForQuery:
                    CloudManager.StopSession();
                    currentWatcher = null;
                    locationProvider = null;
                    currentAppState = AppState.DemoStepComplete;
                    break;
                case AppState.DemoStepComplete:
                    currentCloudAnchor = null;
                    currentAppState = AppState.DemoStepCreateSession;
                    CleanupSpawnedObjects();
                    break;
                default:
                    Debug.Log("Shouldn't get here for app state " + currentAppState.ToString());
                    break;
            }
*/
            // 登録・検索
            switch (procNumber)
            {
                case 1:
                    switch (currentAppState)
                    {
                        case AppState.DemoStepCreateStart:
                            scrollView.SetActive(false);
                            panelInput.SetActive(true);
                            InputField inputDays = panelInput.GetComponentInChildren<InputField>();
                            inputDays.text = EXPIRATION_DAYS_DEFAULT.ToString();
                            currentAppState = AppState.DemoStepCreateExpiration;
                            break;
                        case AppState.DemoStepCreateExpiration:
                            panelInput.SetActive(false);
                            InputField inputDays2 = panelInput.GetComponentInChildren<InputField>();
                            expirationDays = int.Parse(inputDays2.text);
                            //Debug.Log("expirationDays=" + expirationDays);

                            string name = DataManager.Instance.GetPrefabName(anchorItemNo);
                            //Debug.Log("###### name=" + name);
                            GameObject gobj = (GameObject)Resources.Load(name);
                            AnchoredObjectPrefab = gobj;

                            if (CloudManager.Session == null)
                            {
                                await CloudManager.CreateSessionAsync();
                            }
                            currentCloudAnchor = null;

                            // AppState.DemoStepConfigSession
                            ConfigureSession();
                            // AppState.DemoStepStartSession
                            await CloudManager.StartSessionAsync();
                            // AppState.DemoStepCreateLocationProvider
                            locationProvider = new PlatformLocationProvider();
                            CloudManager.Session.LocationProvider = locationProvider;
                            // AppState.DemoStepConfigureSensors
                            SensorPermissionHelper.RequestSensorPermissions();
                            ConfigureSensors();
                            currentAppState = AppState.DemoStepCreateLocalAnchor;   // !!!  DemoStepCreateAnchor;
                            // Enable advancing to next step on Air Tap, which is an easier interaction for placing the anchor.
                            // (placing the anchor with Air tap automatically advances the demo).
                            enableAdvancingOnSelect = true;

                            break;
                        case AppState.DemoStepCreateLocalAnchor:    // !!! DemoStepCreateAnchor:
                            if (spawnedObject != null)
                            {
                                string prefabName = GetPrefabNameBySpawnedObject();
                                Debug.Log("spawnedObject.name=" + spawnedObject.name + "==>" + prefabName);
                                Text[] outText = spawnedObject.GetComponentsInChildren<Text>(true);
                                if (outText != null)
                                {
                                    foreach (Text oText in outText)
                                    {
                                        if (oText.name == "TextPrefabName")
                                        {
                                            oText.text = prefabName;
                                            Debug.Log("oText.text=" + oText.text);
                                            break;
                                        }
                                    }
                                }

                                //Debug.Log("*** kokoで入力可能");
                                boardEnable = true; //伝言板用制御
                                InputField inItem = spawnedObject.GetComponentInChildren<InputField>();
                                if (inItem != null)
                                {
                                    inItem.interactable = true;
                                    inItem.readOnly = false;
                                }
                                currentAppState = AppState.DemoStepCreateSave;
                            }
//                            enableAdvancingOnSelect = false;
                            break;
                        case AppState.DemoStepCreateSave:
                            enableAdvancingOnSelect = false;

                            //Debug.Log("*** kokoで入力不可");
                            boardEnable = false; //伝言板用制御
                            InputField inField = spawnedObject.GetComponentInChildren<InputField>();
                            if (inField != null)
                            {
                                inField.interactable = false;
                                inField.readOnly = true;
                            }

                            currentAppState = AppState.DemoStepSavingCloudAnchor;
                            await SaveCurrentObjectAnchorToCloudAsync();
                            break;
                        case AppState.DemoStepCreateExit:
                            enableAdvancingOnSelect = false;
                            Debug.Log("写真撮影！！");
                            StartCoroutine(ScreenShot());
                            break;
                        //case AppState.DemoStepCreatePicture:
                        //    enableAdvancingOnSelect = false;
                        //    break;
                        default:
                            Debug.Log("Shouldn't get here for app state " + currentAppState.ToString());
                            break;
                    }
                    break;
                case 2:
                    switch (currentAppState)
                    {
                        case AppState.DemoStepSeachStart:
                            if (CloudManager.Session == null)
                            {
                                await CloudManager.CreateSessionAsync();
                            }
                            currentCloudAnchor = null;

                            // AppState.DemoStepCreateSessionForQuery
                            ConfigureSession();
                            await CloudManager.StartSessionAsync();
                            locationProvider = new PlatformLocationProvider();
                            CloudManager.Session.LocationProvider = locationProvider;
                            SensorPermissionHelper.RequestSensorPermissions();
                            ConfigureSensors();

                            // AppState.DemoStepStartSessionForQuery
                        //    await CloudManager.StartSessionAsync();

                            // CurrentCloudAnchorの設定
                            SetCurrentCloudAnchor();

                            // AppState.DemoStepLookForAnchorsNearDevice
                            currentAppState = AppState.DemoStepLookingForAnchorsNearDevice;
                            currentWatcher = CreateWatcher();

                            boardEnable = true; //伝言板用制御
                            buttnCamera.SetActive(true);
                            break;
/*
                        case AppState.DemoStepCreateSessionForQuery:
                            ConfigureSession();
                            locationProvider = new PlatformLocationProvider();
                            CloudManager.Session.LocationProvider = locationProvider;
                            ConfigureSensors();
                            currentAppState = AppState.DemoStepStartSessionForQuery;
                            break;
                        case AppState.DemoStepStartSessionForQuery:
                            await CloudManager.StartSessionAsync();
                            currentAppState = AppState.DemoStepLookForAnchorsNearDevice;
                            break;
                        case AppState.DemoStepLookForAnchorsNearDevice:
                            currentAppState = AppState.DemoStepLookingForAnchorsNearDevice;
                            currentWatcher = CreateWatcher();
                            break;
*/
                        case AppState.DemoStepLookingForAnchorsNearDevice:
                            break;
                        case AppState.DemoStepSeachStop:
/*                        
                            Debug.Log("DemoStepSeachStop(1)");
                            // AppState.DemoStepStopWatcher
                            if (currentWatcher != null)
                            {
                                currentWatcher.Stop();
                                currentWatcher = null;
                            }
                            Debug.Log("DemoStepSeachStop(2)");
                            // AppState.DemoStepStopSessionForQuery
                            CloudManager.StopSession();
                            currentWatcher = null;
                            locationProvider = null;
                            // AppState.DemoStepComplete
                            currentCloudAnchor = null;
                            //currentAppState = AppState.DemoStepCreateSession;   // !!!
                            CleanupSpawnedObjects();

                            Debug.Log("DemoStepSeachStop(3)");
                            //base.ReturnToMenu();
*/
                            break;
                        default:
                            Debug.Log("Shouldn't get here for app state " + currentAppState.ToString());
                            break;
                    }
                  break;
            }
        }

        public async override Task EnumerateAllNearbyAnchorsAsync()
        {
            Debug.Log("Enumerating near-device spatial anchors in the cloud");

            NearDeviceCriteria criteria = new NearDeviceCriteria();
            criteria.DistanceInMeters = 5;
            criteria.MaxResultCount = 20;

            var cloudAnchorSession = CloudManager.Session;

            var spatialAnchorIds = await cloudAnchorSession.GetNearbyAnchorIdsAsync(criteria);

            Debug.LogFormat("Got ids for {0} anchors", spatialAnchorIds.Count);

            List<CloudSpatialAnchor> spatialAnchors = new List<CloudSpatialAnchor>();

            foreach (string anchorId in spatialAnchorIds)
            {
                var anchor = await cloudAnchorSession.GetAnchorPropertiesAsync(anchorId);
                Debug.LogFormat("Received information about spatial anchor {0}", anchor.Identifier);
                spatialAnchors.Add(anchor);
            }

            //feedbackBox.text = $"Found {spatialAnchors.Count} anchors nearby";
            feedbackBox.text = $"近くのアンカー数: {spatialAnchors.Count}";
        }

        protected override void CleanupSpawnedObjects()
        {
//            Debug.Log("*** CleanupSpawnedObjects (1)");

            base.CleanupSpawnedObjects();

            foreach (GameObject anchor in allDiscoveredAnchors)
            {
//                Debug.Log("*** CleanupSpawnedObjects (2)");
                Destroy(anchor);
            }

//            Debug.Log("*** CleanupSpawnedObjects (3)");

            allDiscoveredAnchors.Clear();

  //          Debug.Log("*** CleanupSpawnedObjects (4)");
        }

        private void ConfigureSession()
        {
            const float distanceInMeters = 8.0f;
            const int maxAnchorsToFind = 25;
            SetNearDevice(distanceInMeters, maxAnchorsToFind);
        }

        private void ConfigureSensors()
        {
            locationProvider.Sensors.GeoLocationEnabled = SensorPermissionHelper.HasGeoLocationPermission();

            locationProvider.Sensors.WifiEnabled = SensorPermissionHelper.HasWifiPermission();

            locationProvider.Sensors.BluetoothEnabled = SensorPermissionHelper.HasBluetoothPermission();
            locationProvider.Sensors.KnownBeaconProximityUuids = CoarseRelocSettings.KnownBluetoothProximityUuids;
        }

        private void SetCurrentCloudAnchor()
        {
            // Get the cloud-native anchor behavior
            CloudNativeAnchor cna = cloudNativeAnchor.GetComponent<CloudNativeAnchor>();

            // If the cloud portion of the anchor hasn't been created yet, create it
            if (cna.CloudAnchor == null) { cna.NativeToCloud(); }

            // Get the cloud portion of the anchor
            CloudSpatialAnchor cloudAnchor = cna.CloudAnchor;

            // Store
            currentCloudAnchor = cloudAnchor;
        }

        private string GetPrefabNameBySpawnedObject()
        {
            string prefabName = spawnedObject.name;                                
            int index = prefabName.IndexOf("(");
            prefabName = prefabName.Substring(0, index);
            return prefabName;
        }

        // 伝言板、伝言メッセージのDB登録・更新
        private IEnumerator MsgBoardCreate(string anchor_id)
        {
            var addUrl = "";
        #if UNITY_ANDROID || UNITY_IOS
            addUrl = "https://shinbashi.sevenseas.tokyo/anchor/db/msgboard/add";
        #else
            addUrl = "https://160.16.137.115/anchor/db/msgboard/add";
        #endif

            string display_3dmodel = DataManager.Instance.GetDisplay3dModel(anchorItemNo);

            WWWForm form = new WWWForm();
            form.AddField ("anchor_id", anchor_id);
            form.AddField ("place", "");
            //form.AddField ("display_3dmodel", DataManager.CONST_DISPLAY_3DMODEL_LIST[anchorItemNo]);
            form.AddField ("display_3dmodel", display_3dmodel);
            form.AddField ("expiration_date", expirationDays);
            form.AddField ("userinfo_id", PlayerPrefs.GetString("UserID"));
            form.AddField ("message", inputBoardMsgValue);
            form.AddField ("end_date", "");

            GeoLocation geoLocation = locationProvider.GetLocationEstimate();
            form.AddField ("latitude", geoLocation.Latitude.ToString());
            form.AddField ("longitude", geoLocation.Longitude.ToString());

            switch (anchorItemNo)
            {
                case 0: // 伝言板
                    form.AddField ("board_name", inputBoardNameValue);
                    break;
                case 1: // メニュー
                case 2: // メニューボード
                    form.AddField ("board_name", PlayerPrefs.GetString("MailAddress") + "さんのメニュー");
                    break;
            }

            UnityWebRequest request = UnityWebRequest.Post(addUrl, form);
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                feedbackBox.text = "MsgBoard Add request error " + request.error;
                GoToErrorScene("API:" + addUrl, "MsgBoard Add request error", request.error);
            }
            else {
                if (request.responseCode == 200)
                {
                    anchorinfo_id = request.downloadHandler.text;
                    Debug.Log("anchorinfo_id=" + anchorinfo_id);
                    int boardCount = PlayerPrefs.GetInt("MessageBoardCount") + 1;
                    PlayerPrefs.SetInt("MessageBoardCount", boardCount);

                    currentAppState = AppState.DemoStepCreateExit;
                }
                else {
                    feedbackBox.text = "MsgBoard Add request error " + request.responseCode;
                    GoToErrorScene("API:" + addUrl, "MsgBoard Add request error", "responseCode:" + request.responseCode);
                }
            }
        }

        // 伝言メッセージのDB登録
        private IEnumerator MessageCreate(UnityAction<string> callback)
        {
            var addUrl = "";
        #if UNITY_ANDROID || UNITY_IOS
            addUrl = "https://shinbashi.sevenseas.tokyo/anchor/db/message/";
        #else
            addUrl = "https://160.16.137.115/anchor/db/message/";
        #endif

            WWWForm form = new WWWForm();

            form.AddField ("msgboard_id", currentMsgBoardId);
            form.AddField ("userinfo_id", PlayerPrefs.GetString("UserID"));
            form.AddField ("message", inputBoardMsgValue);
            form.AddField ("end_date", "");
            UnityWebRequest request = UnityWebRequest.Post(addUrl, form);
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                feedbackBox.text = "Message Add request error " + request.error;
                GoToErrorScene("API:" + addUrl, "Message Add request error", request.error);
            }
            else {
                if (request.responseCode == 200)
                {
                    Debug.Log(request.downloadHandler.text);
                    callback(request.downloadHandler.text);
                }
                else {
                    feedbackBox.text = "Message Add request error " + request.responseCode;
                    GoToErrorScene("API:" + addUrl, "Message Add request error", "responseCode:" + request.responseCode);
                }
            }
        }

        // 伝言板のDB削除
        private IEnumerator BoardDelete()
        {
            var url = "";
        #if UNITY_ANDROID || UNITY_IOS
            url = "https://shinbashi.sevenseas.tokyo/anchor/db/msgboard/del";
        #else
            url = "https://160.16.137.115/anchor/db/msgboard/del";
        #endif

            WWWForm form = new WWWForm();

            Debug.Log("!!!!! BoardDelete anchorId=" + currentAnchorId + ",boardId=" + currentMsgBoardId);

            form.AddField ("anchor_id", currentAnchorId);
            form.AddField ("msgboard_id", currentMsgBoardId);
            form.AddField ("userinfo_id", PlayerPrefs.GetString("UserID"));
            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                feedbackBox.text = "MsgBoard Delete request error " + request.error;
                GoToErrorScene("API:" + url, "MsgBoard Delete request error", request.error);
            }
            else {
                if (request.responseCode == 200)
                {
                    //Debug.Log(www.text);
                    int boardCount = PlayerPrefs.GetInt("MessageBoardCount") - 1;
                    PlayerPrefs.SetInt("MessageBoardCount", boardCount);
                    feedbackBox.text = "アンカーの削除が完了しました。";
                    StartCoroutine(feedbackBoxClean());
                }
                else {
                    feedbackBox.text = "MsgBoard Delete request error " + request.responseCode;
                    GoToErrorScene("API:" + url, "MsgBoard Delete request error", "responseCode:" + request.responseCode);
                }
            }
        }

        public static void InputDialogShow(string boardId, string prefabName)
        {
            dialogShow = 1;
            currentMsgBoardId = boardId;
            currentPrefabName = prefabName;
//            Debug.Log("!!!!! InputDialogShow currentMsgBoardId=" + currentMsgBoardId);
            dialogdisp = true;
        }
        public static void DeleteDialogShow(string anchorId, string boardId)
        {
            dialogShow = 2;
            currentAnchorId = anchorId;
            currentMsgBoardId = boardId;
            Debug.Log("!!!!! DeleteDialogShow anchorId=" + anchorId + ",boardId=" + boardId);
            dialogdisp = true;
        }
        public static void DialogClose(bool disp)
        {
            XRUXPicker.Instance.MobileAndEditorUXTree.SetActive(true);
            if (!disp)
            {
                dialogShow = 9;
            }
        }
        public static void HomePageDialogShow(string prefabName)
        {
            dialogShow = 3;
            currentPrefabName = prefabName;
            dialogdisp = true;
        }
        public static void SetInputBoardMessage(string boardName, string msg)
        {
            inputBoardNameValue = boardName;
            inputBoardMsgValue = msg;
            msgReflect = 1;
        }
        public static void SetInputMessage(string msg)
        {
            inputBoardMsgValue = msg;
            msgReflect = 2;
        }
        public static void SetDelete()
        {
            msgReflect = 3;
        }

        // 伝言板を削除できるか判定
        public static bool IsBoardDelete(string userID)
        {
            Debug.Log("userID=" + userID + ",PlayerPrefs=" + PlayerPrefs.GetString("UserID"));
            if (userID == PlayerPrefs.GetString("UserID"))
            {
                return true;
            }
            else {
                return false;
            }
        }

        // 伝言板用制御
        public static bool IsMsgBoardInput()
        {
            return boardEnable;
        }

        public void ReturnToHome()
        {
            Debug.Log("ReturnToHome(1)");
            // AppState.DemoStepStopWatcher
            if (currentWatcher != null)
            {
                Debug.Log("ReturnToHome(1-1)");
                currentWatcher.Stop();
                currentWatcher = null;
            }

            Debug.Log("ReturnToHome(2)");
            if (CloudManager != null)
            {
            Debug.Log("ReturnToHome(2-1)");
                    CloudManager.StopSession();
            }
            currentWatcher = null;
            locationProvider = null;

            currentCloudAnchor = null;
            CleanupSpawnedObjects();

            Debug.Log("ReturnToHome(3)");

            base.ReturnToMenu();
            //SceneManager.LoadScene("MenuScene");
        }

        private string EditMessgaeDate(string paraDate)
        {
            string editDate = "";

            DateTime nowDateTime = DateTime.Now;
            string strNowDate = nowDateTime.ToString("yyyy/MM/dd");
            DateTime dtNow = DateTime.ParseExact(strNowDate, "yyyy/M/d", null);

            string createDate = paraDate.Substring(0, 10);
            createDate = createDate.Replace("-", "/");
            DateTime dtCreate = DateTime.ParseExact(createDate, "yyyy/M/d", null);
            
            switch ((dtCreate - dtNow).TotalDays)
            {
                case 0:
                    editDate = "今日 " + paraDate.Substring(11, 5);
                    break;
                case -1:
                    editDate = "昨日 " + paraDate.Substring(11, 5);
                    break;
                default:
                    editDate = paraDate.Substring(0, 16);
                    break;
            }
            return editDate;
        }

        private IEnumerator ScreenShot()
        {
            string fileName = PlayerPrefs.GetString("MailAddress") + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
            string path = Application.persistentDataPath + "/"  + fileName;

            ScreenCapture.CaptureScreenshot(fileName);

            // シャッター音を出す
            #if UNITY_IOS
            VibrateIOS.PlaySystemSound(1108);
            #endif

            // カメラアイコンを非表示
            XRUXPicker.Instance.GetDemoButtons()[0].gameObject.SetActive(false);

            feedbackBox.text = "写真をアップロード中...";
            feedbackPanel.SetActive(true);
            yield return new WaitUntil( () => File.Exists(path) );

            //Debug.Log("*** 写真SAVE END path=" + path + " & API CALL");

            byte[] img = File.ReadAllBytes(path);
            WWWForm form = new WWWForm();
            form.AddBinaryData("file", img, fileName, "image/jpeg");
            string url = "https://shinbashi.sevenseas.tokyo/anchor/fileupload/";

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.Send();

            //Debug.Log("*** 写真 DELETE START");

            if (File.Exists(path))
            {
                File.Delete(path);
                yield return new WaitWhile( () => File.Exists(path) );
            }
            //Debug.Log("*** 写真 DELETE END request.responseCode=" + request.responseCode);

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("request.error=" + request.error);
                //callback(request.error);
                GoToErrorScene("API:" + url, "ImageFile Upload request error", request.error);
            }
            else {
                if (request.responseCode == 200)
                {
                    //Debug.Log("**** request.responseCode=" + request.responseCode);
                    if (procNumber == 1)
                    {
                        StartCoroutine(ImageUpdate(fileName));
                    }
                    else {
                        feedbackBox.text = "写真のアップロードが完了しました。";
                        StartCoroutine(feedbackBoxClean());
                    }
                }
                else {
                    GoToErrorScene("API:" + url, "ImageFile Upload request error", "responseCode:" + request.responseCode);
                }
            }
        } 
        private IEnumerator ImageUpdate(string fileName)
        {
            WWWForm form = new WWWForm();
            string updateUrl = "https://shinbashi.sevenseas.tokyo/anchor/db/anchorinfo/image/" + anchorinfo_id;
            //Debug.Log("**** updateUrl=" + updateUrl);

            form.AddField("place_image", fileName);
            UnityWebRequest request = UnityWebRequest.Post(updateUrl, form);
            yield return request.Send();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
                //callback(request.error);
                GoToErrorScene("API:" + updateUrl, "AnchorInfo Update request error", request.error);
            }
            else {
                if (request.responseCode == 200)
                {
                    //Debug.Log("**** request.responseCode=" + request.responseCode);
                    //currentAppState = AppState.DemoStepCreatePicture;
                    feedbackBox.text = "写真のアップロードが完了しました。";
                    StartCoroutine(feedbackBoxClean());
                }
                else {
                    GoToErrorScene("API:" + updateUrl, "AnchorInfo Update request error", "responseCode:" + request.error);
                }
            }
        }

        public void OnBtnCamera()
        {
            StartCoroutine(SocialScreenShot());
        }

        private IEnumerator SocialScreenShot()
        {
            string fileName = "/scshot.png";
            string path = Application.persistentDataPath + "/"  + fileName;

            // カメラアイコンを非表示
            buttnCamera.SetActive(false);

            if (File.Exists(path))
            {
                File.Delete(path);
                yield return new WaitWhile( () => File.Exists(path) );
            }

            ScreenCapture.CaptureScreenshot(fileName);

            // シャッター音を出す
            #if UNITY_IOS
            VibrateIOS.PlaySystemSound(1108);
            #endif

            //feedbackBox.text = "写真をアップロード中...";
            //feedbackPanel.SetActive(true);

            yield return new WaitUntil( () => File.Exists(path) );

            string text = "";
            string url = "";
            SocialConnector.SocialConnector.Share(text, url, path);
        }

        public static void SetAnchorItemNo(int itemNo)
        {
            anchorItemNo = itemNo;
        }

// ************************************************** 

        private int GetIndexedMsgBoard(string anchor_id)
        {
            int ix = -1;
            for (int i = 0; i < msgBoardData.root.Length; i++)
            {
                //if (msgBoardData.root[i].anchor_id == anchor_id)
                if (msgBoardData.root[i].anchorinfo_id[0].anchor_id == anchor_id)
                {
                    ix = i;
                    i = msgBoardData.root.Length;
                }
            }
            return ix;
        }

        private string GetBoardName(string _id)
        {
            string boardName = "";
            for (int i = 0; i < msgBoardData.root.Length; i++)
            {
                if (msgBoardData.root[i]._id == _id)
                {
                    boardName = msgBoardData.root[i].board_name;
                    i = msgBoardData.root.Length;
                }
            }
            return boardName;
        }

        public IEnumerator GetDbData()
        {
            int nextButtonIndex = 0;

            //Debug.Log("*** NEXT false");
            XRUXPicker.Instance.GetDemoButtons()[nextButtonIndex].gameObject.SetActive(false);

            //Debug.Log("*** GetMsgBoardData Start");
            yield return StartCoroutine(GetMsgBoardData());

            //Debug.Log("*** GetMessgaeData Start");
            yield return StartCoroutine(GetMessgaeData());

            //Debug.Log("*** NEXT true");
            XRUXPicker.Instance.GetDemoButtons()[nextButtonIndex].gameObject.SetActive(true);
        }

        public IEnumerator GetMsgBoardData()
        {
            var url = "";
        #if UNITY_ANDROID || UNITY_IOS
            url ="https://shinbashi.sevenseas.tokyo/anchor/db/msgboard/";
        #else
            url ="https://160.16.137.115/anchor/db/msgboard/";
        #endif

            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                //Debug.Log(request.error);
                feedbackBox.text = "MsgBoard Data request error " + request.error;
                GoToErrorScene("API:" + url, "MsgBoard Data request error", request.error);
            }
            else
            {
                if (request.responseCode == 200)
                {
                    string jsonText = request.downloadHandler.text;
                    msgBoardData = new MsgBoardData();
                    var json = "{" + $"\"root\": {jsonText}" + "}";
                    JsonUtility.FromJsonOverwrite(json, msgBoardData);
                    Debug.Log("*** data userinfo_id=" + msgBoardData.root[0].userinfo_id[0]);
                }
                else {
                    feedbackBox.text = "DB Data request error " + request.responseCode;
                    GoToErrorScene("API:" + url, "MsgBoard Data request error", "responseCode:" + request.responseCode);
                }
            }
        }

        public IEnumerator GetMessgaeData()
        {
            var url = "";
        #if UNITY_ANDROID || UNITY_IOS
            url = "https://shinbashi.sevenseas.tokyo/anchor/db/message/";
        #else
            url = "https://160.16.137.115/anchor/db/message/";
        #endif

            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
                feedbackBox.text = "Message Data request error " + request.error;
                GoToErrorScene("API:" + url, "Message Data request error", request.error);
            }
            else
            {
                if (request.responseCode == 200)
                {
                    string jsonText = request.downloadHandler.text;
                    messageData = new MessageData();
                    var json = "{" + $"\"root\": {jsonText}" + "}";
                    //Debug.Log("json=" + json);
                    JsonUtility.FromJsonOverwrite(json, messageData);
                    Debug.Log("*** data message[0]=" + messageData.root[0].message);
                }
                else {
                    feedbackBox.text = "Message Data request error " + request.responseCode;
                    GoToErrorScene("API:" + url, "Message Data request error", "responseCode:" + request.responseCode);
                }
            }
        }

        private void GoToErrorScene(string paraErrProc, string paraErrDetail, string paraErrMessage)
        {
            errProc = paraErrProc;
            errDetail = paraErrDetail;
            errMessage = paraErrMessage;
            SceneManager.LoadScene("ErrorScene");            
        }


    }

    [System.Serializable]
    public class MsgBoardModel
    {
        public string _id;
        public AnchorInfoModel[] anchorinfo_id;
        public string board_name;
        public UserInfoModel[] userinfo_id;
        public string create_date;
        public string update_date;
        public int __v;
    }

    [System.Serializable]
    public class MessageModel
    {
        public string _id;
        public MsgBoardModel[] msgboard_id;
        public UserInfoModel[] userinfo_id;
        public string message;
        public string end_date;
        public string create_date;
        public string update_date;
        public int __v;
    }

    [System.Serializable]
    public class UserInfoModel
    {
        public string _id;
        public string handle_name;
        public string mails_address;
        public int message_board_count;
        public string create_date;
        public string update_date;
        public int __v;
    }

    [System.Serializable]
    public class AnchorInfoModel
    {
        public string anchor_id;
        public string place_image;
        public string place;
        public string display_3dmodel;
        public string display_notice;
        public string expiration_date;
        public int latitude;
        public int longitude;
        public string create_date;
        public string update_date;
        public int __v;
    }

    [System.Serializable]
    public class MsgBoardData
    {
        public MsgBoardModel[] root;
    }

    [System.Serializable]
    public class MessageData
    {
        public MessageModel[] root;
    }

    [System.Serializable]
    public class UserInfoData
    {
        public UserInfoModel[] root;
    }

    [System.Serializable]
    public class AnchorInfoData
    {
        public AnchorInfoModel[] root;
    }

}