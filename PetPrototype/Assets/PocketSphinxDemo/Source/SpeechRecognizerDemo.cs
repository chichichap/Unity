using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class SpeechRecognizerDemo : MonoBehaviour, IPocketSphinxEvents
{
    #region Public serialized fields
    [SerializeField]
    private GameObject _pocketSphinxPrefab;
    [SerializeField]
    private Text _infoText;
    [SerializeField]
    private Text _SpeechResult;
    #endregion

    public Transform AIs;

    #region Private fields
    private UnityPocketSphinx.PocketSphinx _pocketSphinx;
    private const String KWS_SEARCH = "wakeup";
    private bool listening = false;
    #endregion

    #region Private methods
    private void SubscribeToPocketSphinxEvents()
    {
        EM_UnityPocketsphinx em = _pocketSphinx.EventManager;

        em.OnBeginningOfSpeech += OnBeginningOfSpeech;
        em.OnEndOfSpeech += OnEndOfSpeech;
        em.OnError += OnError;
        em.OnInitializeFailed += OnInitializeFailed;
        em.OnInitializeSuccess += OnInitializeSuccess;
        em.OnPartialResult += OnPartialResult;
        em.OnPocketSphinxError += OnPocketSphinxError;
        em.OnResult += OnResult;
        em.OnTimeout += OnTimeout;
    }

    private void UnsubscribeFromPocketSphinxEvents()
    {
        EM_UnityPocketsphinx em = _pocketSphinx.EventManager;

        em.OnBeginningOfSpeech -= OnBeginningOfSpeech;
        em.OnEndOfSpeech -= OnEndOfSpeech;
        em.OnError -= OnError;
        em.OnInitializeFailed -= OnInitializeFailed;
        em.OnInitializeSuccess -= OnInitializeSuccess;
        em.OnPartialResult -= OnPartialResult;
        em.OnPocketSphinxError -= OnPocketSphinxError;
        em.OnResult -= OnResult;
        em.OnTimeout -= OnTimeout;
    }

    private void switchSearch(string searchKey)
    {
        listening = false;
        _infoText.text = "before stopping";
        _pocketSphinx.CancelRecognizer(); // _pocketSphinx.StopRecognizer() will trigger OnResult()
        _infoText.text = "stopped";

        StartCoroutine(delayStartListening());
    }

    private void startListening()
    {
        _infoText.text = "before starting";
        _pocketSphinx.StartListening(KWS_SEARCH);
        _infoText.text = "listening";
        listening = true;
    }

    IEnumerator delayStartListening()
    {
        yield return new WaitForSecondsRealtime(1f);
        startListening();
    }

    private string TruncateLongString(string str, int maxLength) {
        return str.Substring(0, Math.Min(str.Length, maxLength));
    }
    #endregion

    #region MonoBehaviour methods
    void Awake()
    {
        UnityEngine.Assertions.Assert.IsNotNull(_pocketSphinxPrefab, "No PocketSphinx prefab assigned.");
        var obj = Instantiate(_pocketSphinxPrefab, this.transform) as GameObject;
        _pocketSphinx = obj.GetComponent<UnityPocketSphinx.PocketSphinx>();

        if (_pocketSphinx == null)
        {
            Debug.LogError("[SpeechRecognizerDemo] No PocketSphinx component found. Did you assign the right prefab???");
        }

        SubscribeToPocketSphinxEvents();

        _infoText.text = "Please wait for Speech Recognition engine to load.";
        _SpeechResult.text = "Loading human dictionary...";
    }

    void Start()
    {
        _pocketSphinx.SetAcousticModelPath("en-us-ptm");
        _pocketSphinx.SetDictionaryPath("cmudict-en-us.dict");
        _pocketSphinx.SetKeywordThreshold(0.1f);
        _pocketSphinx.AddBoolean("-allphone_ci", true);
        
        _pocketSphinx.AddKeywordSearchPath(KWS_SEARCH, "menu.gram");
        _pocketSphinx.SetupRecognizer(); 
    }

    void OnDestroy()
    {
        if (_pocketSphinx != null)
        {
            UnsubscribeFromPocketSphinxEvents();
            _pocketSphinx.DestroyRecognizer();
        }
    }
    #endregion

    #region PocketSphinx event methods
    public void OnPartialResult(string hypothesis)
    {
        if (listening == false)
            return;

        //DEBUG: showing recognized word
        _SpeechResult.text = ">" + hypothesis + "<";
        _SpeechResult.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        
        foreach (Transform AI in AIs) {
            string petName = AI.GetComponent<AIStateMachine>().petName;
            if (petName == TruncateLongString(hypothesis, petName.Length))
            {
                Transform player = transform.Find("CenterEyeAnchor"); //rotating headset will move this, causing AI to follow the player weirdly

                AI.GetComponent<AIStateMachine>().setAttention(true, 15f);
                AI.GetComponent<AIStateMachine>().setMoveTarget(player.position, 15f); //will chase Player for 15 seconds before stopping
                AI.GetComponent<AIStateMachine>().setLookTarget(player.position, 5f); //overiding setLook forward in prepareMove()
                AI.GetComponent<AIStateMachine>().setBark();

                continue;
            }

            if (AI.GetComponent<AIStateMachine>().attention == false)
                continue;

            bool successful = false;
            string command = TruncateLongString(hypothesis, 3);
            switch (command)
            {
                //problem: gotta prevent multiple calls of OnPartialResult() = multiple commands even when said once...
                case "sit":
                    successful = AI.GetComponent<AIStateMachine>().doCommand(1);
                    break;
                case "paw":
                    successful = AI.GetComponent<AIStateMachine>().doCommand(2);
                    break;
                case "dow":
                    successful = AI.GetComponent<AIStateMachine>().doCommand(3);
                    break;
                case "up ":
                    successful = AI.GetComponent<AIStateMachine>().doCommand(4);
                    break;
                default:
                    break;
            }

            if (successful)
                AI.GetComponent<AIStateMachine>().setAttention(true, 10f);
        }

        switchSearch(KWS_SEARCH);
    }
    
    public void OnResult(string hypothesis)
    {
        if (listening == false)
            return;

        _SpeechResult.text = ">" + hypothesis + "<" + " (final)";
        _SpeechResult.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f); 
    }

    public void OnBeginningOfSpeech()
    {
        _infoText.text = "OnBeginningOfSpeech";
    }

    public void OnEndOfSpeech()
    {
        _infoText.text = "OnEndOfSpeech";
    }

    public void OnError(string error)
    {
        Debug.LogError("[SpeechRecognizerDemo] An error ocurred at OnError()");
        Debug.LogError("[SpeechRecognizerDemo] error = " + error);
    }

    public void OnTimeout()
    {
        Debug.Log("[SpeechRecognizerDemo] Speech Recognition timed out");
        switchSearch(KWS_SEARCH);
    }

    public void OnInitializeSuccess()
    {
        _SpeechResult.text = "Say something! Init Done!";
        startListening();
    }

    public void OnInitializeFailed(string error)
    {
        Debug.LogError("[SpeechRecognizerDemo] An error ocurred on Initialization PocketSphinx.");
        Debug.LogError("[SpeechRecognizerDemo] error = " + error);
    }

    public void OnPocketSphinxError(string error)
    {
        Debug.LogError("[SpeechRecognizerDemo] An error ocurred on OnPocketSphinxError().");
        Debug.LogError("[SpeechRecognizerDemo] error = " + error);
    }
    #endregion
}
