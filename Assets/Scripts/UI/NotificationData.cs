using TMPro;
using UnityEngine;


[CreateAssetMenu(fileName = "NotificationData", menuName = "Data/NotificationData", order = 2)]
public class NotificationData : ScriptableObject
{
    public int id;
    public string m_sTitle = "Attention!";

    [TextArea(7, 10)]
    public string m_sDescription = "This is a notification";

    public NotificationType m_type;

    //public string[] m_sAdditionalText;
}
