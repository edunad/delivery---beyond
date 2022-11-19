
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Conversation {
    public string text;
    public float maxPitch;
    public float minPitch;

    public Conversation(string author, string text, float minPitch = 1f, float maxPitch = 1f) {
        this.text = author + ": " + text;
        this.minPitch = minPitch;
        this.maxPitch = maxPitch;
    }
}

[DisallowMultipleComponent]
public class ConversationController : MonoBehaviour {
	public static ConversationController Instance;

    public TextMeshPro text;
    public TextMeshPro textBG;

    [Header("Settings")]
    public List<AudioClip> talkSnd = new List<AudioClip>();
    public float talkSpeed = 0.3f;
    public float cooldown = 2f;

    [Header("Speaker")]
    public Transform speakerPosition;


    #region PRIVATE
        private Queue<Conversation> _talkQueue = new Queue<Conversation>();
        private Conversation _currentChat;
        private int _chatIndx;

        private util_timer charTimer;
        private util_timer cdTimer;
    #endregion

    public void Awake() {
		if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        /*this.queueConversation(new Conversation("KEK", "AAAAAAAA ALLALALALALA1"));
        this.queueConversation(new Conversation("MEOW", "MEOW MEMEMWE MEWMEOWMOW", 1.4f, 1.8f));
        this.queueConversation(new Conversation("WOOF", "BARK BBARKBARK AWOO BARK", 0.6f, 0.8f));*/
	}

    public void clear() {
        this._talkQueue.Clear();
        this.reset();
    }

    public void queueConversation(Conversation conv) {
        this._talkQueue.Enqueue(conv);
        if(this.charTimer == null && this.cdTimer == null) this.getNextConv(); // Start conversation
    }

    private void reset() {
        this.text.text = "";
        this.textBG.text = "";

        this._chatIndx = 0;
        this._currentChat = null;

        if (this.charTimer != null) this.charTimer.Stop();
        if (this.cdTimer != null) this.cdTimer.Stop();

        this.charTimer = null;
        this.cdTimer = null;
    }

    private void getNextConv() {
        this.reset();

        if (this._talkQueue == null || this._talkQueue.Count <= 0) return;
        this._currentChat = this._talkQueue.Dequeue();

        int strSize = this._currentChat.text.Length;
        this.charTimer = util_timer.Create(strSize, talkSpeed, () => {
            if(this._currentChat == null) return;

            this.text.text += this._currentChat.text[this._chatIndx];
            this.textBG.text = "<mark=#000000 padding=\"10, 10, 10, 0\">"+this.text.text+"</mark>";

            SoundController.Instance.Play3DSound(this.talkSnd[Random.Range(0, this.talkSnd.Count)], this.speakerPosition, Random.Range(this._currentChat.minPitch, this._currentChat.maxPitch));

            if (this._chatIndx < strSize - 1){
                this._chatIndx += 1;
            } else {
                this.cdTimer = util_timer.UniqueSimple("chat_cooldown", this.cooldown, () => {
                    this.getNextConv();
                });
            }
        });
    }
}
