using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baguette_Ability : MonoBehaviour, I_Ability
{
    private CharacterController characterController; // For adding movement when dashing
    private ItemData itemData; // For finding the durability or cooldown of baguette
    private Camera playerCamera; // For dashing towards the camera's view
    // Start is called before the first frame update

    void Start()
    {
        Invoke("GetCharacterController", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetCharacterController()
    {
        playerCamera = null;
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        if (1 < player.Length)
            Debug.LogError("More than 1 object with player tag found!");
        else if (0 == player.Length)
            Debug.LogError("No player object was found!");
        else
        {
            characterController = player[0].GetComponent<CharacterController>();
            if (null == characterController)
                Debug.LogError("Failed to get character controller");
        }
        GameObject[] cameras = GameObject.FindGameObjectsWithTag("MainCamera");
        if (cameras.Length == 0)
            Debug.LogError("failed to get camera of the player");
        else if (cameras.Length == 1)
        {
            playerCamera = cameras[0].GetComponent<Camera>();
        }
        if (null == playerCamera)
            Debug.LogError("No Camera could be found in camera list");
        itemData = GetComponent<ItemTemplate>().itemData;
        if (null == itemData)
            Debug.LogError("No itemData found!");
    }

    public bool AbilityA()
    {
        // make characterController charge in direction of player
        characterController.Move(playerCamera.transform.forward * 20f);
        return true;
    }
}
