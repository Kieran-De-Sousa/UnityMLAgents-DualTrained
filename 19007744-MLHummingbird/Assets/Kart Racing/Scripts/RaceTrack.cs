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

    // Start position of the racer
    public Transform StartPosition { get; private set; }
    // The list of all track pieces in this Track.
    private List<GameObject> trackPieces;
    // A lookup dictionary for looking up a track from a checkpoint collider
    private Dictionary<Collider, Track> checkpointTrackDictionary;

    /// <summary>
    /// The list of all tracks in the race track.
    /// </summary>
    public List<Track> Tracks { get; private set; }

    private void Awake()
    {
        // Initialize variables
        trackPieces = new List<GameObject>();
        checkpointTrackDictionary = new Dictionary<Collider, Track>();
        Tracks = new List<Track>();
        StartPosition = transform.Find(SPAWN).GetComponent<Transform>();

        FindChildTrackPieces(transform);
    }

    private void FindChildTrackPieces(Transform parent)
    {
        for (int i = 0; i < parent.childCount; ++i)
        {
            Transform child = parent.GetChild(i);

            if (child.CompareTag(TRACK_PIECE))
            {
                // Found a track piece, add it to the trackPieces list.
                trackPieces.Add(child.gameObject);

                // Look for track component in track piece.
                Track track = child.GetComponent<Track>();

                if (track != null)
                {
                    // Found a track, add it to the track list
                    Tracks.Add(track);

                    // Add the checkpoint collider to the lookup dictionary
                    checkpointTrackDictionary.Add(track.checkpointCollider, track);

                    // Note: there are no track that are children of other track.
                }
            }
        }
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
