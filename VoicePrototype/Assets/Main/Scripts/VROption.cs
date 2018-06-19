using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VROption : VRInteractable {
    [SerializeField]
    private int optionIndex;
    public int OptionIndex
    {
        get
        {
            return optionIndex;
        }

        set
        {
            optionIndex = value;
        }
    }

    private TextMaterialController _textMaterialController;
    private SpriteMaterialController _spriteMaterialController;

    private void Awake() {
        _collider = GetComponent<BoxCollider>();
        _textMaterialController = GetComponent<TextMaterialController>();
        _spriteMaterialController = GetComponentInChildren<SpriteMaterialController>();
    }

    public override void Highlight()
    {
        _textMaterialController.DoHighlight();
        _spriteMaterialController.DoHighlight();

        if (OnHighlighted != null)
            OnHighlighted.Invoke();
    }

    public override void Unhighlight()
    {
        //no need to unhighlight when interacted
        if (_collider.enabled) {
            _textMaterialController.DoUnhighlight();
            _spriteMaterialController.DoUnhighlight();
        }
    }

    public override void DoInteract()
    {
        base.DoInteract();

        _textMaterialController.DoBlinkandFadeOut();
        _spriteMaterialController.DoBlinkandFadeOut();
    }

    public override void DoShow()
    {
        base.DoShow();

        _textMaterialController.DoFadeIn();
        _spriteMaterialController.DoFadeIn();
    }

    public override void DoHide()
    {
        base.DoHide();

        _textMaterialController.DoFadeOut();
        _spriteMaterialController.DoFadeOut();
    }
}
