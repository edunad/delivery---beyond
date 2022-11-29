
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class entity_item : MonoBehaviour {

    [Header("Settings")]
    public string id;
    public bool isBig = false;
    public bool supportMultiplePickup = false;
    public Vector3 pickupAngle = Vector3.zero;

    #region PRIVATE
        private GameObject _owner;
        private Collider _collision;
    #endregion

    public void Awake() {
        this._collision = GetComponent<Collider>();

        this.gameObject.layer = 2;
        if(!this.name.StartsWith("itm-")) this.name = "itm-" + this.id + "-" + this.name;
    }

    public GameObject getOwner() { return this._owner; }
    public void setOwner(GameObject owner, Transform obj = null) {
        this._owner = owner;

        if(owner != null) {
            this.gameObject.layer = 2;

            this.transform.parent = obj != null ? obj : owner.transform; // Equip it
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.Euler(this.pickupAngle);
        } else {
            this.gameObject.layer = 6;
            this.transform.parent = null;
            this.transform.localRotation = Quaternion.identity;
            this.transform.rotation = Quaternion.identity;
        }
    }
}
