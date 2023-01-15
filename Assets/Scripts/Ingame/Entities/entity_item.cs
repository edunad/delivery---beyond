
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
        private MeshRenderer[] _meshRenders;

        private Shader _originalShader;
        private Shader _frontShader;
        private bool _renderTop;
    #endregion

    public void Awake() {
        this._collision = GetComponent<Collider>();
        this._meshRenders = GetComponentsInChildren<MeshRenderer>(true);
        this._renderTop = false;

        this._originalShader = this._meshRenders[0].material.shader;
        this._frontShader = AssetsController.GetResource<Shader>("Shader/render_top");

        this.gameObject.layer = 2; // ignore raycast
        if(!this.name.StartsWith("itm-")) this.name = "itm-" + this.id + "-" + this.name;
    }

    public GameObject getOwner() { return this._owner; }
    public void setOwner(GameObject owner, Transform obj = null) {
        this._owner = owner;

        if(owner != null) {
            this.gameObject.layer = 2; // ignore raycast
            this.setRenderTop(owner.tag == "Player");

            this.transform.parent = obj != null ? obj : owner.transform; // Equip it
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.Euler(this.pickupAngle);
        } else {
            this.gameObject.layer = 6; // item_usable
            this.setRenderTop(false);

            this.transform.parent = null;
            this.transform.localRotation = Quaternion.identity;
            this.transform.rotation = Quaternion.identity;
        }
    }

    private void setRenderTop(bool top) {
        if(this._renderTop == top) return;

        foreach(MeshRenderer mesh in this._meshRenders) {
            mesh.rendererPriority = top ? mesh.rendererPriority + 1000 : mesh.rendererPriority - 1000;
            mesh.sortingOrder = top ? mesh.sortingOrder + 1000 : mesh.sortingOrder - 1000;
            /*foreach(Material mat in mesh.materials) {
                mat.shader = top ? this._frontShader : this._originalShader;
            }*/
        }

        this._renderTop = top;
    }
}