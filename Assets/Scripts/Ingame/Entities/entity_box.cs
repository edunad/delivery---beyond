
using UnityEngine;

[RequireComponent(typeof(entity_item))]
public class entity_box : MonoBehaviour {
    [Header("Settings")]
    public BoxSize size = BoxSize._5x5x5;
    public int weight;

    [HideInInspector]
    public GAME_REGIONS region;

    [Header("Objects")]
    public GameObject paper;

    #region PRIVATE
        private entity_item _item;
        private Vector3 _paperScale;
    #endregion
    public void Awake() {
        this._item = GetComponent<entity_item>();
        this._paperScale = this.paper.transform.localScale;

        this.setHasPaper(false);
        this.scaleBox();
    }

    public void OnValidate() {
        this.scaleBox();
    }

    public void setHasPaper(bool paper) {
        this.paper.SetActive(paper);
    }

    public void setRegion(GAME_REGIONS region) {
        this.region = region;
    }

    public void setSize(BoxSize size) {
        this.size = size;
        this.scaleBox();

        this.paper.transform.localScale = this._paperScale;
    }

    public void setWeight(int weight) {
        this.weight = weight;
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
