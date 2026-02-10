namespace PokerLabBlazor.Logic;

// 1. The Data Model (Defines what "Advice" is)
public class FlopAdvice
{
    public string Headline { get; set; } = "";
    public string Body { get; set; } = "";
    public string Action { get; set; } = "";
    public string Freq { get; set; } = "";
    public string Why { get; set; } = "";
    public string Badge { get; set; } = "";
    public string BadgeClass { get; set; } = ""; 
}

// 2. The Logic
public static class Postflop
{
    private static readonly Dictionary<string, FlopAdvice> Strategies = new()
    {
        // SRP - Aggressor
        { "srp_aggressor_dry", new FlopAdvice { Headline = "Advantage: HIGH", Body = "Hits range perfectly.", Action = "Bet Small", Freq = "Very High", Why = "Deny equity cheaply.", Badge = "FREQ BET", BadgeClass = "bg-emerald-500" } },
        { "srp_aggressor_wet", new FlopAdvice { Headline = "Advantage: LOW", Body = "Hits caller range.", Action = "Check", Freq = "High", Why = "Pot control.", Badge = "DEFENSIVE", BadgeClass = "bg-amber-500" } },
        { "srp_aggressor_mono", new FlopAdvice { Headline = "Advantage: NEUTRAL", Body = "Everyone scared.", Action = "Check/Small", Freq = "Check High", Why = "Protect range.", Badge = "CAUTION", BadgeClass = "bg-amber-500" } },
        { "srp_aggressor_paired", new FlopAdvice { Headline = "Advantage: HIGH", Body = "J-J-5.", Action = "Bet Small", Freq = "High", Why = "Easy fold equity.", Badge = "FREQ BET", BadgeClass = "bg-emerald-500" } },
        
        // ... (You can paste the rest of your dictionary here) ...
    };

    public static FlopAdvice GetAdvice(string pot, string role, string tex)
    {
        string key = $"{pot}_{role}_{tex}";
        if (Strategies.ContainsKey(key)) return Strategies[key];
        
        // Return a safe default if key not found
        return new FlopAdvice { Headline = "Unknown Scenario", Body = "Try selecting different options." };
    }
}