using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Manages a collection of track pieces and their checkpoints.
/// </summary>
public class RaceTrack : MonoBehaviour
{
    [Header("Start Position")]
    private static readonly string SPAWN = "SpawnPosition";
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

        FindChildTrackPieces(transform);
    }

    private void FindChildTrackPieces(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            if (child.CompareTag("flower_plant"))
            {
                // Found a track piece, add it to the flowerPlants list
                trackPieces.Add(child.gameObject);

                // Look for flowers within the flower plant
                FindChildTrackPieces(child);
            }
            else
            {
                // Not a flower plant, look for a Flower component
                Track track = child.GetComponent<Track>();
                if (track != null)
                {
                    // Found a track, add it to the track list
                    Tracks.Add(track);

                    // Add the nectar collider to the lookup dictionary
                    checkpointTrackDictionary.Add(track.checkpointCollider, track);

                    // Note: there are no track that are children of other track
                }
                else
                {
                    // Flower component not found, so check children
                    FindChildTrackPieces(child);
                }
            }
        }
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
