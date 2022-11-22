
using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public enum GAMEPLAY_STATUS {
    PREPARING = 0,
    IDLE,
    SERVING,
    ITEM_REQUESTED,
    THINKING,
    WEIGHT_ITEM,
    COMPLETING
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
    CREATURE_LAND
}

public enum GAME_REGIONS {
    NORTH = 0,
    WEST,
    SOUTH,
    CENTER,
    EAST
}

[DisallowMultipleComponent]
[DefaultExecutionOrder(1)]
public class CoreController : MonoBehaviour {
	public static CoreController Instance;

    [Header("Level settings")]
    public List<entity_customer> customerTemplates = new List<entity_customer>();
    public int maxMistakes = 3;
    public int patienceMult = 1;
    public int maxClients = 10;

    [Header("Level Entities")]
    public entity_movement prop_client;
    public entity_computer computer_client;
    public entity_button button_next_client;

    public delegate void onClientCompleted();
    public event onClientCompleted OnClientCompleted;

    public delegate void onClientRequested();
    public event onClientRequested OnClientRequested;

    public delegate void onGameStatusUpdated(GAMEPLAY_STATUS oldStatus, GAMEPLAY_STATUS newStatus);
    public event onGameStatusUpdated OnGameStatusUpdated;

    #region GAME STATUS
        [HideInInspector]
        public Queue<entity_customer> customerQueue = new Queue<entity_customer>();
        [HideInInspector]
        public Dictionary<GAME_REGIONS, Tuple<List<GAME_COUNTRIES>, Color>> floppyCodes = new Dictionary<GAME_REGIONS, Tuple<List<GAME_COUNTRIES>, Color>>();
        [HideInInspector]
        public entity_customer servingClient;
        [HideInInspector]
        public int totalClients;
        [HideInInspector]
        public GAMEPLAY_STATUS status = GAMEPLAY_STATUS.PREPARING;
        [HideInInspector]
        public int totalFails;
    #endregion

    public CoreController() { Instance = this; }

    public void Awake() {
        this.generateQUEUE();
        this.generateFloppies();

        this.setupEvents();
        this.setGameStatus(GAMEPLAY_STATUS.IDLE);
    }

    public void Update() {
        util_timer.update();
    }

    public void OnDestroy() {
        util_timer.clear();
    }

    public void penalize(string mistake) {
        if(this.servingClient == null) return;

        this.totalFails++;
        if(this.totalFails > this.maxMistakes) {
            // TODO: GAME OVER
        } else {
            // TODO: Print mistake ticket
            this.computer_client.queueCmd("#" + mistake);
        }
    }

    public void proccedEvent() {
        if(status == GAMEPLAY_STATUS.ITEM_REQUESTED) {
            this.servingClient.chat(ChatType.OK_ITEM);

            util_timer.simple(0.5f, () => {
                this.setGameStatus(GAMEPLAY_STATUS.THINKING);

                util_timer.simple(2f, () => {
                    this.computer_client.queueCmd("$ITEM RECEIVED");

                    if(!this.servingClient.hasRequestsRemaining()) {
                        if(this.servingClient.hasRequest(RequestType.SEND_BOX)) {
                            this.setGameStatus(GAMEPLAY_STATUS.WEIGHT_ITEM);

                            this.computer_client.queueCmd("=--=--=--=--=");
                            this.computer_client.queueCmd("PLEASE WEIGHT ITEM");
                        } else if(this.servingClient.hasRequest(RequestType.WANT_BOX)) {
                            // TODO: GO GET BOX FROM BASEMENT
                        } else {
                            this.completeClient();
                        }
                    } else {
                        this.servingClient.getRequest(); // Fulfill all client requests first
                        this.setGameStatus(GAMEPLAY_STATUS.ITEM_REQUESTED);
                    }
                });
            });
        } else if(status == GAMEPLAY_STATUS.WEIGHT_ITEM) {
            this.computer_client.queueCmd("$ITEM WEIGHTED");
            this.computer_client.queueCmd("=--=--=--=--=");
            this.computer_client.queueCmd("PLEASE PLACE ITEM ON THE SHIPPING ELEVATOR");

            this.setGameStatus(GAMEPLAY_STATUS.COMPLETING);
        }else if(status == GAMEPLAY_STATUS.COMPLETING) {
            this.completeClient();
        }
    }

    private void completeClient() {
        this.computer_client.queueCmd("$TASK COMPLETED");
        this.servingClient.chat(ChatType.OUTRO);
        this.setGameStatus(GAMEPLAY_STATUS.IDLE);

        util_timer.simple(2f, () => {
            this.computer_client.queueCmd("CALL NEXT CLIENT");

            this.servingClient = null;
            this.button_next_client.setButtonLocked(false);
        });
    }

