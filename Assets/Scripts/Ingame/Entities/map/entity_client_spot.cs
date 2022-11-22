
using UnityEngine;

public class entity_client_spot : MonoBehaviour {

    #region PRIVATE
        private entity_item_spot _spot;
    #endregion

    public void Awake() {
        this._spot = GetComponent<entity_item_spot>();
        this._spot.OnItemDrop += this.onItemDrop;
        this._spot.setLocked(true);

        CoreController.Instance.OnGameStatusUpdated += this.gameStatusChange;
    }

    private void onItemDrop(entity_item itm) {
        entity_customer customer = CoreController.Instance.servingClient;

        bool isOK = false;
        if(customer.currentRequest == RequestType.WANT_FLAT_BOX && itm.id == "item_flat_box") {
            entity_flat_box box = itm.GetComponent<entity_flat_box>();
            if(box != null && box.size == customer.getSetting<BoxSize>("box_size")) {
                isOK = true;
            }
        } else if(customer.currentRequest == RequestType.WANT_MAGAZINES && itm.id == "item_magazine") {
            entity_magazine magazine = itm.GetComponent<entity_magazine>();
            if(magazine != null && magazine.type == customer.getSetting<MAGAZINE_TYPE>("magazine_type")) {
                isOK = true;
            }
        }

        if(!isOK) {
            CoreController.Instance.penalize("Invalid given item");
            CoreController.Instance.servingClient.chat(ChatType.WRONG_ITEM);
        } else {
            this._spot.setLocked(true);
            CoreController.Instance.proccedEvent();
        }
    }

    private void gameStatusChange(GAMEPLAY_STATUS prevStatus, GAMEPLAY_STATUS newStatus) {
        bool isOK = (newStatus == GAMEPLAY_STATUS.ITEM_REQUESTED);
        if(!isOK) this._spot.deleteItem();
        this._spot.setLocked(!isOK);
    }
}
