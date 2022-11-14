using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerController : MonoBehaviour, IPunInstantiateMagicCallback, IPunObservable
{
    private Vector3 playerScale;
    private FixedJoystick fixedJoystick;
    private Vector3 playerPos;
    private float horizontal;
    private float vertical;
    private Rigidbody2D body;
    private PhotonView photonView;
    private float runSpeed = 5f;


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

    public void ChangeSizeFromMaster(List<PlayerInfo> playersInfo)
    {
        foreach(PlayerInfo playerInfo in playersInfo)
        {
            if (photonView.Owner.ActorNumber == playerInfo.actorNumber)
                this.playerScale = playerInfo.size;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (photonView.IsMine)
            {
                //get scale of object this player collided with
                float scaleOther = collision.transform.localScale.x;
                //get scale of this player
                float scaleMine = transform.localScale.x;

                //get id of smallest player
                int destroyPlayerId;
                if (scaleMine > scaleOther)
                    destroyPlayerId = collision.gameObject.GetComponent<PlayerController>().photonView.Owner.ActorNumber;
                else
                    destroyPlayerId = this.photonView.Owner.ActorNumber;

                //inform everyone to destroy eaten (smallest) player (box)
                GameObject.Find("Scripts").GetComponent<NetworkManager>().DestroyPlayer(destroyPlayerId);

            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if we own this instance clone, then we need to send our position and scale to the other connected clients
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.localScale);
        }

        //if we don't own this instance, then we need data for this clone that was streamed for us by photon
        if (stream.IsReading)
        {
            playerPos = (Vector3)stream.ReceiveNext();
            playerScale = (Vector3)stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);

        if (photonView.IsMine)
        {
            //this clone own this photon, therefore give it control to the joystick
            fixedJoystick = GameObject.FindWithTag("Joystick").GetComponent<FixedJoystick>();
            body = GetComponent<Rigidbody2D>();
        }
        else
        {
            //if this clone is not managed by the player, then we should destroy the rigidbody so that it's
            //movement can be controlled by photon data
            Destroy(GetComponent<Rigidbody2D>());
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(playerScale.x, playerScale.y, playerScale.z);

        if (photonView.IsMine)
        {
            //we own this clone, therefore move object with the data from the joystick
            horizontal = fixedJoystick.Horizontal;
            vertical = fixedJoystick.Vertical;
        }
        else
        {
            //if we don't own this photonView, then we need to manually change its position with
            //the data from the server. The stream received from OnPhotonSerializeView

            transform.position = Vector3.Lerp(transform.position,playerPos,Time.deltaTime*10);
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
            body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }
}
