
using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public enum GAMEPLAY_STATUS {
    IDLE = 0,
    SERVING,
    FLAT_BOX_REQUESTED,
    WEIGHT_ITEM,
    PAYING
}

public enum GAME_COUNTRIES {
    ZUSTUANIA = 0,
    PUDROAQUE,
    YADRYA,
    UDRUS,
    SHOIDAL,
    FLIULAND,
    OSTRAX,
    ESCAD,
    PLUOD_FLINES,
    WHUAV_BLAD,
    CREATURE_LAND
}

[DisallowMultipleComponent]
[DefaultExecutionOrder(-2)]
public class CoreController : MonoBehaviour {
	public static CoreController Instance;

    [Header("Level settings")]
    public List<entity_customer> customerTemplates = new List<entity_customer>();
    public int maxMistakes = 3;
    public int patienceMult = 1;
    public int maxNPCS = 10;

    [Header("Level Spots")]
    public entity_item_spot client_spot;
    public entity_item_spot delivery_spot;

    [Header("Level Entities")]
    public entity_movement clientProp;
    public entity_computer computer;
    public entity_button counterBTN;

    [Header("Level Templates")]
    public GameObject boxTemplate;

    public delegate void onClientCompleted();
    public event onClientCompleted OnClientCompleted;

    public delegate void onGameStatusUpdated(GAMEPLAY_STATUS status);
    public event onGameStatusUpdated OnGameStatusUpdated;

    #region PRIVATE
        private Queue<entity_customer> _customerQueue = new Queue<entity_customer>();
        private Dictionary<GAME_COUNTRIES, Color> _floppyColors = new Dictionary<GAME_COUNTRIES, Color>();

        private entity_customer _servingClient;
        private int _totalClients;
        private GAMEPLAY_STATUS _status;
        private int _totalFails;
    #endregion

    public CoreController() { Instance = this; }

    public void Awake() {
        // SETUP
        for(int i = 0; i < this.maxNPCS; i++) this._customerQueue.Enqueue(this.customerTemplates[Random.Range(0, this.customerTemplates.Count)]);
        this.generateFloppies();
        // ----

        // Setup spots
        this.client_spot.locked = true;
        this.delivery_spot.locked = true;

        this.counterBTN.setButtonLocked(false);

        this.setGameStatus(GAMEPLAY_STATUS.IDLE);
        this.setupEvents();
    }

    public void requestNextClient() {
        if(this._customerQueue == null || this._customerQueue.Count <= 0) return; // Done serving clients
        if(this._servingClient != null) return; // Already serving

        this._servingClient = this._customerQueue.Dequeue();
        this._servingClient.init();

        // Move client
        this.clientProp.reverse = false;
        this.clientProp.start();

        this.setGameStatus(GAMEPLAY_STATUS.SERVING);
    }

    public void Update() {
        util_timer.update();
    }

    public void OnDestroy() {
        util_timer.clear();
    }

    private void setupEvents() {
        #region CORE EVENTS
            ConversationController.Instance.OnConversationCompleted += this.onChatCompleted;
        #endregion

        #region OBJECT EVENTS
            this.clientProp.OnMovementFinish += this.onClientMovementFinish;
        #endregion

        #region ITEM SPOT EVENTS
            this.client_spot.OnItemDrop += (entity_item itm) => this.onItemDrop("client_spot", itm);
            this.delivery_spot.OnItemDrop += (entity_item itm) => this.onItemDrop("delivery_spot", itm);
            /*this.client_spot.OnItemPickup += (entity_item itm) => this.onItemPickup("client_spot", itm);
            this.delivery_spot.OnItemPickup += (entity_item itm) => this.onItemPickup("delivery_spot", itm);*/
        #endregion
    }

    private void setGameStatus(GAMEPLAY_STATUS status) {
        if(this._status == status) return;

        this._status = status;
        if(this.OnGameStatusUpdated != null) this.OnGameStatusUpdated.Invoke(status);
    }

