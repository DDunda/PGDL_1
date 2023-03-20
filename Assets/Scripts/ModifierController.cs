using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ModifierController : MonoBehaviour
{
    public GameObject player;
    public GameObject modifierUI;

    // an enum is probably not the best way to do this, but i don't know what i'm doing!!!! yippee!!!!
    // and it doesn't really matter for a prototype like this anyways
    enum Modifier
	{
        AirJumps,
        WalkSpeed,
        SprintSpeed,
        GroundDrag,
        JumpForce
	}

    // Start is called before the first frame update
    // set the values inside the text field to that of the player's initial values
    void Start()
    {
        PlayerMovement pMov = player.transform.GetComponent<PlayerMovement>();

        for (var i = modifierUI.transform.childCount - 1; i >= 0; i--)
        {
            var field = modifierUI.transform.GetChild(i).GetComponent<TMP_InputField>();

            switch (i)
            {
                case (int)Modifier.AirJumps:
                    field.text = Convert.ToString(pMov.airJumpsMax);
                    break;
                case (int)Modifier.WalkSpeed:
                    field.text = Convert.ToString(pMov.walkSpeed);
                    break;
                case (int)Modifier.SprintSpeed:
                    field.text = Convert.ToString(pMov.sprintSpeed);
                    break;
                case (int)Modifier.GroundDrag:
                    field.text = Convert.ToString(pMov.groundDrag);
                    break;
                case (int)Modifier.JumpForce:
                    field.text = Convert.ToString(pMov.jumpForce);
                    break;
                default:
                    Debug.LogWarning(string.Format("Could not change the text of text field {0}", i));
                    break;
            }
        }
    }

    // Read all values from all text fields, and update the player's parameters accordingly
    public void UpdateModifiers()
	{
        PlayerMovement pMov = player.transform.GetComponent<PlayerMovement>();

        for (var i = modifierUI.transform.childCount - 1; i >= 0; i--)
		{
            var field = modifierUI.transform.GetChild(i).GetComponent<TMP_InputField>();

            if (int.TryParse(field.text, out int result))
			{
                switch (i)
                {
                    case (int)Modifier.AirJumps:
                        pMov.airJumpsMax = result;
                        break;
                    case (int)Modifier.WalkSpeed:
                        pMov.walkSpeed = result;
                        break;
                    case (int)Modifier.SprintSpeed:
                        pMov.sprintSpeed = result;
                        break;
                    case (int)Modifier.GroundDrag:
                        pMov.groundDrag = result;
                        break;
                    case (int)Modifier.JumpForce:
                        pMov.jumpForce = result;
                        break;
                    default:
                        Debug.LogWarning(string.Format("Could not find a variable to change for the modifier of value {0}", i));
                        break;
                }
            } 
            else
			{
                Debug.LogWarning(string.Format("Text field {0} cannot be converted into an integer!", i));
			}
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
