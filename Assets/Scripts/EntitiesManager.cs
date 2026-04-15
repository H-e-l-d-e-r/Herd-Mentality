using System;
using System.Collections.Generic;
using DialogueSystem;
using UnityEngine;

public class EntitiesManager : MonoBehaviour
{
    public EntitiesTablesPerDay[] Tables;

    private List<EntitesBehaviour> m_entities;

    private void Start()
    {
        m_entities = new List<EntitesBehaviour>();

    }

    void SpawnEntities(int day)
    {
        
    }

    [Serializable]
    public struct EntitiesTablesPerDay
    {
        public DialogueTable[] Tables;
        public uint NumberOfEntities;
    }
}
