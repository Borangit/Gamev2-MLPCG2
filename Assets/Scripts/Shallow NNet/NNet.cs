using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System;

public class NNet : MonoBehaviour
{
    public Matrix<float> inputLayer;
    public List<Matrix<float>> hiddenLayers;
    public Matrix<float> outputLayer;
    public List<Matrix<float>> weights;
    public List<float> biases;
    public float fitness;
    private float lastx = 6;
    private float lastz = 6;

    // Initialization with the number of inputs and outputs
    public void Initialise(int inputCount, int outputCount, int hiddenLayerCount, int hiddenNeuronCount)
    {
        // Initialize layers
        inputLayer = Matrix<float>.Build.Dense(1, inputCount);
        outputLayer = Matrix<float>.Build.Dense(1, outputCount);
        hiddenLayers = new List<Matrix<float>>();

        // Initialize weights and biases
        weights = new List<Matrix<float>>();
        biases = new List<float>();

        // Initialize hidden layers
        for (int i = 0; i < hiddenLayerCount; i++)
        {
            hiddenLayers.Add(Matrix<float>.Build.Dense(1, hiddenNeuronCount));
            biases.Add(UnityEngine.Random.Range(-1f, 1f));

            // Initialize weights
            int inputDim = i == 0 ? inputCount : hiddenNeuronCount;
            weights.Add(Matrix<float>.Build.Dense(inputDim, hiddenNeuronCount));
        }

        // Weight between last hidden layer and output layer
        weights.Add(Matrix<float>.Build.Dense(hiddenNeuronCount, outputCount));
        biases.Add(UnityEngine.Random.Range(-1f, 1f));

        // Randomize weights
        RandomiseWeights();
    }

    public void RandomiseWeights()
    {
        for (int i = 0; i < weights.Count; i++)
        {
            for (int x = 0; x < weights[i].RowCount; x++)
            {
                for (int y = 0; y < weights[i].ColumnCount; y++)
                {
                    weights[i][x, y] = UnityEngine.Random.Range(-1f, 1f);
                }
            }
        }
    }

    public (float, float) RunNetwork(float a, float b, float c, float d, float e, float f)
    {

        inputLayer[0, 0] = a;
        inputLayer[0, 1] = b;
        inputLayer[0, 2] = c;
        inputLayer[0, 3] = d;
        inputLayer[0, 4] = e;
        inputLayer[0, 5] = f;

        // Process through layers
        var currentLayer = inputLayer;
        for (int i = 0; i < hiddenLayers.Count; i++)
        {
            hiddenLayers[i] = ((currentLayer * weights[i]) + biases[i]).PointwiseTanh();
            currentLayer = hiddenLayers[i];
        }

        outputLayer = ((currentLayer * weights[weights.Count - 1]) + biases[biases.Count - 1]).PointwiseTanh();
        float x = outputLayer[0, 0];
        float z = outputLayer[0, 1];
        //compare the absolute value of x and z, return the larrger  one
        if (Math.Abs(x) > Math.Abs(z))
        {
            z = 0;
        }
        else
        {
            x = 0;
        }





        // Return outputs (you may adjust activation functions as needed)
        return (CustomActivation(x), CustomActivation(z));
    } 
    private float CustomActivation(float value)
    {
        if (value > 0) return 1;
        else if (value < 0) return -1;
        else return 0; // value == 0
    }



    private float Sigmoid(float s)
    {
        return 1 / (1 + Mathf.Exp(-s));
    }
    public NNet DuplicateNNet()
    {
        GameObject nnGameObject = new GameObject("NNet_Duplicate");
        NNet newNet = nnGameObject.AddComponent<NNet>();

        newNet.Initialise(inputLayer.ColumnCount, outputLayer.ColumnCount, hiddenLayers.Count, hiddenLayers[0].ColumnCount);

        for (int i = 0; i < weights.Count; i++)
        {
            newNet.weights[i] = weights[i].Clone();
        }

        for (int i = 0; i < biases.Count; i++)
        {
            newNet.biases[i] = biases[i];
        }

        return newNet;
    }
    
}
