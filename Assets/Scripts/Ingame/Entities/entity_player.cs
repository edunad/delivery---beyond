
using UnityEngine;

public enum ShakeMode {
    SHAKE_UP = 1,
    SHAKE_DOWN = 2,
    SHAKE_LEFT = 3,
    SHAKE_RIGHT = 4,
    SHAKE_ALL = 5
}

[RequireComponent(typeof(CharacterController))]
public class entity_player : MonoBehaviour {

	[Header("Player")]
	public float moveSpeed = 3f;
	public float runSpeed = 5f;
	public float maxGrabDistance = 1f;
	public float maxZoom = 0.25f;

	public LayerMask usableMask = 1 << 6;
	public Transform big_item_position;
	public Transform small_item_position;

	[Header("Camera")]
	public float sensitivity = 10f;

	[HideInInspector]
	public entity_item big_holding_item;

	[HideInInspector]
	public entity_item small_holding_item;

	// PRIVATE ---
	#region PRIVATE
		#region SHAKE
			private bool _isCameraShaking;
			private ShakeMode _shakemode;
			private float _shakePower;
		#endregion

		#region OBJS
			private CharacterController _controller;
			private Camera _camera;
		#endregion

		#region CAMERA
			private float _camRotationY;
			private float _originalCamZoom;
		#endregion

		#region OTHER
			private bool _frozen;
		#endregion
	#endregion


    // Use this for initialization
	public void Awake () {
		this._controller = GetComponent<CharacterController>();

		this._camera = GetComponentInChildren<Camera>(true);
		this._originalCamZoom = this._camera.fieldOfView;

		// Hide cursor
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		this.name = "entity_player";
	}

	public void Update() {
		if(this._frozen) return;

		if(this._controller != null) {
			float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;
			this._controller.Move((transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal")).normalized * speed * Time.deltaTime);
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

			if(Input.GetMouseButtonDown(0) && this.isHoldingItem()) {
				if(this.small_holding_item != null) this.small_holding_item.BroadcastMessage("onPrimaryUse", this, SendMessageOptions.DontRequireReceiver);
				if(this.big_holding_item != null) this.big_holding_item.BroadcastMessage("onPrimaryUse", this, SendMessageOptions.DontRequireReceiver);
			}

			if(Input.GetMouseButton(1)) {
				this._camera.fieldOfView = this._originalCamZoom - this.maxZoom;
			} else {
				this._camera.fieldOfView = this._originalCamZoom;
			}
			// --------------

			if (this._isCameraShaking) {
       			Vector3 final = this._camera.transform.localPosition;

				switch (_shakemode) {
					case ShakeMode.SHAKE_ALL:
						final.z += Random.value * this._shakePower * 2 - this._shakePower;
						final.x += Random.value * this._shakePower * 2 - this._shakePower;
					break;
					case ShakeMode.SHAKE_UP:
						final.z += Random.value * this._shakePower * 2 - this._shakePower;
					break;
					case ShakeMode.SHAKE_DOWN:
						final.z -= Random.value * this._shakePower * 2 - this._shakePower;
					break;
					case ShakeMode.SHAKE_LEFT:
						final.x += Random.value * this._shakePower * 2 - this._shakePower;
					break;
					case ShakeMode.SHAKE_RIGHT:
						final.x -= Random.value * this._shakePower * 2 - this._shakePower;
					break;

					default:
					break;
				}

       			this._camera.transform.localPosition = final;
			}
		}
	}

	public void shakeCamera(float time, float power = 0.005f, ShakeMode shakemode = ShakeMode.SHAKE_ALL) {
		this._isCameraShaking = true;
		this._shakePower = power;
		this._shakemode = shakemode;

		util_timer.simple(time, () => {
			this._isCameraShaking = false;
			this._camera.transform.localPosition = Vector3.zero;
		});
	}

	public void freeze(bool set) {
		this._frozen = set;
	}

	public bool isHoldingItem() {
		return this.big_holding_item != null || this.small_holding_item != null;
	}

	public bool isHoldingItem(bool big) {
		if(big) return this.big_holding_item != null;
		else return this.small_holding_item != null;
	}

	private void holdObject(entity_item pick) {
		if(pick == null) return;

		bool isBig = pick.isBig;
		if(this.isHoldingItem(isBig)) return;

		Transform pos = small_item_position;
		if(isBig) {
			this.big_holding_item = pick;
			pos = big_item_position;
		} else {
 			this.small_holding_item = pick;
		}

		pick.setOwner(this.gameObject, pos);
	}

	public void onUse(GameObject obj) {
		if(obj == null) return;

		if(obj.name.StartsWith("itm-spot-")) {
			entity_item_spot spot = obj.GetComponent<entity_item_spot>();
			if(spot == null) throw new System.Exception("Invalid entity_item_spot");

			if(spot.hasItem()) {
				if(this.isHoldingItem() && !spot.item.supportMultiplePickup) return;

				if(this.isHoldingItem(spot.item.isBig)) return;
				this.holdObject(spot.takeItem());
			} else {
				if(this.isHoldingItem(false)) {
					if(spot.placeItem(this.small_holding_item)) this.small_holding_item = null;
				}

				if(this.isHoldingItem(true)) {
					if(spot.placeItem(this.big_holding_item)) this.big_holding_item = null;
				}
			}
		} else if(obj.name.StartsWith("itm-spawner-")) {
			entity_item_spawner spawner = obj.GetComponent<entity_item_spawner>();
			if(spawner == null) throw new System.Exception("Invalid entity_item_spawner");
			if(spawner.canSpawnItem()) {
				entity_item itm = spawner.getItem(spawner.template);

				if(this.isHoldingItem() && !itm.supportMultiplePickup) return;
				if(this.isHoldingItem(itm.isBig)) return;

				this.holdObject(spawner.takeItem(this.gameObject));
			}

		} else if(obj.name.StartsWith("itm-trash-")) {
			if(!this.isHoldingItem()) return;

			entity_item_trash trash = obj.GetComponent<entity_item_trash>();
			if(trash == null) throw new System.Exception("Invalid entity_item_trash");

			if(trash.canTrash(this.small_holding_item)) {
				trash.trashItem(this.small_holding_item);
				this.small_holding_item = null;
			}

			if(trash.canTrash(this.big_holding_item)) {
				trash.trashItem(this.big_holding_item);
				this.big_holding_item = null;
			}

		} else if(obj.name.StartsWith("btn-")) {
			if(this.isHoldingItem()) return;

			entity_button btn = obj.GetComponent<entity_button>();
			if(btn == null) throw new System.Exception("Invalid entity_button");

			btn.onPlayerUse(this);
		}
	}
}
