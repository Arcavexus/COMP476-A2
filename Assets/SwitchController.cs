using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public GameObject navMesh;
    public GameObject pathfinding;


    public void Switch()
    {
        if(navMesh.activeSelf)
        {
            navMesh.SetActive(false);
            pathfinding.SetActive(true);
        }
        else
        {
            navMesh.SetActive(true);
            pathfinding.SetActive(false);
        }
            
    }
}
