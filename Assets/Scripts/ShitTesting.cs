using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShitTesting : MonoBehaviour
{
    NotificationBase n;
    public LevelUpData test;
    public StatusEffect effect;
    public void AddEffectToUnit(Unit target)
    {
        Unit.ApplyStatusEffect(effect, target);
    }

    public void TestLevelUp()
    {
        n = UIManager.Instance.CreateNotification(test);

        n.OnNotificationRaised += N_OnNotificationRaised;
    }

    private void N_OnNotificationRaised(object sender, NotificationBase.NotificationArgs e)
    {
        n.OnNotificationRaised -= N_OnNotificationRaised;

        if(e is LevelUpArgs args)
        {
            Debug.Log("Wow");
        }
        
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }
}
