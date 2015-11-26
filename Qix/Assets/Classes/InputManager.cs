using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;

public static class InputManager
{
    static bool playerIndexSet = false;
    public static List<PlayerIndex> _indexOfControllers = new List<PlayerIndex>();
    static private int currentController = 0;
    static GamePadState state;
    static GamePadState prevState;

    public static void GetControllers()
    {
        //get index of connected controllers
        if (!playerIndexSet || !prevState.IsConnected)
        {
            //loop over max controller amount
            for (int i = 0; i < 4; ++i)
            {
                //set up index
                PlayerIndex CurrentIndex = (PlayerIndex)i;
                GamePadState controller = GamePad.GetState(CurrentIndex);
                //if controller is connected 
                if (controller.IsConnected)
                {
                    Debug.Log(string.Format("GamePad found {0}", CurrentIndex));

                    //add to list
                    _indexOfControllers.Add(CurrentIndex);
                    _indexOfControllers.Add(CurrentIndex);
                    playerIndexSet = true;
                }
            }
        }
    }

    public static bool SetUpPlayers(int playerIndex, GamePadState prevState, GamePadState state)
    {      
        //the player divisible by two - assume left hand side of controller
        if (playerIndex % 2 == 0)
        {
            if (prevState.Buttons.Back == ButtonState.Released && state.Buttons.Back == ButtonState.Pressed)
            {
                Debug.Log(string.Format("Player {0} Connected", playerIndex));
                return true;
                
            }
        }
        else 
        {
            if (prevState.Buttons.Start == ButtonState.Released && state.Buttons.Start == ButtonState.Pressed)
            {
                Debug.Log(string.Format("Player {0} Connected", playerIndex));
                return true;
                
            }
        }
        return false;       
    }

    /// <summary>
    /// returns whether the D-pad up or Y button is pressed
    /// </summary>
    /// <param name="playerIndex"></param>
    /// <param name="prevState"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public static bool UpPressed(int playerIndex, GamePadState prevState, GamePadState state)
    {
        //if player index is divisible by 2 I'm left hand side of the controller
        if (playerIndex % 2 == 0)
        {
            //if button has been pressed return true else false
            return (prevState.DPad.Up == ButtonState.Released && state.DPad.Up == ButtonState.Pressed);
        }
        else
        {
            return (prevState.Buttons.Y == ButtonState.Released && state.Buttons.Y == ButtonState.Pressed);
        }
    }

    public static bool DownPressed(int playerIndex, GamePadState prevState, GamePadState state)
    {
        //if player index is divisible by 2 I'm left hand side of the controller
        if (playerIndex % 2 == 0)
        {
            //if button has been pressed return true else false
            return (prevState.DPad.Down == ButtonState.Released && state.DPad.Down == ButtonState.Pressed);
        }
        else
        {
            return (prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed);
        }
    }

    public static bool RightPressed(int playerIndex, GamePadState prevState, GamePadState state)
    {
        //if player index is divisible by 2 I'm left hand side of the controller
        if (playerIndex % 2 == 0)
        {
            //if button has been pressed return true else false
            return (prevState.DPad.Right == ButtonState.Released && state.DPad.Right == ButtonState.Pressed);
        }
        else
        {
            return (prevState.Buttons.B == ButtonState.Released && state.Buttons.B == ButtonState.Pressed);
        }
    }

    public static bool LeftPressed(int playerIndex, GamePadState prevState, GamePadState state)
    {
        //if player index is divisible by 2 I'm left hand side of the controller
        if (playerIndex % 2 == 0)
        {
            //if button has been pressed return true else false
            return (prevState.DPad.Left == ButtonState.Released && state.DPad.Left == ButtonState.Pressed);
        }
        else
        {
            return (prevState.Buttons.X == ButtonState.Released && state.Buttons.X == ButtonState.Pressed);
        }
    }

