
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class Conversation {
    public string id;

    public string text;
    public float maxPitch;
    public float minPitch;

    public Conversation(string id, string author, string text, float minPitch = 1f, float maxPitch = 1f) {
        this.id = id;

        this.text = author + ": " + text;
        this.minPitch = minPitch;
        this.maxPitch = maxPitch;
    }
}

[DisallowMultipleComponent]
public class ConversationController : MonoBehaviour {
    public static ConversationController Instance { get; private set; }

    public TextMeshPro text;
    public TextMeshPro textBG;

    [Header("Settings")]
    public List<AudioClip> talkSnd = new List<AudioClip>();
    public float talkSpeed = 0.3f;
    public float cooldown = 2f;

    [Header("Speaker")]
    public Transform speakerPosition;

    #region EVENTS
        public delegate void onConversationCompleted(string id);
        public event onConversationCompleted OnConversationCompleted;

        public delegate void onSingleConversationCompleted(string id);
        public event onSingleConversationCompleted OnSingleConversationCompleted;
    #endregion

    #region PRIVATE
        private Queue<Conversation> _talkQueue = new Queue<Conversation>();
        private Conversation _currentChat;
        private int _chatIndx;
        private bool _started = false;
        private string _currentChatID = "default";
    #endregion

    public void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }else Instance = this;
    }

    public void clear(bool reset = true) {
        this._talkQueue.Clear();
        if(reset) this.reset();
    }

    public void setConversationID(string id) {
        this._currentChatID = id;
    }

    public void queueConversation(Conversation conv) {
        this._talkQueue.Enqueue(conv);
        if(!this._started) this.getNextConv(); // Start conversation
    }

    private void reset() {
        this.text.text = "";
        this.textBG.text = "";

        this._chatIndx = 0;
        this._currentChat = null;
    }

    private void getNextConv() {
        this.reset();

        if (this._talkQueue == null || this._talkQueue.Count <= 0) {
            if(OnConversationCompleted != null) OnConversationCompleted.Invoke(this._currentChatID);

            this._currentChatID = "default";
            this._started = false;

            return;
        }

        this._started = true;
        this._currentChat = this._talkQueue.Dequeue();

        if(string.IsNullOrEmpty(this._currentChat.text)) {
            if(OnSingleConversationCompleted != null) OnSingleConversationCompleted.Invoke(this._currentChat.id);
            util_timer.simple(this.cooldown, () => this.getNextConv());
            return;
        }

        int strSize = this._currentChat.text.Length;
        util_timer.create(strSize, talkSpeed, () => {
            if(this._currentChat == null) return;

            this.text.text += this._currentChat.text[this._chatIndx];
            this.textBG.text = "<mark=#000000 padding=\"15, 15, 15, 0\">"+this.text.text+"</mark>";

            SoundController.Instance.Play3DSound(
                this.talkSnd[Random.Range(0, this.talkSnd.Count)],
                this.speakerPosition,
                Random.Range(this._currentChat.minPitch, this._currentChat.maxPitch),
                8f);

            if (this._chatIndx < strSize - 1){
                this._chatIndx += 1;
            } else {
                if(OnSingleConversationCompleted != null) OnSingleConversationCompleted.Invoke(this._currentChat.id);
                util_timer.simple(this.cooldown, () => this.getNextConv());
            }
        });
    }
}
