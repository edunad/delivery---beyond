
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class entity_item : MonoBehaviour {

    [Header("Settings")]
    public string id;
    public bool isBig = false;

    private GameObject _owner;
    private Collider _collision;

    public void Awake() {
        this._collision = GetComponent<Collider>();

        this.gameObject.layer = 2;
        if(!this.name.StartsWith("itm-")) this.name = "itm-" + this.id + "-" + this.name;
    }

    public GameObject getOwner() { return this._owner; }
    public void setOwner(GameObject owner, Quaternion rotation, Transform obj = null) {
        this._owner = owner;

        if(owner != null) {
            this.gameObject.layer = 2;

            this.transform.parent = obj != null ? obj : owner.transform; // Equip it
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = rotation;
        } else {
            this.gameObject.layer = 6;
            this.transform.parent = null;
        }
    }
}
