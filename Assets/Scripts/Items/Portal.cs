using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;
using UnityEngine.UIElements;

public class Portal : MonoBehaviour
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
            PoolManager.Instance.DisableAllEnemies(EnemyPoolType.GenericEnemy);
            player.enabled = false;
            player.GetComponent<SpriteRenderer>().enabled = false;
            GameManager.Instance._currentState = GameManager.GameState.AfterBoss;
            GameManager.Instance.miniBossInterval = GameManager.Instance.miniBossInterval *0.6f;

            Vector3 pos = playerMainSpawn.transform.position;

            mainCamera.enabled = false;

            GameManager.Instance.SetNormalSettings();

            player.transform.position = playerMainSpawn.transform.position;
            player.GetComponent<SpriteRenderer>().enabled = true;
            player.enabled = true;
            mainCamera.transform.position = new Vector3(pos.x, pos.y, -10f);
            mainCamera.enabled = true;
            this.enabled = false;
        }
        
    }
}
