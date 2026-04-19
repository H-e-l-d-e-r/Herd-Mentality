using System;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem;
using UnityEngine;

public class EntitiesManager : MonoBehaviour
{
    public GameObject EntityTemplate;
    
    public EntitiesTablesPerDay[] Tables;
    public DialogueTable[] Barks;

    public Transform[] SpawnPoints;

    private List<EntitesBehaviour> m_entities;

    private void Start()
    {
        m_entities = new List<EntitesBehaviour>();
        SpawnEntities(GameManager.Instance.CurrentDay);
    }

    void OnDisable()
    {
        ClearEntities();  
    }

    void SpawnEntities(int day)
    {
        if(day >= Tables.Length)
        {
            Debug.Log($"no tables registered for day {day}!");
            return;
        }

        ClearEntities();

        // try to avoid spawning redundant entities
        int count = (int)Mathf.Min(Tables[day].NumberOfEntities, SpawnPoints.Length);
        for (int i = 0; i < count; i++)
        {
            Transform target = SpawnPoints[UnityEngine.Random.Range(0, SpawnPoints.Length)];
            EntitesBehaviour entity = Instantiate(EntityTemplate, target).GetComponent<EntitesBehaviour>();
            
            // check for correct type
            Debug.Assert(entity.GetType() == typeof(EntitiesBot));

            EntitiesBot bot = entity as EntitiesBot;
            bot.transform.localPosition = Vector3.zero;
            bot.PlayOnce = Tables[day].PlayOnce;
            
            bot.Tables = new DialogueTable[Tables[day].Tables.Length];
            Array.Copy(Tables[day].Tables, bot.Tables, bot.Tables.Length);

            m_entities.Add(bot);
        }
    }

    void ClearEntities()
    {
        foreach (EntitesBehaviour entity in m_entities)
        {
            Destroy(entity.gameObject);
        }
    }

    [Serializable]
    public struct EntitiesTablesPerDay
    {
        public DialogueTable[] Tables;
        public bool PlayOnce;

        public uint NumberOfEntities;
    }
}
