namespace SceneControllers
{
    using UnityEngine;

    public class IntroCutsceneStarter : MonoBehaviour
    {
        public CutsceneManager cutsceneManager;
        public Sprite[] introImages;
        public string[] captions;

        void Start()
        {
            if (Save_Load_Options.GameState.playIntro)
            {
                Save_Load_Options.GameState.playIntro = false;
                cutsceneManager.StartCutscene(introImages, captions, OnIntroFinished);
            }
        }

        void OnIntroFinished()
        {
            Debug.Log("Intro tamam, Chapter 1 başlıyor.");
            // Şimdilik bişe yapmicam umarım kimse klavyeye abanmaz.
        }
    }

}