    public bool validateCountry(GAME_COUNTRIES country, GAME_REGIONS region) {
        return floppyCodes[region].Item1.Contains(country);
    }

    private void setupEvents() {
        #region CORE EVENTS
            ConversationController.Instance.OnConversationCompleted += this.onChatCompleted;
        #endregion

        #region OBJECT EVENTS
            this.prop_client.OnMovementFinish += this.onClientMovementFinish;
            this.button_next_client.OnUSE += this.onRequestNextClientBTN;
        #endregion
    }

    private void onRequestNextClientBTN(entity_player ply) {
        if(this.customerQueue == null || this.customerQueue.Count <= 0) return; // Done serving clients
        if(this.servingClient != null) return; // Already serving

        this.servingClient = this.customerQueue.Dequeue();
        this.servingClient.init();

        // Move client
        this.prop_client.reverse = false;
        this.prop_client.start();

        this.totalClients++;

        if(OnClientRequested != null) OnClientRequested.Invoke();
        this.setGameStatus(GAMEPLAY_STATUS.SERVING);
    }

    private void generateQUEUE() {
        this.customerQueue.Clear();

        for(int i = 0; i < this.maxClients; i++) {
            this.customerQueue.Enqueue(this.customerTemplates[Random.Range(0, this.customerTemplates.Count)]);
        }
    }

    private void setGameStatus(GAMEPLAY_STATUS status) {
        if(this.status == status) return;

        if(this.OnGameStatusUpdated != null) this.OnGameStatusUpdated.Invoke(this.status, status);
        this.status = status;
    }

    private void generateFloppies() {
        this.floppyCodes.Clear();

        List<Color> rndColors = new List<Color>(new[] {
            new Color(19, 140, 130, 255),// rgba(19, 140, 130)
            new Color(240, 238, 205, 255),// rgba(240, 238, 205)
            new Color(241, 156, 51, 255),// rgba(241, 156, 51)
            new Color(88, 70, 50, 255),// rgba(88, 70, 50)
            new Color(217, 75, 37, 255),// rgba(217, 75, 37)
        });

        Array regions = Enum.GetValues(typeof(GAME_REGIONS));
        Array countries = Enum.GetValues(typeof(GAME_COUNTRIES));

        if(regions.Length != rndColors.Count) throw new Exception("Region count does not match color count");

        for(int i = 0; i < regions.Length; i++) {
            int clIndx = Random.Range(0, rndColors.Count);

            GAME_REGIONS region = (GAME_REGIONS)regions.GetValue(i);

            GAME_COUNTRIES c1 = (GAME_COUNTRIES)countries.GetValue(i * 2);
            GAME_COUNTRIES c2 = (GAME_COUNTRIES)countries.GetValue(i * 2 + 1);

            Color color = rndColors[clIndx] / 255f;
            this.floppyCodes[region] = new Tuple<List<GAME_COUNTRIES>, Color>(new List<GAME_COUNTRIES>(){
                c1,
                c2
            }, color);

            rndColors.RemoveAt(clIndx);
        }
    }

    private void onClientMovementFinish(bool isReverse) {
        if(isReverse || this.servingClient == null) return;

        util_timer.simple(0.5f, () => {
            this.servingClient.chat(ChatType.INTRO);

            this.computer_client.clear();
            this.computer_client.queueCmd("$CLIENT " + this.totalClients + " / " + this.maxClients);
        });
    }

    private void onChatCompleted(string id) {
        if(this.servingClient == null) return;

        if(id == "INTRO") {
            this.computer_client.queueCmd("REQUESTED ITEMS (IN ORDER) :");

            List<string> dt = new List<string>();
            foreach(RequestType request in this.servingClient.requestTemplate) {
                if(request == RequestType.SEND_BOX) {
                    dt.Add("BOX SIZE '" + this.servingClient.getSetting<BoxSize>("send_box_size").ToString().Replace("_", "") + "'");
                } else if(request == RequestType.WANT_MAGAZINES) {
                    dt.Add("MAGAZINE '" + this.servingClient.getSetting<MAGAZINE_TYPE>("magazine_type").ToString() + "'");
                } else if(request == RequestType.WANT_BOX) {
                    // TODO
                }
            }

            for(int i = 0; i < dt.Count; i++) this.computer_client.queueCmd("   "+(i+1)+". " + dt[i]);
            this.setGameStatus(GAMEPLAY_STATUS.ITEM_REQUESTED);
        }
    }
}
