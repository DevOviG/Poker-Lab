using Microsoft.AspNetCore.Components;

namespace PokerLabBlazor.Pages;

public partial class Trainer : ComponentBase
{
    // 1. STATE
    private HashSet<string> UserSelection = new();
    private bool IsChecked = false;
    private double Score = 0;
    private string ScoreColorClass = "text-slate-800"; // Default text color
    
    private string CurrentPosition = "UTG"; 
    private List<string> Positions = new() { "UTG", "HJ", "CO", "BTN", "SB" };

    // 2. SOLUTIONS (Same as before)
    private Dictionary<string, HashSet<string>> Solutions = new()
    {
        ["UTG"] = new() { "AA", "KK", "QQ", "JJ", "TT", "99", "88", "77", "AKs", "AQs", "AJs", "ATs", "A9s", "A8s", "AKo", "AQo", "KQs", "KJs", "KTs", "QJs", "QTs", "JTs" },
        ["HJ"]  = new() { "AA", "KK", "QQ", "JJ", "TT", "99", "88", "77", "66", "AKs", "AQs", "AJs", "ATs", "A9s", "A8s", "A7s", "A6s", "A5s", "AKo", "AQo", "AJo", "KQs", "KJs", "KTs", "K9s", "QJs", "QTs", "JTs", "T9s" },
        ["CO"]  = new() { "AA", "KK", "QQ", "JJ", "TT", "99", "88", "77", "66", "55", "44", "AKs", "AQs", "AJs", "ATs", "A9s", "A8s", "A7s", "A6s", "A5s", "A4s", "A3s", "A2s", "AKo", "AQo", "AJo", "ATo", "KQs", "KJs", "KTs", "K9s", "K8s", "KQo", "KJo", "QJs", "QTs", "Q9s", "JTs", "J9s", "T9s", "98s", "87s" },
        ["BTN"] = new() { "AA", "KK", "QQ", "JJ", "TT", "99", "88", "77", "66", "55", "44", "33", "22", "AKs", "AQs", "AJs", "ATs", "A9s", "A8s", "A7s", "A6s", "A5s", "A4s", "A3s", "A2s", "AKo", "AQo", "AJo", "ATo", "A9o", "A8o", "KQs", "KJs", "KTs", "K9s", "K8s", "K7s", "K6s", "K5s", "K4s", "K3s", "K2s", "KQo", "KJo", "KTo", "QJs", "QTs", "Q9s", "Q8s", "Q7s", "Q6s", "Q5s", "QJo", "QTo", "JTs", "J9s", "J8s", "J7s", "J6s", "JTo", "T9s", "T8s", "T7s", "T6s", "98s", "97s", "87s", "86s", "76s", "65s", "54s" },
        ["SB"]  = new() { "AA", "KK", "QQ", "JJ", "TT", "99", "88", "77", "66", "55", "44", "33", "22", "AKs", "AQs", "AJs", "ATs", "A9s", "A8s", "A7s", "A6s", "AKo", "AQo", "AJo", "ATo", "A9o", "KQs", "KJs", "KTs", "K9s", "KQo", "KJo", "KTo", "QJs", "QTs", "Q9s", "QJo", "QTo", "JTs", "J9s", "JTo", "T9s", "T8s", "98s", "87s", "76s" }
    };

    // 3. LOGIC
    private string[] Ranks = { "A", "K", "Q", "J", "T", "9", "8", "7", "6", "5", "4", "3", "2" };
    
    private string GetHand(int row, int col)
    {
        if (row < col) return $"{Ranks[row]}{Ranks[col]}s";
        if (row > col) return $"{Ranks[col]}{Ranks[row]}o";
        return $"{Ranks[row]}{Ranks[col]}";
    }

    private void ChangePosition(string newPosition)
    {
        CurrentPosition = newPosition;
        Reset();
    }

    private void ToggleHand(string hand)
    {
        if (IsChecked) return;

        if (UserSelection.Contains(hand))
            UserSelection.Remove(hand);
        else
            UserSelection.Add(hand);
    }

    private void CheckAnswer()
    {
        IsChecked = true;
        var currentSolution = Solutions[CurrentPosition];

        if (currentSolution.Count > 0)
        {
            int correctPicks = UserSelection.Count(h => currentSolution.Contains(h));
            int wrongPicks = UserSelection.Count(h => !currentSolution.Contains(h));
            
            // Score calc
            double rawScore = (double)(correctPicks - (wrongPicks * 0.5)) / currentSolution.Count * 100;
            Score = Math.Max(0, Math.Round(rawScore));
        }
        else 
        {
            Score = 0;
        }

        ScoreColorClass = Score == 100 ? "text-emerald-500" : (Score >= 80 ? "text-yellow-500" : "text-red-500");
    }

    // --- STYLE HELPERS (TAILWIND) ---

    // 1. Grid Cell Coloring
    private string GetResultClass(string hand)
    {
        // WHILE PLAYING:
        if (!IsChecked) 
        {
            // Selected = Blue-500 (Matches your theme but distinct from the Emerald answer key)
            // Unselected = White bg, light grey text
            return UserSelection.Contains(hand) 
                ? "bg-blue-500 text-white border-blue-600" 
                : "bg-white text-slate-300 hover:bg-slate-50"; 
        }

        // AFTER CHECKING:
        bool isUser = UserSelection.Contains(hand);
        bool isCorrect = Solutions[CurrentPosition].Contains(hand);

        if (isUser && isCorrect) return "bg-emerald-500 text-white"; // Correct (Green)
        if (isUser && !isCorrect) return "bg-red-500 text-white";     // Wrong (Red)
        if (!isUser && isCorrect) return "bg-yellow-400 text-white";  // Missed (Yellow)
        
        return "bg-slate-100 text-slate-200"; // Irrelevant
    }

    // 2. Position Button Styling
    private string GetButtonClass(string pos)
    {
        // Matches your Learn Page: Emerald for active, Transparent/Slate for inactive
        if (CurrentPosition == pos) return "bg-emerald-600 text-white shadow-sm";
        return "bg-transparent text-slate-600 hover:text-slate-900";
    }

    private void Reset()
    {
        UserSelection.Clear();
        IsChecked = false;
        Score = 0;
    }
}