    private void generateFloppies() {
        List<Color> rndColors = new List<Color>(new[] {
            new Color(255, 82, 82, 255),// rgb(255, 82, 82)
            new Color(51, 217, 178, 255),// rgb(51, 217, 178)
            new Color(52, 172, 224, 255),// rgb(52, 172, 224)
            new Color(112, 111, 211, 255),// rgb(112, 111, 211)
            new Color(179, 55, 113, 255),// rgb(179, 55, 113)
            new Color(252, 66, 123, 255),// rgb(252, 66, 123)
            new Color(241, 196, 15, 255),// rgb(241, 196, 15)
            new Color(88, 177, 159, 255),// rgb(88, 177, 159)
            new Color(27, 20, 100, 255),// rgb(27, 20, 100)
            new Color(87, 88, 187, 255),// rgb(87, 88, 187)
            new Color(255, 255, 255, 255) // rgb(255, 255, 255)
        });

        Array countries = Enum.GetValues(typeof(GAME_COUNTRIES));
        for(int i = 0; i < countries.Length; i++) {
            int clIndx = Random.Range(0, rndColors.Count);
            this._floppyColors.Add((GAME_COUNTRIES)countries.GetValue(i), rndColors[clIndx] / 255f);
            rndColors.RemoveAt(clIndx);
        }


    }

    private void onClientMovementFinish(bool isReverse) {
        if(isReverse || this._servingClient == null) return;

        util_timer.simple(0.5f, () => {
            this._servingClient.chat(ChatType.INTRO);

            this.computer.clear();
            this.computer.queueCmd("$> CLIENT N#" + this._totalClients + 1);
        });
    }

    private void onItemDrop(string spotID, entity_item itm) {
        if(this._servingClient == null) return;

        if(this._status == GAMEPLAY_STATUS.FLAT_BOX_REQUESTED) {
            if(spotID != "client_spot") return;

            entity_flat_box box = this.getItmFlatBox(itm);
            if(box.size != this._servingClient.boxSize) {
                this._servingClient.chat(ChatType.WRONG_ITEM);
                this.onClientFailed("Wrong item delivered to customer");
            } else {
                this._servingClient.chat(ChatType.OK_ITEM);

                this.setGameStatus(GAMEPLAY_STATUS.WEIGHT_ITEM);
                this.delivery_spot.locked = true;

                util_timer.simple(0.5f, () => {
                    this.client_spot.deleteItem();

                    util_timer.simple(2f, () => {
                        this._servingClient.chat(ChatType.DONE_ITEM);
                        this.delivery_spot.locked = false;

                        this.computer.queueCmd("\n");
                        this.computer.queueCmd("$> ITEM RECEIVED");
                        this.computer.queueCmd("PLEASE WEIGHT ITEM");

                        // Create box
                        GameObject boxInstance = GameObject.Instantiate(this.boxTemplate, new Vector3(-300, 0, 0), Quaternion.identity);
                        entity_box box = boxInstance.GetComponent<entity_box>();
                        if(box == null) throw new System.Exception("Invalid box template, missing entity_box");

                        box.setSize(this._servingClient.boxSize);
                        box.setWeight(this._servingClient.boxWeight);
                        // ----

                        this.delivery_spot.placeItem(boxInstance);
                    });
                });
            }
        }
    }

    private void onClientFailed(string mistake) {
        if(this._servingClient == null) return;

        this._totalFails++;
        if(this._totalFails >= this.maxMistakes) {
            // TODO: GAME OVER
        } else {
            // TODO: Print mistake
            this.computer.queueCmd("$" + mistake);
        }
    }

    private entity_flat_box getItmFlatBox(entity_item itm) {
        entity_flat_box box = itm.GetComponent<entity_flat_box>();
        if(box == null) throw new System.Exception("Invalid entity_item, missing entity_flat_box");

        return box;
    }

    private void onChatCompleted(string id) {
        if(this._servingClient == null) return;

        if(id == "INTRO") {
            this.computer.queueCmd("LOCATION: " + this._servingClient.country);
            if(this._servingClient.type == RequestType.SEND_ITEM) {
                this.computer.queueCmd("REQUESTED ITEM :");
                this.computer.queueCmd("   1. BOX SIZE '" + this._servingClient.boxSize.ToString().Replace("_", "") + "'");

                this.client_spot.locked = false;
                this.setGameStatus(GAMEPLAY_STATUS.FLAT_BOX_REQUESTED);
            }
        }
    }
}
