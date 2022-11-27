
using System;
using System.Collections.Generic;
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
    public Dictionary<string, object> settings = new Dictionary<string, object>();

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
    #endregion

    public void init() {
        this.customerName = names[Random.Range(0, names.Count)];
        this.voice = Random.Range(0.25f, 1.85f);

        this._requests = new Queue<RequestType>(this.requestTemplate);
        this.generateRequests();
    }

    public T getSetting<T>(string id) {
        if(!this.settings.ContainsKey(id)) return default(T);
        return (T)this.settings[id];
    }

    private void generateRequests() {
        this.settings.Clear();

        foreach (RequestType request in this.requestTemplate) {
            if(request == RequestType.WANT_FLAT_BOX) {
                Array sizes = Enum.GetValues(typeof(BoxSize));
                this.settings.Add("box_size", (BoxSize)sizes.GetValue(Random.Range(0, sizes.Length)));
            } else if(request == RequestType.WANT_MAGAZINES) {
                Array magazines = Enum.GetValues(typeof(MAGAZINE_TYPE));
                this.settings.Add("magazine_type", (MAGAZINE_TYPE)magazines.GetValue(Random.Range(0, magazines.Length)));
            } else if(request == RequestType.WANT_SEND_BOX) {
                Array countries = Enum.GetValues(typeof(GAME_COUNTRIES));
                this.settings.Add("box_weight", Random.Range(2, 100));
                this.settings.Add("country", (GAME_COUNTRIES)countries.GetValue(Random.Range(0, countries.Length)));

                if(!this.settings.ContainsKey("box_size")) {
                    Array sizes = Enum.GetValues(typeof(BoxSize));
                    this.settings.Add("box_size", (BoxSize)sizes.GetValue(Random.Range(0, sizes.Length)));
                }
            } else if(request == RequestType.WANT_RETRIEVE_BOX) {
                this.settings.Add("box_id", CoreController.Instance.reserveBoxCode());
            }
        }
    }

    public bool hasRequest(RequestType type) { return this.requestTemplate.Contains(type); }
    public bool hasRequestsRemaining() { return this._requests != null && this._requests.Count > 0; }
    public void getRequest() {
        if(!this.hasRequestsRemaining()) return;
        this.currentRequest = this._requests.Dequeue();
    }

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

        if(list == null || list.Count <= 0) throw new System.Exception("Customer template missing chat templates for " + type + "!");
        list.ForEach((chat) => {
            string fixedChat = chat.Replace("%country%", this.getSetting<GAME_COUNTRIES>("country").ToString().Replace("_", " "));
            fixedChat = fixedChat.Replace("%box_size%", this.getSetting<BoxSize>("box_size").ToString().Replace("_", ""));
            fixedChat = fixedChat.Replace("%magazine_type%", this.getSetting<MAGAZINE_TYPE>("magazine_type").ToString());

            ConversationController.Instance.setConversationID(type.ToString());
            ConversationController.Instance.queueConversation(new Conversation(type.ToString(), this.customerName, fixedChat, this.voice - 0.10f, this.voice + 0.10f));
        });
    }
}
