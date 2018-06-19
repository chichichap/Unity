using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using TMPro;
using System.Collections.Generic;

public class SpeechRecognizer : MonoBehaviour, IPocketSphinxEvents
{
    public static SpeechRecognizer main;
    public bool IsListening { get; set; }

    #region Serialized fields
    [SerializeField]
    private GameObject _pocketSphinxPrefab;
    [SerializeField]
    private TextMeshPro _infoText;
    [SerializeField]
    private TextMeshPro _SpeechResult;
    #endregion
   
    #region Private fields
    private UnityPocketSphinx.PocketSphinx _pocketSphinx;
    private string currentSearchKey;
    
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

    private void switchSearch(string searchKey) {
        StopListening();

        StartCoroutine(delayStartListening(searchKey));
    }
    
    IEnumerator delayStartListening(string searchKey) {
        yield return new WaitForSecondsRealtime(1f);
        StartListening(searchKey);
    }
    #endregion

    #region Public methods
    public void SetUp(List<string> paths) {
        if (paths == null || paths.Count == 0)
            return;

        _pocketSphinx.SetAcousticModelPath("en-us-ptm");
        _pocketSphinx.SetDictionaryPath("cmudict-en-us.dict");
        _pocketSphinx.SetKeywordThreshold(0.1f);
        _pocketSphinx.AddBoolean("-allphone_ci", true);

        //register paths from conversations
        for (int i = 0; i < paths.Count; i++)
            _pocketSphinx.AddKeywordSearchPath(paths[i], paths[i] + ".gram");

        currentSearchKey = paths[0];

        _pocketSphinx.SetupRecognizer();
    }

    public void StartListening(string searchKey)
    {
        _infoText.text = "before starting " + searchKey;
        _pocketSphinx.StartListening(searchKey);
        currentSearchKey = searchKey;
        _infoText.text = "listening to " + searchKey;
        IsListening = true;
    }

    public void StopListening()
    {
        IsListening = false;
        _infoText.text = "before stopping";
        _pocketSphinx.CancelRecognizer(); // _pocketSphinx.StopRecognizer() will trigger OnResult()
        _infoText.text = "stopped";
    }
    #endregion

    #region MonoBehaviour methods
    void Awake()
    {
        main = this;

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

    void OnDestroy()
    {
        if (_pocketSphinx != null)
        {
            UnsubscribeFromPocketSphinxEvents();
            _pocketSphinx.DestroyRecognizer();
        }
    }
    #endregion

    #region PocketSphinx Events
    public void OnPartialResult(string hypothesis) {
        if (IsListening == false)
            return;

        _SpeechResult.text = ">" + hypothesis + "<";
        _SpeechResult.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        ConversationController.main.OnPlayerSpeech(hypothesis);

        StopListening();
    }
    
    public void OnResult(string hypothesis) {
        if (IsListening == false)
            return;

        _SpeechResult.text = ">" + hypothesis + "<" + " (final)";
        _SpeechResult.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        ConversationController.main.OnPlayerSpeech(hypothesis);

        StopListening();
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
        switchSearch(currentSearchKey);
    }

    public void OnInitializeSuccess()
    {
        _SpeechResult.text = "Init Done!";
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
