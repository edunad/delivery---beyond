
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class entity_teleport : MonoBehaviour {
    public Transform teleportDest;
    public LayerMask mask;

    private BoxCollider _trigger;

    public void Awake(){
        this._trigger = GetComponent<BoxCollider>();
        this._trigger.isTrigger = true;
    }

    public void teleport() {
        Collider[] objs = Physics.OverlapBox(this.transform.position, this._trigger.size, Quaternion.identity, this.mask);
        if(objs.Length <= 0) return;

        for(int i = 0; i < objs.Length; i++){
            Debug.Log("teleporting: " + objs[i].name);
            Vector3 localPos = this.transform.InverseTransformPoint(objs[i].transform.position);
            objs[i].transform.position = localPos + this.teleportDest.position;
            Physics.SyncTransforms();
        }
    }
}
