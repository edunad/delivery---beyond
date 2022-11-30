
using TMPro;
using UnityEngine;

public class entity_scale_machine : MonoBehaviour {
    [Header("Settings")]
    public TextMeshPro scaleText;

    public entity_item_spot item_spot;
    public entity_item_spot item_floppy_spot;
    public entity_item_spot item_shipment_spot;
    public entity_computer computer;
    public entity_button btn_print;
    public entity_printer printer;

    #region PRIVATE
        private AudioClip[] _audioClips;
        private util_timer _floppyLoadingTimer;
        private GAME_REGIONS _selectedRegion;
        private entity_box _box;
    #endregion

    public void Awake() {
        this.item_spot.OnItemDrop += this.onBoxDrop;
        this.item_spot.OnItemPickup += this.onBoxPickup;

        this.item_floppy_spot.OnItemDrop += this.onFloppyDrop;
        this.item_floppy_spot.OnItemPickup += this.onFloppyPickup;
        this.item_floppy_spot.setLocked(false);

        this.item_shipment_spot.OnItemDrop += this.onShipmentDrop;

        this.item_spot.setLocked(true);

        this.btn_print.OnUSE += this.onButtonPress;
        this.btn_print.setButtonLocked(true);

        this.computer.OnCMDCompleted += this.onCMDPrinted;

        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Computer/floppy_insert"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Computer/floppy_eject"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Computer/floppy_access1"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Computer/floppy_access2"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Computer/floppy_access3"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Computer/floppy_access4"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Computer/floppy_access5"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Computer/floppy_access6"),
        };

        this.resetCounter();

        CoreController.Instance.OnGameStatusUpdated += this.gameStatusChange;
        CoreController.Instance.OnGamePowerStatusChange += this.onPowerChange;
    }

    private void onPowerChange(GAMEPLAY_POWER_STATUS status) {
        bool power = status == GAMEPLAY_POWER_STATUS.HAS_POWER;

        this.btn_print.resetCooldown = power ? -1f : 1f;
        this.scaleText.enabled = power;

        if(this._floppyLoadingTimer != null && !power) this._floppyLoadingTimer.stop();
    }

    private void gameStatusChange(GAMEPLAY_STATUS oldStatus, GAMEPLAY_STATUS newStatus) {
        if(newStatus == GAMEPLAY_STATUS.WEIGHT_ITEM) {
            this.item_spot.setLocked(false);
            this.item_shipment_spot.setLocked(true);
        } else {
            this.item_spot.setLocked(true);
            this.item_shipment_spot.setLocked(true);

            this.item_spot.deleteItem();
            this.item_shipment_spot.deleteItem();
        }
    }

    private void resetCounter() {
        this.scaleText.text = "000";
        this.computer.clear();
    }

    private void setCounter(int weight) {
        if(weight < 10) this.scaleText.text = "00" + weight.ToString();
        else if(weight >= 10 && weight < 100) this.scaleText.text = "0" + weight.ToString();
        else this.scaleText.text = weight.ToString();
    }

    private void onBoxDrop(entity_item itm) {
        entity_box box = itm.GetComponent<entity_box>();
        if(box == null) throw new System.Exception("Invalid item, missing entity_box");

        this.setCounter(box.weight);
        this._box = box;

        this.computer.queueCmd("$DETECTED ITEM WEIGHT: " + box.weight + "kg");
        this.computer.queueCmd("PLEASE INSERT REGION FLOPPY AND PRESS BUTTON");

        this.item_spot.setLocked(true);
        if(this.item_floppy_spot.hasItem()) this.onFloppyDrop(this.item_floppy_spot.item);
    }

    private void onFloppyDrop(entity_item itm) {
        entity_floppy floppy = itm.GetComponent<entity_floppy>();
        if(floppy == null) throw new System.Exception("Invalid item, missing entity_floppy");

        SoundController.Instance.Play3DSound(this._audioClips[0], this.transform);
        this.btn_print.setButtonLocked(this._box == null);
    }

    private void onFloppyPickup(entity_item itm) {
        SoundController.Instance.Play3DSound(this._audioClips[1], this.transform);
        this.btn_print.setButtonLocked(true);
    }

    private void onShipmentDrop(entity_item itm) {
        if(this._box == null) throw new System.Exception("Missing box on the shipment!");

        this.item_shipment_spot.deleteItem();
        this.item_shipment_spot.setLocked(true);

        this._box.setHasPaper(true);

        this.computer.queueCmd("$PLEASE RETRIEVE ITEM");
        this.item_spot.setLocked(false);
    }

    private void onBoxPickup(entity_item itm) {
        if(this._box == null) throw new System.Exception("Game did not follow the expected workflow!");
        this.resetCounter();

        this._box = null;
        this.item_spot.setLocked(true);
        this.item_floppy_spot.setLocked(false);

        CoreController.Instance.proccedEvent();
    }

    private void onCMDPrinted(string cmd) {
        if(cmd != "LOADING....") return;

        if(this._floppyLoadingTimer != null) {
            this._floppyLoadingTimer.stop();
            this._floppyLoadingTimer = null;
        }

        this.computer.queueCmd("SUCCESS");

        this.computer.queueCmd("\n");
        this.computer.queueCmd("PLEASE RETRIEVE RECEIPT FROM PRINTER");
        this.computer.queueCmd("AND PLACE IT ON THE ITEM");

        this.printer.print();
        this.item_shipment_spot.setLocked(false);
    }

    private void onButtonPress(entity_player ply) {
        if(CoreController.Instance.power == GAMEPLAY_POWER_STATUS.NO_POWER) return;
        if(this._box == null) throw new System.Exception("Missing box");

        this.item_floppy_spot.setLocked(true);

        entity_floppy floppy = this.item_floppy_spot.item.GetComponent<entity_floppy>();
        if(floppy == null) throw new System.Exception("Invalid item, missing entity_floppy");

        this._box.setRegion(floppy.region);

        this.computer.queueCmd("$PLEASE WAIT");
        this.computer.queueCmd("@LOADING....");

        if(this._floppyLoadingTimer != null) this._floppyLoadingTimer.stop();
        this._floppyLoadingTimer = util_timer.create(-1, 0.8f, () => {
            SoundController.Instance.Play3DSound(this._audioClips[Random.Range(2, 8)], this.transform);
        });
    }
}
