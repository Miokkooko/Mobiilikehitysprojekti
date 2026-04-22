using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoreScreen : MonoBehaviour
{
    public TMP_Text CoinsText;

    public PullAnimator pullAnimator;

    public Banner[] bannerObjects;

    public Transform resultCardPrefab;
    public Transform resultContent;

    CarouselArray<Banner> banners;

    Queue<ResultCard> cards = new Queue<ResultCard>();
    List<BannerItem> bannerData = new List<BannerItem>();

    public Transform content;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CoinsText.text = DataManager.Instance.Coins.ToString() + " G";
        banners = CarouselArray<Banner>.FromArray(bannerObjects);

        pullAnimator.OnAnimationFinished += HandleQueue;
    }

    private void OnDestroy()
    {
        pullAnimator.OnAnimationFinished -= HandleQueue;
    }

    void SpawnResults()
    {
        Transform t;
        Sprite icon = null;
        int currentCopies = 0;
        Rarity rarity = Rarity.Common;
        string name = "";

        foreach (var item in bannerData)
        {
            if (item is CharacterBannerItem cbi)
            {
                icon = cbi.charData.baseSprite;
                currentCopies = SaveManager.GetCharacterEntry(cbi.charData.characterType).value;
                rarity = cbi.charData.rarity;
                name = cbi.charData.GetName();

                SaveManager.AddCharacter(cbi.charData.characterType, 1);
            }

            t = Instantiate(resultCardPrefab, resultContent);

            if (t.GetComponent<ResultCard>() is ResultCard r)
            {
                r.Initialize(rarity, icon, currentCopies, name);
                r.OnDisappear += Test;
                cards.Enqueue(r);
            }
        }
        content.gameObject.SetActive(false);
        resultContent.gameObject.SetActive(true);

        SaveManager.Save();
    }

    void Test(ResultCard r)
    {
        r.OnDisappear -= Test;

        cards.Dequeue();

        Destroy(r.gameObject);

        HandleQueue();
    }

    void HandleQueue()
    {
        if (cards.Count > 0)
        {
            if (!cards.Peek().gameObject.activeInHierarchy)
                cards.Peek().AppearAnim();
        }
        else
        {
            content.gameObject.SetActive(true);
            resultContent.gameObject.SetActive(false);
        }
            
    }


    public void TryPull()
    {
        bannerData.Clear();
        BannerItem item = banners.Current.Pull();
        bannerData.Add(item);
        pullAnimator.StartAnimation(item.rarity);

        SpawnResults();
    }

    public void TryPullFive()
    {
        bannerData.Clear();
        Rarity highest = Rarity.Common;
        BannerItem latest;
        for (int i = 0; i < 5; i++)
        {
            latest = banners.Current.Pull();    

            if (latest.rarity > highest)
                highest = latest.rarity;

            bannerData.Add(latest);
        }

        pullAnimator.StartAnimation(highest);

        SpawnResults();
    }


    public void OnPurchaseCharacter(int cost)
    {
        if (DataManager.Instance.SpendCoins(cost))
        {
            Debug.Log("Character bought!");
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    public void OnPurchasePerk(int cost)
    {
        if (DataManager.Instance.SpendCoins(cost))
        {
            Debug.Log("Perk bought!");
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    public void UpdateUI()
    {
        CoinsText.text = DataManager.Instance.Coins.ToString() + " G";
    }
}
