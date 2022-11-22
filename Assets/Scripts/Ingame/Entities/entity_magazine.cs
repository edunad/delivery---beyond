
using UnityEngine;

public enum MAGAZINE_TYPE {
    GAME,
    SPORTS,
    NEWS,
    UNDERWORLD
}

[RequireComponent(typeof(entity_item))]
public class entity_magazine : MonoBehaviour {
    [Header("Settings")]
    public MAGAZINE_TYPE type;

    #region PRIVATE
        private entity_item _item;
        private MeshRenderer _renderer;
    #endregion

    public void Awake() {
        this._item = GetComponent<entity_item>();
    }
}
