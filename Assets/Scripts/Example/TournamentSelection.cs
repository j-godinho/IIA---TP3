using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TournamentSelection : SelectionMethod {

	public TournamentSelection(): base() {

	}


	public override List<Individual> selectIndividuals (List<Individual> oldpop, int num)
	{
		return tournamentSelection (oldpop, num);
	}


	List<Individual> tournamentSelection(List<Individual>oldpop, int num) {

		List<Individual> selectedInds = new List<Individual> ();
		int popsize = oldpop.Count;
		for (int i = 0; i<num; i++) {
			//make sure selected individuals are different
			Individual ind = selectIndividual(oldpop, popsize);
			while (selectedInds.Contains(ind)) {
				ind = selectIndividual(oldpop, popsize);
			}
			selectedInds.Add (ind.Clone()); //we return copys of the selected individuals
		}

		return selectedInds;
	}

	Individual selectIndividual(List<Individual> population, int tournametSize){
		//Debug.Log("selectIndividual\n");
		List<Individual> tournament = new List<Individual> ();
		//Debug.Log(tournametSize);
		for(int i = 0; i < tournametSize; i++){
			int index = (int)(Random.Range(0, tournametSize-1));

			tournament.Add(population[index]);
			//Debug.Log("oi");
		}

		Individual bestIndividual = tournament[0];
		float bestFitness = bestIndividual.fitness;
		for(int i = 1; i < tournametSize; i++){
			if(tournament[i].fitness < bestFitness){
				bestIndividual = tournament[i];
				bestFitness = tournament[i].fitness;
			}
		}
		return bestIndividual;
	}

}
