namespace PokerLabBlazor.Logic;

// 1. Define what a Defense Strategy looks like
public class DefendStrategy
{
    public HashSet<string> Value3Bet { get; set; } = new();
    public HashSet<string> Bluff3Bet { get; set; } = new();
    public HashSet<string> Call { get; set; } = new();
}

public static class Preflop
{
    // --- OPENING RANGES (Existing) ---
    private static readonly Dictionary<string, HashSet<string>> OpenRanges = new()
    {
        { "UTG", new HashSet<string> { "AA","KK","QQ","JJ","TT","99","88","AKs","AQs","AJs","ATs","KQs","KJs","QJs","JTs","T9s","98s","AKo","AQo" } },
        { "HJ",  new HashSet<string> { "AA","KK","QQ","JJ","TT","99","88","77","66","AKs","AQs","AJs","ATs","A9s","A8s","A5s","KQs","KJs","KTs","QJs","QTs","JTs","T9s","98s","87s","AKo","AQo","AJo","KQo" } },
        { "CO",  new HashSet<string> { "AA","KK","QQ","JJ","TT","99","88","77","66","55","44","33","22","AKs","AQs","AJs","ATs","A9s","A8s","A7s","A6s","A5s","A4s","A3s","A2s","KQs","KJs","KTs","K9s","QJs","QTs","Q9s","JTs","J9s","T9s","98s","87s","76s","65s","AKo","AQo","AJo","ATo","KQo","KJo","QJo" } },
        { "BTN", new HashSet<string> { "AA","KK","QQ","JJ","TT","99","88","77","66","55","44","33","22","AKs","AQs","AJs","ATs","A9s","A8s","A7s","A6s","A5s","A4s","A3s","A2s","KQs","KJs","KTs","K9s","K8s","K7s","QJs","QTs","Q9s","Q8s","JTs","J9s","J8s","T9s","T8s","98s","87s","76s","65s","54s","43s","AKo","AQo","AJo","ATo","A9o","A8o","A7o","A5o","KQo","KJo","KTo","K9o","QJo","QTo","Q9o","JTo","J9o","T9o","98o" } },
        { "SB",  new HashSet<string> { "AA","KK","QQ","JJ","TT","99","88","77","66","AKs","AQs","AJs","ATs","A9s","A5s","KQs","KJs","KTs","QJs","QTs","JTs","T9s","98s","AKo","AQo","AJo","KQo" } }
    };

