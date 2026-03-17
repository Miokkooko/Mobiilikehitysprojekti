using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Class that handles UI tasks such as menus and notifications
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Menu MainMenu;

    [SerializeField]
    Canvas canvas;

    [SerializeField]
    Transform NotificationPrefab;

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
        Transform t = Instantiate(NotificationPrefab, canvas.transform);
        if (t.GetComponent<NotificationYesNo>() is NotificationYesNo n)
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

        if (e.Data.m_type != NotificationType.PopUp)
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
            if (menu.m_bDisappearPreviousMenu)
                DisappearMenu(MenuStack.Peek());
            else
                MenuStack.Peek().AllowInteraction(false);

        MenuStack.Push(menu);
        AppearMenu(menu);
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
        Menu menuToClose = MenuStack.Peek();

        if (MenuStack.Count > 1)
        {
            menuToClose.Disappear();

            MenuStack.Pop();
        }

        // If the menu we are closing force disappeared a previous menu, open the previous one again
        if (menuToClose.m_bDisappearPreviousMenu)
            AppearMenu(MenuStack.Peek());
        else
            MenuStack.Peek().AllowInteraction(true);
    }
    #endregion
}