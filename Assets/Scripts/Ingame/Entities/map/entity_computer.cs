
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class entity_computer : MonoBehaviour {
    [Header("Command line Settings")]
    public TextMeshPro cmdLineText;
    public float writeSpeed;
    public float writeCooldown;


    #region PRIVATE
        private AudioClip[] _audioClips;

        #region CMD LINE
            private Queue<string> _cmds = new Queue<string>();
            private string _currentCMD;
            private int _chatIndx;
            private bool _started = false;
        #endregion
    #endregion

    public void Awake() {
        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Computer/beep_single")
        };

        this.clear();
    }

    public void queueCmd(string cmd) {
        this._cmds.Enqueue(cmd);
        if(!this._started) this.getNextCMD();
    }

    public void clear() {
        this.cmdLineText.text = "";
        this.reset();
    }

    private void reset() {
        this._chatIndx = 0;
        this._currentCMD = null;
    }

    private void getNextCMD() {
        this.reset();

        if (this._cmds == null || this._cmds.Count <= 0) {
            this._started = false; // Queue Done
            return;
        }

        this._started = true;

        string cmd = this._cmds.Dequeue();
        this._currentCMD = cmd.Replace("$", "");

        if(cmd.StartsWith("$")) {
            int strSize = this._currentCMD.Length;

            util_timer.create(strSize, this.writeSpeed, () => {
                if(this._currentCMD == null) return;

                this.cmdLineText.text += this._currentCMD[this._chatIndx];

                SoundController.Instance.Play3DSound(
                    this._audioClips[0],
                    this.transform,
                    Random.Range(1f, 1.1f),
                    3f, 0.2f);

                if (this._chatIndx < strSize - 1){
                    this._chatIndx += 1;
                } else {
                    this.cmdLineText.text += "\n\n";
                    util_timer.simple(this.writeCooldown, () => this.getNextCMD());
                }
            });
        } else {
            this.cmdLineText.text += this._currentCMD + "\n";

            SoundController.Instance.Play3DSound(
                    this._audioClips[0],
                    this.transform,
                    1.5f,
                    3f, 0.2f);

            util_timer.simple(this.writeCooldown, () => this.getNextCMD());
        }
    }
}
