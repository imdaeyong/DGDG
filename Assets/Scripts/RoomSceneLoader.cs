using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbySceneLoader : MonoBehaviour
{
    public string nextSceneName = "RoomScene";

    public void GoToRoomScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}