
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class entity_item_trash : MonoBehaviour {

    [Header("Settings")]
    public bool disabled = false;
    public List<string> whitelist = new List<string>();

    #region EVENTS
        public delegate void onItemTrashed(entity_item item);
        public event onItemTrashed OnItemTrashed;
    #endregion

    #region PRIVATE
        private BoxCollider _trigger;
    #endregion

    public void Awake() {
        this._trigger = GetComponent<BoxCollider>();
        this._trigger.isTrigger = true;

        if(!this.name.StartsWith("itm-trash-")) this.name = "itm-trash-" + string.Join(",", this.whitelist.ToArray()) + "-" + this.name;
        this.gameObject.layer = 6;
    }

    public bool canTrash(string itemId) { return !this.disabled && this.whitelist.Contains(itemId); }
    public bool canTrash(entity_item itm) {
        if(itm == null) return false;
        return this.canTrash(itm.id);
    }

    public bool trashItem(entity_item item) {
        if(!this.canTrash(item.id)) return false;

        if(OnItemTrashed != null) OnItemTrashed.Invoke(item);
        DestroyImmediate(item.gameObject);

        return true;
    }

    /* *************
     * DEBUG
     ===============*/
    public void OnDrawGizmos() {
        if(this._trigger == null) this._trigger = GetComponent<BoxCollider>();

        Matrix4x4 original = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(this.transform.TransformPoint(this._trigger.center), this.transform.rotation, this.transform.lossyScale);
        Gizmos.color = Color.white;

        Gizmos.DrawWireCube(Vector3.zero, this._trigger.size);
        Gizmos.color = new Color(1f, 0f, 0, 0.5f);

        Gizmos.DrawCube(Vector3.zero, this._trigger.size);
        Gizmos.matrix = original;
    }
}
