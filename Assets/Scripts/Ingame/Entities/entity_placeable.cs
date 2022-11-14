
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider))]
public class entity_placeable : MonoBehaviour {

    public string acceptId;
    public entity_item item;
    public bool locked = false;

    [HideInInspector]
    public GameObject glue;

    [EditorButton("Setup")]
    public void Setup() => setup();

    public delegate void onItemPickup(entity_item item);
    public event onItemPickup OnItemPickup;

    public delegate void onItemDrop(entity_item item);
    public event onItemDrop OnItemDrop;


    private BoxCollider _trigger;

    public void Awake() {
        this._trigger = GetComponent<BoxCollider>();
        this._trigger.isTrigger = true;

        this.name = "place-" + this.acceptId;
        this.gameObject.layer = 6;
    }

    public bool canAcceptItem(entity_item pick) {
        return pick != null && this.item == null && pick.id == this.acceptId;
    }

    public bool isLocked() { return this.locked; }
    public bool hasItem() { return this.item != null; }
    public entity_item getItem() { return this.item; }
    public entity_item takeItem() {
        if(!this.hasItem() || this.isLocked()) return null;

        entity_item itm = this.item;
        this.item = null;

        if(OnItemPickup != null) OnItemPickup(itm);
        return itm;
    }

    public bool placeItem(entity_item itm) {
        if(!this.canAcceptItem(itm) || this.isLocked()) return false;

        itm.setOwner(this.gameObject, this.transform.rotation, this.glue.transform);
        this.item = itm;

        if(OnItemDrop != null) OnItemDrop(itm);
        return true;
    }

    /* *************
     * DEBUG
     ===============*/
    public void OnDrawGizmos() {
        if(this._trigger == null) return;

        Matrix4x4 original = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(this.transform.TransformPoint(this._trigger.center), this.transform.rotation, this.transform.lossyScale);
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(Vector3.zero, this._trigger.size);
        Gizmos.color = new Color(1f, 0.5f, 0, 0.5f);
        Gizmos.DrawCube(Vector3.zero, this._trigger.size);
        Gizmos.color = new Color(1f, 0.2f, 0, 1f);
        Gizmos.matrix = original;
    }

    private void setup() {
        if(transform.childCount != 0) {
            this.glue = transform.GetChild(0).gameObject;
        } else {
            this.glue = new GameObject();
            this.glue.transform.parent = this.transform;
            this.glue.name = "hold";
        }

        this.glue.transform.localPosition = this._trigger.center;
        if(this.item != null) this.item.setOwner(this.gameObject, this.transform.rotation, this.glue.transform);
    }
}
