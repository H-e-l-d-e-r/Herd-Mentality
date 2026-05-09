using System;
using UnityEngine;

public class UIGroupBehaviour : MonoBehaviour
{
    public GroupContainer[] Groups;
    
    public int CurrentGroup
    {
        get => m_current;
        set
        {
            m_current = Mathf.Min(value, Groups.Length - 1);

            foreach (GroupContainer group in Groups)
            {
                group.SetActive(false);
            }            

            Groups[m_current].SetActive(true);
        }
    }

    private int m_current;


    [Serializable] 
    public struct GroupContainer
    {
        public Transform[] Elements;

        /// <summary>
        /// activate or deactivate a group
        /// </summary>
        /// <param name="active"></param>
        public void SetActive(bool active)
        {
            foreach (Transform item in Elements)
            {
                item.gameObject.SetActive(active);
            }
        }
    }
}
