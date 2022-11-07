using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerController : MonoBehaviour, IPunInstantiateMagicCallback
{
    private Vector3 playerScale;

    //called automatically when the prefab is instantiated (from the GameManager using PhotonNetwork.instantiate())
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        //update the player's name
        GetComponentInChildren<TextMeshProUGUI>().text = info.photonView.Owner.NickName;

        //the data that we passed when we were instantiating the prefab
        object[] instantiateData = info.photonView.InstantiationData;
        string colour = (string)instantiateData[0];
        float boxRandomSize = (float)instantiateData[1];

        //change colour
        if (colour == "Red")
            GetComponent<SpriteRenderer>().color = Color.red;
        else if (colour == "Green")
            GetComponent<SpriteRenderer>().color = Color.green;
        else if (colour == "Blue")
            GetComponent<SpriteRenderer>().color = Color.blue;

        //change size
        playerScale = new Vector3(boxRandomSize, boxRandomSize, 1);


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(playerScale.x, playerScale.y, playerScale.z);
    }
}
