using UnityEngine;

public class InGameManager : MonoBehaviour
{
    void Start()
    {
        Physics.gravity = new Vector3(0f, GameManager.Instance.gravityStrength, 0f);
    }
}
