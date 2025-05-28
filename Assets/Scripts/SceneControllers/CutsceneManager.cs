namespace SceneControllers
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class CutsceneManager : MonoBehaviour
    {
        public Image cutsceneImage;
        public TextMeshProUGUI captionText;
        public Button nextButton;

        private Sprite[] _scenes;
        private string[] _captions;
        private int _currentIndex;
        private System.Action _onComplete;

        void Start()
        {
            nextButton.onClick.AddListener(Next);
        }

        public void StartCutscene(Sprite[] newScenes, string[] newCaptions = null, System.Action onDone = null)
        {
            _scenes = newScenes;
            _captions = newCaptions;
            _currentIndex = 0;
            _onComplete = onDone;
            gameObject.SetActive(true);
            ShowCurrent();
        }

        void ShowCurrent()
        {
            cutsceneImage.sprite = _scenes[_currentIndex];
            captionText.text = (_captions != null && _currentIndex < _captions.Length) ? _captions[_currentIndex] : "";
            Debug.Log(_scenes[_currentIndex].name);
        }
        

        public void Next()
        {
            _currentIndex++;
            if (_currentIndex >= _scenes.Length)
            {
                gameObject.SetActive(false);
                _onComplete?.Invoke();
            }
            else
            {
                ShowCurrent();
            }
        }
    }
}