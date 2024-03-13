using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using System.IO;
using System.Text;

[RequireComponent(typeof(NNet))]
public class RobotController : MonoBehaviour
{
    private Vector3 startPosition, startRotation;
    private NNet network;
    private shallowManager gridDisplayer;
    private int length, width;

    public float x, z;
    private int tilex, tilez;

    private GeneticManager geneticManager;
    public float timeSinceStart = 0f;

    [Header("Fitness")]
    public float overallFitness;
    public float distanceMultipler = 20f;
    public int goal = 0;
    public int timeToDie = 2;
    public int penalty;

    [Header("Network Options")]
    public int hiddenLayers = 1;
    public int hiddenNeurons = 5;

    private Vector3 lastPosition;
    private float totalDistanceTravelled;

    public float aSensor, bSensor, cSensor, dSensor, eSensor, fSensor; // a: left, b: forward, c: right. Detects blocks from the edge
    


    private int lastx, lastz;

    private void Awake() {
        gridDisplayer = FindObjectOfType<shallowManager>();
        startPosition = transform.position;
        length = gridDisplayer.length;
        width = gridDisplayer.width;

        network = GetComponent<NNet>();
        // Initialize the neural network with proper parameters
        network.Initialise(6, 2, hiddenLayers, hiddenNeurons); // 6 inputs (sensors), 2 outputs (x, z), hiddenLayers, hiddenNeurons
        
    }
    public NNet LoadNNet()
    {
        string directoryPath = @"C:\Users\daphe\Documents\GitHub\Tower-Defence-PCG\NNets";
        string fileName = "SavedNNet.txt"; // Ensure this matches the file name used in the save function
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
                for (int i = 0; i < loadedNNet.weights[k].RowCount; i++)
                {
                    for (int j = 0; j < loadedNNet.weights[k].ColumnCount; j++)
                    {
                        loadedNNet.weights[k][i, j] = float.Parse(sr.ReadLine());
                    }
                }
            }

            // Read and set biases
            for (int i = 0; i < loadedNNet.biases.Count; i++)
            {
                loadedNNet.biases[i] = float.Parse(sr.ReadLine());
            }
        }

        return loadedNNet;
    }


    public void ResetWithNetwork(NNet net) {
        network = net;
        Reset();
        
    }

    public void Reset() {
       
        timeSinceStart = 0f;
        totalDistanceTravelled = 0f;
        goal = 0;
        overallFitness = 0f;

        penalty = 0;
        tilex = (int)Math.Round(transform.position.x);
        tilez = (int)Math.Round(transform.position.z);
        lastx = tilex;
        lastz = tilez;
        // network = LoadNNet();
    }

 

    private void FixedUpdate() {
        InputSensors();
        lastPosition = transform.position;

        (x, z) = network.RunNetwork(aSensor, bSensor, cSensor, dSensor, eSensor, fSensor); // which direction to move, x and z

        MoveRobot(x, z);

        timeSinceStart += Time.deltaTime;
        bool f = gridDisplayer.fixedUpdates(tilex, tilez);

        CalculateFitness();
        if (aSensor < 0 || bSensor < 0 || cSensor < 0 || dSensor < 0 ) {
            
            Death();
        }
        if (timeSinceStart > timeToDie) {
            Death();
        }
        lastx = tilex;
        lastz = tilez;
        tilex = (int)Math.Round(transform.position.x);
        tilez = (int)Math.Round(transform.position.z);
        
        // if path is broken add penalty
        // if (f == true){
        //     if (lastx - (int)Math.Round(transform.position.x) != 0 && lastz - (int)Math.Round(transform.position.z) != 0){
        //         penalty += 1000;
        //     }
        //     lastx = tilex;
        //     lastz = tilez;
           
        // }
        // if walk on existing path add penalty
        if (f == false && (lastx != tilex || lastz != tilez)){
            penalty += 100;
           
        }
    }

    private void Death ()
    {
        if (goal == 1){
            overallFitness += 10000;
        }
        gridDisplayer.reset();
        lastPosition = transform.position;
        geneticManager = FindObjectOfType<GeneticManager>();
        geneticManager.Death(overallFitness, network);
        //Reset();

    }

    private void CalculateFitness() {
        totalDistanceTravelled += Vector3.Distance(transform.position, lastPosition);
        hitGoal();
        overallFitness = (totalDistanceTravelled * distanceMultipler) - penalty;



    }
    private void hitGoal() {
        if (gridDisplayer.distanceToGoal(transform.position.x, transform.position.z) < 0.5) {
            Debug.Log("hit goal");
            goal = 1;
            Death();    
        }
        
    }

    // a: left, b: forward, c: right. Detects blocks from the edge
    private void InputSensors() {
        aSensor = width - transform.position.z - 1;
        bSensor = length - transform.position.x - 1;
        cSensor = transform.position.z;
        dSensor = transform.position.x;
        eSensor = gridDisplayer.distanceToGoalX(transform.position.x);
        fSensor = gridDisplayer.distanceToGoalZ(transform.position.z);
    }

    private Vector3 inp;
    public void MoveRobot(float v, float h) {


        Vector3 moveZ = new Vector3(0, 0, v); // Move forward/backward along the Z-axis
        Vector3 moveX = new Vector3(h, 0, 0); // Move left/right along the X-axis

        Vector3 movement = moveZ + moveX;
        transform.position += movement; // Apply movement
    }
}
