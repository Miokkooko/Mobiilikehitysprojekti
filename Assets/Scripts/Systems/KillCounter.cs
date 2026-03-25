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
        addOneKill();
    }

    public void addOneKill(){
        killCount += 1;
        Debug.Log(killCount);
        //countText.text = killCount.ToString();
    }

    public void resetKillCount(){
        killCount = 0;
    }
}
