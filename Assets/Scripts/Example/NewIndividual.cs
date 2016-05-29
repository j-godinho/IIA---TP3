using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class NewIndividual : Individual {


  private float MinX;
  private float MaxX;
  private float MinY;
  private float MaxY;

  public List<float> deltas;

  public NewIndividual(ProblemInfo info) : base(info) {
    
    MinX = info.startPointX;
    MaxX = info.endPointX;
    MaxY = info.startPointY > info.endPointY ? info.startPointY : info.endPointY;
    
    MinY = MaxY - 2 * (Mathf.Abs (info.startPointY - info.endPointY));
    deltas = new List<float>();
  }
  
  public override void Initialize() {
    this.deltas.Clear();
    trackPoints.Clear();
    RandomInitialization();
  }

  public override void Mutate(float probability, bool gaussian) {
    if(gaussian)
      ValueMutationGaussian(probability);
    else
      NewValueMutation (probability);
  }
  
  public override void Crossover(Individual partner, float probability, int n) {

    if (n != 0) {
      NCrossover (partner, probability, n);
    } else {
      HalfCrossover (partner, probability);
    }
    
  }
  
  public override void CalcTrackPoints() {
    trackPoints.Clear();
    float step = (info.endPointX - info.startPointX) / (info.numTrackPoints - 1);
    trackPoints.Add(info.startPointX, info.startPointY);
    float lastPointX = info.startPointX;
    for(int i=0;i<deltas.Count;i++){
      trackPoints.Add(info.startPointX + (i+1)*step, trackPoints[lastPointX] + deltas[i]);
      lastPointX = info.startPointX + (i+1)*step;
    }
    trackPoints.Add(info.endPointX, info.endPointY);
    
  }
  
  public override void CalcFitness() {
    fitness = eval.time; //in this case we only consider time
  }
  
  
  public override Individual Clone() {
    NewIndividual newobj = (NewIndividual)this.MemberwiseClone ();
    newobj.fitness = 0f;
    newobj.trackPoints = new Dictionary<float, float>();
    newobj.deltas = new List<float> (this.deltas);
    return newobj;
  }



  void RandomInitialization() {
    float step = (info.endPointX - info.startPointX ) / (info.numTrackPoints - 1);
    float delta = 0;
    float currentY = info.startPointY;
    
    for(int i = 0; i < info.numTrackPoints - 2; i++) {
      float limInf = (MinY + 0.01f) - currentY;
      float limSup = (MaxY - 0.01f) - currentY;
      delta = UnityEngine.Random.Range(limInf, limSup);
      deltas.Add(delta);
      currentY+=delta;
    }
    
  }
  
  void NewValueMutation(float probability) {
    float currentY = info.startPointY;

    for(int x = 0; x < deltas.Count; x++) {
      float limInf = (MinY + 0.01f) - currentY;
      float limSup = (MaxY - 0.01f) - currentY;
      if(UnityEngine.Random.Range (0f, 1f) < probability) {
	deltas[x] = UnityEngine.Random.Range(limInf,limSup);
      }
      else{
	deltas[x] = (float)clamp(deltas[x],limInf,limSup);
      }
      currentY += deltas[x];
    }
  }


  void ValueMutationGaussian(float probability) {
    float stddev = (MaxY - MinY)/6f;
    double mean;
    float currentY = info.startPointY;
    
    for(int x = 0; x< deltas.Count;x++) {
      float limInf = (MinY + 0.01f) - currentY;
      float limSup = (MaxY - 0.01f) - currentY;
      if(UnityEngine.Random.Range (0f, 1f) < probability) {
	float tempValue = (float)gaussianMutation(deltas[x], stddev);
	float finalValue = (float)clamp(tempValue, limInf, limSup);
	deltas[x] = finalValue;
      }
      else{
	deltas[x] = (float)clamp(deltas[x],limInf ,limSup);
      }
      currentY += deltas[x];
    }
    
  }


  double gaussianMutation(double mean, float stddev)
  {
    double x1 = UnityEngine.Random.Range (0f, 1f);
    double x2 = UnityEngine.Random.Range (0f, 1f);
    
    if(x1 == 0)
      x1 = 1;
    if(x2 == 0)
      x2 = 1;
    
    double y1 = Math.Sqrt(-2.0 * Math.Log(x1)) * Math.Cos(2.0 * Math.PI * x2);
    return y1 * stddev + mean;
  }

  float Getstddev() {
    float mean = (MinY + MaxY) /2;
    float sigma = (MaxY - mean) / 3;
    
    return UnityEngine.Random.Range(mean, sigma);
  }

  double clamp(double val, float min, float max)
  {
    if(val >= max){
      return max;
    }
    if(val <= min){
      return min;
    }

    return val;
  }


  void NCrossover(Individual partner, float probability,int cutPoints) {
    
    if (UnityEngine.Random.Range (0f, 1f) > probability) {
      return;
    }
    
    NewIndividual newPartner = (NewIndividual)partner;
    
    List<int> points = new List<int>();

    int aux;
    
    while(points.Count != cutPoints)
      {
	aux = UnityEngine.Random.Range(0, info.numTrackPoints - 1);
	
	if (!points.Contains(aux))
	  {
	    points.Add(aux);
	  }
      }
    
    points.Sort ();

    float currentY1 = info.startPointY;
    float currentY2 = info.startPointY;
    List<float> test = new List<float>(deltas);
    for (int j = 0; j < points.Count; j++)
      {
	int limit = (j == points.Count-1)?cutPoints-1:points[j+1];
	for (int i = points[j]; i < limit; i++)
	  {
	    float tmp = deltas[i];
	    deltas[i] = newPartner.deltas[i];
	    newPartner.deltas[i] = tmp;
	    j++;
	  }
      }
    float limSup;
    float limInf;

    for(int i=0;i<deltas.Count;i++){
      
      limSup = (MaxY - 0.01f) - currentY1;
      limInf = (MinY + 0.01f) - currentY1;
      deltas[i] = (float)clamp(deltas[i],limInf,limSup);
      currentY1+=deltas[i];

      limSup = (MaxY - 0.01f) - currentY2;
      limInf = (MinY + 0.01f) - currentY2;
      newPartner.deltas[i] = (float)clamp(deltas[i],limInf,limSup);      
      currentY2+=newPartner.deltas[i];
    }
  }
  
  void HalfCrossover(Individual partner, float probability) {

    if (UnityEngine.Random.Range (0f, 1f) > probability) {
      return;
    }
    
    NewIndividual newPartner = (NewIndividual)partner;
    
    //this example always splits the chromosome in half
    int crossoverPoint = Mathf.FloorToInt ((info.numTrackPoints -1) / 2f);
    for (int i=0; i<crossoverPoint; i++) {
      float tmp = deltas[i];
      deltas[i] = newPartner.deltas[i];
      newPartner.deltas[i]=tmp;
    }

    float currentY1 = info.startPointY;
    float currentY2 = info.startPointY;

    float limSup;
    float limInf;

    for(int i=0;i<deltas.Count;i++){
      limSup = (MaxY - 0.01f) - currentY1;
      limInf = (MinY + 0.01f) - currentY1;
      deltas[i] = (float)clamp(deltas[i],limInf,limSup);
      currentY1+=deltas[i];

      limSup = (MaxY - 0.01f) - currentY2;
      limInf = (MinY + 0.01f) - currentY2;
      newPartner.deltas[i] = (float)clamp(deltas[i],limInf,limSup);      
      currentY2+=newPartner.deltas[i];
    }
  }
}
