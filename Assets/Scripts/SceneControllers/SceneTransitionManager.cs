using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneControllers
{
    public class SceneTransitionManager : MonoBehaviour
    {
        [Tooltip("Sahne geçişinden önce kaç saniye beklensin?")]
        public float transitionDelay = 1f;

        [Tooltip("Bölüm sonu tetikleyicisi buradan ayarlanabilir.")]
        public bool autoTriggerNextScene;

        private bool _hasTriggered;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!autoTriggerNextScene || _hasTriggered) return;

            if (other.CompareTag("Player"))
            {
                _hasTriggered = true;
                LoadNextSceneWithDelay();
            }
        }

        public void LoadNextScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;

            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(nextSceneIndex);
            else
                Debug.Log("Son sahnedesin, ileri gidecek sahne kalmadı.");
        }

        public void LoadNextSceneWithDelay()
        {
            Invoke(nameof(LoadNextScene), transitionDelay);
        }
    }
}