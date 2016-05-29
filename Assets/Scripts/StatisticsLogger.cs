using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class StatisticsLogger {

  public string values = "";
  public Dictionary<int,float> bestFitness;
  public Dictionary<int,float> meanFitness;

  public List<float> allBestsFitness = new List<float>();
  public List<float> allMeansFitness = new List<float>();
  public float bestsMean;
  private StreamWriter logger;

  private int n_test = 0;
  public StatisticsLogger() {
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
    if(n_test == 0){
      allBestsFitness.Add(bestFitness[currentGen]);
      allMeansFitness.Add(meanFitness[currentGen]);
    }
    else{
      allBestsFitness[currentGen] += bestFitness[currentGen];
      allMeansFitness[currentGen] += meanFitness[currentGen];
    }
    Debug.Log ("generation: "+currentGen+"\tbest: " + bestFitness [currentGen] + "\tmean: " + meanFitness [currentGen]+"\n");
  }

  //writes to file
  public void finalLog() {
    n_test++;
    //writes with the following format: generation, bestfitness, meanfitness
    int n_gen = bestFitness.Count;
    /*for (int i=0; i<n_gen; i++) {
      values += (i+" "+bestFitness[i]+" "+meanFitness[i] + "\n");
    }*/
  }

  public void writeStats(string header, int n_tests){
    int n_gen = allBestsFitness.Count;
    logger = File.CreateText ("Stats/stats" + (float)allBestsFitness[n_gen - 1]/n_tests);

    logger.WriteLine(header);

    for (int i=0; i<n_gen; i++) {
      values += (i+" "+(float)allBestsFitness[i]/n_tests+" "+(float)allMeansFitness[i]/n_tests + "\n");
    }

    logger.WriteLine("Best Result(mean): " + (float)allBestsFitness[n_gen -1]/n_tests + "\n");

    logger.WriteLine(values);

    logger.Close ();

  }
}
