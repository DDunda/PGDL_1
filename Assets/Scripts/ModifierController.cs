using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ModifierController : MonoBehaviour
{
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
        PlayerMovement pMov = Controller.playerMovement;
        GameObject modifierUI = Controller.modifierUI;

        for (var i = modifierUI.transform.childCount - 1; i >= 0; i--)
        {
            var input = modifierUI.transform.GetChild(i).gameObject;
            var field = input.GetComponent<TMP_InputField>();

            switch (input.name)
            {
                case "AirJumps":
                    field.text = Convert.ToString(pMov.airJumpsMax);
                    break;
                case "WalkSpeed":
                    field.text = Convert.ToString(pMov.walkSpeed);
                    break;
                case "SprintSpeed":
                    field.text = Convert.ToString(pMov.sprintSpeed);
                    break;
                case "GroundDrag":
                    field.text = Convert.ToString(pMov.groundDrag);
                    break;
                case "JumpForce":
                    field.text = Convert.ToString(pMov.jumpForce);
                    break;
                default:
                    Debug.LogWarning(string.Format("Could not change the text of text field {0}", i));
                    break;
            }
        }
    }

    // Read all values from all text fields, and update the player's parameters accordingly
    public static void UpdateModifiers()
	{
        PlayerMovement pMov = Controller.playerMovement;
        GameObject modifierUI = Controller.modifierUI;

        for (var i = modifierUI.transform.childCount - 1; i >= 0; i--)
		{
            var input = modifierUI.transform.GetChild(i).gameObject;
            var field = input.GetComponent<TMP_InputField>();

            if (int.TryParse(field.text, out int result))
			{
                switch (input.name)
                {
                    case "AirJumps":
                        pMov.airJumpsMax = result;
                        break;
                    case "WalkSpeed":
                        pMov.walkSpeed = result;
                        break;
                    case "SprintSpeed":
                        pMov.sprintSpeed = result;
                        break;
                    case "GroundDrag":
                        pMov.groundDrag = result;
                        break;
                    case "JumpForce":
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
}
