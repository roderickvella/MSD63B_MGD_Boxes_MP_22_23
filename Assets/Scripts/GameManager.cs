using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab; //make sure that the prefab is in the resources folder -> this is a must for photon

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnButton()
    {
        //load the colour selected by the user from the dropdown
        TMP_Dropdown dropdown = GameObject.Find("DropdownColor").GetComponent<TMP_Dropdown>();
        string colour = dropdown.options[dropdown.value].text; //the selected colour from the dropdown
        //set the size of the box using Random
        float boxRandomSize = Random.Range(0.5f, 0.8f);

        //we need to send data over the network to inform photon to instantiate the playerPrefab with our settings(colour & size)
        object[] myCustomInitData = new object[2] { colour, boxRandomSize };

        //instantiate the playerPref object over all connected devices
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f)
            , Quaternion.identity, 0, myCustomInitData);
    
    }
}
