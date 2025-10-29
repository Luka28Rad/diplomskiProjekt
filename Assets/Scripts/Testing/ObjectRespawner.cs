using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectRespawner : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The GameObject (prefab) to be spawned.")]
    private GameObject obj;

    [SerializeField]
    [Tooltip("The transform where the object will be spawned.")]
    private Transform respawnPos;

    [SerializeField]
    [Tooltip("The Input Action to trigger the spawn. We will link the 'A' button to this.")]
    private InputActionReference spawnAction;

    private void OnEnable()
    {
        spawnAction.action.performed += SpawnObject;
        spawnAction.action.Enable();
    }

    private void OnDisable()
    {
        spawnAction.action.performed -= SpawnObject;
        spawnAction.action.Disable();
    }

    private void SpawnObject(InputAction.CallbackContext context)
    {
        if (obj != null && respawnPos != null)
        {
            int randomMass = Random.Range(1, 7);
            randomMass *= 5;
            //float randomMass = 0.1f;
            GameObject createdObj = Instantiate(obj, respawnPos.position, respawnPos.rotation);
            createdObj.GetComponent<Rigidbody>().mass = randomMass;
            createdObj.GetComponentInChildren<TextMeshPro>().text = randomMass.ToString();
        }
        else
        {
            if (obj == null)
            {
                Debug.LogWarning("Object to spawn (Prefab) is not assigned in the ObjectRespawner script.");
            }
            if (respawnPos == null)
            {
                Debug.LogWarning("Respawn Position is not assigned in the ObjectRespawner script.");
            }
        }
    }
}
