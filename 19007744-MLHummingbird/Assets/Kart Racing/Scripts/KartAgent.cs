using System;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class KartAgent : Agent
{
    [Header("Kart Stats")]
    public float maxSpeed = 8.0f;
    public float acceleration = 2.0f;
    public float steeringStrength = 3.0f;
    public float drag = 0.5f;

    [Space]

    [Tooltip("The agent's camera")]
    public Camera agentCamera;
    [Tooltip("Whether this is training mode or gameplay mode")]
    public bool trainingMode;

    new private Rigidbody _rigidbody;

    // The track the karting agent is in
    private RaceTrack _raceTrack;
    private Track _nearestTrackPiece;

    // Whether the agent is frozen (intentionally not moving)
    private bool _frozen = false;

    /// <summary>
    /// The number of checkpoints the agent has reached this episode.
    /// </summary>
    public float CheckpointsReached { get; private set; }

    private void Start()
    {
        UpdateNearestCheckpoint();
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Called every .02 seconds
    /// </summary>
    private void FixedUpdate()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Initialize the agent
    /// </summary>
    public override void Initialize()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.drag = drag;

        _raceTrack = GetComponentInParent<RaceTrack>();

        // if not in training mode, no max step, play forever
        if (!trainingMode)
        {
            MaxStep = 0;
        }
    }

    /// <summary>
    /// reset agent when ep begins
    /// </summary>
    public override void OnEpisodeBegin()
    {
        if (trainingMode)
        {

        }

        // Reset the race track.
        _raceTrack.ResetRaceTrack();
        // Reset number of checkpoints reached.
        CheckpointsReached = 0f;

        // Reset all movement.
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        // TODO: UNCOMMENT OUT WHEN IMPLEMENTATION OF START POSITION IS CREATED!
        /*// Reset to starting position.
        transform.position = _raceTrack.StartPosition.position;
        transform.rotation = _raceTrack.StartPosition.rotation;*/
    }

    /// <summary>
    /// Called when and action is received from either the player input or the neural network
    ///
    /// vectorAction[i] represents:
    /// Index 0: move vector z (+1 = forward, -1 = backward)
    /// Index 1: move float x (+1 = turn right, -1 = turn left)
    /// </summary>
    /// <param name="vectorAction">The actions to take</param>
    ///
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Don't take any actions if frozen.
        if (_frozen) return;

        // Get input for acceleration and steering from neural network
        float moveInput = actions.ContinuousActions[0];
        float turnInput = actions.ContinuousActions[1];

        // Apply acceleration
        Vector3 force = transform.forward * moveInput * acceleration;
        _rigidbody.AddForce(force);

        // Limit speed
        if (_rigidbody.velocity.magnitude > maxSpeed)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * maxSpeed;
        }

        // Apply steering
        Quaternion turn = Quaternion.Euler(0, turnInput * steeringStrength, 0);
        _rigidbody.MoveRotation(_rigidbody.rotation * turn);
    }

    /// <summary>
    /// Collect vector observations from the environment
    /// </summary>
    /// <param name="sensor">The vector sensor</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        // TODO: Need 10 OBSERVATIONS TO MATCH HUMMINGBIRD
    }

    /// <summary>
    /// When Behavior Type is set to "Heuristic Only" on the agent's Behavior Parameters,
    /// this function will be called. Its return values will be fed into
    /// <see cref="OnActionReceived(float[])"/> instead of using the neural network
    /// </summary>
    /// <param name="actionsOut">And output action array</param>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // TODO: Need 5 CONTINUOUS ACTIONS TO MATCH HUMMINGBIRD

        // Create placeholders for all movement/turning
        Vector3 forward = Vector3.zero;
        Vector3 turnInput = Vector3.zero;

        // Convert keyboard inputs to movement and turning
        // All values should be between -1 and +1

        // Forward/backward
        if (Input.GetKey(KeyCode.W)) forward = transform.forward;
        else if (Input.GetKey(KeyCode.S)) forward = -transform.forward;

        // Turn left/right
        if (Input.GetKey(KeyCode.A)) turnInput.y = -1f; // Turn left
        else if (Input.GetKey(KeyCode.D)) turnInput.y = 1f; // Turn right

        // Add the movement and turning values to the actionsOut array
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = forward.magnitude; // Forward/backward
        continuousActions[1] = turnInput.y; // Turn left/right
    }


    /// <summary>
    /// Prevent the agent from moving and taking actions
    /// </summary>
    public void FreezeAgent()
    {
        Debug.Assert(trainingMode == false, "Freeze/Unfreeze not supported in training");
        _frozen = true;
        _rigidbody.Sleep();
    }

    /// <summary>
    /// Resume agent movement and actions
    /// </summary>
    public void UnfreezeAgent()
    {
        Debug.Assert(trainingMode == false, "Freeze/Unfreeze not supported in training");
        _frozen = false;
        _rigidbody.WakeUp();
    }

    private void UpdateNearestCheckpoint()
    {

    }
}
