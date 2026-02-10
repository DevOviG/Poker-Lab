namespace PokerLabBlazor.Logic;

public static class Core
{
    public static readonly string[] RANKS = { "A", "K", "Q", "J", "T", "9", "8", "7", "6", "5", "4", "3", "2" };

    // Helper to generate hand name (e.g., "AKs" or "77")
    public static string GetHandName(int row, int col)
    {
        string r1 = RANKS[row];
        string r2 = RANKS[col];
        if (row == col) return r1 + r2;       // Pair
        if (row < col) return r1 + r2 + "s";  // Suited
        return r2 + r1 + "o";                 // Offsuit
    }
}