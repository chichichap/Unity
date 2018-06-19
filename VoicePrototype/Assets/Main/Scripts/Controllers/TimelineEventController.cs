using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimelineEventController : MonoBehaviour {
    [SerializeField]
    private UnityEvent OnObjectEnabled;

    [SerializeField]
    private UnityEvent OnObjectDisabled;

    private void OnEnable() {
        if (OnObjectEnabled != null)
            OnObjectEnabled.Invoke();
    }

    private void OnDisable()
    {
        if (OnObjectDisabled != null)
            OnObjectDisabled.Invoke();
    }
}
