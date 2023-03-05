using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    // Sets the variables for population size, elitism, the desired output and the mutation for each individual of population
    static Random random = new Random();
    static string target = "Decryption is key to human success";
    static int populationSize = 100;
    static double mutationRate = 0.01;
    static int elitism = 5;

    static void Main(string[] args)
    {
        // Create initial population
        List<string> population = GeneratePopulation(populationSize);

        // Evolve the population and display the current best generation and individual within that population
        int generation = 1;
        while (!population.Contains(target))
        {
            Console.WriteLine("Generation " + generation);
            population = EvolvePopulation(population);
            Console.WriteLine("Best: " + population[0]);
            Console.WriteLine();
            generation++;
        }

        // Find the individual in the population that matches the required output
        var i = 0;
        foreach (string dump in population)
        {
            if (dump == target)
            {
                break;
            }
            i++;
        }

        // End of program, ending the permutation loop
        Console.WriteLine("Best individual: " + i + " " + population[i]);
        Console.WriteLine("Target found in " + generation + " generations!");
        Console.ReadLine();
    }

    // Function creates the first generation of population, enumerating through the population size
    static List<string> GeneratePopulation(int size)
    {
        List<string> population = new List<string>();

        for (int i = 0; i < size; i++)
        {
            population.Add(GenerateRandomString());
        }

        return population;
    }

    // Generating each individuals dna using the chars string 
    static string GenerateRandomString()
    {
        string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ ,.!?";
        char[] stringChars = new char[target.Length];

        for (int i = 0; i < target.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        // Returns the individual to be added to the populatiom
        return new string(stringChars);
    }

    // Evolution Function
    static List<string> EvolvePopulation(List<string> population)
    {
        // Evaluate fitness of each individual
        Dictionary<string, double> fitnessScores = new Dictionary<string, double>();
        foreach (string individual in population)
        {
            double fitness = 0;
            for (int i = 0; i < individual.Length; i++)
            {
                // Sets score for each individual by how correct they characters are in the right placw
                if (individual[i] == target[i])
                {
                    fitness += 1;
                }
            }
            fitnessScores[individual] = fitness;
        }

        // Sort population by fitness score
        List<string> sortedPopulation = population.OrderByDescending(individual => fitnessScores[individual]).ToList();

        // Elitism, preserve the 5 best individuals
        List<string> newPopulation = sortedPopulation.Take(elitism).ToList();

        // Crossover, create new individuals by combining existing ones
        while (newPopulation.Count < populationSize)
        {
            string parent1 = SelectIndividual(sortedPopulation);
            string parent2 = SelectIndividual(sortedPopulation);
            string child = Crossover(parent1, parent2);
            newPopulation.Add(child);
        }

        // Mutate, introduce random changes in some individuals
        for (int i = elitism; i < newPopulation.Count; i++)
        {
            if (random.NextDouble() < mutationRate)
            {
                newPopulation[i] = Mutate(newPopulation[i]);
            }
        }

        return newPopulation;
    }

    // Selects 2 individuals randomly, the top 5 indiviudals splice their dna as the basiss fr the next generation
    static string SelectIndividual(List<string> population)
    {
        // Roulette wheel selection
        double totalFitness = population.Sum(individual => FitnessScore(individual)); //Examines the fitnesss of the entire generation
        double rouletteValue = random.NextDouble() * totalFitness;
        double runningTotal = 0;
        foreach (string individual in population)
        {
            runningTotal += FitnessScore(individual);
            if (runningTotal >= rouletteValue)
            {
                return individual;
            }
        }
        return population[random.Next(population.Count)];
    }

    // Slices Parent1 DNA with Parent2
    static string Crossover(string parent1, string parent2)
    {
        int crossoverPoint = random.Next(0, target.Length);
        return parent1.Substring(0, crossoverPoint) + parent2.Substring(crossoverPoint);
    }

    // Runs populatiomn through mutation chancwe function
    static string Mutate(string individual)
    {
        char[] chars = individual.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (random.NextDouble() < mutationRate)
            {
                chars[i] = GenerateRandomString()[i];
            }
        }
        return new string(chars);
    }

    // Where the fitness score is examined
    static double FitnessScore(string individual)
    {
        double score = 0;
        for (int i = 0; i < individual.Length; i++)
        {
            if (individual[i] == target[i])
            {
                score += 1;
            }
        }
        return score;
    }
}
