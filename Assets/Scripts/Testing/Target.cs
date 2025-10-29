using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private int pointsValue;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ThrowablePoints"))
        {
            ScoreManager.Instance.AddPoints(pointsValue);
            Destroy(collision.gameObject);
        }
    }
}