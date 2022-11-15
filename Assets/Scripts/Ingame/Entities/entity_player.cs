
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class entity_player : MonoBehaviour {

	[Header("Player")]
	public float moveSpeed = 10f;
	public float maxGrabDistance = 1f;

	public LayerMask usableMask = 1 << 6;
	public GameObject big_item_position;
	public GameObject small_item_position;

	[Header("Camera")]
	public float sensitivity = 10f;

	[Header("Other")]
	public entity_item holdingItem; // TODO: Support multiple items?


    private CharacterController _controller;
    private Camera _camera;
	private float _camRotationY;
	private bool _frozen;

    // Use this for initialization
	public void Awake () {
		this._controller = GetComponent<CharacterController>();
		this._camera = GetComponentInChildren<Camera>();

		// Hide cursor
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		this.name = "entity_player";
	}

	public void Update() {
		if(this._frozen) return;

		if(this._controller != null) {
			this._controller.Move((transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal")).normalized * moveSpeed * Time.deltaTime);
			if (!this._controller.isGrounded) this._controller.Move(Vector3.up * Time.deltaTime * Physics.gravity.y);
		}

		if(this._camera != null) {
			// Camera movement
			_camRotationY += Input.GetAxis("Mouse Y") * sensitivity;
			_camRotationY = Mathf.Clamp(_camRotationY, -90f, 90f);

			this._camera.transform.localRotation = Quaternion.Euler(-_camRotationY, 0f, 0f);
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
			// --------------

			// INPUTS
			RaycastHit hit;
			if (Physics.Raycast(this._camera.ScreenPointToRay(Input.mousePosition), out hit, maxGrabDistance, usableMask)) {
				if(Input.GetKeyDown(KeyCode.E)) this.onUse(hit.collider.gameObject); // USE
				// TODO: Draw highlight on shader
			}

			if(this.holdingItem != null){
				if(Input.GetMouseButton(0)) {
					this.holdingItem.gameObject.BroadcastMessage("onPrimaryUse", this, SendMessageOptions.DontRequireReceiver);
				} else if(Input.GetMouseButton(1)) {
					this.holdingItem.gameObject.BroadcastMessage("onSecondaryUse", this, SendMessageOptions.DontRequireReceiver);
				}
			}
			// --------------
		}
	}

	public void freeze(bool set) {
		this._frozen = set;
	}

	public bool isHoldingItem() {
		return this.holdingItem != null;
	}

	private void holdObject(entity_item pick) {
		if(pick == null || this.isHoldingItem()) return;

		this.holdingItem = pick;

		Transform pos = small_item_position.transform;
		if(pick.isBig) pos = big_item_position.transform;

		this.holdingItem.setOwner(this.gameObject, pos.localRotation, pos);
	}

	public void onUse(GameObject obj) {
		if(obj == null) return;

		if(obj.name.StartsWith("itm-spot-")) {
			entity_item_spot spot = obj.GetComponent<entity_item_spot>();
			if(spot == null) throw new System.Exception("Invalid entity_item_spot");

			if(spot.hasItem()) {
				if(this.isHoldingItem()) return;
				this.holdObject(spot.takeItem());
			} else {
				if(!this.isHoldingItem()) return;
				if(spot.placeItem(this.holdingItem)) this.holdingItem = null;
			}
		} else if(obj.name.StartsWith("itm-spawner-")) {
			if(this.isHoldingItem()) return;

			entity_item_spawner spawner = obj.GetComponent<entity_item_spawner>();
			if(spawner == null) throw new System.Exception("Invalid entity_item_spawner");
			if(!spawner.canTakeItem(this.gameObject)) return;

			if(spawner.canSpawnItem()) this.holdObject(spawner.takeItem(this.gameObject));
		} else if(obj.name.StartsWith("itm-trash-")) {
			if(!this.isHoldingItem()) return;

			entity_item_trash trash = obj.GetComponent<entity_item_trash>();
			if(trash == null) throw new System.Exception("Invalid entity_item_trash");

			if(trash.canTrash(this.holdingItem.id)) trash.trashItem(this.holdingItem);
		} else {
			obj.BroadcastMessage("onPlayerUse", this, SendMessageOptions.RequireReceiver);
		}
	}

	/* *************
     * DEBUG
     ===============*/
    public void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
		//Gizmos.DrawLine(this.transform.position, this.transform.forward * this.maxGrabDistance);
        Gizmos.color = Color.white;
    }
}
