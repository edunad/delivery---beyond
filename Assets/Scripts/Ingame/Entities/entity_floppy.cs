
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(entity_item))]
public class entity_floppy : MonoBehaviour {
    public GAME_REGIONS region;

    private entity_item _item;
    private MeshRenderer _renderer;

    public void Awake() {
        this._item = GetComponent<entity_item>();
        this._renderer = GetComponent<MeshRenderer>();
    }

    public void setRegion(GAME_REGIONS region, Color cl) {
        this.region = region;
        this._renderer.material.SetColor("_FloppyColor", cl);
    }
}
