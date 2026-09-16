using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace RedMenuClient
{
    static class Util
    {

        /// <summary>
        /// Returns true if the control is pressed.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static bool IsControlPressed(Control control)
        {
            if (CitizenFX.Core.Native.Function.Call<bool>(CitizenFX.Core.Native.Hash.IS_CONTROL_PRESSED, 0, (uint)control))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the control is just pressed.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static bool IsControlJustPressed(Control control)
        {
            if (CitizenFX.Core.Native.Function.Call<bool>(CitizenFX.Core.Native.Hash.IS_CONTROL_JUST_PRESSED, 0, (uint)control))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the disabled control is pressed.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static bool IsDisabledControlPressed(Control control)
        {
            if (CitizenFX.Core.Native.Function.Call<bool>(CitizenFX.Core.Native.Hash.IS_DISABLED_CONTROL_PRESSED, 0, (uint)control))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the disabled control is just pressed.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static bool IsDisabledControlJustPressed(Control control)
        {
            if (CitizenFX.Core.Native.Function.Call<bool>(CitizenFX.Core.Native.Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, (uint)control))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the control is released.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static bool IsControlReleased(Control control)
        {
            if (CitizenFX.Core.Native.Function.Call<bool>(CitizenFX.Core.Native.Hash.IS_CONTROL_RELEASED, 0, (uint)control))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the control is just released.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static bool IsControlJustReleased(Control control)
        {
            if (CitizenFX.Core.Native.Function.Call<bool>(CitizenFX.Core.Native.Hash.IS_CONTROL_JUST_RELEASED, 0, (uint)control))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the disabled control is released.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static bool IsDisabledControlReleased(Control control)
        {
            if (CitizenFX.Core.Native.Function.Call<bool>(CitizenFX.Core.Native.Hash.IS_DISABLED_CONTROL_PRESSED, 0, (uint)control))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns true if the disabled control is just released.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static bool IsDisabledControlJustReleased(Control control)
        {
            if (CitizenFX.Core.Native.Function.Call<bool>(CitizenFX.Core.Native.Hash.IS_DISABLED_CONTROL_JUST_RELEASED, 0, (uint)control))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the control is enabled.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static bool IsControlEnabled(Control control)
        {
            return CitizenFX.Core.Native.Function.Call<bool>(CitizenFX.Core.Native.Hash.IS_CONTROL_ENABLED, 0, (uint)control);
        }


        /// <summary>
        /// Draws 3D text with a box background at the specified world coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="text"></param>
        /// <param name="lineCount"></param>
        public static void DrawText3DWithBox(float x, float y, float z, string text, int lineCount)
        {
            float screenX = 0f;
            float screenY = 0f;

            // small offset to avoid hiding in ground
            z += 0.5f;

            if (GetScreenCoordFromWorldCoord(x, y, z, ref screenX, ref screenY))
            {
                // calc camera distance
                Vector3 camCoords = GetGameplayCamCoord();
                float camDist = (float)Math.Sqrt(
                    (camCoords.X - x) * (camCoords.X - x) +
                    (camCoords.Y - y) * (camCoords.Y - y) +
                    (camCoords.Z - z) * (camCoords.Z - z)
                );

                if (camDist < 1.0f) camDist = 1.0f;

                // stop box swinging while moving camera and looking at it
                float fov = (1.0f / GetGameplayCamFov()) * 100.0f;
                float scale = (1.25f / camDist) * (fov * 0.8f);


                float baseTextScale = 0.35f * scale;
                float lineHeight = 0.032f * scale;
                float boxHeight = (lineCount * lineHeight) + (0.032f * scale);
                float boxWidth = 0.19f * scale;


                float boxCenterY = screenY + (boxHeight / 2.0f);

                // background
                DrawRect(screenX, boxCenterY, boxWidth, boxHeight, 0, 0, 0, 190, false, false);

                // text
                long textHash = Function.Call<long>((Hash)0xFA925AC00EB830B9, 10, "LITERAL_STRING", text);
                SetTextColor(255, 255, 255, 240);
                SetTextScale(baseTextScale, baseTextScale);
                SetTextFontForCurrentCommand(4);
                SetTextCentre(false);

                float textLeftX = screenX - (boxWidth / 2.0f) + (0.005f * scale);
                DisplayText(textHash, textLeftX, screenY);
            }
        }

    }
}
