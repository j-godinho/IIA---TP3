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
		int i = 0;
		while(float.IsNaN(pop[i].fitness) && i < pop.Count-1){
				if(!float.IsNaN(pop[i+1].fitness)){
					pop[0].fitness = pop[i+1].fitness;
					break;
				}
				else{
					i++;
				}
		}

		bestFitness.Add (currentGen, pop[0].fitness);
		meanFitness.Add (currentGen, 0f);
		int NaNNumbers = 0;
		foreach (Individual ind in pop) {
			if(!float.IsNaN(ind.fitness)){
					meanFitness[currentGen]+=ind.fitness;
			}
			else{
				NaNNumbers++;
			}

		}
		meanFitness [currentGen] /= (pop.Count - NaNNumbers);

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
