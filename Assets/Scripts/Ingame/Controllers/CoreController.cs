
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CoreController : MonoBehaviour {
	public static CoreController Instance;

    [Header("Level settings")]
    public List<entity_customer> customerTemplates = new List<entity_customer>();
    public int maxMistakes = 3;
    public int patienceMult = 1;
    public int maxNPCS = 10;

    private Queue<entity_customer> _customerQueue = new Queue<entity_customer>();
    private entity_customer _servingClient;

    public void Awake() {
		if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        // Setup the queue
        for(int i = 0; i < this.maxNPCS; i++) this._customerQueue.Enqueue(this.customerTemplates[Random.Range(0, this.customerTemplates.Count)]);
	}

    public void requestNextClient() {
        if(this._servingClient != null) return; // Already serving

        this._servingClient = this._customerQueue.Dequeue();
        this._servingClient.init();
    }

    public void Update() {
        util_timer.Update();
    }

    public void OnDestroy() {
        util_timer.Clear();
    }
}
