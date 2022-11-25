
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class entity_monster : MonoBehaviour {
    [Header("Settings")]
    public float speed = 1.5f;
    public float thinkTime = 2f;

    [Header("Pathfinding")]
    public List<Vector3> points = new List<Vector3>();
    public Transform startPoint;

    #region PRIVATE
        private ParticleSystem _particles;
        private NavMeshAgent _agent;
        private bool _thinking;
    #endregion

    public void Awake() {
        this._particles = GetComponentInChildren<ParticleSystem>(true);

        this._agent = GetComponent<NavMeshAgent>();
        this._agent.updateRotation = false;
        this._agent.speed = this.speed;

        CoreController.Instance.OnGameStatusUpdated += this.gameStatusChange;
    }

    public void pickDestination() {
        Vector3 pos = points[Random.Range(0, points.Count)] + this.startPoint.position;
        this._agent.SetDestination(pos);
        this._thinking = false;

        Debug.Log("Moving monster to: " + pos);
    }

    public void Update(){
        if(this._agent == null || this._thinking) return;

        if(util_ai.pathFinished(this._agent)) {
            this._thinking = true;
            util_timer.simple(this.thinkTime, this.pickDestination);
        }
    }

    public void OnDrawGizmos() {
        Gizmos.color = new Color(1f, 0f, 0f, 1f);

        for(int i = 0; i < this.points.Count; i++) {
            if(i == this.points.Count - 1) Gizmos.color = new Color(1f, 0f, 1f, 1f);
            Gizmos.DrawCube(this.points[i] + this.startPoint.position, new Vector3(0.1f, 0.1f, 0.1f));
        }
    }

    private void gameStatusChange(GAMEPLAY_STATUS prevStatus, GAMEPLAY_STATUS newStatus) {
        this.gameObject.SetActive(newStatus == GAMEPLAY_STATUS.ITEM_RETRIEVE);
    }
}
