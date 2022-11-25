
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider))]
public class entity_item_spawner : MonoBehaviour {

    [Header("Settings")]
    public bool disabled = false;
    public int count = -1;

    [Header("Item")]
    public GameObject template;

    private BoxCollider _trigger;

    public void Awake() {
        this._trigger = GetComponent<BoxCollider>();
        this._trigger.isTrigger = true;

        if(this.template == null) throw new System.Exception("Missing item template");
        if(!this.name.StartsWith("itm-spawner-")) this.name = "itm-spawner-" + this.template.name + "-" + this.name;
        this.gameObject.layer = 6;
    }

    public bool isInfinite() { return this.count == -1; }
    public bool canSpawnItem() { return this.disabled || this.isInfinite() || this.count > 0; }
    public void setDisabled(bool disabled) { this.disabled = disabled; }
    public entity_item takeItem(GameObject taker) {
        if(!this.canSpawnItem()) return null;

        GameObject instance = GameObject.Instantiate(this.template);
        instance.SetActive(true);

        if(!this.isInfinite()) this.count = Mathf.Min(this.count - 1, 0);
        return this.getItem(instance);
    }

    public entity_item getItem(GameObject obj) {
        entity_item itm = obj.GetComponent<entity_item>();
        if(itm == null) throw new System.Exception("Invalid item template, missing entity_item");
        return itm;
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
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);

        Gizmos.DrawCube(Vector3.zero, this._trigger.size);
        Gizmos.matrix = original;
    }
}
