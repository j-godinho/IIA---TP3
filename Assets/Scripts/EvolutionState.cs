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

	private ProblemInfo info;


	public int numGenerations;
	public int populationSize;
	public float mutationProbability;
	public float crossoverProbability;

	private List<Individual> population;
	private SelectionMethod selection;

	private int evaluatedIndividuals;
	private int currentGeneration;
	public int EvaluationsPerStep;

	private StatisticsLogger stats;
	public string statsFilename;

	private PolygonGenerator drawer;

	public int unchangedNumber;

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

		if (selectionMethod == 0) {
			selection = new TournamentSelection (tournamentSize);
		} else if (selectionMethod == 1) {
			selection = new RouletteSelection ();
		}

		stats = new StatisticsLogger (statsFilename);

		drawer = new PolygonGenerator ();

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
			drawer.drawCurve(population[0].trackPoints,info);
			drawing=false;
			Application.CaptureScreenshot ("Images/Program"+actual+".png");

			if (actual != testTimes-1) {
				actual++;
				Start ();
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
			ExampleIndividual newind = new ExampleIndividual(info); //change accordingly
			newind.Initialize();
			population.Add (newind);
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
				selectedInds[0].Mutate(mutationProbability);
				selectedInds[1].Mutate(mutationProbability);
				selectedInds.RemoveRange(0,2);
			}
			if(selectedInds.Count==1) {
				selectedInds[0].Mutate(mutationProbability);
				selectedInds.RemoveAt(0);
			}
		}
			
		return newpop;
	}

}
