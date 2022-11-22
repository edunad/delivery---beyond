
using UnityEngine;

public class entity_client_deliver_spot : MonoBehaviour {
    public GameObject boxTemplate;

    #region PRIVATE
        private entity_item_spot _spot;
        private entity_box _box;
    #endregion

    public void Awake() {
        this._spot = GetComponent<entity_item_spot>();
        this._spot.OnItemDrop += this.onItemDrop;
        this._spot.setLocked(true);

        CoreController.Instance.OnGameStatusUpdated += this.gameStatusChange;
    }

    private void createItemBox(BoxSize size, int weight) {
        if(this._box != null) throw new System.Exception("Already have a box template, forgot to delete?");

        GameObject boxInstance = GameObject.Instantiate(this.boxTemplate, new Vector3(-300, 0, 0), Quaternion.identity);
        entity_box box = boxInstance.GetComponent<entity_box>();
        if(box == null) throw new System.Exception("Invalid box template, missing entity_box");

        box.setSize(size);
        box.setWeight(weight);

        this._box = box;
        this._spot.placeItem(boxInstance, true);
    }

    private void onItemDrop(entity_item itm) {

    }

    private void gameStatusChange(GAMEPLAY_STATUS prevStatus, GAMEPLAY_STATUS newStatus) {
        this._spot.setLocked(newStatus != GAMEPLAY_STATUS.WEIGHT_ITEM);
        if(newStatus == GAMEPLAY_STATUS.WEIGHT_ITEM) {
            entity_customer customer = CoreController.Instance.servingClient;
            this.createItemBox(customer.getSetting<BoxSize>("send_box_size"), customer.getSetting<int>("send_box_weight"));
        }
    }
}
