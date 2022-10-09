using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class PlayerMovementController : NetworkBehaviour
{
    public float Speed = 0.1f;
    public GameObject PlayerModel;

    private void Start()
    {
        PlayerModel.SetActive(false);
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name == "Game")
        {
            if(PlayerModel.activeSelf == false)
            {
                SetPosition();
                PlayerModel.SetActive(true);
            }
            
            if(hasAuthority)
            {
                MovePlayer();
            }
        }
    }

    void SetPosition()
    {
        transform.position = new Vector3(Random.RandomRange(-5, 5), 1, Random.RandomRange(-15, 7));
    }

    public void MovePlayer()
    {
        float XDirection = Input.GetAxis("Horizontal");
        float ZDirection = Input.GetAxis("Vertical");
        Vector3 MoveDirection = new Vector3(XDirection, 0, ZDirection);
        transform.position += MoveDirection * Speed;
    }
}
