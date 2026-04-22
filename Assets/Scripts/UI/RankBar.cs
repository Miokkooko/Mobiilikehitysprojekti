using TMPro;
using UnityEngine;

public class RankBar : MonoBehaviour
{
    public BarUIElement xpBar;

    public TMP_Text rankLevelText;
    public TMP_Text rankText;

    int currentCopies;
    public void Initialize(int min, int current, int max, int _currentCopies)
    {
        xpBar.SetMinValue(min);
        xpBar.SetMaxValue(max);
        currentCopies = _currentCopies;

        if (currentCopies >= RankManager.MaxCopies)
        {
            xpBar.SetCurrentValueInstant(RankManager.CopiesPerRank);
            rankLevelText.SetText("Max");
        }
        else if (RankManager.GetCopiesUntilNextRank(currentCopies) == RankManager.CopiesPerRank)
        {
            xpBar.SetCurrentValueInstant(0);
            rankLevelText.SetText("0 / " + RankManager.CopiesPerRank);
        }
        else 
        {
            xpBar.SetCurrentValueInstant(RankManager.GetRankProgress(currentCopies));
            rankLevelText.SetText(RankManager.GetRankProgress(currentCopies) + " / " + RankManager.CopiesPerRank);
        }

        rankText.SetText(RankManager.GetRank(currentCopies).ToString());
    }

    public void SetCurrentCopies(int amount)
    {
        currentCopies = amount;

        xpBar.SetCurrentValue(RankManager.GetRankProgress(currentCopies));
        rankLevelText.SetText(RankManager.GetRankProgress(currentCopies) + " / " + RankManager.CopiesPerRank);

    }

    public void RankUp()
    {
        if (currentCopies == RankManager.MaxCopies)
            rankLevelText.SetText("Max");
        else
        {
            rankLevelText.SetText("0 / " + RankManager.CopiesPerRank);
            xpBar.SetCurrentValueInstant(0);
        }

        rankText.SetText(RankManager.GetRank(currentCopies).ToString());
    }
}
