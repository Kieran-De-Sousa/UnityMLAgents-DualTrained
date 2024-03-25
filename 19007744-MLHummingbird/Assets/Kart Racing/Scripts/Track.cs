using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Manages a single track piece and its checkpoint
/// </summary>
public class Track : MonoBehaviour
{

    /// <summary>
    /// The trigger collider representing the checkpoint.
    /// </summary>
    public Collider checkpointCollider;

    public bool checkpointTriggered = false;


    // ------------------------ Class Methods ------------------------ //
    /// <summary>
    /// Called when the track wakes up.
    /// </summary>
    private void Awake()
    {

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
