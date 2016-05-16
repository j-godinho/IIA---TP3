using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class StatisticsLogger {
	
	public Dictionary<int,float> bestFitness;
	public Dictionary<int,float> meanFitness;

	private string filename;
	private StreamWriter logger;


	public StatisticsLogger(string name) {
		filename = name;
		bestFitness = new Dictionary<int,float> ();
		meanFitness = new Dictionary<int,float> ();

	}

	//saves fitness info and writes to console
	public void PostGenLog(List<Individual> pop, int currentGen) {
		pop.Sort((x, y) => x.fitness.CompareTo(y.fitness));
	
		bestFitness.Add (currentGen, pop[0].fitness);
		meanFitness.Add (currentGen, 0f);

		foreach (Individual ind in pop) {
			meanFitness[currentGen]+=ind.fitness;
		}
		meanFitness [currentGen] /= pop.Count;

		Debug.Log ("generation: "+currentGen+"\tbest: " + bestFitness [currentGen] + "\tmean: " + meanFitness [currentGen]+"\n");
	}

	//writes to file
	public void finalLog() {
		logger = File.CreateText (filename);

		//writes with the following format: generation, bestfitness, meanfitness
		for (int i=0; i<bestFitness.Count; i++) {
			logger.WriteLine(i+" "+bestFitness[i]+" "+meanFitness[i]);
		}

		logger.Close ();
	}
}
