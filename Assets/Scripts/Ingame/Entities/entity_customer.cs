
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

public class entity_customer : MonoBehaviour {
    [Header("Request settings")]
    public List<RequestType> requestTemplate = new List<RequestType>();

    [Header("Chat settings")]
    public List<string> chatINTRO = new List<string>();
    public List<string> chatWRONG_ITEM = new List<string>();
    public List<string> chatOK_ITEM = new List<string>();
    public List<string> chatOUTRO = new List<string>();
    public List<string> chatPLACED_ITEM = new List<string>();
    public List<string> chatGAME_OVER = new List<string>(){"I want to talk to the manager"};

    [HideInInspector]
    public Dictionary<string, List<object>> settings = new Dictionary<string, List<object>>();

    [HideInInspector]
    public RequestType currentRequest;

    [HideInInspector]
    public float voice;
    [HideInInspector]
    public string customerName;

    #region GENERATORS
        private readonly static List<string> names = new List<string>() {
            "Reptifur",
            "Kooda",
            "D3lta",
            "Gathilo",
            "Bromvlieg",
            "Thomas Wake",
            "Goose",
            "Gnu Phelps",
            "Kaleb Bamantis",
            "Coyote",
            "Bryson Biraccoon",
            "Augustin Blanco",
            "Justin Tuwolf",
            "John Bowen",
            "John Swann",
            "Mash",
            "Risa",
            "Zari",
            "Tish",
            "Kama",
            "Lith",
            "Viro",
            "Kosh",
            "Lal",
            "Zazi"
        };
    #endregion

    #region PRIVATE
        private Queue<RequestType> _requests;
        private Dictionary<RequestType, int> _requestIndex = new Dictionary<RequestType, int>();
    #endregion

    public void init() {
        this.customerName = names[Random.Range(0, names.Count)];
        this.voice = Random.Range(0.25f, 1.85f);

        this._requests = new Queue<RequestType>(this.requestTemplate);
        this.generateRequests();
    }

    public T getSetting<T>(string id, int index) {
        if(!this.settings.ContainsKey(id)) return default(T);
        return (T)this.settings[id][index];
    }

    public T getSetting<T>(string id) {
        if(!this.settings.ContainsKey(id)) return default(T);
        return (T)this.settings[id][this.getRequestCount()];
    }

    public bool hasRequest(RequestType type) { return this.requestTemplate.Contains(type); }
    public bool hasRequestsRemaining() { return this._requests != null && this._requests.Count > 0; }
    public void getRequest() {
        if(!this.hasRequestsRemaining()) return;
        this.currentRequest = this._requests.Dequeue();

        if(!this._requestIndex.ContainsKey(this.currentRequest)) this._requestIndex.Add(this.currentRequest, 0);
        else this._requestIndex[this.currentRequest]++;
    }

    public int getRequestCount(RequestType type) { return this._requestIndex[type];}
    public int getRequestCount() { return this._requestIndex[this.currentRequest]; }

    public void chat(ChatType type) {
        List<string> list = null;
        switch(type) {
            case ChatType.INTRO:
                list = this.chatINTRO;
                break;
            case ChatType.WRONG_ITEM:
                list = this.chatWRONG_ITEM;
                break;
            case ChatType.OK_ITEM:
                list = this.chatOK_ITEM;
                break;
            case ChatType.OUTRO:
                list = this.chatOUTRO;
                break;
            case ChatType.PLACED_ITEM:
                list = this.chatPLACED_ITEM;
                break;

            case ChatType.GAME_OVER:
                list = this.chatGAME_OVER;
                break;
        }

        if(list == null || list.Count <= 0) return;

        list.ForEach((chat) => {
            ConversationController.Instance.setConversationID(type.ToString());
            ConversationController.Instance.queueConversation(new Conversation(type.ToString(), this.customerName, this.fixChat(chat), this.voice - 0.10f, this.voice + 0.10f));
        });
    }

    private string fixChat(string chat) {
        string fixedChat = chat;

        foreach(var setting in this.settings) {
            var data = setting.Value.ToList();
            for(int i = 0; i < data.Count; i++) {
                fixedChat = fixedChat.Replace("%" + setting.Key + "-" + i +"%", data[i].ToString().Replace("_", ""));
            }
        }

        return fixedChat;
    }

    private void addSetting(string id, object val) {
        if(!this.settings.ContainsKey(id)) this.settings.Add(id, new List<object>(){ val });
        else this.settings[id].Add(val);
    }

    private int hasSetting(string id) {
        if(!this.settings.ContainsKey(id)) return 0;
        return this.settings[id].Count;
    }

    private void generateRequests() {
        this.settings.Clear();

        foreach (RequestType request in this.requestTemplate) {
            if(request == RequestType.WANT_FLAT_BOX) {
                Array sizes = Enum.GetValues(typeof(BoxSize));
                this.addSetting("box_size", (BoxSize)sizes.GetValue(Random.Range(0, sizes.Length)));
            } else if(request == RequestType.WANT_MAGAZINES) {
                Array magazines = Enum.GetValues(typeof(MAGAZINE_TYPE));
                this.addSetting("magazine", (MAGAZINE_TYPE)magazines.GetValue(Random.Range(0, magazines.Length)));
            } else if(request == RequestType.WANT_SEND_BOX) {
                Array countries = Enum.GetValues(typeof(GAME_COUNTRIES));
                this.addSetting("box_weight", Random.Range(2, 100));
                this.addSetting("country", (GAME_COUNTRIES)countries.GetValue(Random.Range(0, countries.Length)));

                if(this.hasSetting("box_size") != this.hasSetting("country")) {
                    Array sizes = Enum.GetValues(typeof(BoxSize));
                    this.addSetting("box_size", (BoxSize)sizes.GetValue(Random.Range(0, sizes.Length)));
                }
            } else if(request == RequestType.WANT_RETRIEVE_BOX) {
                this.addSetting("box_id", CoreController.Instance.reserveBoxCode());
            }
        }
    }
}
