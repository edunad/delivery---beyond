

using System.Linq;
using UnityEngine;

public class entity_box_pickup_container : MonoBehaviour {
    #region PRIVATE
        private entity_box_pickup_spot[] _boxesPickup;
        private entity_item_spot[] _boxSpots;
        private bool _hasDisabled;
    #endregion

    public void Awake () {
        this._boxesPickup = GetComponentsInChildren<entity_box_pickup_spot>(true);
        this._boxSpots = GetComponentsInChildren<entity_item_spot>(true);

        CoreController.Instance.OnGameStatusUpdated += this.gameStatusChange;
    }

    private void gameStatusChange(GAMEPLAY_STATUS prevStatus, GAMEPLAY_STATUS newStatus) {
        if(prevStatus == GAMEPLAY_STATUS.PREPARING && newStatus == GAMEPLAY_STATUS.IDLE){
            var codes = CoreController.Instance.boxCodes;
            var codeKeys = codes.Keys.ToList();

            if(this._boxesPickup.Length != codes.Count) throw new System.Exception("entity_box_pickup_spot do not match box codes length");
            if(this._boxSpots.Length != codes.Count) throw new System.Exception("entity_item_spot do not match box codes length");

            for(int i = 0; i < codeKeys.Count; i++) {
                this._boxesPickup[i].setID(codeKeys[i]);
                this._boxSpots[i].fixPosition();
            }
        }

        // Prob bad performance?
        foreach(Transform obj in this.transform) obj.gameObject.SetActive(newStatus == GAMEPLAY_STATUS.ITEM_RETRIEVE);
    }
}
