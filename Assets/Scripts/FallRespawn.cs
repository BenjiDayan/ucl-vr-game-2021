using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FallRespawn : MonoBehaviour
{
    [SerializeField] private Transform player;

    private void OnTriggerEnter(Collider other)
    {
        //check if player collides with plane
        if(other.CompareTag("Player"))
        {
            //Reload Scene if player collides with plane
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Physics.SyncTransforms();
        }
    }

}
