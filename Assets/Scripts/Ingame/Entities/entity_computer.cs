
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class entity_computer : MonoBehaviour {
    [Header("Settings")]
    public GameObject canvas;

    [Header("Command line Settings")]
    public TextMeshPro cmdLineTemplate;
    public float writeSpeed;
    public float slowWriteSpeed;
    public float writeCooldown;
    public int totalLines = 17;
    public delegate void onCMDCompleted(string cmd);
    public event onCMDCompleted OnCMDCompleted;

    #region PRIVATE
        private AudioClip[] _audioClips;
        private util_timer _timer;

        #region CMD LINE
            private Queue<string> _cmds = new Queue<string>();
            private List<TextMeshPro> _cmdLine = new List<TextMeshPro>();
            private int _chatIndx;
            private int _cmdCursor;
            private bool _started = false;
            private int _reservedLines;
        #endregion
    #endregion

    public void Awake() {
        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Computer/beep_single"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Computer/142608__autistic-lucario__error"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Computer/404358__kagateni__success.ogg")
        };

        this.generateCMDLine();

        CoreController.Instance.OnGamePowerStatusChange += this.onPowerChange;
    }

    private void onPowerChange(GAMEPLAY_POWER_STATUS status) {
        this.canvas.SetActive(status == GAMEPLAY_POWER_STATUS.HAS_POWER);
    }

    public void queueCmd(string cmd) {
        this._cmds.Enqueue(cmd);
        if(!this._started) this.getNextCMD();
    }

    public void setReserve(int lines) {
        this._reservedLines = lines;
    }

    public TextMeshPro getNextCmdLine() {
        TextMeshPro line = this._cmdLine[this._cmdCursor];
        line.text = ""; // Reset

        this._cmdCursor++;
        if(this._cmdCursor >= this._cmdLine.Count) {
            this._cmdCursor = this._reservedLines + 1;
        }

        return line;
    }

    public void clear() {
        foreach(TextMeshPro txt in this._cmdLine) txt.text = "";

        this._cmdCursor = 0;
        this._reservedLines = 0;
        this._started = false;

        this.reset();
    }

    private void generateCMDLine() {
        for(int i = 0; i < this.totalLines; i++) {
            GameObject instance = GameObject.Instantiate(
                this.cmdLineTemplate.gameObject,
                Vector3.zero,
                Quaternion.identity,
                this.canvas.transform
            );

            instance.transform.localPosition = new Vector3(0f, -i * 0.3f, 0f);

            TextMeshPro text = instance.GetComponent<TextMeshPro>();
            if(text == null) throw new System.Exception("Invalid command line template");

            this._cmdLine.Add(text);
        }
    }

    private void reset() {
        this._chatIndx = 0;

        if(this._timer != null) {
            this._timer.stop();
            this._timer = null;
        }
    }

    private void getNextCMD() {
        this.reset();

        if (this._cmds == null || this._cmds.Count <= 0) {
            this._started = false; // Queue Done
            return;
        }

        this._started = true;

        string cmd = this._cmds.Dequeue();
        string niceCMD = cmd.Replace("$", "").Replace("@", "").Replace("#", "").Replace("+", "");
        TextMeshPro cmdLineText = this.getNextCmdLine();

        if(cmd.StartsWith("#")) {
                cmdLineText.text += "<mark=#ff000007><color=\"red\">>  " + niceCMD + "  </color></mark>\n";

                if(this.canvas.activeInHierarchy) {
                    SoundController.Instance.Play3DSound(
                        this._audioClips[1],
                        this.transform,
                        1.5f,
                        4f, 0.7f);
                }

                util_timer.simple(this.writeCooldown, () => {
                    if(this.OnCMDCompleted != null) OnCMDCompleted.Invoke(niceCMD);
                    this.getNextCMD();
                });
        } else if(cmd.StartsWith("+")) {
                cmdLineText.text += "<mark=#00ff0007><color=\"green\">>  " + niceCMD + "  </color></mark>\n";

                if(this.canvas.activeInHierarchy) {
                    SoundController.Instance.Play3DSound(
                        this._audioClips[2],
                        this.transform,
                        1.5f,
                        4f, 0.7f);
                }

                util_timer.simple(this.writeCooldown, () => {
                    if(this.OnCMDCompleted != null) OnCMDCompleted.Invoke(niceCMD);
                    this.getNextCMD();
                });
        } else {
            if(cmd.StartsWith("$") || cmd.StartsWith("@")) {
                int strSize = niceCMD.Length;

                // PREFIX
                cmdLineText.text += "> ";
                // ---

                this._timer = util_timer.create(strSize, cmd.StartsWith("@") ? this.slowWriteSpeed : this.writeSpeed, () => {
                    cmdLineText.text += niceCMD[this._chatIndx];

                    if(this.canvas.activeInHierarchy) {
                        SoundController.Instance.Play3DSound(
                            this._audioClips[0],
                            this.transform,
                            Random.Range(1f, 1.1f),
                            4f, 0.3f);
                    }

                    if (this._chatIndx < strSize - 1){
                        this._chatIndx += 1;
                    } else {
                        cmdLineText.text += "\n";
                        this._timer = util_timer.simple(this.writeCooldown, () => {
                            if(this.OnCMDCompleted != null) OnCMDCompleted.Invoke(niceCMD);
                            this.getNextCMD();
                        });
                    }
                });
            } else {
                cmdLineText.text += niceCMD + "\n";

                if(niceCMD != "\n" && this.canvas.activeInHierarchy) {
                    SoundController.Instance.Play3DSound(
                            this._audioClips[0],
                            this.transform,
                            1.5f,
                            4f, 0.3f);
                }

                this._timer = util_timer.simple(this.writeCooldown, () => {
                    if(this.OnCMDCompleted != null) OnCMDCompleted.Invoke(niceCMD);
                    this.getNextCMD();
                });
            }
        }
    }
}
