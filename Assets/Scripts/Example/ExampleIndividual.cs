using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ExampleIndividual : Individual {


	private float MinX;
	private float MaxX;
	private float MinY;
	private float MaxY;

	public ExampleIndividual(ProblemInfo info) : base(info) {

		MinX = info.startPointX;
		MaxX = info.endPointX;
		MaxY = info.startPointY > info.endPointY ? info.startPointY : info.endPointY;

		MinY = MaxY - 2 * (Mathf.Abs (info.startPointY - info.endPointY));
	}

	public override void Initialize() {
		RandomInitialization();
	}

	public override void Mutate(float probability) {
		NewValueMutation (probability);
	}

	public override void Crossover(Individual partner, float probability) {
		HalfCrossover (partner, probability);
	}

	public override void CalcTrackPoints() {
		//the representation used in the example individual is a list of trackpoints, no need to convert
	}

	public override void CalcFitness() {
		fitness = eval.time; //in this case we only consider time
	}


	public override Individual Clone() {
		ExampleIndividual newobj = (ExampleIndividual)this.MemberwiseClone ();
		newobj.fitness = 0f;
		newobj.trackPoints = new Dictionary<float,float> (this.trackPoints);
		return newobj;
	}



	void RandomInitialization() {
		float step = (info.endPointX - info.startPointX ) / (info.numTrackPoints - 1);
		float y = 0;

		trackPoints.Add (info.startPointX, info.startPointY);//startpoint
		for(int i = 1; i < info.numTrackPoints - 1; i++) {
			y = UnityEngine.Random.Range(MinY, MaxY);
			trackPoints.Add(info.startPointX + i * step, y);
		}
		trackPoints.Add (info.endPointX, info.endPointY); //endpoint
	}

	void NewValueMutation(float probability) {
		List<float> keys = new List<float>(trackPoints.Keys);
		foreach (float x in keys) {
			//make sure that the startpoint and the endpoint are not mutated 
			if(Math.Abs (x-info.startPointX)<0.01 || Math.Abs (x-info.endPointX)<0.01) {
				continue;
			}
			if(UnityEngine.Random.Range (0f, 1f) < probability) {
				trackPoints[x] = UnityEngine.Random.Range(MinY,MaxY);
			}
		}
	}

	void HalfCrossover(Individual partner, float probability) {

		if (UnityEngine.Random.Range (0f, 1f) > probability) {
			return;
		}
		//this example always splits the chromosome in half
		int crossoverPoint = Mathf.FloorToInt (info.numTrackPoints / 2f);
		List<float> keys = new List<float>(trackPoints.Keys);
		for (int i=0; i<crossoverPoint; i++) {
			float tmp = trackPoints[keys[i]];
			trackPoints[keys[i]] = partner.trackPoints[keys[i]];
			partner.trackPoints[keys[i]]=tmp;
		}

	}


}
