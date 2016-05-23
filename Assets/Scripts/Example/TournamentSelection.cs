using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TournamentSelection : SelectionMethod {

	int tournamentSize;

	public TournamentSelection(int tournament): base() {
		tournamentSize = tournament;
	}


	public override List<Individual> selectIndividuals (List<Individual> oldpop, int num)
	{
		return tournamentSelection (oldpop, num);
	}


	List<Individual> tournamentSelection(List<Individual>oldpop, int num) {

		List<Individual> selectedInds = new List<Individual> ();

		for (int i = 0; i<num; i++) {
			//make sure selected individuals are different
			Individual ind = selectIndividual(oldpop);
			while (selectedInds.Contains(ind)) {
				ind = selectIndividual(oldpop);
			}
			selectedInds.Add (ind.Clone()); //we return copys of the selected individuals
		}

		return selectedInds;
	}

	Individual selectIndividual(List<Individual> population){
		//Debug.Log("selectIndividual\n");

		List<Individual> tournament = new List<Individual> ();
		//Debug.Log(tournametSize);
		for(int i = 0; i < tournamentSize; i++){
			int index = (int)(Random.Range(0, tournamentSize-1));

			tournament.Add(population[index]);
			//Debug.Log("oi");
		}

		Individual bestIndividual = tournament[0];
		float bestFitness = bestIndividual.fitness;
		for(int i = 1; i < tournamentSize; i++){
			if(tournament[i].fitness < bestFitness){
				bestIndividual = tournament[i];
				bestFitness = tournament[i].fitness;
			}
		}
		return bestIndividual;
	}

}
