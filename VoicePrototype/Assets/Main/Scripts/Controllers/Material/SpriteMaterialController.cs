using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMaterialController : UIMaterialController
{
    [SerializeField]
    private Sprite _sprite;
    [SerializeField]
    private Sprite _highlightedSprite;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override bool HasMaterialColor
    {
        get {
            return (_spriteRenderer != null && _spriteRenderer.material != null);
        }
    }

    public override Color MaterialColor
    {
        get
        {
            if (HasMaterialColor)
                return _spriteRenderer.material.color;
            else
                return Color.red;
        }
        set
        {
            if (HasMaterialColor)
                _spriteRenderer.material.color = value;
        }
    }

    //also change the sprite
    public override void DoHighlight() {
        _spriteRenderer.sprite = _highlightedSprite;

        base.DoHighlight();
    }

    public override void DoUnhighlight()
    {
        _spriteRenderer.sprite = _sprite;

        base.DoUnhighlight();
    }
}