    // --- DEFEND RANGES (New) ---
    // --- DEFEND RANGES (Expanded) ---
    private static readonly Dictionary<string, DefendStrategy> DefendRanges = new()
    {
        // --- VS UTG OPEN ---
        { "HJ_VS_UTG", new DefendStrategy {
            Value3Bet = new() { "AA","KK","AKs" },
            Bluff3Bet = new() { "A5s","A4s" },
            Call = new() { "QQ","JJ","TT","99","AQs","AJs","KQs" } 
        }},
        { "CO_VS_UTG", new DefendStrategy {
            Value3Bet = new() { "AA","KK","AKs" },
            Bluff3Bet = new() { "A5s","A4s","QJs","JTs" },
            Call = new() { "QQ","JJ","TT","99","88","AQs","AJs","KQs","KJs" }
        }},
        { "BTN_VS_UTG", new DefendStrategy {
            Value3Bet = new() { "AA","KK","AKs","AKo" },
            Bluff3Bet = new() { "A5s","A4s","T9s","98s" },
            Call = new() { "QQ","JJ","TT","99","88","77","AQs","AJs","ATs","KQs","KJs","QJs","JTs","AQo" }
        }},
        { "BB_VS_UTG", new DefendStrategy {
            Value3Bet = new() { "AA","KK","QQ","AKs","AKo" },
            Bluff3Bet = new() { "A5s","A4s","KJs","ATs" },
            Call = new() { "JJ","TT","99","88","77","66","55","44","33","22","AQs","AJs","ATs","KQs","KJs","KTs","QJs","QTs","JTs","T9s","98s","87s","AQo" }
        }},

        // --- VS CO OPEN ---
        { "BTN_VS_CO", new DefendStrategy {
            Value3Bet = new() { "AA","KK","QQ","JJ","AKs","AQs","AKo" },
            Bluff3Bet = new() { "A5s","A4s","K9s","Q9s","J9s","T8s","87s" },
            Call = new() { "TT","99","88","77","66","55","AJs","ATs","KQs","KJs","KTs","QJs","QTs","JTs","T9s","98s","AQo","AJo","KQo" }
        }},
        { "BB_VS_CO", new DefendStrategy {
            Value3Bet = new() { "AA","KK","QQ","JJ","TT","AKs","AQs","AJs","AKo","AQo" },
            Bluff3Bet = new() { "K9s","Q9s","J9s","T8s","87s","76s","A5o","A4o","KJo" },
            Call = new() { "99","88","77","66","55","44","33","22","ATs","A9s","A8s","A7s","A6s","A5s","A4s","A3s","A2s","KQs","KJs","KTs","K8s","QJs","QTs","Q9s","JTs","J9s","T9s","T8s","98s","97s","87s","76s","65s","54s","AJo","ATo","A9o","KQo","KJo","KTo","QJo","QTo","JTo","J9o","T9o","98o" }
        }},

        // --- VS BTN OPEN ---
        { "SB_VS_BTN", new DefendStrategy {
             Value3Bet = new() { "AA","KK","QQ","JJ","TT","AKs","AQs","AJs","ATs","AKo","AQo" },
             Bluff3Bet = new() { "A9s","A8s","A5s","A4s","K9s","Q9s","J9s","T9s","98s","87s","KJo","QJo" },
             Call = new() { /* SB mostly calls 0% vs BTN in modern theory */ }
        }},
        { "BB_VS_BTN", new DefendStrategy {
            Value3Bet = new() { "AA","KK","QQ","JJ","TT","AKs","AQs","AJs","AKo","AQo" },
            Bluff3Bet = new() { "K9s","Q9s","J9s","T8s","97s","86s","75s","64s","54s","A5o","A4o","KJo","KTo" },
            Call = new() { "99","88","77","66","55","44","33","22","ATs","A9s","A8s","A7s","A6s","A5s","A4s","A3s","A2s","KQs","KJs","KTs","K8s","K7s","K6s","QJs","QTs","Q9s","Q8s","JTs","J9s","J8s","T9s","T8s","T7s","98s","97s","87s","86s","76s","65s","54s","AJo","ATo","A9o","KQo","KJo","KTo","QJo","QTo","JTo","J9o","T9o","98o" }
        }},

        // --- BLIND vs BLIND ---
        { "BB_VS_SB", new DefendStrategy {
            Value3Bet = new() { "AA","KK","QQ","JJ","TT","99","88","AKs","AQs","AJs","ATs","AKo","AQo","AJo","KQo" },
            Bluff3Bet = new() { "K5s","Q6s","J7s","T7s","96s","85s","KTo","QTo","JTo" },
            Call = new() { "77","66","55","44","33","22","A9s","A8s","A7s","A6s","A5s","A4s","A3s","A2s","KQs","KJs","KTs","K9s","K8s","K7s","K6s","QJs","QTs","Q9s","Q8s","JTs","J9s","J8s","T9s","T8s","98s","87s","76s","65s","54s","ATo","A9o","A8o","KJo","KTo","QJo","JTo" }
        }}
    };

    // Helper: Is hand in Open range?
    public static bool IsHandInRange(string hand, string position)
    {
        return OpenRanges.ContainsKey(position) && OpenRanges[position].Contains(hand);
    }

    // Helper: Get Defend Action (returns "val", "bluff", "call", or "fold")
    public static string GetDefendAction(string hand, string scenario)
    {
        if (!DefendRanges.ContainsKey(scenario)) return "fold";
        
        var range = DefendRanges[scenario];
        if (range.Value3Bet.Contains(hand)) return "val";
        if (range.Bluff3Bet.Contains(hand)) return "bluff";
        if (range.Call.Contains(hand)) return "call";
        
        return "fold";
    }
}