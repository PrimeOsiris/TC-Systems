using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    //Editor Inputs
    public bool IsWave, IsRepeat, IsDelayed;
    public float delay;
    public int repeatLimit;
    public GameObject Entity;
    public GameObject[] Entities;
    public Transform SpawnPosition;
    float TimeCounter = 0;
    int counter = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (IsWave == false && IsRepeat == false && IsDelayed == false)
        {
            Spawn(Entity, SpawnPosition.position);
        }

        if (IsWave == false && IsRepeat == false && IsDelayed == true)
        {
            TimerSpawn(Entity, delay, SpawnPosition.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsWave == false && IsRepeat == true && IsDelayed == false)
        {
            if (counter < repeatLimit)
            {
                Spawn(Entity, SpawnPosition.position);
                counter++;
            }
        }

        if (IsWave == false && IsRepeat == true && IsDelayed == true)
        {
            if (counter < repeatLimit)
            {
                TimerSpawn(Entity, delay, SpawnPosition.position);
                counter++;
            }
        }

        if (IsWave == true && IsRepeat == true && IsDelayed == true)
        {
            if (counter < repeatLimit)
            {
                TimerSpawn(Entities[counter], delay, SpawnPosition.position);
                counter++;
            }
        }
    }

    void TimerSpawn(GameObject entity, float delay, Vector3 location)
    {
        TimeCounter += Time.deltaTime;
        if (TimeCounter >= delay)
        {
            Instantiate(entity, location, Quaternion.identity);
            TimeCounter = 0;
        }
    }

    void Spawn(GameObject entity, Vector3 location)
    {
        Instantiate(entity, location, Quaternion.identity);
    }
}
