using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System.Collections;

public static class SaveSystem
{
    public static void SavePlayer(Player player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        // Serialize the player data.
        PlayerData data = new PlayerData(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void LoadPlayer(Player player)
    {
        string path = Application.persistentDataPath + "/player.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            // Deserialize the player data.
            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            // Restore player stats.
            player.level = data.level;
            player.health = data.health;

            // Load the saved scene if it's different.
            if (SceneManager.GetActiveScene().name != data.currentScene)
            {
                player.StartCoroutine(LoadSceneAndSetPosition(player, data));
            }
            else
            {
                // Restore position in the current scene.
                player.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
            }
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
        }
    }

    private static IEnumerator LoadSceneAndSetPosition(Player player, PlayerData data)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(data.currentScene);

        // Wait until the scene is fully loaded.
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // After loading, restore the player's position.
        player.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
    }
}
