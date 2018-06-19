using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class VRInteractable : MonoBehaviour {
    [SerializeField]
    protected UnityEvent OnHighlighted;

    [SerializeField]
    protected UnityEvent OnInteracted;
    
    public bool IsHighlighted { get; set; }
    public bool IsInteractable { get { return _collider.enabled && transform.gameObject.activeSelf; }  }
    
    protected BoxCollider _collider;

    private void Awake() {
        _collider = GetComponent<BoxCollider>();
    }

    public virtual void Highlight() {
        
    }

    public virtual void Unhighlight() {
        
    }

    public virtual void DoInteract() {
        _collider.enabled = false;

        if (OnInteracted != null)
            OnInteracted.Invoke();
    }

    public virtual void DoShow() {
        if (_collider != null)
            _collider.enabled = true;

        transform.gameObject.SetActive(true);
    }

    public virtual void DoHide() {
        if (_collider != null)
            _collider.enabled = false;

        //SetActive(false) after fading out
    }
}
