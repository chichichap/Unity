using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugLogController : MonoBehaviour {
    public static DebugLogController main;

    [SerializeField]
    private TextMeshPro _debugLog;

    private void Awake()
    {
        main = this;
    }

    public void Log(string s) {
        //if (_debugLog != null)
            _debugLog.text = s;
    }
}
