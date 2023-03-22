using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
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
        SetModifiers();
    }

    public static void SetModifiers()
    {
        FirstPersonController fpc = Controller.fpController;
        GameObject modifierUI = Controller.modifierUI;

        for (var i = modifierUI.transform.childCount - 1; i >= 0; i--) {
            var input = modifierUI.transform.GetChild(i).gameObject;
            var field = input.GetComponent<TMP_InputField>();
            var check = input.GetComponent<UnityEngine.UI.Toggle>();

            switch (input.name)
            {
                case "CanSprint":
                    check.isOn = fpc.canSprint;
                    break;
                case "CanJump":
                    check.isOn = fpc.canJump;
                    break;
                case "SlopeSliding":
                    check.isOn = fpc.willSlideOnSlopes;
                    break;
                case "AirMovement":
                    check.isOn = fpc.canMoveInAir;
                    break;
                case "WalkSpeed":
                    field.text = fpc.walkSpeed.ToString();
                    break;
                case "SprintSpeed":
                    field.text = fpc.sprintSpeed.ToString();
                    break;
                case "SlopeSpeed":
                    field.text = fpc.slopeSpeed.ToString();
                    break;
                case "MaxSlopeAngle":
                    field.text = Controller.charController.slopeLimit.ToString();
                    break;
                case "JumpForce":
                    field.text = fpc.jumpForce.ToString();
                    break;
                case "AirJumps":
                    field.text = fpc.airJumpsMax.ToString();
                    break;
                case "AirMultiplier":
                    field.text = fpc.airMultiplier.ToString();
                    break;
                case "Gravity":
                    field.text = fpc.gravity.ToString();
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
        FirstPersonController pMov = Controller.fpController;
        GameObject modifierUI = Controller.modifierUI;

        EventSystem.current.SetSelectedGameObject(null);

        for (var i = modifierUI.transform.childCount - 1; i >= 0; i--)
		{
            var input = modifierUI.transform.GetChild(i).gameObject;
            var field = input.GetComponent<TMP_InputField>();
            var check = input.GetComponent<UnityEngine.UI.Toggle>();

            int iresult = int.Parse(field.text);
            float fresult = float.Parse(field.text);

            switch (input.name) {
                case "CanSprint":
                    pMov.canSprint = check.isOn;
                    break;
                case "CanJump":
                    pMov.canJump = check.isOn;
                    break;
                case "SlopeSliding":
                    pMov.willSlideOnSlopes = check.isOn;
                    break;
                case "AirMovement":
                    pMov.canMoveInAir = check.isOn;
                    break;
                case "WalkSpeed":
                    pMov.walkSpeed = fresult;
                    break;
                case "SprintSpeed":
                    pMov.sprintSpeed = fresult;
                    break;
                case "SlopeSpeed":
                    pMov.slopeSpeed = fresult;
                    break;
                case "MaxSlopeAngle":
                    Controller.charController.slopeLimit = fresult;
                    break;
                case "JumpForce":
                    pMov.jumpForce = fresult;
                    break;
                case "AirJumps":
                    pMov.airJumpsMax = iresult;
                    break;
                case "AirMultiplier":
                    pMov.airMultiplier = fresult;
                    break;
                case "Gravity":
                    pMov.gravity = fresult;
                    break;
                default:
                    Debug.LogWarning(string.Format("Could not find a variable to change for the modifier of value {0}", i));
                    break;
            }
		}
	}
}
