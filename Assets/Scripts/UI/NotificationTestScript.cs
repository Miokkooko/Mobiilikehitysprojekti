using UnityEngine;

public class NotificationTestScript : MonoBehaviour
{
    NotificationBase n;

    private void Notification_OnNotificationResult(object sender, NotificationYesNo.NotificationArgs e)
    {
        n = (NotificationBase)sender;
        n.OnNotificationRaised -= Notification_OnNotificationResult;

        if (e.Confirmed)
        {
            Test();
        }
    }

    public void SpawnNotification(NotificationData data)
    {
        for (int i = 0; i < 2; i++)
        {
            n = UIManager.Instance.CreateNotification(data);

            n.OnNotificationRaised += Notification_OnNotificationResult;
        }
    }

    void Test()
    {
        Debug.Log("Works!");
    }
}
