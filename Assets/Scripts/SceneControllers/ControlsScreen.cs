using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SceneControllers
{
    public class ControlsScreen : MonoBehaviour
    {
        public Image keyA, keySpace, keyD, keyShift, keyE; // Tuş görselleri
        public Sprite keyANormal, keyAPressed;
        public Sprite keySpaceNormal, keySpacePressed;
        public Sprite keyDNormal, keyDPressed;
        public Sprite keyShiftNormal, keyShiftPressed;
        public Sprite keyENormal, keyEPressed;
        private Animator _characterAnimator; // Karakterin Animator bileşeni
        private SpriteRenderer _spriteRenderer; // Karakterin SpriteRenderer bileşeni

        private void Awake()
        {
            _characterAnimator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            StartCoroutine(TutorialLoop());
        }

        IEnumerator TutorialLoop()
        {
            while (true)
            {
                yield return StartCoroutine(PressKey(keyA, keyAPressed, keyANormal, "Run", true, true, true));  // A → sola koş (Run = true)
                yield return StartCoroutine(PressKey(keySpace, keySpacePressed, keySpaceNormal, "Jump")); // SPACE → zıpla
                yield return StartCoroutine(PressKey(keyD, keyDPressed, keyDNormal, "Run", true, true, false)); // D → sağa koş (Run = true)
                yield return StartCoroutine(PressKey(keyShift, keyShiftPressed, keyShiftNormal, "Pull", true, true)); // Shift → çekme (Pull = true)
                yield return StartCoroutine(PressKey(keyE, keyEPressed, keyENormal, "Attack")); // E → saldırı

            }
        }

        IEnumerator PressKey(Image key, Sprite pressedSprite, Sprite normalSprite, string param, bool isBool = false, bool boolValue = false, bool flipX = false)
        {
            key.sprite = pressedSprite; // Tuş basıldı görseli
            _spriteRenderer.flipX = flipX; // Karakterin yönünü ayarla

            if (isBool)
            {
                _characterAnimator.SetBool(param, boolValue); // Bool değerini ata
            }
            else
            {
                _characterAnimator.SetTrigger(param); // Trigger tetikle
            }

            yield return new WaitForSeconds(2f);
    
            if (isBool)
            {
                _characterAnimator.SetBool(param, !boolValue); // Bool'u eski haline getir
            }
    
            key.sprite = normalSprite; // Tuşu eski haline getir
            yield return new WaitForSeconds(1f);
        }


        public void GoToChapter1()
        {
            SceneManager.LoadScene("Chapter 1");
        }
    
        public void GoToDevelopScene()
        {
            SceneManager.LoadScene("DevelopScene");
        }
    
        public void GoToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
