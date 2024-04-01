using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Manages a single track piece and its checkpoint
/// </summary>
public class Track : MonoBehaviour
{
    // The name of the checkpoint collider in the Unity Hierarchy.
    private static readonly string CHECKPOINT_COLLIDER = "CheckpointCollider";

    /// <summary>
    /// The trigger collider representing the checkpoint.
    /// </summary>
    public Collider checkpointCollider;

    public bool checkpointTriggered = false;

    /// <summary>
    /// A vector pointing forward from the checkpoint.
    /// </summary>
    public Vector3 CheckpointForwardVector => checkpointCollider.transform.forward;

    /// <summary>
    /// The centre position of the checkpoint collider.
    /// </summary>
    public Vector3 CheckpointCentrePosition => checkpointCollider.transform.position;

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

    public void CheckpointReached()
    {

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
