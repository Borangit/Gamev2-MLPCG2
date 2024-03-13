using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.LinearAlgebra;
using System;

using Random = UnityEngine.Random;
using System.IO;
using UnityEngine.UI;



public class GeneticManager : MonoBehaviour
{
    [Header("References")]
    public RobotController controller;

    [Header("Controls")]
    public int initialPopulation;
    [Range(0.0f, 1.0f)]
    public float mutationRate = 0.055f;

    [Header("Crossover Controls")]
    public int bestAgentSelection;
    public int worstAgentSelection;
    public int numberToCrossover;

    private List<int> genePool = new List<int>();

    private int naturallySelected;

    private NNet[] population;

    [Header("Public View")]
    public int currentGeneration;
    public int currentGenome = 0;
    public int lastScore;
    public Text scoreText;
    public Text genText;
    public Text childText;

    void Update()
    {

        scoreText.text = "Score: " + lastScore.ToString();
        genText.text = "Generation: " + currentGeneration.ToString();
        childText.text = "Child: " + currentGenome.ToString();
    }

    private void Start()
    {
        CreatePopulation();
    }

    private void CreatePopulation()
    {
        population = new NNet[initialPopulation];
        FillPopulationWithRandomValues(population, 0);
        ResetToCurrentGenome();
    }
    // private void TrainFromLoadedNNets()
    // {
    //     population = new NNet[initialPopulation];
    //     LoadNNets(initialPopulation);
    //     ResetToCurrentGenome();
    // }

    private void ResetToCurrentGenome()
    {
        controller = FindObjectOfType<RobotController>();
        controller.ResetWithNetwork(population[currentGenome]);
    }

    private void FillPopulationWithRandomValues(NNet[] newPopulation, int startingIndex)
    {
        while (startingIndex < initialPopulation)
        {
            // Check if there is an existing GameObject and destroy it
            if (newPopulation[startingIndex] != null)
            {
                Destroy(newPopulation[startingIndex].gameObject);
            }

            // Create a new GameObject for the neural network
            GameObject nnGameObject = new GameObject("NNet_" + startingIndex);

            // Add NNet as a component to the GameObject
            newPopulation[startingIndex] = nnGameObject.AddComponent<NNet>();

            // Initialize the neural network
            newPopulation[startingIndex].Initialise(6, 2, controller.hiddenLayers, controller.hiddenNeurons);

            startingIndex++;
        }
    }



    public void Death (float fitness, NNet network)
    {
        lastScore = Mathf.RoundToInt(fitness);
        population[currentGenome].fitness = fitness;
        
       
        if (currentGenome < initialPopulation - 1)
        {
            currentGenome++;
            ResetToCurrentGenome();
        }
        else
        {
            RePopulate();
        }
        
    }

    private void RePopulate()
    {
        // for (int i = 0; i < population.Length; i++) // Save all NNets, not used in final version
        // {
        //     SaveNNet(population[i], i);
        // }
        genePool.Clear();
        currentGeneration++;
        naturallySelected = 0;
        SortPopulation();

        NNet[] newPopulation = PickBestPopulation();
        
        

        Crossover(newPopulation);
        Mutate(newPopulation);

        // Destroy old GameObjects associated with the previous population
        for (int i = 0; i < population.Length; i++)
        {
            if (population[i] != null)
            {
                Destroy(population[i].gameObject);
            }
        }

        FillPopulationWithRandomValues(newPopulation, naturallySelected);

        population = newPopulation;
        

        currentGenome = 0;

        ResetToCurrentGenome();

    }
    public void SaveNNet(NNet network, int index)
    {
        string directoryPath = @"C:\Users\daphe\Documents\GitHub\Tower-Defence-PCG\NNets";
        // add index to file name
        string fileName = "savedNNet" + index + ".txt"; // Ensure this matches the file name used in the save function
        string filePath = Path.Combine(directoryPath, fileName);
        using (StreamWriter sw = new StreamWriter(filePath))
        {
            // Save layer sizes
            sw.WriteLine(network.inputLayer.ColumnCount);
            sw.WriteLine(network.hiddenLayers.Count);
            if (network.hiddenLayers.Count > 0)
            {
                sw.WriteLine(network.hiddenLayers[0].ColumnCount);
            }
            sw.WriteLine(network.outputLayer.ColumnCount);

            // Save weights
            foreach (var matrix in network.weights)
            {
                for (int i = 0; i < matrix.RowCount; i++)
                {
                    for (int j = 0; j < matrix.ColumnCount; j++)
                    {
                        sw.WriteLine(matrix[i, j]);
                    }
                }
            }

            // Save biases
            foreach (var bias in network.biases)
            {
                sw.WriteLine(bias);
            }
        }
    }   
    public void LoadNNets(int index){
        for (int i = 0; i < index; i++)
        {
            string directoryPath = @"C:\Users\daphe\Documents\GitHub\Tower-Defence-PCG\NNets";
            string fileName = "savedNNet" + i + ".txt"; // Ensure this matches the file name used in the save function
            string filePath = Path.Combine(directoryPath, fileName);
            GameObject temp = new GameObject("temp_nnet");

            NNet loadedNNet = temp.AddComponent<NNet>();

            using (StreamReader sr = new StreamReader(filePath))
            {
                // Read and set layer sizes
                int inputSize = int.Parse(sr.ReadLine());
                int hiddenLayerCount = int.Parse(sr.ReadLine());
                int hiddenNeuronCount = hiddenLayerCount > 0 ? int.Parse(sr.ReadLine()) : 0;
                int outputSize = int.Parse(sr.ReadLine());

                loadedNNet.Initialise(inputSize, outputSize, hiddenLayerCount, hiddenNeuronCount);

                // Read and set weights
                for (int k = 0; k < loadedNNet.weights.Count; k++)
                {
                    for (int o = 0; o < loadedNNet.weights[k].RowCount; o++)
                    {
                        for (int j = 0; j < loadedNNet.weights[k].ColumnCount; j++)
                        {
                            loadedNNet.weights[k][o, j] = float.Parse(sr.ReadLine());
                        }
                    }
                }

                // Read and set biases
                for (int p = 0; p < loadedNNet.biases.Count; p++)
                {
                    loadedNNet.biases[p] = float.Parse(sr.ReadLine());
                }
            }
            population[i] = loadedNNet;
        }
    }



