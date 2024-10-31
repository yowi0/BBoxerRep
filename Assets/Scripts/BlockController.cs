using System.Collections;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    public GameObject cutDirectionArrow;
    private HealthBar healthBar;
    private bool hasBeenCut = false; // Afegeix una variable per controlar si el cub ha estat tallat

    private Vector3[] cutDirectionRotations = new Vector3[]
    {
        new Vector3(0, 0, 0),      // Up
        new Vector3(0, 0, 180),    // Down
        new Vector3(0, 0, 90),     // Left
        new Vector3(0, 0, -90),    // Right
        new Vector3(0, 0, 45),    // Up-Left
        new Vector3(0, 0, -45),   // Up-Right
        new Vector3(0, 0, 130),    // Down-Left
        new Vector3(0, 0, -130),     // Down-Right
        new Vector3(0, 90, 90)      // Any
    };

    private Vector3 moveDirection = Vector3.back;
    public float moveSpeed;
    private int cutDirectionIndex;

    private void OnDisable()
    {
        GetComponentInChildren<Light>().enabled = false;
    }
    private void OnDestroy()
    {
        Destroy(gameObject);
    }

    public void Initialize(BeatSaberBlockSpawner.BeatSaberBlockData blockData)
    {
        cutDirectionIndex = blockData._cutDirection;

        if (cutDirectionIndex >= 0 && cutDirectionIndex < cutDirectionRotations.Length)
        {
            transform.eulerAngles = cutDirectionRotations[cutDirectionIndex];
        }
    }

    private IEnumerator FailureCheck()
    {
        yield return new WaitForSeconds(1.8f);

        if (gameObject.activeSelf  && !hasBeenCut)
        {
            OnFailedToBreakCube();
        }
    }

    private void OnFailedToBreakCube()
    {
        if (healthBar != null)
        {
            healthBar.TakeDamage();
        }
    }

    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if (transform.position.magnitude > 50)
        {
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        healthBar = FindObjectOfType<HealthBar>();
        StartCoroutine(FailureCheck());
        Destroy(gameObject, 5);
    }

    public Vector3 GetCutDirection()
    {
        if (cutDirectionIndex >= 0 && cutDirectionIndex < cutDirectionRotations.Length)
        {
            return transform.TransformDirection(cutDirectionRotations[cutDirectionIndex]);
        }
        return Vector3.zero;
    }

    // Afegeix aquesta funció per indicar que el cub ha estat tallat
    public void MarkAsCut()
    {
        hasBeenCut = true;
    }
}
