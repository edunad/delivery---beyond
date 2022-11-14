
using UnityEngine;

[RequireComponent(typeof(entity_item))]
public class entity_box : MonoBehaviour {

    [Header("Settings")]
    public Vector3 boxSize = Vector3.one;

    private entity_item _item;

    public void Awake() {
        this._item = GetComponent<entity_item>();

        this.name = "entity_box";
        this.transform.localScale = boxSize;
    }

}