    private void Mutate (NNet[] newPopulation)
    {

        for (int i = 0; i < naturallySelected; i++)
        {

            for (int c = 0; c < newPopulation[i].weights.Count; c++)
            {

                if (Random.Range(0.0f, 1.0f) < mutationRate)
                {
                    newPopulation[i].weights[c] = MutateMatrix(newPopulation[i].weights[c]);
                }

            }

        }

    }

    Matrix<float> MutateMatrix (Matrix<float> A)
    {

        int randomPoints = Random.Range(1, (A.RowCount * A.ColumnCount) / 7);

        Matrix<float> C = A;

        for (int i = 0; i < randomPoints; i++)
        {
            int randomColumn = Random.Range(0, C.ColumnCount);
            int randomRow = Random.Range(0, C.RowCount);

            C[randomRow, randomColumn] = Mathf.Clamp(C[randomRow, randomColumn] + Random.Range(-1f, 1f), -1f, 1f);
        }

        return C;

    }

    private void Crossover (NNet[] newPopulation)
    {
        for (int i = 0; i < numberToCrossover; i+=2)
        {
            int AIndex = i;
            int BIndex = i + 1;

            if (genePool.Count >= 1)
            {
                for (int l = 0; l < 100; l++)
                {
                    AIndex = genePool[Random.Range(0, genePool.Count)];
                    BIndex = genePool[Random.Range(0, genePool.Count)];

                    if (AIndex != BIndex)
                        break;
                }
            }

        GameObject child1GO = new GameObject("Child1_NNet");
        GameObject child2GO = new GameObject("Child2_NNet");

        NNet Child1 = child1GO.AddComponent<NNet>();
        NNet Child2 = child2GO.AddComponent<NNet>();

        Child1.Initialise(6, 2, controller.hiddenLayers, controller.hiddenNeurons);
        Child2.Initialise(6, 2, controller.hiddenLayers, controller.hiddenNeurons);

        Child1.fitness = 0;
        Child2.fitness = 0;


            for (int w = 0; w < Child1.weights.Count; w++)
            {

                if (Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    Child1.weights[w] = population[AIndex].weights[w];
                    Child2.weights[w] = population[BIndex].weights[w];
                }
                else
                {
                    Child2.weights[w] = population[AIndex].weights[w];
                    Child1.weights[w] = population[BIndex].weights[w];
                }

            }


            for (int w = 0; w < Child1.biases.Count; w++)
            {

                if (Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    Child1.biases[w] = population[AIndex].biases[w];
                    Child2.biases[w] = population[BIndex].biases[w];
                }
                else
                {
                    Child2.biases[w] = population[AIndex].biases[w];
                    Child1.biases[w] = population[BIndex].biases[w];
                }

            }

            newPopulation[naturallySelected] = Child1;
            naturallySelected++;

            newPopulation[naturallySelected] = Child2;
            naturallySelected++;

        }
    }

    private NNet[] PickBestPopulation()
    {
        NNet[] newPopulation = new NNet[initialPopulation];

        for (int i = 0; i < bestAgentSelection; i++)
        {
            newPopulation[naturallySelected] = population[i].DuplicateNNet();
            newPopulation[naturallySelected].fitness = 0;
            naturallySelected++;
            
            int f = Mathf.RoundToInt(population[i].fitness * 10);

            for (int c = 0; c < f; c++)
            {
                genePool.Add(i);
            }
        }

        for (int i = 0; i < worstAgentSelection; i++)
        {
            int last = population.Length - 1 - i;

            int f = Mathf.RoundToInt(population[last].fitness * 10);

            for (int c = 0; c < f; c++)
            {
                genePool.Add(last);
            }
        }

        return newPopulation;
    }

    private void SortPopulation()
    {
        for (int i = 0; i < population.Length; i++)
        {
            for (int j = i; j < population.Length; j++)
            {
                if (population[i].fitness < population[j].fitness)
                {
                    NNet temp = population[i];
                    population[i] = population[j];
                    population[j] = temp;
                }
            }
        }

    }
    
}
