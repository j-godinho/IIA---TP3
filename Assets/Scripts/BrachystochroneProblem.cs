using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct ProblemInfo {
	public float startPointX;
	public float startPointY;
	public float endPointX;
	public float endPointY;
	
	public float g;
	public float startVelocity;
	
	public int numTrackPoints;
}

public struct FitnessInfo {
	public float time;
	public float pointsCleared;
	public float distanceTraveled;
}


public class BrachystochroneProblem {

	public ProblemInfo info ;

	public BrachystochroneProblem(ProblemInfo inf){
		info = inf;
	}


	public FitnessInfo evaluate(Dictionary<float,float>trackpoints) {

		FitnessInfo eval = new FitnessInfo ();
		eval.time = 0;
		eval.pointsCleared = 0;
		float ParTime = 0;

		List<float> pointsX = new List<float> (trackpoints.Keys);
		pointsX.Sort ();

		float pointA = pointsX [0];
		float velocity = info.startVelocity;

		for (int i=1;i<pointsX.Count;i++) {

			float pointB = pointsX[i];

			float dX = pointB-pointA;
			float dY = trackpoints[pointB]-trackpoints[pointA];
			float l = Mathf.Sqrt (dX*dX + dY*dY);

			if (dY != 0) {
				ParTime = (-velocity + Mathf.Sqrt (velocity*velocity + 2 * info.g * dY)) / ((info.g * dY) / l);
			} else {
				ParTime = l / velocity;
			}

			if (ParTime <= 0) {
				eval.time=Mathf.Infinity;
				return eval;
			}

			velocity += (info.g*dY) / l * ParTime;
			if(velocity<0) {
				eval.time=Mathf.Infinity;
				return eval;
			}

			pointA=pointB;
			eval.time+=ParTime;
			eval.pointsCleared++;
			eval.distanceTraveled+=dX;
		}

		return eval;
	}
}
