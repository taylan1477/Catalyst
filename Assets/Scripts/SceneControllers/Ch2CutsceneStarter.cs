namespace SceneControllers
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    public class Ch2CutsceneStarter : MonoBehaviour
    {
        public CutsceneManager cutsceneManager;
        public Sprite[] cutsceneSprites;
        public string[] captions;

        private bool _hasPlayed;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (_hasPlayed) return;
            if (other.CompareTag("Player"))
            {
                _hasPlayed = true;
                StartCutscene();
            }
        }

        void StartCutscene()
        {
            cutsceneManager.StartCutscene(cutsceneSprites, captions, OnCutsceneComplete);
        }

        void OnCutsceneComplete()
        {
            SceneManager.LoadScene("Chapter 2");
        }
    }
}