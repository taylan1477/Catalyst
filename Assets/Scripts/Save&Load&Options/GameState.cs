using UnityEngine;

namespace Save_Load_Options
{
    public static class GameState
    {
        public static int activeSlot = -1;
        public static Vector2 loadPosition = Vector2.zero;
        public static bool playIntro = false; // introyu da tutuyom artık
    }
}