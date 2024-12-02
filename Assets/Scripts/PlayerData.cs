using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class PlayerData
{
    public int level;
    public int health;
    public float[] position;

    public PlayerData(Player player)
    {
        level = player.level;
        health = player.health;

        position = new float[3];
        position[0] = player.transform.poition.x;
        position[1] = player.transform.poition.y;
        position[2] = player.transform.poition.z;
    }
}
