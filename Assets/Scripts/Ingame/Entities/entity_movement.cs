
using System.Collections.Generic;
using UnityEngine;

public class entity_movement : MonoBehaviour {
    public GameObject obj;

    [Header("Settings")]
    public List<Vector3> points = new List<Vector3>();
    public float speed = 1f;
    public bool isActive = false;
    public bool reverse = false;
    public bool loop = false;

    [Header("Sound")]
    public string stopSound = "silent";
    public string startSound = "silent";

    public delegate void onMovementFinish(bool reverse);
    public event onMovementFinish OnMovementFinish;

    // PRIVATE ---
    #region PRIVATE
        private int _pointIndex = 0;
        private float _time = 0;
        private AudioClip[] _audioClips;
    #endregion

    #if UNITY_EDITOR
        [EditorButton("Fix")]
        public void Fix() => this.reset();
    #endif

    public void Awake() {
        if(this.obj == null) throw new System.Exception("Missing game object");
        if(this.points.Count < 2) throw new System.Exception("At least 2 points are needed");

        this.obj.isStatic = false;

        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/" + this.startSound),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/" + this.stopSound)
        };

        this.reset();
    }

    public void start() {
        this.isActive = true;

        SoundController.Instance.Play3DSound(this._audioClips[0], this.transform);
        this.reset();
    }

    public void reset() {
        if(reverse) this._pointIndex = this.points.Count - 1;
        else this._pointIndex = 0;

        this._time = 0f;
        this.setPosition(this.points[this._pointIndex]);
    }

    private void onFinish(bool reverse) {
        this.isActive = false;

        SoundController.Instance.Play3DSound(this._audioClips[1], this.transform);
        if(OnMovementFinish != null) OnMovementFinish.Invoke(reverse);
    }

    public void Update() {
        if(!this.isActive) return;

        Vector3 pos = this.obj.transform.localPosition;
        Vector3 current = this.points[this._pointIndex];

        int nextPoint = this._pointIndex + (reverse ? -1 : 1);
        if(nextPoint < 0 || nextPoint >= this.points.Count) {
            this.isActive = false;
            Debug.LogWarning("[entity_movement] Something went wrong");
            return;
        }

        Vector3 dest = this.points[nextPoint];

        this._time += Mathf.Clamp(Time.fixedDeltaTime * this.speed, 0, 1f);
        this.setPosition(Vector3.Lerp(current, dest, this._time));

        if(this._time >= 1f) {
            this._pointIndex += reverse ? -1 : 1;

            if(reverse && this._pointIndex <= 0){
                if(!loop) {
                    this.onFinish(reverse);
                    return;
                }

                this._pointIndex = this.points.Count - 1;
            } else if(this._pointIndex >= this.points.Count - 1) {
                if(!loop) {
                    this.onFinish(reverse);
                    return;
                }

                this._pointIndex = 0;
            }

            this._time = 0f;
        }
    }

    private void setPosition(Vector3 pos) {
        if(this.obj.transform.parent != null) {
            this.obj.transform.localPosition = pos;
        } else {
            this.obj.transform.position = pos + this.transform.position;
        }
    }

    public void OnDrawGizmos() {
        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);

        for(int i = 0; i < this.points.Count; i++) {
            Vector3 p1 = this.points[i] + this.transform.position;
            Vector3 p2 = ((i + 1) > this.points.Count - 1 ? this.points[i] : this.points[i + 1]) + this.transform.position;

            Gizmos.DrawCube(p1, new Vector3(0.1f, 0.1f, 0.1f));
            Gizmos.DrawLine(p1, p2);
        }
    }
}
