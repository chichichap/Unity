using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Conversation", menuName = "Conversations", order = 1)]
public class Conversation : ScriptableObject {
    public string Name;
    public List<PlayerOption> PlayerOptions;
}

[Serializable]
public class PlayerOption {
    public string DisplayText;
    public List<string> Speeches;
}