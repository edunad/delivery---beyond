
using UnityEngine;

public class entity_client_spot : MonoBehaviour {

    [Header("Settings")]
    public GameObject ticketTemplate;

    #region PRIVATE
        private entity_item_spot _spot;
        private GameObject _ticket;
    #endregion

    public void Awake() {
        this._spot = GetComponent<entity_item_spot>();
        this._spot.OnItemDrop += this.onItemDrop;
        this._spot.OnItemPickup += this.onItemPickup;
        this._spot.setLocked(true);

        CoreController.Instance.OnGameStatusUpdated += this.gameStatusChange;
    }

    private void onItemDrop(entity_item itm) {
        entity_customer customer = CoreController.Instance.servingClient;
        if(customer == null) throw new System.Exception("No customer set!");

        bool isOK = false;
        if(customer.currentRequest == RequestType.WANT_FLAT_BOX && itm.id == "item_flat_box") {
            entity_flat_box box = itm.GetComponent<entity_flat_box>();
            if(box != null && box.size == customer.getSetting<BoxSize>("box_size")) {
                isOK = true;
            }
        } else if(customer.currentRequest == RequestType.WANT_MAGAZINES && itm.id == "item_magazine") {
            entity_magazine magazine = itm.GetComponent<entity_magazine>();
            if(magazine != null && magazine.type == customer.getSetting<MAGAZINE_TYPE>("magazine")) {
                isOK = true;
            }
        }

        if(!isOK) {
            if(CoreController.Instance.penalize("Invalid given item")) {
                CoreController.Instance.servingClient.chat(ChatType.WRONG_ITEM);
            }
        } else {
            this._spot.setLocked(true);
            CoreController.Instance.proccedEvent();
        }
    }

    private void generateTicket() {
         if(this._ticket != null) throw new System.Exception("Already have a ticket template, forgot to delete?");

        this._ticket = GameObject.Instantiate(this.ticketTemplate, new Vector3(-300, 0, 0), Quaternion.identity);
        this._spot.placeItem(this._ticket, true);
    }

    private void onItemPickup(entity_item itm) {
        entity_customer customer = CoreController.Instance.servingClient;
        if(itm.id != "itm_miss_paper") return;

        DestroyImmediate(itm.gameObject);
        CoreController.Instance.proccedEvent();
    }

    private void gameStatusChange(GAMEPLAY_STATUS prevStatus, GAMEPLAY_STATUS newStatus) {
        bool isOK = (newStatus == GAMEPLAY_STATUS.ITEM_REQUESTED || newStatus == GAMEPLAY_STATUS.ITEM_WAIT_PLY_PICKUP);

        if(!isOK) this._spot.deleteItem();
        this._spot.setLocked(!isOK);

        if(newStatus == GAMEPLAY_STATUS.ITEM_WAIT_PLY_PICKUP) { this.generateTicket(); }
    }
}
