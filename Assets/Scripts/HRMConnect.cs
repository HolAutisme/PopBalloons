using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using WSAUnityHRMStub;

public class HRMConnect : MonoBehaviour
{
    private const float HEART_SCALE_MIN = 0.25f;
    private const float HEART_SCALE_MAX = 0.26f;
    private const float HEART_SCALE_RANGE = HEART_SCALE_MAX - HEART_SCALE_MIN;

    public Queue<Action> callbackQueue = new Queue<Action>();

    private HRMPlugin _hrmPlugin;
    private HeartRateServiceDevice _selectedDevice;

    private int _lastHRValue;
    private float _beatStartTime;
    private float _beatCompleteTime;
    private float _rrDuration; // time in seconds for one heart beat

    /*
    [SerializeField]
    private Text _BPMDisplay;
    */

    private Participant participant;
    private HRMData data;

    public enum HRMstatus {
        SCANNING    = 0,
        CONNECTING  = 1,
        NO_DEVICE   = 2        
    }

    private string _hrmStatusMsgStart               = "Scan for Paired HRM Devices";
    private string _hrmStatusMsgScanning            = "Scanning...";
    private string _hrmStatusMsgScanCompleteSuccess = "Select an HRM Device";
    private string _hrmStatusMsgScanNoDevices       = "Pair your HRM in Settings then Try Again";
    private string _hrmStatusMsgDeviceSelected      = "Click Connect";
    private string _hrmStatusMsgConnecting          = "Connecting ";
    private string _hrmStatusMsgConnected           = "Connected";
    private string _dropdownBlankEntryLabel         = "None";
    private string _dropdownCaptionStart            = "HRM Device List";
    private string _dropdownCaptionSelect           = "Select an HRM Device";
    private List<Text> _values;
    private GameObject _canvasValueDisplay;

    void Start()
    {
        //TODO : Check if host

        data = this.GetComponent<HRMData>();

        Application.targetFrameRate = 60;

        resetValues();
        GameObject tCanvasGO = GameObject.Find("Canvas");
        //_BPMDisplay.text = _hrmStatusMsgStart;

        _hrmPlugin = new HRMPlugin();
        //Debug.LogFormat("HRM Plugin Version: {0}", HRMPlugin.VERSION);
        onScanClicked();
    }

    private void resetAll()
    {
        resetValues();
        //_BPMDisplay.text = _hrmStatusMsgStart;

        callbackQueue.Clear();
    }

    private void resetValues()
    {
        _lastHRValue = 0;
        _beatStartTime = 0;
        _beatCompleteTime = 0;
        _rrDuration = 0;
    }
    
    private void onScanClicked()
    {
        //_BPMDisplay.text = _hrmStatusMsgScanning;
        data.CmdUpdateBPM((int)HRMstatus.SCANNING);

        _hrmPlugin.disconnectService();
        resetAll();

        _hrmPlugin.scan(callbackQueue, onScanComplete);
    }

    public void onScanComplete()
    {
        if (_hrmPlugin.hrsDevices.Count > 0)
        {
            Connect();
        }
        else
        {
            //_BPMDisplay.text = "No Device Detected";
            data.CmdUpdateBPM((int)HRMstatus.NO_DEVICE);
        }
    }

    private void Connect()
    {
        _selectedDevice = _hrmPlugin.hrsDevices[0];

        resetValues();

        //_BPMDisplay.text = _hrmStatusMsgConnecting + _hrmPlugin.hrsDevices[0].name +"...";
        data.CmdUpdateBPM((int)HRMstatus.CONNECTING);

        // connect with Queue/Action Producer/Consumer pattern
        _hrmPlugin.initializeService(_selectedDevice, callbackQueue, processHRValues);

        // default is 100 saved byte arrays. 
        _hrmPlugin.ReceivedMeasurementDataStorageMax = 20;
    }

    private void processHRValues()
    {
        if (_hrmPlugin.hrms.Count <= 0)
            return;

        // returns ushort if there's a new entry, otherwise returns 0
        // pass true (default) to reset status of *new* available data in plugin
        ushort tHRM = _hrmPlugin.getLastHRM(true);
        if (tHRM != 0)
        {
            _lastHRValue = tHRM;

            //_BPMDisplay.text = "BPM : " + _lastHRValue.ToString();
            data.CmdUpdateBPM(_lastHRValue);//"BPM VALUE"
        }
    }

    void Update()
    {
        // check queue for new Actions added by the plugin
        if (callbackQueue.Count > 0)
        {
            Action tActionHolder = callbackQueue.Dequeue();
            if (tActionHolder != null)
            {
                tActionHolder();
            }
        }

        if (!_hrmPlugin.isServiceInitialized)
        {
            return;
        }
    }
}