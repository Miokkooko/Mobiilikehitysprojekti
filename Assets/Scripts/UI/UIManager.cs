using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
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
    Transform NotificationYesNoPrefab;
    [SerializeField]
    Transform NotificationLevelUpPrefab;

    Stack<Menu> MenuStack = new Stack<Menu>();

    Queue<NotificationBase> NotificationQueue = new Queue<NotificationBase>();
    Queue<NotificationBase> PopUpNotificationQueue = new Queue<NotificationBase>();

    private void Awake()
    {
        Instance = this;

        if (MainMenu != null)
            OpenMenu(MainMenu);
        else
            Debug.Log("Main Menu not set!");
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
                t = Instantiate(NotificationYesNoPrefab, canvas.transform);
                break;
            case NotificationType.PopUp:
                break;
            case NotificationType.LevelUp:
                t = Instantiate(NotificationLevelUpPrefab, canvas.transform);
                break;
            default:
                break;
        }

        if(t == null)
        {
            Debug.Log("No prefab found for notification!");
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

        return null;
    }
  
    /// <summary>
    /// Handles the queueing, allowing only 1 of each type of popup to be active at the same time
    /// </summary>
    void HandleQueue()
    {
        if (NotificationQueue.Count > 0)
        {
            if (!NotificationQueue.Peek().gameObject.activeInHierarchy)
            {
                NotificationQueue.Peek().gameObject.SetActive(true);
            }
        }

        if (PopUpNotificationQueue.Count > 0)
        {
            if (!PopUpNotificationQueue.Peek().gameObject.activeInHierarchy)
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
    void OnNotificationDestroyed(object sender, NotificationBase.NotificationArgs e)
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

            previousMenu = CurrentMenu;
        }
        if(!MenuStack.Contains(menu))
            MenuStack.Push(menu);
        AppearMenu(menu);

        Debug.Log("Menu opened | " + MenuStack.Count + " - Menus total");
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

        if (MenuStack.Count > 1)
        {
            DisappearMenu(menuToClose);

            MenuStack.Pop();
            Debug.Log("Menu closed | " + MenuStack.Count + " - Menus total");
        }

        // If the menu we are closing force disappeared a previous menu, open the previous one again
        if (menuToClose.m_bDisappearPreviousMenu)
            AppearMenu(CurrentMenu);
        else
            CurrentMenu.AllowInteraction(true);
    }
    #endregion
}