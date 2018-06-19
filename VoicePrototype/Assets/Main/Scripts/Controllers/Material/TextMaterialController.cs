using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextMaterialController : UIMaterialController {
    private TextMeshPro textMeshPro;

    private void Awake() {
        textMeshPro = GetComponent<TextMeshPro>();
    }

    public override bool HasMaterialColor
    {
        get
        {
            return textMeshPro != null;
        }
    }

    public override Color MaterialColor {
        get {
            if (HasMaterialColor)
                return textMeshPro.color;
            else
                return Color.red;
        }
        set {
            if (HasMaterialColor)
                textMeshPro.color = value;
        }
    }
}
