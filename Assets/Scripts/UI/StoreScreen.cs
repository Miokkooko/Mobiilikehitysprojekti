using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoreScreen : MonoBehaviour
{
    public TMP_Text CoinsText;
    public TMP_Text onePullText;
    public TMP_Text fivePullText;

    public PullAnimator pullAnimator;

    public Banner[] bannerObjects;

    public Transform resultCardPrefab;
    public Transform resultContent;

    CarouselArray<Banner> banners;

    Queue<ResultCard> cards = new Queue<ResultCard>();
    List<BannerItem> bannerData = new List<BannerItem>();

    public int pullCost = 100;

    public Transform content;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CoinsText.text = SaveManager.GetCoins().ToString() + " G";
        banners = CarouselArray<Banner>.FromArray(bannerObjects);
        onePullText.SetText(pullCost.ToString());
        fivePullText.SetText((pullCost * 5).ToString());
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

            if(item is PerkBannerItem pbi)
            {
                icon = pbi.upgradeData.GetIcon();
                currentCopies = SaveManager.GetPerkEntry(pbi.upgradeData.type).value;
                rarity = pbi.upgradeData.rarity;
                name = pbi.upgradeData.GetName();

                SaveManager.AddPerk(pbi.upgradeData.type, 1);
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
        if (!SaveManager.TrySpendCoins(100))
            return;

        bannerData.Clear();
        BannerItem item = banners.Current.Pull();
        bannerData.Add(item);
        pullAnimator.StartAnimation(item.rarity);

        SpawnResults();
        UpdateUI();
    }

    public void TryPullFive()
    {
        if (!SaveManager.TrySpendCoins(pullCost*5))
            return;

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
        UpdateUI();
    }

    public void MoveRosterRight()
    {
        banners.Current.gameObject.SetActive(false);
        banners.Next();
        banners.Current.gameObject.SetActive(true);
    }
    public void MoveRosterLeft()
    {
        banners.Current.gameObject.SetActive(false);
        banners.Previous();
        banners.Current.gameObject.SetActive(true);
    }

    public void UpdateUI()
    {
        CoinsText.text = SaveManager.GetCoins().ToString() + " G";
    }
}
