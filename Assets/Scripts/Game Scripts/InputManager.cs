using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    public struct CapturedInput
    {
        public bool left;
        public bool leftPressed;
        public bool right;
        public bool rightPressed;
        public bool up;
        public bool upPressed;
        public bool down;
        public bool downPressed;
        public bool escapePressed;
    }
    private CapturedInput currentInput;
    public CapturedInput current
    {
        get
        {
            return this.currentInput;
        }
    }

	void Update () 
    {
        this.currentInput = GetInput();
	}

    private CapturedInput GetInput()
    {
        CapturedInput currentInput = new CapturedInput();
        currentInput.up = Input.GetButton("Up");
        currentInput.upPressed = Input.GetButtonDown("Up");
        currentInput.down = Input.GetButton("Down");
        currentInput.downPressed = Input.GetButtonDown("Down");
        currentInput.left = Input.GetButton("Left");
        currentInput.leftPressed = Input.GetButtonDown("Left");
        currentInput.right = Input.GetButton("Right");
        currentInput.rightPressed = Input.GetButtonDown("Right");
        currentInput.escapePressed = Input.GetButtonDown("Escape");
        return currentInput;
    }
}
