
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class entity_computer : MonoBehaviour {
    [Header("Command line Settings")]
    public TextMeshPro cmdLineText;
    public float writeSpeed;
    public float slowWriteSpeed;
    public float writeCooldown;

    public delegate void onCMDCompleted(string cmd);
    public event onCMDCompleted OnCMDCompleted;

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
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Computer/beep_single"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Computer/142608__autistic-lucario__error")
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
        this._currentCMD = cmd.Replace("$", "").Replace("@", "").Replace("#", "");

        if(cmd.StartsWith("#")) {
                this.cmdLineText.text += "<mark=#ff000007><color=\"red\">>  " + this._currentCMD + "  </color></mark>\n";

                SoundController.Instance.Play3DSound(
                    this._audioClips[1],
                    this.transform,
                    1.5f,
                    4f, 0.7f);

                util_timer.simple(this.writeCooldown, () => {
                    if(this.OnCMDCompleted != null) OnCMDCompleted.Invoke(this._currentCMD);
                    this.getNextCMD();
                });
        } else {
            if(cmd.StartsWith("$") || cmd.StartsWith("@")) {
                int strSize = this._currentCMD.Length;

                // PREFIX
                this.cmdLineText.text += "> ";
                // ---

                util_timer.create(strSize, cmd.StartsWith("@") ? this.slowWriteSpeed : this.writeSpeed, () => {
                    if(this._currentCMD == null) return;

                    this.cmdLineText.text += this._currentCMD[this._chatIndx];

                    SoundController.Instance.Play3DSound(
                        this._audioClips[0],
                        this.transform,
                        Random.Range(1f, 1.1f),
                        4f, 0.3f);

                    if (this._chatIndx < strSize - 1){
                        this._chatIndx += 1;
                    } else {
                        this.cmdLineText.text += "\n";
                        util_timer.simple(this.writeCooldown, () => {
                            if(this.OnCMDCompleted != null) OnCMDCompleted.Invoke(this._currentCMD);
                            this.getNextCMD();
                        });
                    }
                });
            } else {
                this.cmdLineText.text += this._currentCMD + "\n";

                if(this._currentCMD != "\n") {
                    SoundController.Instance.Play3DSound(
                            this._audioClips[0],
                            this.transform,
                            1.5f,
                            4f, 0.3f);
                }

                util_timer.simple(this.writeCooldown, () => {
                    if(this.OnCMDCompleted != null) OnCMDCompleted.Invoke(this._currentCMD);
                    this.getNextCMD();
                });
            }
        }
    }
}
