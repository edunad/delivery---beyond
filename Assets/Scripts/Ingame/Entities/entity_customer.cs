
using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public enum RequestType {
    SEND_ITEM = 0,
    WANT_ITEM
}

public enum ChatType {
    INTRO = 0,
    WRONG_ITEM,
    OK_ITEM,
    OUTRO,
    DONE_ITEM,
}


public class entity_customer : MonoBehaviour {
    [Header("Request settings")]
    public RequestType type;

    [Header("Chat settings")]
    public List<string> chatINTRO = new List<string>();
    public List<string> chatWRONG_ITEM = new List<string>();
    public List<string> chatOK_ITEM = new List<string>();
    public List<string> chatOUTRO = new List<string>();
    public List<string> chatDONE_ITEM = new List<string>();

    [HideInInspector]
    public GAME_COUNTRIES country;
    [HideInInspector]
    public BoxSize boxSize;
    [HideInInspector]
    public int boxWeight;

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
        private float _voice;
    #endregion

    public void init() {
        this.name = names[Random.Range(0, names.Count)];

        Array countries = Enum.GetValues(typeof(GAME_COUNTRIES));
        this.country = (GAME_COUNTRIES)countries.GetValue(Random.Range(0, countries.Length));

        Array sizes = Enum.GetValues(typeof(BoxSize));
        this.boxSize = (BoxSize)sizes.GetValue(Random.Range(0, sizes.Length));
        this.boxWeight = Random.Range(2, 100);

        this._voice = Random.Range(0.25f, 1.85f);
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
            case ChatType.DONE_ITEM:
                list = this.chatDONE_ITEM;
                break;
        }

        if(list == null || list.Count <= 0) throw new System.Exception("Customer template missing chat templates for " + type + "!");
        list.ForEach((chat) => {
            string fixedChat = chat.Replace("%LOCATION%", this.country.ToString().Replace("_", " "));
            fixedChat = fixedChat.Replace("%BOX_SIZE%", this.boxSize.ToString().Replace("_", ""));

            ConversationController.Instance.setConversationID(type.ToString());
            ConversationController.Instance.queueConversation(new Conversation(type.ToString(), this.name, fixedChat, this._voice - 0.10f, this._voice + 0.10f));
        });
    }
}
