
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class entity_trigger : MonoBehaviour {

    #region EVENTS
        public delegate void onTriggerEnter(Collider col);
        public event onTriggerEnter OnEnter;

        public delegate void onTriggerExit(Collider col);
        public event onTriggerExit OnExit;
    #endregion

    #region PRIVATE
        private Collider _trigger;
    #endregion

    public void Awake() {
        this._trigger = GetComponent<Collider>();
        this._trigger.isTrigger = true;

        if(!this.name.StartsWith("trigger-")) this.name = "trigger-" + this.name;
        this.gameObject.layer = 10; // Trigger
    }

    public void OnTriggerEnter(Collider col) {
        if(OnEnter == null) return;
        OnEnter.Invoke(col);
    }

    public void OnTriggerExit(Collider col) {
        if(OnExit == null) return;
        OnExit.Invoke(col);
    }
}
