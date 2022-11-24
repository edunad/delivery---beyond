using System;
using UnityEngine.AI;

public static class util_ai {
	public static bool pathFinished(NavMeshAgent agent) {
		if (agent == null) return false;
		if (!agent.pathPending) {
			if (agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.pathStatus == NavMeshPathStatus.PathPartial) {
				return true; // Cannot reach
			}

			if (agent.remainingDistance <= agent.stoppingDistance) {
				if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
					return true;
				}
			}
		}

		return false;
	}
}