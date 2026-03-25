using UnityEngine;
using UnityEngine.UI;

public class KillCounter : MonoBehaviour
{
    public int killCount = 0;
    //public Text countText;

    public void addOneKill(){
        killCount += 1;
        Debug.Log(killCount);
        //countText.text = killCount.ToString();
    }

    public void resetKillCount(){
        killCount = 0;
    }
}
