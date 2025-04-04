using UnityEngine;
using UnityEngine.SceneManagement;  // Sahne yönetimi için gerekli kütüphane

public class MainMenu : MonoBehaviour
{
    // Start butonuna basınca çağrılacak fonksiyon
    public void StartGame()
    {
        SceneManager.LoadScene("ControlsScreen");
    }

    // Options butonu (Henüz işlev eklenmedi, placeholder)
    public void OpenOptions()
    {
        Debug.Log("Options Menu Açılacak (Henüz eklenmedi)");  // Konsola mesaj gönder
    }

    // Quit butonuna basınca çağrılacak fonksiyon
    public void QuitGame()
    {
        Debug.Log("Oyun Kapandı!");  // Editörde deneme yaparken bu mesajı göreceksin.
        Application.Quit();  // Oyun kapanır (Bu sadece build alınca çalışır, Unity Editör'de değil)
    }
}