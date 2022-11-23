
using UnityEngine;

public class entity_airlock_control : MonoBehaviour {
    [Header("Settings")]
    public entity_movement entrance;
    public entity_movement exit;
    public entity_item_spot spot;

    public void Awake() {
        this.spot.OnItemPickup += this.itemPickup;
        this.spot.OnItemDrop += this.itemDrop;
    }

    private void itemPickup(entity_item itm) {
        this.entrance.reverse = true;
        this.entrance.start();

        this.exit.reverse = false;
        this.exit.start();
    }

    private void itemDrop(entity_item itm) {
        this.entrance.reverse = false;
        this.entrance.start();

        this.exit.reverse = true;
        this.exit.start();
    }

}
