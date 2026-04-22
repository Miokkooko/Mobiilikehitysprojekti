using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPortal : MonoBehaviour
{
    public GameObject playerMainSpawn;
    public Camera mainCamera;
    public Player player;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !gameObject.activeInHierarchy)
        {
            return;
        }

        if (other.GetComponent<Player>() is Player p)
        {
            SaveManager.SaveRun(player, GameManager.Instance.Kills, GameManager.Instance.GameTime, GameManager.Instance.Coins);
            SceneManager.LoadScene(0);
        }

    }
}
