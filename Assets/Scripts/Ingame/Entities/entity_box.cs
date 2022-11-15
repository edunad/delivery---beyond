
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(entity_item))]
public class entity_box : MonoBehaviour {

    public BoxSize size = BoxSize._5x5x5;

    private entity_item _item;
    public void Awake() {
        this._item = GetComponent<entity_item>();
        this.scaleBox();
    }

    public void OnValidate() {
        this.scaleBox();
    }

    private void scaleBox() {
        switch(size) {
            case BoxSize._7x6x3:
                this.transform.localScale = new Vector3(2.7f, 2.4f, 1.8f);
                break;
            case BoxSize._4x7x7:
                this.transform.localScale = new Vector3(2f, 2.7f, 2.7f);
                break;
            case BoxSize._7x3x7:
                this.transform.localScale = new Vector3(2.7f, 1.8f, 2.7f);
                break;


            case BoxSize._5x5x5:
                this.transform.localScale = new Vector3(2.2f, 2.2f, 2.2f);
                break;
            case BoxSize._4x4x5:
                this.transform.localScale = new Vector3(2f, 2f, 2.2f);
                break;
            case BoxSize._5x3x2:
                this.transform.localScale = new Vector3(2f, 1.8f, 1.2f);
                break;
            case BoxSize._3x2x3:
                this.transform.localScale = new Vector3(1.8f, 1.2f, 1.8f);
                break;

            default:
                throw new System.Exception("Invalid box size");
        }
    }
}
