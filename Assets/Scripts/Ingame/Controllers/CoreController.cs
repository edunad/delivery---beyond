
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(-1)]
[DisallowMultipleComponent]
public class CoreController : MonoBehaviour {
    public static CoreController Instance { get; private set; }

    [Header("UI")]
    public ui_fade fade;

    [Header("Level settings")]
    public List<entity_customer> customerTemplates = new List<entity_customer>();
    public int maxMistakes = 3;
    public int patienceMult = 1;
    public int maxClients = 10;

    [Header("Level Entities")]
    public entity_movement prop_client;
    public entity_computer computer_client;
    public entity_button button_next_client;
    public Transform manager_position;

    #if UNITY_EDITOR
        [Header("DEBUG")]
        public GAMEPLAY_STATUS debugStatus = GAMEPLAY_STATUS.PREPARING;

        [EditorButton("Set")]
        public void Set() => this.setGameStatus(debugStatus);
    #endif

    #region GAME STATUS
        [HideInInspector]
        public Queue<entity_customer> customerQueue = new Queue<entity_customer>();
        [HideInInspector]
        public Dictionary<GAME_REGIONS, Tuple<List<GAME_COUNTRIES>, Color>> floppyCodes = new Dictionary<GAME_REGIONS, Tuple<List<GAME_COUNTRIES>, Color>>();
        [HideInInspector]
        public Dictionary<int, bool> boxCodes = new Dictionary<int, bool>();
        [HideInInspector]
        public entity_customer servingClient;
        [HideInInspector]
        public int totalClients;
        [HideInInspector]
        public GAMEPLAY_STATUS status = GAMEPLAY_STATUS.PREPARING;
        [HideInInspector]
        public GAMEPLAY_STATUS oldStatus = GAMEPLAY_STATUS.PREPARING;
        [HideInInspector]
        public int totalFails;
    #endregion

    #region EVENTS
        public delegate void onGameStatusUpdated(GAMEPLAY_STATUS oldStatus, GAMEPLAY_STATUS newStatus);
        public event onGameStatusUpdated OnGameStatusUpdated;
    #endregion


