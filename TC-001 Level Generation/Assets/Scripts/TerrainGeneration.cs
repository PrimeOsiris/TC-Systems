using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    //objects that can be spawned
    public GameObject[] options;

    //adjustable variables within the editor
    public Vector3 spawnLocation;
    public float chunkSize; //Size of each floor
    public int depth; //How deep each terrain piece is
    public int adjacent; //How wide each terrain piece is
    public int stories; //How tall each terrain piece is

    //variables that will be incremented/adjusted within the script
    float xOffset;
    float yOffset;
    float zOffset;

    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < adjacent; x++){
            for (int z = 0; z < depth; z++){
                for (int y = 0; y < stories; y++){
                    GameObject piece = options[Random.Range(0,options.Length)]; //Randomly selects one of the pieces

                    xOffset = x * chunkSize;
                    yOffset = y * chunkSize / 2;
                    zOffset = z * chunkSize;

                    Vector3 position = new Vector3(spawnLocation.x + xOffset, spawnLocation.y + yOffset, spawnLocation.z + zOffset ); //Determines the position of each piece

                    Instantiate(piece, position, Quaternion.identity); //Spawns each piece
                }
            }
        }
    }
}
