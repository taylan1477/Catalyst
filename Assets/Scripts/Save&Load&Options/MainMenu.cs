using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Save_Load_Options
{
    public class MainMenu : MonoBehaviour
    {
        public TMP_InputField nameInput;
        public GameObject newGamePanel;
        public GameObject loadGamePanel;
        public GameObject optionsPanel;

        public AudioMixer mixer;
        public Slider musicSlider;
        public Slider sfxSlider;
    
        public void DeleteSaveSlot(int slotIndex)
        {
            SaveSystem.DeleteSave(slotIndex);
            RefreshLoadSlotTexts();
        }

        public TextMeshProUGUI[] loadSlotTexts;

        // Her chapter için başlangıç spawn pozisyonları
        private readonly Dictionary<int, Vector3> _chapterSpawnPoints = new Dictionary<int, Vector3>()
        {
            { 0, new Vector3(-103f, 4.4f, 0f) },
            { 1, new Vector3(-36f, -4f, 0f) },
            { 2, new Vector3(10f, 1f, 0f) }
        };
    
        void Awake()
        {
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        
            float savedMusic = PlayerPrefs.GetFloat("Music", 1f);
            float savedSfx = PlayerPrefs.GetFloat("SFX", 1f);

            musicSlider.value = savedMusic;
            sfxSlider.value = savedSfx;

            SetMusicVolume(savedMusic);
            SetSfxVolume(savedSfx);
        }

        void Start()
        {
            RefreshLoadSlotTexts();
        }

        void RefreshLoadSlotTexts()
        {
            for (int i = 0; i < loadSlotTexts.Length; i++)
            {
                if (!SaveSystem.SaveExists(i))
                {
                    loadSlotTexts[i].text = $"Slot {i + 1} - [Empty]";
                }
                else
                {
                    SaveData data = SaveSystem.LoadGame(i);

                    int chapter = SceneNameToChapterIndex(data.sceneName) + 1;
                    string playerName = string.IsNullOrEmpty(data.slotName) ? $"Slot {i + 1}" : data.slotName;

                    loadSlotTexts[i].text = $"{playerName} - Chapter {chapter}";
                }
            }
        }

        int SceneNameToChapterIndex(string sceneName)
        {
            if (sceneName.StartsWith("Chapter"))
            {
                string numPart = sceneName.Substring(7);
                if (int.TryParse(numPart, out int num))
                {
                    return Mathf.Clamp(num - 1, 0, 3);
                }
            }
            return 0;
        }

        public void ShowNewGamePanel()
        {
            newGamePanel.SetActive(true);
            loadGamePanel.SetActive(false);
            RefreshLoadSlotTexts();
        }

        public void ShowLoadGamePanel()
        {
            loadGamePanel.SetActive(true);
            newGamePanel.SetActive(false);
            RefreshLoadSlotTexts();
        }

        public void HidePanels()
        {
            newGamePanel.SetActive(false);
            loadGamePanel.SetActive(false);
            optionsPanel.SetActive(false);
        }

        public void StartNewGame(int slotIndex, string playerName)
        {
            SaveData newSave = new SaveData();
            newSave.slotName = playerName;
            newSave.chapterIndex = 0;
            newSave.sceneName = "ControlsScreen"; // İlk sahne

            if (_chapterSpawnPoints.TryGetValue(0, out Vector3 spawnPos))
                newSave.playerPosition = spawnPos;
            else
                newSave.playerPosition = Vector2.zero;

            SaveSystem.SaveGame(newSave, slotIndex);
            GameState.activeSlot = slotIndex;

            GameState.loadPosition = Vector2.zero;

            SceneManager.LoadScene(newSave.sceneName);
        }

        public void StartNewGameSlot0()
        {
            StartNewGame(0, nameInput.text);
        }

        public void StartNewGameSlot1()
        {
            StartNewGame(1, nameInput.text);
        }

        public void StartNewGameSlot2()
        {
            StartNewGame(2, nameInput.text);
        }

        public void OpenOptions()
        {
            optionsPanel.SetActive(true);
            newGamePanel.SetActive(false);
            loadGamePanel.SetActive(false);
        }

    
        void SetMusicVolume(float value)
        {
            PlayerPrefs.SetFloat("Music", value);
            PlayerPrefs.Save();

            float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
            mixer.SetFloat("Music", dB);
        }

        void SetSfxVolume(float value)
        {
            PlayerPrefs.SetFloat("SFX", value);
            PlayerPrefs.Save();

            float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
            mixer.SetFloat("SFX", dB);
        }

        public void LoadExistingGame(int slotIndex)
        {
            if (!SaveSystem.SaveExists(slotIndex))
            {
                Debug.LogWarning("Seçilen slot boş.");
                return;
            }

            SaveData save = SaveSystem.LoadGame(slotIndex);
            GameState.activeSlot = slotIndex;
            GameState.loadPosition = save.playerPosition;

            SceneManager.LoadScene(save.sceneName);
        }

        public void LoadExistingGameSlot0()
        {
            LoadExistingGame(0);
        }

        public void LoadExistingGameSlot1()
        {
            LoadExistingGame(1);
        }

        public void LoadExistingGameSlot2()
        {
            LoadExistingGame(2);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