    public void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }else Instance = this;

        PlayerPrefs.SetInt("current_scene", SceneManager.GetActiveScene().buildIndex); // For gameover
        StartCoroutine(onLoaded());
    }

    private IEnumerator onLoaded() {
        yield return new WaitForEndOfFrame();
        this.init();
    }

    public void init() {
        this.generateQUEUE();
        this.generateFloppies();
        this.generateBoxCodes();

        this.setupEvents();
        this.setGameStatus(GAMEPLAY_STATUS.IDLE);

        Debug.Log("INIT");
    }

    public void Update() {
        util_timer.update();
    }

    public void OnDestroy() {
        util_timer.clear();
    }

    public bool penalize(string mistake) {
        if(this.servingClient == null) return false;

        if(this.totalFails >= this.maxMistakes) {
            this.gameOver(GAMEOVER_TYPE.CLIENT);
            return false;
        } else {
            this.computer_client.queueCmd("#" + mistake + " (" + (this.totalFails + 1) + " / " + this.maxMistakes + ")");
        }

        this.totalFails++;
        return true;
    }

    public void gameOver(GAMEOVER_TYPE type) {
        Debug.Log("GAMEOVER: " + type);

        this.setGameStatus(GAMEPLAY_STATUS.GAMEOVER);

        if(type == GAMEOVER_TYPE.CLIENT) {
            ConversationController.Instance.clear(false);
            this.servingClient.chat(ChatType.GAME_OVER);
        } else {
            UIController.Instance.spook.jumpscare();
            this.fadeToGameOver(1f); // Fast fade
        }
    }

    public void proccedEvent() {
        if(this.servingClient == null) throw new Exception("Missing client");

        if(status == GAMEPLAY_STATUS.ITEM_REQUESTED) {
            this.servingClient.chat(ChatType.OK_ITEM);

            util_timer.simple(0.5f, () => {
                this.setGameStatus(GAMEPLAY_STATUS.THINKING);

                util_timer.simple(2f, () => {
                    this.computer_client.queueCmd("$ITEM RECEIVED");

                    if(!this.servingClient.hasRequestsRemaining()) {
                        this.completeClient();
                    } else {
                        this.servingClient.getRequest(); // Fulfill all client requests first
                        this.clientLogic();
                    }
                });
            });
        } else if(status == GAMEPLAY_STATUS.WEIGHT_ITEM) {
            this.computer_client.queueCmd("$ITEM WEIGHTED");
            this.computer_client.queueCmd("──────────────────────────────────────────");
            this.computer_client.queueCmd("PLEASE PLACE ITEM ON THE SHIPPING ELEVATOR");

            this.setGameStatus(GAMEPLAY_STATUS.ITEM_SHIPPING);
        } else if(status == GAMEPLAY_STATUS.ITEM_WAIT_PLY_PICKUP) {
            this.computer_client.queueCmd("PICKUP ITEM ID '"+ this.servingClient.getSetting<int>("box_id") +"'");
            this.setGameStatus(GAMEPLAY_STATUS.ITEM_RETRIEVE);
        } else if(status == GAMEPLAY_STATUS.ITEM_SHIPPING) {
            this.setGameStatus(GAMEPLAY_STATUS.ITEM_REQUESTED);
            this.proccedEvent();
        }
    }

    public bool validateCountry(GAME_COUNTRIES country, GAME_REGIONS region) {
        return floppyCodes[region].Item1.Contains(country);
    }

    public int reserveBoxCode() {
        var keys = this.boxCodes.Keys.ToList();
        int key = keys[Random.Range(0, keys.Count)];
        while(this.boxCodes[key]) key = keys[Random.Range(0, keys.Count)];

        this.boxCodes[key] = true;
        return key;
    }

    public void setGameStatus(GAMEPLAY_STATUS status) {
        if(status == this.status) return;

        this.oldStatus = this.status;
        this.status = status;

        Debug.Log("Setting game status to: " + status.ToString());
        if(this.OnGameStatusUpdated != null) this.OnGameStatusUpdated.Invoke(this.oldStatus, this.status);
    }

    private void completeClient() {
        this.computer_client.queueCmd("$TASK COMPLETED");

        this.servingClient.chat(ChatType.OUTRO);
        this.servingClient = null;

        // Move client
        this.prop_client.reverse = true;
        this.prop_client.start();

        this.setGameStatus(GAMEPLAY_STATUS.IDLE);
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
        if(this.servingClient != null) throw new Exception("Already serving a customer!");

        if(this.customerQueue == null) throw new Exception("Missing customer queue");
        if(this.customerQueue.Count <= 0) return;

        this.servingClient = this.customerQueue.Dequeue();
        this.servingClient.init();
        this.servingClient.getRequest();

        // Move client
        this.prop_client.reverse = false;
        this.prop_client.start();

        this.totalClients++;
        this.setGameStatus(GAMEPLAY_STATUS.SERVING);
    }

    private void generateQUEUE() {
        this.customerQueue.Clear();

        for(int i = 0; i < this.maxClients; i++) {
            this.customerQueue.Enqueue(this.customerTemplates[Random.Range(0, this.customerTemplates.Count)]);
        }
    }

    private void generateBoxCodes() {
        this.boxCodes.Clear();

        for(int i = 0; i < 80; i++) {
            int id = Random.Range(1000,9999);
            while(this.boxCodes.ContainsKey(id)) id = Random.Range(1000,9999);
            this.boxCodes.Add(id, false);
        }
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
        if(!isReverse && this.servingClient != null) {
            util_timer.simple(0.5f, () => {
                this.servingClient.chat(ChatType.INTRO);

                this.computer_client.clear();
                this.computer_client.queueCmd("$CLIENT " + this.totalClients + " / " + this.maxClients);
            });
        } else if(isReverse && this.servingClient == null) {
            if(this.customerQueue.Count <= 0) {
                this.setGameStatus(GAMEPLAY_STATUS.WIN);
                util_timer.simple(2f, () => {
                    ConversationController.Instance.speakerPosition = manager_position;
                    ConversationController.Instance.clear(false);
                    ConversationController.Instance.setConversationID("WIN");
                    ConversationController.Instance.queueConversation(new Conversation("WIN", "MANAGER", "GOOD JOB, LOOKS LIKE YOU SURVIVE ANOTHER DAY", 0.45f, 0.6f));
                });
            } else {
                util_timer.simple(0.5f, () => {
                    this.computer_client.queueCmd("PLEASE CALL NEXT CLIENT");
                    this.button_next_client.setButtonLocked(false);
                });
            }
        }
    }

    private void onChatCompleted(string id) {
        if(id == "INTRO") {
            if(this.servingClient == null) throw new Exception("Missing customer");
            this.computer_client.queueCmd("REQUESTED ITEMS (IN ORDER) :");

            List<string> dt = new List<string>();
            Dictionary<RequestType, int> count = new Dictionary<RequestType, int>();

            foreach(RequestType request in this.servingClient.requestTemplate) {
                if(!count.ContainsKey(request)) count.Add(request, 0);
                else count[request]++;

                if(request == RequestType.WANT_FLAT_BOX) {
                    dt.Add("BOX SIZE '" + this.servingClient.getSetting<BoxSize>("box_size", count[request]).ToString().Replace("_", "") + "'");
                } else if(request == RequestType.WANT_MAGAZINES) {
                    dt.Add("MAGAZINE '" + this.servingClient.getSetting<MAGAZINE_TYPE>("magazine", count[request]).ToString() + "'");
                } else if(request == RequestType.WANT_SEND_BOX) {
                    dt.Add("SHIP BOX TO '" + this.servingClient.getSetting<GAME_COUNTRIES>("country", count[request]).ToString().Replace("_", " ") + "'");
                } else if(request == RequestType.WANT_RETRIEVE_BOX) {
                    dt.Add("RETRIEVE MISSED DELIVERY");
                }
            }

            for(int i = 0; i < dt.Count; i++) this.computer_client.queueCmd("   " + (i+1) + ". " + dt[i]);
            this.computer_client.queueCmd("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯");
            this.computer_client.setReserve(dt.Count + 2); // + Customer count + split

            // Check what he wants
            this.clientLogic();
        } else if(id == "GAME_OVER") {
            this.fadeToGameOver();
        } else if(id == "WIN") {
            this.fadeToVictory();
        }
    }

    private void clientLogic() {
        if(this.servingClient == null) throw new Exception("Missing customer");

        if(this.servingClient.currentRequest == RequestType.WANT_SEND_BOX) {
            this.servingClient.chat(ChatType.PLACED_ITEM);
            this.computer_client.queueCmd("───────────────────");
            this.computer_client.queueCmd("PLEASE WEIGHT ITEM");
            this.setGameStatus(GAMEPLAY_STATUS.WEIGHT_ITEM);
        } else if(this.servingClient.currentRequest == RequestType.WANT_RETRIEVE_BOX) {
            this.computer_client.queueCmd("──────────────────────────────");
            this.computer_client.queueCmd("$PLEASE PICKUP DELIVERY TICKET");
            this.setGameStatus(GAMEPLAY_STATUS.ITEM_WAIT_PLY_PICKUP);
        } else {
            this.setGameStatus(GAMEPLAY_STATUS.ITEM_REQUESTED);
        }
    }

    private void fadeToGameOver(float speed = 0.8f) {
        this.fade.fadeIn = true;
        this.fade.fadeDelay = 0f;
        this.fade.fadeSpeed = speed;
        this.fade.play();
        this.fade.OnFadeComplete += (bool fadein) => {
            if(!fadein) return;
            SceneManager.LoadScene("gameover", LoadSceneMode.Single);
        };
    }

    private void fadeToVictory() {
        this.fade.fadeIn = true;
        this.fade.fadeDelay = 0f;
        this.fade.fadeSpeed = 0.8f;
        this.fade.play();

        this.fade.OnFadeComplete += (bool fadein) => {
            if(!fadein) return;

            PlayerPrefs.SetInt("loading_scene_index", SceneManager.GetActiveScene().buildIndex + 1);
            SceneManager.LoadScene("loading", LoadSceneMode.Single);
        };
    }
}
