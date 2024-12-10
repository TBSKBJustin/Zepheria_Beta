using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeDisperse : MonoBehaviour
{
  public GameObject slimePrefab;
  public int slimeCount = 10;
    
    private Vector3 terrainSize;

    void Start()
    {
        // Get the size of the active terrain
        terrainSize = Terrain.activeTerrain.terrainData.size;
        SpawnSlimes();
    }

    void SpawnSlimes()
    {
        for (int i = 0; i < slimeCount; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(0, terrainSize.x),
                0,
                Random.Range(0, terrainSize.z)
            );

            // Adjust y position based on terrain height at the (x, z) position
            position.y = Terrain.activeTerrain.SampleHeight(position);

            Instantiate(slimePrefab, position, Quaternion.identity);
        }
    }
}
