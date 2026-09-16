using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using static CitizenFX.Core.Native.API;

namespace RedMenuClient.features.player
{
    public enum CamPosition
    {
        Full,
        Face,
        Torso,
        Lower
    }

    public class WardrobeCamera
    {
        private static int wardrobeCamera = -1;

        public static void Start(string position)
        {
            switch (position.ToLower())
            {
                case "off":
                    Stop();
                    break;
                case "face":
                    Start(CamPosition.Face);
                    break;
                case "torso":
                case "uppertorso":
                    Start(CamPosition.Torso);
                    break;
                case "lower":
                case "legs":
                    Start(CamPosition.Lower);
                    break;
                default:
                    Start(CamPosition.Full);
                    break;
            }
        }

        public static void Start(CamPosition position = CamPosition.Full)
        {
            int ped = PlayerPedId();
            Vector3 pedPos = GetEntityCoords(ped, true, true);
            float pedHeading = GetEntityHeading(ped);

            
            bool isFirstTime = (wardrobeCamera == -1 || !DoesCamExist(wardrobeCamera));
            if (isFirstTime)
            {
                ClearPedTasksImmediately(ped, 0, 0);
                TaskStandStill(ped, -1);
                FreezeEntityPosition(ped, true);

                uint camType = (uint)GetHashKey("DEFAULT_SCRIPTED_CAMERA");
                wardrobeCamera = Function.Call<int>((Hash)0x57CDF879EA466C46, camType, true);

                SetCamActive(wardrobeCamera, true);
                RenderScriptCams(true, true, 500, true, true, 1);
            }

      
            float dist = 1.9f;
            float camHeightOffset = 0.2f;
            float pointAtHeightOffset = 0.0f;
            float fov = 45.0f;

            switch (position)
            {
                case CamPosition.Face:
                    dist = 0.75f;
                    camHeightOffset = 0.55f;
                    pointAtHeightOffset = 0.55f;
                    fov = 35.0f;
                    break;

                case CamPosition.Torso:
                    dist = 1.15f;
                    camHeightOffset = 0.25f;
                    pointAtHeightOffset = 0.25f;
                    fov = 40.0f;
                    break;

                case CamPosition.Lower:
                    dist = 1.35f;
                    camHeightOffset = -0.40f;
                    pointAtHeightOffset = -0.40f;
                    fov = 40.0f;
                    break;

                case CamPosition.Full:
                default:
                    dist = 1.9f;
                    camHeightOffset = 0.2f;
                    pointAtHeightOffset = 0.0f;
                    fov = 45.0f;
                    break;
            }

            float rad = pedHeading * ((float)Math.PI / 180.0f);
            float camX = pedPos.X + (float)Math.Sin(-rad) * dist;
            float camY = pedPos.Y + (float)Math.Cos(-rad) * dist;
            float camZ = pedPos.Z + camHeightOffset;

            SetCamCoord(wardrobeCamera, camX, camY, camZ);
            PointCamAtCoord(wardrobeCamera, pedPos.X, pedPos.Y, pedPos.Z + pointAtHeightOffset);
            SetCamFov(wardrobeCamera, fov);
        }

        public static void Stop()
        {
            if (wardrobeCamera != -1)
            {
                int ped = PlayerPedId();
                RenderScriptCams(false, true, 500, true, true, 1);
                DestroyCam(wardrobeCamera, false);
                wardrobeCamera = -1;

                FreezeEntityPosition(ped, false);
                ClearPedTasks(ped, 0, 0);
            }
        }
    }
}