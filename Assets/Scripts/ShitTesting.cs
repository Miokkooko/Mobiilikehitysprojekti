using UnityEngine;
using UnityEngine.SceneManagement;

public class ShitTesting : MonoBehaviour
{
    NotificationBase n;
    public NotificationData test;
    public StatusEffect effect;
    public void AddEffectToUnit(Unit target)
    {
        Unit.ApplyStatusEffect(effect, target);
    }

    public void TestNotification()
    {
        n = UIManager.Instance.CreateNotification(test);

        n.OnNotificationRaised += N_OnNotificationRaised;
    }

    private void N_OnNotificationRaised(object sender, NotificationArgs e)
    {
        n.OnNotificationRaised -= N_OnNotificationRaised;

        if (e.Confirmed)
        {
            SaveManager.DeleteSave();
        }
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }
}
