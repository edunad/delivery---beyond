
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(entity_item))]
public class entity_floppy : MonoBehaviour {
    public GAME_COUNTRIES country;

    private entity_item _item;
    private MeshRenderer _renderer;

    public void Awake() {
        this._item = GetComponent<entity_item>();
        this._renderer = GetComponent<MeshRenderer>();
    }

    public void setCountry(GAME_COUNTRIES country, Color cl) {
        this.country = country;
        this._renderer.material.color = cl;
    }
}
