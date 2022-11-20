
using TMPro;
using UnityEngine;

public class entity_scale : MonoBehaviour {
    [Header("Settings")]
    public TextMeshPro text;

    public entity_item_spot item_spot;
    public entity_item_spot item_floppy_spot;
    public entity_computer computer;
    public entity_button btn_print;

    public void Awake() {
        CoreController.Instance.OnGameStatusUpdated += this.onStatusChange;

        this.item_spot.OnItemDrop += this.onBoxDrop;
        this.item_spot.OnItemPickup += this.onBoxPickup;

        this.item_spot.locked = true;
        this.btn_print.setButtonLocked(true);

        this.resetCounter();
    }

    private void onStatusChange(GAMEPLAY_STATUS status) {
        if(status == GAMEPLAY_STATUS.WEIGHT_ITEM) {
            this.computer.clear();
            this.item_spot.locked = false;
        } else {
            this.item_spot.locked = true;
        }
    }

    private void resetCounter() {
        this.text.text = "000";
        this.computer.clear();
    }

    private void onBoxDrop(entity_item itm) {
        entity_box box = itm.GetComponent<entity_box>();
        if(box == null) throw new System.Exception("Invalid item, missing entity_box");

        if(box.weight < 10) this.text.text = "00" + box.weight.ToString();
        else if(box.weight >= 10 && box.weight < 100) this.text.text = "0" + box.weight.ToString();
        else this.text.text = box.weight.ToString();

        this.item_spot.locked = true;

        this.computer.queueCmd("$> DETECTED ITEM WEIGHT: " + box.weight + "kg");
        this.computer.queueCmd("PLEASE INSERT COUNTRY FLOPPY AND PRESS BUTTON");
        this.computer.queueCmd("\n");
    }

    private void onBoxPickup(entity_item itm) { this.resetCounter(); }
}
