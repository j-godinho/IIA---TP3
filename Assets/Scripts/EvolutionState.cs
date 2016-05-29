using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EvolutionState : MonoBehaviour {

  public float startPointX;
  public float startPointY;
  public float endPointX;
  public float endPointY;
  public float g;
  public float startVelocity;
  public int numTrackPoints;
  public int selectionMethod;
  public int tournamentSize;
  public int nCrossings;
  public int stddev;

  private ProblemInfo info;


  public int numGenerations;
  public int populationSize;
  public float mutationProbability;
  public float crossoverProbability;

  public bool gaussian;

  private List<Individual> population;
  private SelectionMethod selection;

  private int evaluatedIndividuals;
  private int currentGeneration;
  public int EvaluationsPerStep;

  public string statsFilename;

  private StatisticsLogger stats;



  private PolygonGenerator drawer;

  public int unchangedNumber;

  public int typeOfRepresentation; //0 - normal one; 1- Delta one;

  bool evolving;
  bool drawing;

  public int testTimes;

  int actual = 0;
  // Use this for initialization
  void Start () {
    info = new ProblemInfo ();
    info.startPointX = startPointX;
    info.startPointY = startPointY;
    info.endPointX = endPointX;
    info.endPointY = endPointY;
    info.g = g;
    info.startVelocity = startVelocity;
    info.numTrackPoints = numTrackPoints;

    stats  = new StatisticsLogger ();

    if (selectionMethod == 0) {
      selection = new TournamentSelection (tournamentSize);
    } else if (selectionMethod == 1) {
      selection = new RouletteSelection ();
    }


    drawer = new PolygonGenerator ();

    StartTest();
  }

  void StartTest(){
    stats.bestFitness.Clear();
    stats.meanFitness.Clear();
    InitPopulation ();
    evaluatedIndividuals = 0;
    currentGeneration = 0;
    evolving = true;
    drawing = false;
  }

  void FixedUpdate () {
    if (evolving) {
      EvolStep ();
    } else if(drawing) {
      for(int i=0;i<population.Count;i++) population[i].evaluate();
      population.Sort((x, y) => x.fitness.CompareTo(y.fitness));
      DestroyObject(GameObject.Find("New Game Object"));
      drawer.drawCurve(population[0].trackPoints,info);
      drawing=false;
      Application.CaptureScreenshot ("Images/Program"+population[0].fitness+".png");


      if (actual != testTimes-1) {
	actual++;
	StartTest ();
      }
      else{
	string header = "StartPoint = (" + startPointX + ", " + startPointY + ")\n";
	header += "EndPoint = (" + endPointX + ", " + endPointY + ")\n";
	header += "N Points = " + numTrackPoints + "\n";
	header += "Selection = " + ((selectionMethod == 0)?"Tournament":"Roulette") + "\n";
	header += "Tournament Size = " + tournamentSize + "\n";
	header += "NCrossings = " + nCrossings + "\n";
	header += "NGenerations = " + numGenerations + "\n";
	header += "PopulationSize = " + populationSize + "\n";
	header += "MutationProb = " + mutationProbability + "\n";
	header += "CrossoverProb = " + crossoverProbability + "\n";
	header += "MutationType = " + ((gaussian)?"Gaussian":"Random") + "\n";
	header += "Elistic = " + unchangedNumber + "\n\n";

	stats.writeStats(header, testTimes);
      }
    }
  }

  void EvolStep() {
    //do for a given number of generations
    if (currentGeneration < numGenerations) {

      //if there are individuals to evaluate on the current generation
      int evalsThisStep = EvaluationsPerStep < (populationSize - evaluatedIndividuals) ? EvaluationsPerStep : (populationSize - evaluatedIndividuals);
      for (int ind = evaluatedIndividuals; ind<evaluatedIndividuals+evalsThisStep; ind++) {
	population[ind].evaluate();
      }
      evaluatedIndividuals += evalsThisStep;

      //if all individuals have been evaluated on the current generation, breed a new population
      if(evaluatedIndividuals==populationSize) {
	stats.PostGenLog(population,currentGeneration);
	population = BreedPopulation();
	evaluatedIndividuals=0;
	currentGeneration++;
      }

    } else {

      stats.finalLog();
      evolving=false;
      drawing = true;
      print ("evolution stopped");
    }

  }


  void InitPopulation () {
    population = new List<Individual>();
    while (population.Count<populationSize) {
      if(typeOfRepresentation == 0){
        ExampleIndividual newind = new ExampleIndividual(info); //change accordingly
        newind.Initialize();
        population.Add (newind);
      }else if(typeOfRepresentation == 1){
        NewIndividual newind = new NewIndividual(info); //change accordingly
        newind.Initialize();
        population.Add (newind);
      }

    }
  }


  List<Individual> BreedPopulation() {
    List<Individual> newpop = new List<Individual>();

    population.Sort((x, y) => x.fitness.CompareTo(y.fitness));
    for(int i = 0; i < unchangedNumber; i++){
      newpop.Add(population[i].Clone());
    }


    //breed individuals and place them on new population. We'll apply crossover and mutation later
    while(newpop.Count<populationSize) {
      List<Individual> selectedInds = selection.selectIndividuals(population,2); //we should propably always select pairs of individuals
      for(int i =0; i< selectedInds.Count;i++) {
	if(newpop.Count<populationSize) {
	  newpop.Add(selectedInds[i]); //added individuals are already copys, so we can apply crossover and mutation directly
	}
	else{ //discard any excess individuals
	  selectedInds.RemoveAt(i);
	}
      }

      //apply crossover between pairs of individuals and mutation to each one
      while(selectedInds.Count>1) {
	selectedInds[0].Crossover(selectedInds[1],crossoverProbability, nCrossings);
	selectedInds[0].Mutate(mutationProbability,gaussian);
	selectedInds[1].Mutate(mutationProbability,gaussian);
	selectedInds.RemoveRange(0,2);
      }
      if(selectedInds.Count==1) {
	selectedInds[0].Mutate(mutationProbability,gaussian);
	selectedInds.RemoveAt(0);
      }
    }

    return newpop;
  }

}
