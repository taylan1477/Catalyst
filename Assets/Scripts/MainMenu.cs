using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public TMP_InputField nameInput;
    public GameObject newGamePanel;
    public GameObject loadGamePanel;

    public TextMeshProUGUI[] loadSlotTexts; // Inspector'dan atanacak

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
                return Mathf.Clamp(num - 1, 0, 3); // 1-based to 0-based
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
    }

    // Sahneye geçmeden önce oyuncunun seçimlerine göre kayıt yaratır ve yükler
    public void StartNewGame(int slotIndex, string playerName)
    {
        SaveData newSave = new SaveData();
        newSave.slotName = playerName;
        newSave.chapterIndex = 0;
        newSave.sceneName = "ControlsScreen"; // İlk sahne
        newSave.playerPosition = Vector2.zero;

        SaveSystem.SaveGame(newSave, slotIndex);
        GameState.activeSlot = slotIndex;

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
    
    // Options butonu (Henüz işlev eklenmedi)
    public void OpenOptions()
    {
        Debug.Log("Options Menu Açılacak (Henüz eklenmedi)"); 
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