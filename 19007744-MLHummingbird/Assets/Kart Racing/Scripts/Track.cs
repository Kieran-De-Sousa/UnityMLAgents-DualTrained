using UnityEngine;

/// <summary>
/// Manages a single track piece and its checkpoint
/// </summary>
public class Track : MonoBehaviour
{
    // The name of the checkpoint collider in the Unity Hierarchy.
    private static readonly string CHECKPOINT_COLLIDER = "CheckpointCollider";

    [Header("Checkpoints")]
    [Tooltip("The trigger collider representing the checkpoint.")]
    public Collider checkpointCollider;
    [Tooltip("Whether the checkpoint has been hit or not.")]
    public bool checkpointTriggered = false;
    [Tooltip("The reward to be given for hitting the checkpoint.")]
    public float checkpointValue = 0.1f;

    /// <summary>
    /// A vector pointing forward from the checkpoint.
    /// </summary>
    public Vector3 CheckpointForwardVector => checkpointCollider.transform.forward;

    /// <summary>
    /// The centre position of the checkpoint collider.
    /// </summary>
    public Vector3 CheckpointCentrePosition => checkpointCollider.bounds.center;

    public bool HasHitCheckpoint => checkpointTriggered;


    // ------------------------ Class Methods ------------------------ //
    /// <summary>
    /// Called when the track wakes up.
    /// </summary>
    private void Awake()
    {
        // Find checkpoint collider.
        checkpointCollider = transform.Find(CHECKPOINT_COLLIDER).GetComponent<Collider>();
    }

    /// <summary>
    /// Attempts to reward checkpoint hit completion.
    /// </summary>
    /// <returns>The value for hitting a checkpoint.</returns>
    public float CheckpointReached()
    {
        if (!HasHitCheckpoint)
        {
            // Turn off checkpoint collider and set triggered state.
            checkpointTriggered = true;
            checkpointCollider.enabled = false;

            return checkpointValue;
        }

        return 0;
    }

    /// <summary>
    /// Resets the track piece.
    /// </summary>
    public void ResetTrackPiece()
    {
        checkpointTriggered = false;
        checkpointCollider.enabled = true;
    }
    // ------------------------ Class Methods ------------------------ //
}
