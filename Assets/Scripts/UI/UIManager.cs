using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Class that handles UI tasks such as menus and notifications
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Menu MainMenu;
    Menu CurrentMenu => MenuStack.Peek();
    Menu previousMenu;

    [SerializeField]
    Canvas canvas;
    [SerializeField]
    Canvas notificationCanvas;
    public Canvas GetCanvas => canvas;

    [SerializeField]
    Transform NotificationYesNoPrefab;
    [SerializeField]
    Transform NotificationLevelUpPrefab;
    [SerializeField]
    Transform NotificationPopUpPrefab;

    Stack<Menu> MenuStack = new Stack<Menu>();

    Queue<NotificationBase> NotificationQueue = new Queue<NotificationBase>();
    Queue<NotificationBase> PopUpNotificationQueue = new Queue<NotificationBase>();

    private void Start()
    {
        Instance = this;

        if (MainMenu != null)
            OpenMenu(MainMenu);
        else
            Debug.LogWarning("Main Menu not set!");
    }

    #region Notifications
    /// <summary>
    /// Creates a notification and returns the reference to it.
    /// Use the reference to subscribe to the OnNotificationRaised event
    /// </summary>
    /// <param name="data"></param>
    /// <returns>NotificationBase
    /// NotificationYesNo
    /// NotificationPopUp</returns>
    public NotificationBase CreateNotification(NotificationData data)
    {
        Transform t = null;
        switch (data.Type)
        {
            case NotificationType.Confirm:
                break;
            case NotificationType.ConfirmCancel:
                NotificationYesNoPrefab.gameObject.SetActive(false);
                t = Instantiate(NotificationYesNoPrefab, notificationCanvas.transform);
                NotificationYesNoPrefab.gameObject.SetActive(true);
                break;
            case NotificationType.PopUp:
                NotificationPopUpPrefab.gameObject.SetActive(false);
                t = Instantiate(NotificationPopUpPrefab, notificationCanvas.transform);
                NotificationPopUpPrefab.gameObject.SetActive(false);
                break;
            case NotificationType.LevelUp:
                NotificationLevelUpPrefab.gameObject.SetActive(false);
                t = Instantiate(NotificationLevelUpPrefab, notificationCanvas.transform);
                NotificationLevelUpPrefab.gameObject.SetActive(false);
                break;
            default:
                break;
        }

        if(t == null)
        {
            Debug.LogError("No prefab found for notification!");
            return null;
        }

        if (t.GetComponent<NotificationBase>() is NotificationBase n)
        {
            n.Initialize(data);

            NotificationQueue.Enqueue(n);

            n.OnNotificationDestroyed += OnNotificationDestroyed;

            HandleQueue();
            return n;
        }

        if (t.GetComponent<NotificationPopUp>() is NotificationPopUp np)
        {
            np.Initialize(data);

            PopUpNotificationQueue.Enqueue(np);

            np.OnNotificationDestroyed += OnNotificationDestroyed;

            HandleQueue();
            return np;
        }

        return null;
    }

    public NotificationBase CreateNotificationPopUp(string title, string description)
    {
        NotificationPopUpPrefab.gameObject.SetActive(false);
        Transform t = Instantiate(NotificationPopUpPrefab, notificationCanvas.transform);
        NotificationPopUpPrefab.gameObject.SetActive(false);

        if(t == null)
        {
            Debug.LogError("No prefab found for notification!");
            return null;
        }

        if (t.GetComponent<NotificationPopUp>() is NotificationPopUp n)
        {
            n.Initialize(title, description);

            PopUpNotificationQueue.Enqueue(n);

            n.OnNotificationDestroyed += OnNotificationDestroyed;

            HandleQueue();
            return n;
        }

        return null;
    }
  
    /// <summary>
    /// Handles the queueing, allowing only 1 of each type of notification to be active at the same time
    /// </summary>
    void HandleQueue()
    {
        if (NotificationQueue.Count > 0)
        {
            if (NotificationQueue.Peek() != null && !NotificationQueue.Peek().gameObject.activeInHierarchy)
            {
                NotificationQueue.Peek().gameObject.SetActive(true);
            }
        }

        if (PopUpNotificationQueue.Count > 0)
        {
            if (PopUpNotificationQueue.Peek() != null && !PopUpNotificationQueue.Peek().gameObject.activeInHierarchy)
            {
                PopUpNotificationQueue.Peek().gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Listens to whenever a notification is destroyed, so we can queue up the next notification
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void OnNotificationDestroyed(object sender, NotificationArgs e)
    {
        if (sender is NotificationBase n)
            n.OnNotificationDestroyed -= OnNotificationDestroyed;

        if (e.Data.Type != NotificationType.PopUp)
            NotificationQueue.Dequeue();
        else
            PopUpNotificationQueue.Dequeue();

        HandleQueue();
    }
    #endregion

    #region Menus
    public void OpenMenu(Menu menu)
    {
        if (MenuStack.Count > 0)
        {
            if (menu == CurrentMenu)
                return;
            else if (menu.m_bDisappearPreviousMenu || menu == previousMenu)
                CloseMenu();
            else
                CurrentMenu.AllowInteraction(false);
        }

        if (MenuStack.Contains(menu))
        {
            Debug.Log("Menu already in MenuStack!");
            return;
        }
            
        
        MenuStack.Push(menu);
        AppearMenu(menu);
        CurrentMenu.AllowInteraction(true);

        Debug.Log($"{CurrentMenu.name} opened | {MenuStack.Count} - Menus total");
    }

    void AppearMenu(Menu menu)
    {
        menu.Appear();
    }

    void DisappearMenu(Menu menu)
    {
        menu.Disappear();
    }

    public void CloseMenu()
    {
        Menu menuToClose = CurrentMenu;

        if (MenuStack.Count >= 1)
        {
            DisappearMenu(menuToClose);

            MenuStack.Pop();
            Debug.Log($"{menuToClose.name} closed | {MenuStack.Count} - Menus total");
        }

        if (MenuStack.Count == 0)
            return;

        // If the menu we are closing force disappeared a previous menu, "Appear" the previous one again
        if (menuToClose.m_bDisappearPreviousMenu)
            AppearMenu(CurrentMenu);
        else
            CurrentMenu.AllowInteraction(true);
    }
    #endregion
}