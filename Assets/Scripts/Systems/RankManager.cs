public class RankManager
{
    public static int MaxCopies = 10;
    public static int CopiesPerRank = 5;

    public static int GetMaxRank()
    {
        return 1 + MaxCopies / CopiesPerRank;
    }

    public static int GetRank(int copies)
    {
        int rank = 1;

        while(copies >= CopiesPerRank)
        {
            copies -= CopiesPerRank;
            rank++;
        }

        return rank;
    }

    public static int GetCopiesUntilNextRank(int copies)
    {
        return CopiesPerRank - copies % CopiesPerRank;
    }

    public static int GetRankProgress(int copies)
    {
        if(copies != 0 && copies % CopiesPerRank == 0)
            return CopiesPerRank;
        
        return copies % CopiesPerRank;
    }
}
