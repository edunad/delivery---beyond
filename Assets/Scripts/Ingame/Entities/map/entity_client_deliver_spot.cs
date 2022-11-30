
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
        if(CoreController.Instance.status != GAMEPLAY_STATUS.ITEM_RETRIEVE || itm.id != "item_box_shipment") return;

        entity_customer client = CoreController.Instance.servingClient;
        if(client == null) throw new System.Exception("Missing client!");

        entity_box box = itm.gameObject.GetComponent<entity_box>();
        if(box == null) throw new System.Exception("Invalid box template, missing entity_box");

        this._spot.setLocked(true);

        util_timer.simple(0.5f, () => {
            if(box.ID != client.getSetting<int>("box_id")) {
                CoreController.Instance.penalize("Delivered Wrong box ID");
                // return; // TODO: Should it be harder? Forcing you to go back to basement and get the correct one?
            }

            this._spot.deleteItem();

            CoreController.Instance.setGameStatus(GAMEPLAY_STATUS.ITEM_REQUESTED);
            CoreController.Instance.proccedEvent();
        });
    }

    private void gameStatusChange(GAMEPLAY_STATUS prevStatus, GAMEPLAY_STATUS newStatus) {
        bool isOK = newStatus == GAMEPLAY_STATUS.WEIGHT_ITEM || newStatus == GAMEPLAY_STATUS.ITEM_RETRIEVE;

        this._spot.setLocked(!isOK);
        if(!isOK) this._spot.deleteItem();

        if(newStatus == GAMEPLAY_STATUS.WEIGHT_ITEM) {
            entity_customer customer = CoreController.Instance.servingClient;
            this.createItemBox(customer.getSetting<BoxSize>("box_size"), customer.getSetting<int>("box_weight"));
        }
    }
}
