
using TMPro;
using UnityEngine;

public class ui_hint_scroller : MonoBehaviour {

    [Header("Hint settings")]
    public string text = "";
    public float speed = 0.2f;
    public bool isEnabled = true;

    private TextMeshPro _text;
    private TextMeshPro _cloneText;

    private RectTransform _rectTransform;
    private Vector2 _startPos;

    private float _sizeW;
    private float _scrollPos;

    public void Awake () {
        // Setup
        this._text = GetComponentInChildren<TextMeshPro>();
        this._text.SetText(this.text);
        this._text.SetLayoutDirty();

        // Get rect
        this._rectTransform = this._text.GetComponent<RectTransform>();

        // Create clone
        GameObject parent = this._text.transform.parent.gameObject;
        this._cloneText = Instantiate<TextMeshPro>(this._text);
        this._cloneText.SetText(this.text);

        RectTransform cloneRect = this._cloneText.GetComponent<RectTransform>();
        cloneRect.SetParent(this._rectTransform);
        cloneRect.anchorMin = new Vector2(1, 0.5f);
        cloneRect.localScale = new Vector3(1f, 1f, 1f);

        cloneRect.transform.localPosition = Vector3.zero;
        cloneRect.anchoredPosition = new Vector2(10f, 0f);

        // vars
        this._sizeW = this._text.preferredWidth + cloneRect.transform.localPosition.x;
        this._startPos = this._rectTransform.anchoredPosition;
        this._scrollPos = 0;
    }

    // Update is called once per frame
    public void Update () {
        if (!this.isEnabled) return;
        this._rectTransform.anchoredPosition = new Vector2(this._startPos.x - this._scrollPos, this._startPos.y);
        this._scrollPos = (this._scrollPos % this._sizeW) + this.speed;
    }
}
