using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RouletteSelection: SelectionMethod {

	public RouletteSelection(): base() {

	}


	public override List<Individual> selectIndividuals (List<Individual> oldpop, int num)
	{
		return rouletteSelection (oldpop, num);
	}



	List<Individual> rouletteSelection(List<Individual>oldpop, int num) {
		float sum = 0;
		float probabilitySum = 0;


		List<Individual> selectedInds = new List<Individual> ();

		for (int i = 0; i < num; i++) {
			sum += oldpop[i].fitness;
		}

		float [] probability = new float [num];
		for (int i = 0; i < num; i++) {
			probability[i] = probabilitySum + (oldpop[i].fitness / sum);
			probabilitySum += probability[i];
		}
			

		for(int i = 0;i<num;i++)
		{
			float number = Random.Range (0f, 1f);
			for (int j = 0; j < num; j++) {
				if (number > probability [j] ) {
					selectedInds.Add (oldpop[j].Clone());
				}
			}
		}
			
		return selectedInds;
	}

}
