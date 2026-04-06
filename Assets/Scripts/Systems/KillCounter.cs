using UnityEngine;
using UnityEngine.UI;

public class KillCounter : MonoBehaviour
{
    public int killCount = 0;
    //public Text countText;
    public Player player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.OnKill += Player_OnKill;
    }
    private void OnDestroy()
    {
        player.OnKill -= Player_OnKill;
    }

    private void Player_OnKill(object sender, KillContext e)
    {
        killCount += 1;

        if(killCount % 2 == 0)
        {
            //LevelUpManager.Instance.TriggerLevelUp();
        }
    }

    public void resetKillCount(){
        killCount = 0;
    }
}
