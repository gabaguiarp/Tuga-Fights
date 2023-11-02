using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class InputDevicesManager
{
    public static bool TryShareDeviceWithPlayer(InputDevice device, PlayerInput player, out string controlScheme, bool isPlayerOne = false)
    {
        controlScheme = null;

        switch (device.name)
        {
            // NOTE: Player 1 has priority over the KeyboardLeft scheme
            case "Keyboard":
                if (IsKeyboardBeingShared())
                {
                    Debug.LogError("The Keyboard is already being shared by 2 players! Unable to pair another one.");
                    return false;
                }
                else if (!isPlayerOne && (player.currentControlScheme == "Keyboard&Mouse" || player.currentControlScheme == "KeyboardLeft"))
                {
                    player.SwitchCurrentControlScheme("KeyboardLeft", player.devices.ToArray());
                    controlScheme = "KeyboardRight";
                }
                else
                {
                    player.SwitchCurrentControlScheme("KeyboardRight", player.devices.ToArray());
                    controlScheme = "KeyboardLeft";
                }

                return true;
        }

        return false;
    }

    public static bool IsKeyboardBeingShared()
    {
        bool usingKeyboardLeft = PlayerInput.all.Any(p => p.currentControlScheme == "KeyboardLeft");
        bool usingKeyboardRight = PlayerInput.all.Any(p => p.currentControlScheme == "KeyboardRight");

        return usingKeyboardLeft && usingKeyboardRight;
    }

    public static InputDevice[] GetKeyboardAndMouseSchemeDevices()
    {
        List<InputDevice> devices = new List<InputDevice>();

        devices.Add(Keyboard.current);

        if (Mouse.current != null)
            devices.Add(Mouse.current);

        return devices.ToArray();
    }
}
