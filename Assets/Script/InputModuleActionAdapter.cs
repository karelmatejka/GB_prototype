﻿using UnityEngine;
using InControl;


[RequireComponent(typeof(InControlInputModule))]
public class InputModuleActionAdapter : MonoBehaviour
{
    public class InputModuleActions : PlayerActionSet
    {
        public PlayerAction Jump;
        public PlayerAction Exit;
        public PlayerAction Submit;
        public PlayerAction Cancel;
        public PlayerAction Left;
        public PlayerAction Right;
        public PlayerAction Up;
        public PlayerAction Down;
        public PlayerTwoAxisAction Move;


        public InputModuleActions()
        {
            Jump = CreatePlayerAction("Jump");
            Exit = CreatePlayerAction("Exit");
            Submit = CreatePlayerAction("Submit");
            Cancel = CreatePlayerAction("Cancel");
            Left = CreatePlayerAction("Move Left");
            Right = CreatePlayerAction("Move Right");
            Up = CreatePlayerAction("Move Up");
            Down = CreatePlayerAction("Move Down");
            Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
        }
    }


    public InputModuleActions actions;


    void OnEnable()
    {
        CreateActions();

        var inputModule = GetComponent<InControlInputModule>();
        if (inputModule != null)
        {
            inputModule.SubmitAction = actions.Submit;
            inputModule.CancelAction = actions.Cancel;
            inputModule.MoveAction = actions.Move;
        }
    }


    void OnDisable()
    {
        DestroyActions();
    }


    void CreateActions()
    {
        actions = new InputModuleActions();

        actions.Jump.AddDefaultBinding(InputControlType.Action1);
        actions.Jump.AddDefaultBinding(Key.Space);

        actions.Exit.AddDefaultBinding(InputControlType.Command);
        actions.Exit.AddDefaultBinding(Key.Escape);

        actions.Submit.AddDefaultBinding(InputControlType.Action1);
        actions.Submit.AddDefaultBinding(Key.Space);
        actions.Submit.AddDefaultBinding(Key.Return);

        actions.Cancel.AddDefaultBinding(InputControlType.Action2);
        actions.Cancel.AddDefaultBinding(Key.Escape);

        actions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
        actions.Up.AddDefaultBinding(InputControlType.DPadUp);
        actions.Up.AddDefaultBinding(Key.UpArrow);

        actions.Down.AddDefaultBinding(InputControlType.LeftStickDown);
        actions.Down.AddDefaultBinding(InputControlType.DPadDown);
        actions.Down.AddDefaultBinding(Key.DownArrow);

        actions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
        actions.Left.AddDefaultBinding(InputControlType.DPadLeft);
        actions.Left.AddDefaultBinding(Key.LeftArrow);

        actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
        actions.Right.AddDefaultBinding(InputControlType.DPadRight);
        actions.Right.AddDefaultBinding(Key.RightArrow);

        actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
        actions.Right.AddDefaultBinding(InputControlType.DPadRight);
        actions.Right.AddDefaultBinding(Key.RightArrow);
    }


    void DestroyActions()
    {
        actions.Destroy();
    }
}