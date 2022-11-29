
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using Random = UnityEngine.Random;

public enum ShakeMode {
    SHAKE_UP = 1,
    SHAKE_DOWN = 2,
    SHAKE_LEFT = 3,
    SHAKE_RIGHT = 4,
    SHAKE_ALL = 5
}

public enum FrozenFlags {
	NONE = 0,
	OPTIONS = 1,
	DEAD = 1 << 1
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

	[HideInInspector]
	public entity_item big_holding_item;

	[HideInInspector]
	public entity_item small_holding_item;

	// PRIVATE ---
	#region PRIVATE
		#region CONTROLS
			private Controls _controls;
		#endregion

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
			private int _sensitivity;

		#endregion

		#region OTHER
			private FrozenFlags _frozen = FrozenFlags.NONE;
			private bool _sprintDown;
		#endregion
	#endregion


	private void onPrimaryUse(CallbackContext ctx) {
		if(this._frozen != FrozenFlags.NONE || !this.isHoldingItem()) return;

		if(this.small_holding_item != null) this.small_holding_item.BroadcastMessage("onPrimaryUse", this, SendMessageOptions.DontRequireReceiver);
		if(this.big_holding_item != null) this.big_holding_item.BroadcastMessage("onPrimaryUse", this, SendMessageOptions.DontRequireReceiver);
	}

	private void onUse(CallbackContext ctx) {
		if(this._frozen != FrozenFlags.NONE) return;

		RaycastHit hit;
		if (Physics.Raycast(this._camera.ScreenPointToRay(Input.mousePosition), out hit, maxGrabDistance, usableMask)) {
			this.onUse(hit.collider.gameObject); // USE
		}
	}

	private void onZoomStart(CallbackContext ctx) { this._camera.fieldOfView = this._originalCamZoom - this.maxZoom; }
	private void onZoomEnd(CallbackContext ctx) { this._camera.fieldOfView = this._originalCamZoom; }
	private void onSprintStart(CallbackContext ctx) { this._sprintDown = true; }
	private void onSprintEnd(CallbackContext ctx) { this._sprintDown = false; }
	private void onPause(CallbackContext ctx) { OptionsController.Instance.toggleOptions(); }

	public void Awake () {
		this._controls = new Controls();
		this._controls.Gameplay.PrimaryUse.performed += this.onPrimaryUse;
		this._controls.Gameplay.Use.performed += this.onUse;
		this._controls.Gameplay.Zoom.performed += this.onZoomStart;
		this._controls.Gameplay.Zoom.canceled += this.onZoomEnd;
		this._controls.Gameplay.Sprint.performed += this.onSprintStart;
		this._controls.Gameplay.Sprint.canceled += this.onSprintEnd;
		this._controls.Gameplay.Pause.performed += this.onPause;

		this._controller = GetComponent<CharacterController>();
		this._camera = GetComponentInChildren<Camera>(true);
		this._originalCamZoom = this._camera.fieldOfView;
		this._sensitivity = PlayerPrefs.GetInt("sensitivity", 3);

		// Hide cursor
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		this.name = "entity_player";

		OptionsController.Instance.OnSettingsUpdated += (string id, object val) => {
			if(id != "sensitivity") return;
			this._sensitivity = (int)val;
		};

		OptionsController.Instance.OnOptionsMenuUpdate += (bool open) => {
			this.setFrozen(FrozenFlags.OPTIONS, open);

			if(open) {
				Cursor.lockState = CursorLockMode.Confined;
				Cursor.visible = true;
			} else {
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		};
	}

	public void OnEnable() {
		if(this._controls == null) return;
		this._controls.Gameplay.Enable();
	}

	public void OnDisable() {
		if(this._controls == null) return;
		this._controls.Gameplay.Disable();
	}

	public void setFrozen(FrozenFlags flag, bool set) {
		if(!set) this._frozen &= ~flag;
		else this._frozen |= flag;
	}

	public void Update() {
		if(this._frozen != FrozenFlags.NONE) return;

		if(this._controller != null) {
			float speed = this._sprintDown ? runSpeed : moveSpeed;

			Vector2 plyMovement = this._controls.Gameplay.Move.ReadValue<Vector2>();
			this._controller.Move((transform.forward * plyMovement.y + transform.right * plyMovement.x).normalized * speed * Time.deltaTime);

			if (!this._controller.isGrounded) this._controller.Move(Vector3.up * Time.deltaTime * Physics.gravity.y);
		}

		if(this._camera != null) {
			// Camera movement
			Vector2 plyLook = this._controls.Gameplay.Look.ReadValue<Vector2>() * Time.smoothDeltaTime * this._sensitivity * 1f;

			this._camRotationY += plyLook.y;
			this._camRotationY = Mathf.Clamp(_camRotationY, -90f, 90f);

			this._camera.transform.localRotation = Quaternion.Euler(-_camRotationY, 0f, 0f);
			this.transform.Rotate(0, plyLook.x, 0);
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
