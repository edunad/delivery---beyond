
using UnityEngine;

[RequireComponent(typeof(entity_movement))]
public class entity_elevator : MonoBehaviour {

    public entity_button button;

    private entity_movement _elevatorGate;

    public void Awake() {
        this._elevatorGate = GetComponent<entity_movement>();
        this._elevatorGate.reverse = false;
        this._elevatorGate.OnMovementFinish += this.onGateMovementFinish;
    }

    public void onButtonPress() {
        this._elevatorGate.reverse = false;
        this._elevatorGate.start();
    }

    private void onGateMovementFinish() {

    }
}
