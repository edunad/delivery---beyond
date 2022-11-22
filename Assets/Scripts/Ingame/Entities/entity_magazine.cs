
using UnityEngine;

public enum MAGAZINE_TYPE {
    GAME,
    SPORTS,
    WORLD,
    UNDERWORLD
}

[RequireComponent(typeof(entity_item))]
public class entity_magazine : MonoBehaviour {
    [Header("Settings")]
    public MAGAZINE_TYPE type;
    private entity_item _item;

    public void Awake() {
        this._item = GetComponent<entity_item>();
    }
}
