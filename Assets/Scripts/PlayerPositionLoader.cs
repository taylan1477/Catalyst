using UnityEngine;
using System.Collections;

public class PlayerPositionLoader : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player bulunamadı!");
            yield break;
        }

        if (GameState.loadPosition != Vector2.zero)
        {
            player.transform.position = GameState.loadPosition;
            Debug.Log($"Oyuncu pozisyonu yüklendi: {GameState.loadPosition}");
        }
    }
}