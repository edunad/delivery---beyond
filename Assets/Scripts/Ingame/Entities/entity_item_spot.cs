
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum AlignSpot {
    CENTER = 0,
    BOTTOM = 1
}

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider))]
public class entity_item_spot : MonoBehaviour {

    [Header("Settings")]
    public List<string> whitelist = new List<string>();
    public bool locked = false;

    [Header("Placement")]
    public AlignSpot placementPosition = AlignSpot.CENTER;
    public Quaternion placementAngle = Quaternion.identity;

    [Header("Item")]
    public entity_item item;

    [HideInInspector]
    public GameObject glue;

    #if UNITY_EDITOR
        [EditorButton("Setup")]
        public void Setup() => setup();
    #endif

    public delegate void onItemPickup(entity_item item);
    public event onItemPickup OnItemPickup;

    public delegate void onItemDrop(entity_item item);
    public event onItemDrop OnItemDrop;

    private BoxCollider _trigger;

    public void Awake() {
        this._trigger = GetComponent<BoxCollider>();
        this._trigger.isTrigger = true;

        if(!this.name.StartsWith("itm-spot-")) this.name = "itm-spot-" + string.Join(",", this.whitelist.ToArray()) + "-" + this.name;
        this.gameObject.layer = 6;
    }

    public bool canAcceptItem(entity_item pick) {
        return pick != null && this.item == null && this.whitelist.Contains(pick.id);
    }

    public void deleteItem() {
        if(this.item == null) return;

        DestroyImmediate(this.item.gameObject);
        if(OnItemPickup != null) OnItemPickup(this.item);

        this.item = null;
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

        this.item = itm;
        StartCoroutine(this.setItemToGlue(this.item)); // Do it next tick, so affects bounds.extends

        if(OnItemDrop != null) OnItemDrop(itm);
        return true;
    }

    private IEnumerator setItemToGlue(entity_item itm) {
        Collider collider = itm.gameObject.GetComponent<Collider>();
        if(collider == null) throw new System.Exception("Invalid entity_item, missing a collider");

        itm.setOwner(this.gameObject, this.glue.transform);
        yield return new WaitForFixedUpdate();

        Vector3 placementPos = Vector3.zero;
        switch(this.placementPosition) {
            case AlignSpot.CENTER:
                this.glue.transform.localPosition = placementPos;
                break;
            case AlignSpot.BOTTOM:
                Vector3 size = collider.bounds.extents;
                this.glue.transform.localPosition = new Vector3(placementPos.x, size.y + placementPos.y, placementPos.z);
                break;
        }
    }

    #if UNITY_EDITOR
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
            Gizmos.DrawCube(this.transform.position, new Vector3(0.1f, 0.1f, 0.1f));
        }

        private void setup() {
            if(transform.childCount != 0) {
                this.glue = transform.GetChild(0).gameObject;
            } else {
                this.glue = new GameObject();
                this.glue.transform.parent = this.transform;
                this.glue.name = "hold";
            }

            this.glue.transform.localPosition = Vector3.zero;
            this.glue.transform.localRotation = this.placementAngle;

            if(this.item != null) {
                Undo.RegisterCompleteObjectUndo(this.item, "Undo setup");
                EditorCoroutine.addCoroutine(this.setItemToGlue(this.item)); // Do it next tick, so affects bounds.extends
            }
        }
    #endif
}
