using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm {
	public List<Genome> genomes;
	public List<Genome> lastGenerationGenomes;

    //default population = 84
    public int populationSize = 84;

    //public int populationSize = 200;

    //default crossover rate = 0.7
    public double crossoverRate = 0.7f;
    //default mutation rate = 0.001f
	public double mutationRate = 0.25f;

    // 51 node
    // modified = 21 node

    //public int chromosomeLength = 100;

    public int chromosomeLength = 42;
    public int geneLength = 2;
	public int fittestGenome;
	public double bestFitnessScore;
	public double totalFitnessScore;
	public int generation;
	public MazeController mazeController;
	public MazeController mazeControllerDisplay;

	// check to see if loop
	public bool busy; 
	
	public GeneticAlgorithm () {
		busy = false;
		genomes = new List<Genome> ();
		lastGenerationGenomes = new List <Genome>();
	}

	public void CreateStartPopulation () {
		genomes.Clear ();
		for (int i = 0; i < populationSize; i ++) {
			Genome baby = new Genome (chromosomeLength);
			genomes.Add (baby);
		}
	}

	public void Run () {
		CreateStartPopulation ();
		busy = true;
	}

	// convert list of bits into an integer
	public int GeneToInt (List<int> gene) {
		int value = 0;
		int multiplier = 1;
		for (int i = gene.Count; i > 0; i --) {
			value += gene [i - 1] * multiplier;
			multiplier *= 2;
		}
		return value;
	}

	// decode list of bits into list of directions (int)
	// 0:north, 1:south, 2:east, 3:west
	public List<int> Decode (List<int> bits) {
		List<int> directions = new List<int> ();
		for (int geneIndex = 0; geneIndex < bits.Count; geneIndex += geneLength) {
			List<int> gene = new List<int> ();
			for (int bitIndex = 0; bitIndex < geneLength; bitIndex ++) {
				gene.Add (bits [geneIndex + bitIndex]);
			}
			directions.Add (GeneToInt (gene));
		}
		return directions;
	}

	 public void UpdateFitnessScore () {
		 fittestGenome = 0;
		 bestFitnessScore = 0;
		 totalFitnessScore = 0;

		 for (int i =0; i < populationSize; i ++) {
			 List<int> directions = Decode (genomes [i].bits);

			 genomes [i].fitness = mazeController.TestRoute (directions);

			totalFitnessScore += genomes [i].fitness;
			
			if (genomes [i].fitness > bestFitnessScore) {
				bestFitnessScore = genomes [i].fitness;
				fittestGenome = i;

				// chromosome found the exit?
				if (genomes [i].fitness == 1) {
					busy = false; // stop the run
					VirtualShopManager.virtualShopManager.search = false;
					return;
				}
			}
		}
	}

	public Genome RouletteWheelSelection () {
		double slice = UnityEngine.Random.value * totalFitnessScore;
		double total = 0f;
		int selectedGenome = 0;

		for (int i = 0; i < populationSize; i ++) {
			total += genomes [i].fitness;

			if (total > slice) {
				selectedGenome = i;
				break;
			}
		}
		return genomes[selectedGenome];
	}

	public void Crossover (List<int> mom, List<int> dad, List <int> baby1, List <int> baby2) {
		if (UnityEngine.Random.value > crossoverRate || mom == dad) {
			baby1.AddRange (mom);
			baby2.AddRange (dad);

			return;
		}

		System.Random rnd = new System.Random ();

		int crossoverPoint = rnd.Next (0, chromosomeLength - 1);
		for (int i = 0; i < crossoverPoint; i ++) {
			baby1.Add (mom [i]);
			baby2.Add (dad [i]);
		}

		for (int i = crossoverPoint; i < mom.Count; i ++) {
			baby1.Add (dad[i]);
			baby2.Add (mom[i]);
		}
	}

	public void Mutate (List<int> bits) {
		for (int i = 0; i < bits.Count; i ++) {
			// flip this bit?
			if (UnityEngine.Random.value < mutationRate) {
				// flip the bit
				bits[i] = bits [i] == 0 ? 1 : 0;
			}
		}
	}

	public void Epoch () {
		if (!busy) {
			return;
		}

		UpdateFitnessScore ();

		if (!busy) {
			lastGenerationGenomes.Clear();
			lastGenerationGenomes.AddRange (genomes);
			return;
		}

		int numberOfNewBabies = 0;

		List <Genome> babies = new List <Genome> ();
		while (numberOfNewBabies < populationSize) {

			Genome mom = RouletteWheelSelection ();
			Genome dad = RouletteWheelSelection ();
			Genome baby1 = new Genome ();
			Genome baby2 = new Genome ();
			Crossover (mom.bits, dad.bits, baby1.bits, baby2.bits);
			Mutate (baby1.bits);
			Mutate (baby2.bits);
			babies.Add (baby1);
			babies.Add (baby2);

			numberOfNewBabies += 2;
		}

		// save last generation for display purpose
		lastGenerationGenomes.Clear();
		lastGenerationGenomes.AddRange (genomes);

		// overwrite population w/ babies
		genomes = babies;
		// increment the generation counter
		generation ++;
	}
}