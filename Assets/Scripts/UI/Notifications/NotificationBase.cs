using UnityEngine;
using System;
using System.Collections;
public class NotificationArgs : EventArgs
{
    public NotificationData Data;

    public bool Confirmed;
    public NotificationArgs() { }
    public NotificationArgs(NotificationData _data)
    {
        Data = _data;
    }
    public NotificationArgs(NotificationData _data, bool confirmed)
    {
        Data = _data;
        Confirmed = confirmed;
    }
}
public enum NotificationType { Confirm, ConfirmCancel, PopUp, LevelUp }
public class NotificationBase : MonoBehaviour
{
    public NotificationData Data;

    public event EventHandler<NotificationArgs> OnNotificationRaised;
    public event EventHandler<NotificationArgs> OnNotificationDestroyed;
    

    public virtual void Initialize(NotificationData data) { Data = data; }
    public virtual void Initialize() { }

    public void RaiseNotificationEvent(NotificationArgs args) 
    {
        OnNotificationRaised?.Invoke(this, args);
    }

    void RaiseNotificationDestroyedEvent(NotificationArgs args)
    {
        OnNotificationDestroyed?.Invoke(this, args);
    }

    private void OnDestroy()
    {
        RaiseNotificationDestroyedEvent(new NotificationArgs(Data));
        Debug.Log("Notification destroyed");

    }
}
