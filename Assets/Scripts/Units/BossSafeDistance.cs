using UnityEngine;

public class BossSafeDistance : MonoBehaviour
{
    public lichBoss boss;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            boss.SetPlayerNear(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            boss.SetPlayerNear(false);
    }
}
