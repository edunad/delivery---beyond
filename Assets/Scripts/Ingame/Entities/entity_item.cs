
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class entity_item : MonoBehaviour {

    [Header("Settings")]
    public string id;

    private GameObject _owner;
    private Rigidbody _body;
    private Collider _collision;

    public void Awake() {
        this._collision = GetComponent<Collider>();
        this._body = GetComponent<Rigidbody>();

        this.gameObject.layer = 2;
        this.name = "itm-" + this.id;
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
