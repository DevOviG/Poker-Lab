namespace PokerLabBlazor.Logic;

public static class Equity
{
    public static double Calculate(string hero, string villain)
    {
        // 1. Parse both hands
        var h1 = ParseHand(hero);
        var h2 = ParseHand(villain);

        double equity = 50.0;

        // --- SCENARIO 1: PAIR VS PAIR ---
        if (h1.IsPair && h2.IsPair)
        {
            if (h1.High > h2.High) return 81.0; // Higher pair (81%)
            if (h1.High < h2.High) return 19.0; // Lower pair (19%)
            return 50.0; // Same pair
        }

        // --- SCENARIO 2: PAIR VS NON-PAIR ---
        if (h1.IsPair || h2.IsPair)
        {
            var pair = h1.IsPair ? h1 : h2;
            var nonPair = h1.IsPair ? h2 : h1;
            
            // Adjust for suitedness of the non-pair (+3% equity)
            double baseWin = 0;

            if (nonPair.High > pair.High && nonPair.Low > pair.High) baseWin = 46.0;      // Pair vs 2 Overcards (Coinflip: 54 vs 46)
            else if (nonPair.High < pair.High && nonPair.Low < pair.High) baseWin = 13.0; // Pair vs 2 Undercards (Crushed: 87 vs 13)
            else baseWin = 29.0;                                                          // Pair vs Over/Under (71 vs 29)

            if (nonPair.IsSuited) baseWin += 3.0;
            
            return h1.IsPair ? (100 - baseWin) : baseWin;
        }

        // --- SCENARIO 3: NON-PAIR VS NON-PAIR ---
        
        // Domination Check (Sharing a card)
        // Example: AK vs AQ (Dominating) or KQ vs AQ (Dominated)
        if (h1.High == h2.High || h1.Low == h2.Low || h1.High == h2.Low || h1.Low == h2.High)
        {
            // If they share a card (e.g. both have an Ace), who has the better kicker?
            int kicker1 = GetKicker(h1, h2);
            int kicker2 = GetKicker(h2, h1);

            double domWin = 72.0; // Standard domination equity (e.g. AK vs AQ)
            if (kicker1 > kicker2) equity = domWin;
            else if (kicker2 > kicker1) equity = 100 - domWin;
            else equity = 50.0; // Chopped pot (e.g. AK vs AK)
            
            // Adjust for suitedness
            if (h1.IsSuited) equity += 3.0;
            if (h2.IsSuited) equity -= 3.0;
            return equity;
        }

        // No shared cards (e.g. AK vs 76)
        // Simply count how many cards act as "Overcards"
        int h1Overcards = CountOvercards(h1, h2);
        int h2Overcards = CountOvercards(h2, h1);

        if (h1Overcards == 2) equity = 63.0;      // 2 Overs vs 2 Unders (e.g. AK vs 76)
        else if (h2Overcards == 2) equity = 37.0; // Reverse
        else equity = 58.0;                       // 1 Over vs 1 Under (e.g. A2 vs K5) -> A2 wins ~58%

        // Suited bonus
        if (h1.IsSuited) equity += 2.5;
        if (h2.IsSuited) equity -= 2.5;

        // Straight connectedness bonus (simple)
        if (h1.High - h1.Low == 1) equity += 1.5; // Connectors
        if (h2.High - h2.Low == 1) equity -= 1.5;

        return Math.Clamp(equity, 5.0, 95.0);
    }

    // --- HELPERS ---

    private struct HandInfo { public int High; public int Low; public bool IsPair; public bool IsSuited; }

    private static HandInfo ParseHand(string s)
    {
        // Parses "AKs", "TT", "72o"
        string ranks = "23456789TJQKA";
        
        char r1Char = s[0];
        char r2Char = s.Length > 1 ? s[1] : s[0]; // Handle partial input safely
        
        int r1 = ranks.IndexOf(r1Char);
        int r2 = ranks.IndexOf(r2Char);

        // Sort so High is always first
        int high = Math.Max(r1, r2);
        int low = Math.Min(r1, r2);

        bool suited = s.Contains("s", StringComparison.InvariantCultureIgnoreCase);
        bool pair = (high == low);

        return new HandInfo { High = high, Low = low, IsPair = pair, IsSuited = suited };
    }

    private static int GetKicker(HandInfo me, HandInfo them)
    {
        // If my High matches one of theirs, my kicker is my Low. And vice versa.
        if (me.High == them.High || me.High == them.Low) return me.Low;
        return me.High;
    }

    private static int CountOvercards(HandInfo me, HandInfo them)
    {
        int count = 0;
        if (me.High > them.High && me.High > them.Low) count++;
        if (me.Low > them.High && me.Low > them.Low) count++;
        return count;
    }
}