    public static bool UpReleased(int playerIndex, GamePadState prevState, GamePadState state)
    {
        //if player index is divisible by 2 I'm left hand side of the controller
        if (playerIndex % 2 == 0)
        {
            //if button has been pressed return true else false
            return (prevState.DPad.Up == ButtonState.Pressed && state.DPad.Up == ButtonState.Released);
        }
        else
        {
            return (prevState.Buttons.Y == ButtonState.Pressed && state.Buttons.Y == ButtonState.Released);
        }
    }

    public static bool DownReleased(int playerIndex, GamePadState prevState, GamePadState state)
    {
        //if player index is divisible by 2 I'm left hand side of the controller
        if (playerIndex % 2 == 0)
        {
            //if button has been pressed return true else false
            return (prevState.DPad.Down == ButtonState.Pressed && state.DPad.Down == ButtonState.Released);
        }
        else
        {
            return (prevState.Buttons.A == ButtonState.Pressed && state.Buttons.A == ButtonState.Released);
        }
    }

    public static bool RightReleased(int playerIndex, GamePadState prevState, GamePadState state)
    {
        //if player index is divisible by 2 I'm left hand side of the controller
        if (playerIndex % 2 == 0)
        {
            //if button has been pressed return true else false
            return (prevState.DPad.Right == ButtonState.Pressed && state.DPad.Right == ButtonState.Released);
        }
        else
        {
            return (prevState.Buttons.B == ButtonState.Pressed && state.Buttons.B == ButtonState.Released);
        }
    }

    public static bool LeftReleased(int playerIndex, GamePadState prevState, GamePadState state)
    {
        //if player index is divisible by 2 I'm left hand side of the controller
        if (playerIndex % 2 == 0)
        {
            //if button has been pressed return true else false
            return (prevState.DPad.Left == ButtonState.Pressed && state.DPad.Left == ButtonState.Released);
        }
        else
        {
            return (prevState.Buttons.X == ButtonState.Pressed && state.Buttons.X == ButtonState.Released);
        }
    }

    public static bool UpHeld(int playerIndex, GamePadState prevState, GamePadState state)
    {
        //if player index is divisible by 2 I'm left hand side of the controller
        if (playerIndex % 2 == 0)
        {
            //if button has been pressed return true else false
            return (prevState.DPad.Up == ButtonState.Pressed && state.DPad.Up == ButtonState.Pressed);
        }
        else
        {
            return (prevState.Buttons.Y == ButtonState.Pressed && state.Buttons.Y == ButtonState.Pressed);
        }
    }

    public static bool DownHeld(int playerIndex, GamePadState prevState, GamePadState state)
    {
        //if player index is divisible by 2 I'm left hand side of the controller
        if (playerIndex % 2 == 0)
        {
            //if button has been pressed return true else false
            return (prevState.DPad.Down == ButtonState.Pressed && state.DPad.Down == ButtonState.Pressed);
        }
        else
        {
            return (prevState.Buttons.A == ButtonState.Pressed && state.Buttons.A == ButtonState.Pressed);
        }
    }

    public static bool RightHeld(int playerIndex, GamePadState prevState, GamePadState state)
    {
        //if player index is divisible by 2 I'm left hand side of the controller
        if (playerIndex % 2 == 0)
        {
            //if button has been pressed return true else false
            return (prevState.DPad.Right == ButtonState.Pressed && state.DPad.Right == ButtonState.Pressed);
        }
        else
        {
            return (prevState.Buttons.B == ButtonState.Pressed && state.Buttons.B == ButtonState.Pressed);
        }
    }

    public static bool LeftHeld(int playerIndex, GamePadState prevState, GamePadState state)
    {
        //if player index is divisible by 2 I'm left hand side of the controller
        if (playerIndex % 2 == 0)
        {
            //if button has been pressed return true else false
            return (prevState.DPad.Left == ButtonState.Pressed && state.DPad.Left == ButtonState.Pressed);
        }
        else
        {
            return (prevState.Buttons.X == ButtonState.Pressed && state.Buttons.X == ButtonState.Pressed);
        }
    }

    public static PlayerIndex GetState(int index)
    {
        return (_indexOfControllers[index]);
    }
}
