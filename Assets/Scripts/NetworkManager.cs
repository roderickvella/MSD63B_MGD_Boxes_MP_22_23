using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Newtonsoft.Json;

public class NetworkManager : MonoBehaviour, IPunObservable
{
    private PhotonView photonView;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    
    }

    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);
    }

    public void ChangeSizes(string jsonSizes)
    {
        //send message to all connected players (even the master client) with random new sizes
        photonView.RPC("ChangeSizesRPC", RpcTarget.All, jsonSizes);
    }

    [PunRPC]
    public void ChangeSizesRPC(string jsonSizes)
    {
        List<PlayerInfo> playerInfos = JsonConvert.DeserializeObject<List<PlayerInfo>>(jsonSizes);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            player.GetComponent<PlayerController>().ChangeSizeFromMaster(playerInfos);
        }

    }

    public void DestroyPlayer(int destroyPlayerId)
    {
        //send messages to all connected players to destroy box (player) with playerId
        photonView.RPC("DestroyPlayerRPC", RpcTarget.All, destroyPlayerId);
    }

    [PunRPC]
    public void DestroyPlayerRPC(int destroyPlayerId)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if(player.GetComponent<PhotonView>().Owner.ActorNumber == destroyPlayerId)
            {
                if (player.GetComponent<PhotonView>().AmOwner) //you have to be owner of that PhotonView to destroy it
                    PhotonNetwork.Destroy(player);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
