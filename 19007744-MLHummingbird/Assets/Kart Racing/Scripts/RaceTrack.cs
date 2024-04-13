using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Manages a collection of track pieces and their checkpoints.
/// </summary>
public class RaceTrack : MonoBehaviour
{
    private static readonly string SPAWN = "SpawnPosition";
    private static readonly string TRACK_PIECE = "track_piece";

    // Start position of the racer.
    public Transform StartPosition { get; private set; }

    /// <summary>
    /// The list of all tracks in the race track.
    /// </summary>
    public List<Track> Tracks;

    // Number of laps to complete the track.
    public int TotalLaps = 3;

    // The diameter of the race track where the agent and the track pieces will be,
    // used for observing the relative distance from an agent to a checkpoint.
    public float TrackDiameter { get; private set; }

    // The list of all track pieces in this Track.
    private List<GameObject> trackPieces;
    // A lookup dictionary for looking up a track from a checkpoint collider
    private Dictionary<Collider, Track> checkpointTrackDictionary;

    private void Awake()
    {
        // Initialize variables
        trackPieces = new List<GameObject>();
        checkpointTrackDictionary = new Dictionary<Collider, Track>();
        StartPosition = transform.Find(SPAWN).GetComponent<Transform>();
        TrackDiameter = CalculateTrackDiameter();

        FindChildTrackPieces();
    }

    private void FindChildTrackPieces()
    {
        foreach (Track track in Tracks)
        {
            // Add the checkpoint collider to the lookup dictionary
            checkpointTrackDictionary.Add(track.checkpointCollider, track);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    private float CalculateTrackDiameter()
    {
        float maxDistance = 0f;

        foreach (Track track in Tracks)
        {
            // Calculate the distance from the start position to the track piece
            float distance = Vector3.Distance(track.transform.position, StartPosition.position);

            // Update the maximum distance if needed
            if (distance > maxDistance)
            {
                maxDistance = distance;
            }
        }

        // Diameter is twice the maximum distance from the start position
        float diameter = maxDistance * 2f;

        return diameter;
    }

    /// <summary>
    /// Gets the <see cref="Track"/> that a checkpoint collider belongs to.
    /// </summary>
    /// <param name="collider">The checkpoint collider.</param>
    /// <returns>The matching track piece.</returns>
    public Track GetTrackPieceFromCheckpointCollider(Collider collider)
    {
        return checkpointTrackDictionary[collider];
    }

    public void CompletedLap()
    {
        ResetRaceTrack();
    }

    public void CompletedTrack()
    {

    }

    /// <summary>
    /// Reset the track pieces.
    /// </summary>
    public void ResetRaceTrack()
    {
        // Reset each track piece.
        foreach (var track in Tracks)
        {
            track.ResetTrackPiece();
        }
    }
}
