using UnityEngine;

/// <summary>
/// Manages a single flower with nectar.
/// </summary>
public class Flower : MonoBehaviour
{
    [Tooltip("The colour when the flower is full.")]
    public Color fullFlowerColour = new Color(1f, 0f, 0.3f);
    [Tooltip("The colour when the flower is empty.")]
    public Color emptyFlowerColour = new Color(0.5f, 0f, 1f);

    /// <summary>
    /// The trigger collider representing the nectar.
    /// </summary>
    [HideInInspector]
    public Collider nectarCollider;
    // Solid collider representing the flower petals.
    private Collider flowerCollider;
    // Flower material.
    private Material flowerMaterial;

    /// <summary>
    /// A vector pointing straight out of the flower.
    /// </summary>
    public Vector3 FlowerUpVector
    {
        get { return nectarCollider.transform.up; }
    }
    /// <summary>
    /// The centre position of the nectar collider.
    /// </summary>
    public Vector3 FlowerCentrePosition
    {
        get { return nectarCollider.transform.position; }
    }

    /// <summary>
    /// The amount of nectar remaining in the flower.
    /// </summary>
    public float NectarAmount { get; private set; }
    /// <summary>
    /// Whether the flower has any nectar remaining
    /// </summary>
    public bool HasNectar
    {
        get { return NectarAmount > 0f; }
    }

    // ------------------------ Class Methods ------------------------ //

    /// <summary>
    /// Called when the flower wakes up.
    /// </summary>
    private void Awake()
    {
        // Find the flower's mesh renderer and get the main material.
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        flowerMaterial = meshRenderer.material;

        // Find flower and nectar colliders.
        flowerCollider = transform.Find("FlowerCollider").GetComponent<Collider>();
        nectarCollider = transform.Find("FlowerNectarCollider").GetComponent<Collider>();
    }

    /// <summary>
    /// Attempts to remove nectar from the flower.
    /// </summary>
    /// <param name="amount">The amount of nectar to remove.</param>
    /// <returns>The amount successfully removed.</returns>
    public float Feed(float amount)
    {
        // Track how much nectar was successfully taken (cannot take more than is available).
        float nectarTaken = Mathf.Clamp(amount, 0f, NectarAmount);

        // Subtract the nectar.
        NectarAmount -= amount;

        if (NectarAmount <= 0)
        {
            // No nectar remaining.
            NectarAmount = 0;

            flowerCollider.gameObject.SetActive(false);
            nectarCollider.gameObject.SetActive(false);

            // Change the flower color to indicate that it is empty
            flowerMaterial.SetColor("_BaseColor", emptyFlowerColour);
        }

        // Return the amount of nectar that was taken.
        return nectarTaken;
    }

    /// <summary>
    /// Resets the flower.
    /// </summary>
    public void ResetFlower()
    {
        // Refill the nectar.
        NectarAmount = 1f;

        // Enable the flower and nectar colliders.
        flowerCollider.gameObject.SetActive(true);
        nectarCollider.gameObject.SetActive(true);

        // Change the flower colour to indicate that it is full.
        flowerMaterial.SetColor("_BaseColor", fullFlowerColour);
    }
    // ------------------------ Class Methods ------------------------ //
}
