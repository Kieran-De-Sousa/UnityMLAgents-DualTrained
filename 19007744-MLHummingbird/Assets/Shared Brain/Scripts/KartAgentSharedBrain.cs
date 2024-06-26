using System;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class KartAgentSharedBrain : Agent
{
    [Header("Kart Stats")]
    public float maxSpeed = 8.0f;
    public float acceleration = 2.0f;
    public float steeringStrength = 3.0f;
    public float drag = 0.5f;
    [Tooltip("The front tip of the kart.")]
    public Transform kartTip;

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

    // The agent's current reward for debugging.
    private float _currentReward = 0f;

    // The agent's current checkpoint on the race track (Resets with new lap).
    private int _currentCheckpoint = 0;

    // The agent's current lap count.
    private int _lapsComplete = 0;

    /// <summary>
    /// The number of checkpoints the agent has reached this episode.
    /// </summary>
    public int TotalCheckpointsReached { get; private set; }

    /// <summary>
    /// The value of the checkpoints the agent has reached this episode.
    /// </summary>
    public float CheckpointsValue { get; private set; }

    private void Start()
    {
        UpdateNearestCheckpoint();
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        // Draw a line from the kart tip to the nearest checkpoint.
        if (_nearestTrackPiece != null)
        {
            Debug.DrawLine(kartTip.position, _nearestTrackPiece.CheckpointCentrePosition, Color.green);
        }
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
        // Reset the agent
        ResetRacer();

        UpdateNearestCheckpoint();
    }

    /// <summary>
    /// Called when and action is received from either the player input or the neural network
    ///
    /// vectorAction[i] represents:
    /// Index 0: move vector z (+1 = forward, -1 = backward)
    /// Index 1: move float x (+1 = turn right, -1 = turn left)
    /// Index 2: Padded input (to match Hummingbird)
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

        // Pad input with non-action.
        float paddedInput = actions.ContinuousActions[2]; // -------- PADDED INPUT --------

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
    /// Collect vector observations from the environment.
    /// </summary>
    /// <param name="sensor">The vector sensor</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        // If _nearestTrackPiece is null, observe an empty array and return early.
        if (_nearestTrackPiece == null)
        {
            sensor.AddObservation(new float[10]);
            return;
        }

        // (4 observations)
        // Observe the agent's local rotation.
        sensor.AddObservation(transform.localRotation.normalized);

        Vector3 toCheckpoint = _nearestTrackPiece.CheckpointCentrePosition - kartTip.position;

        // (3 observations)
        // Observe a normalized vector pointing to the nearest checkpoint.
        sensor.AddObservation(toCheckpoint.normalized);

        // (1 observation)
        // Observe a dot product that indicates whether the kart tip is in front of the checkpoint.
        // (+1 means that the kart tip is directly in front of the checkpoint, -1 means directly behind)
        sensor.AddObservation(Vector3.Dot(toCheckpoint.normalized, -_nearestTrackPiece.CheckpointForwardVector.normalized));

        // (1 observation)
        // Observe a dot product that indicates whether the kart is pointing toward the checkpoint.
        // (+1 means that the kart is pointing directly at the checkpoint, -1 means directly away)
        sensor.AddObservation(Vector3.Dot(kartTip.forward.normalized, -_nearestTrackPiece.CheckpointForwardVector.normalized));

        // (1 observation)
        // Observe the relative distance from the kart tip to the checkpoint.
        sensor.AddObservation(toCheckpoint.magnitude / _raceTrack.TrackDiameter);
    }

    /// <summary>
    /// When Behavior Type is set to "Heuristic Only" on the agent's Behavior Parameters,
    /// this function will be called. Its return values will be fed into
    /// <see cref="OnActionReceived(float[])"/> instead of using the neural network
    /// </summary>
    /// <param name="actionsOut">And output action array</param>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Create placeholders for all movement/turning
        float forward = 0f;
        Vector3 turnInput = Vector3.zero;

        // -------- PADDED INPUT --------
        float paddedInput = 0f;

        // Convert keyboard inputs to movement and turning
        // All values should be between -1 and +1

        // Forward/backward
        if (Input.GetKey(KeyCode.W)) forward = 1f; // Move forward
        else if (Input.GetKey(KeyCode.S)) forward = -1f; // Move backward

        // Turn left/right
        if (Input.GetKey(KeyCode.A)) turnInput.y = -1f; // Turn left
        else if (Input.GetKey(KeyCode.D)) turnInput.y = 1f; // Turn right

        Debug.Log(forward);

        // Add the movement and turning values to the actionsOut array
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = forward; // Forward/backward
        continuousActions[1] = turnInput.y; // Turn left/right
        continuousActions[2] = paddedInput; // -------- PADDED INPUT --------
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

    private void ResetRacer()
    {
        // Reset number of checkpoints reached and value.
        TotalCheckpointsReached = 0;
        CheckpointsValue = 0f;

        // Private tracking variables.
        _currentCheckpoint = 0;
        _currentReward = 0f;
        _lapsComplete = 0;

        // Reset all movement.
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        // Reset to starting position.
        transform.position = _raceTrack.StartPosition.position;
        transform.rotation = _raceTrack.StartPosition.rotation;
    }

    private void UpdateNearestCheckpoint()
    {
        _nearestTrackPiece = _raceTrack.Tracks[_currentCheckpoint];
    }

    /// <summary>
    /// Called when the agent's collider enters a trigger collider.
    /// </summary>
    /// <param name="other">The trigger collider.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if agent has collided with checkpoint.
        if (other.CompareTag("objective"))
        {
            // Look up the track piece from this checkpoint collider.
            Track track = _raceTrack.GetTrackPieceFromCheckpointCollider(other);

            // Check if we have hit this checkpoint already, and if this is the next checkpoint.
            if (!track.HasHitCheckpoint && _nearestTrackPiece == track)
            {
                // Increment total number of checkpoints hit by 1.
                TotalCheckpointsReached += 1;

                // Attempt to reward agent for hitting a checkpoint.
                float checkpointValue = track.CheckpointReached();

                // Keep track of checkpoint value reached.
                CheckpointsValue += checkpointValue;
                _currentReward += CheckpointsValue * TotalCheckpointsReached;

                // Reward the agent for training mode.
                if (trainingMode)
                {
                    // Calculate reward for hitting a checkpoint
                    // Add reward by multiplying number of checkpoints reached by the value of all checkpoints combined.
                    AddReward(CheckpointsValue * TotalCheckpointsReached);
                }

                if (_currentCheckpoint >= _raceTrack.Tracks.Count - 1)
                {
                    _currentCheckpoint = 0;
                    _lapsComplete += 1;
                    _raceTrack.CompletedLap();
                }
                else
                {
                    _currentCheckpoint += 1;
                }

                UpdateNearestCheckpoint();
            }

            else if (!track.HasHitCheckpoint && _nearestTrackPiece != track)
            {
                // Punish the agent heavily for hitting a checkpoint that was not the next one.
                AddReward(-5.0f);
                _currentReward -= 5.0f;
            }
        }
    }

    /// <summary>
    /// Called when the agent collides with something solid.
    /// </summary>
    /// <param name="other">The collision info.</param>
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("obstacle"))
        {
            // Collided with the walls of track, give a negative reward.
            _currentReward -= 0.5f;

            if (trainingMode)
            {
                // Collided with the walls of track, give a negative reward.
                AddReward(-0.5f);
            }
        }
    }
}