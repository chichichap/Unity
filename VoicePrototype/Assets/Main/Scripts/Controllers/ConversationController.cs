using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConversationController : MonoBehaviour {
    public static ConversationController main;

    public UnityEvent OnBeginingOfSpeech;
    
    private Conversation currentConversation;
    private List<VROption> currentOptions;

    [SerializeField]
    private List<string> sceneConversations;

    private void Awake() {
        main = this;
    }

    private void Start()
    {
         SpeechRecognizer.main.SetUp(sceneConversations);
    }

    public void StartListening(Conversation conversation) {
        currentConversation = conversation;

        if (SpeechRecognizer.main.IsListening)
            SpeechRecognizer.main.StopListening();
        SpeechRecognizer.main.StartListening(conversation.Name);
    }

    public void SetUpOptions(Transform parent) {
        currentOptions = new List<VROption>();

        foreach (Transform child in parent) {
            VROption option = child.GetComponent<VROption>();

            if (option != null) {
                option.DoShow();
                currentOptions.Add(option);
            }
        }
    }
    
    public void OnPlayerSpeech(string speech) {
        for (int i = 0; i < currentConversation.PlayerOptions.Count; i++)
        {
            foreach (string s in currentConversation.PlayerOptions[i].Speeches)
            {
                if (s == TruncateLongString(speech, s.Length))
                {
                    DebugLogController.main.Log("SelectOption: " + i);

                    SelectOption(i);
                    return;
                }
            }
        }
    }

    public void SelectOption(int index) {
        if (SpeechRecognizer.main.IsListening)
            SpeechRecognizer.main.StopListening();

        currentOptions[index].DoInteract();

        for (int i = 0; i < currentOptions.Count; i++)
            if (i != index)
                currentOptions[i].DoHide();
    }

    //helper method for string comparison
    private string TruncateLongString(string str, int maxLength) {
        return str.Substring(0, Math.Min(str.Length, maxLength));
    }
}
