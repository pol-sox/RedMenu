using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using CitizenFX.Core.Native;
using RedMenuShared;
using RedMenuClient.util;

namespace RedMenuClient.menus
{
    class MiscSettingsMenu
    {
        private static Menu menu = new Menu("Misc Settings", $"Version {ConfigManager.Version}");
        private static bool setupDone = false;

        private static bool isEspRunning = false;

        private static void SetupMenu()
        {
            if (setupDone) return;
            setupDone = true;

            MenuCheckboxItem minimapKeybind = new MenuCheckboxItem("Minimap Controls", "Holding down the Select Radar Option button will allow you to toggle the minimap on/off when this option is enabled.", UserDefaults.MiscMinimapControls);
            MenuCheckboxItem showCores = new MenuCheckboxItem("Always Show Cores", "The cores above your radar will always be displayed when this option is enabled. The game will automatically show or hide the cores if this is disabled.", UserDefaults.MiscAlwaysShowCores);
            MenuCheckboxItem objectESP = new MenuCheckboxItem("Object ESP", "Allows you to see information about objects near you, such as their hash and entity ID. Using first-person also allows you to view the text closer.", UserDefaults.ObjectESP);
            MenuItem clearArea = new MenuItem("Clear Area", "Clears the area around your player.");
            

            menu.AddMenuItem(minimapKeybind);
            menu.AddMenuItem(showCores);

            if (PermissionsManager.IsAllowed(Permission.MSClearArea))
            {
                menu.AddMenuItem(objectESP);
                menu.AddMenuItem(clearArea);
            }

            menu.OnItemSelect += (m, item, index) =>
            {
                if (item == clearArea)
                {
                    int ent = 0;
                    int handle;
                    Vector3 coords1 = GetEntityCoords(PlayerPedId(), true, true);
                    Vector3 coords2;
                    
                    handle = FindFirstObject(ref ent);
                    coords2 = GetEntityCoords(ent, true, true);
                    if (GetDistanceBetweenCoords(coords1.X, coords1.Y, coords1.Z, coords2.X, coords2.Y, coords2.Z, true) <= 100)
                    {
                        DeleteObject(ref ent);
                    }
                    while (FindNextObject(handle, ref ent))
                    {
                        coords2 = GetEntityCoords(ent, true, true);
                        if (GetDistanceBetweenCoords(coords1.X, coords1.Y, coords1.Z, coords2.X, coords2.Y, coords2.Z, true) <= 100)
                        {
                            DeleteObject(ref ent);
                        }
                    }
                    EndFindObject(handle);

                    handle = FindFirstPed(ref ent);
                    if (!IsPedAPlayer(ent))
                    {
                        coords2 = GetEntityCoords(ent, true, true);

                        if (GetDistanceBetweenCoords(coords1.X, coords1.Y, coords1.Z, coords2.X, coords2.Y, coords2.Z, true) <= 100)
                        {
                            DeletePed(ref ent);
                        }
                    }
                    while (FindNextPed(handle, ref ent))
                    {
                        if (!IsPedAPlayer(ent))
                        {
                            coords2 = GetEntityCoords(ent, true, true);
                            if (GetDistanceBetweenCoords(coords1.X, coords1.Y, coords1.Z, coords2.X, coords2.Y, coords2.Z, true) <= 100)
                            {
                                DeletePed(ref ent);
                            }
                        }
                    }
                    EndFindPed(handle);

                    handle = FindFirstVehicle(ref ent);
                    coords2 = GetEntityCoords(ent, true, true);
                    if (GetDistanceBetweenCoords(coords1.X, coords1.Y, coords1.Z, coords2.X, coords2.Y, coords2.Z, true) <= 100)
                    {
                        DeleteVehicle(ref ent);
                    }
                    while (FindNextVehicle(handle, ref ent))
                    {
                        coords2 = GetEntityCoords(ent, true, true);
                        if (GetDistanceBetweenCoords(coords1.X, coords1.Y, coords1.Z, coords2.X, coords2.Y, coords2.Z, true) <= 100)
                        {
                            DeleteVehicle(ref ent);
                        }
                    }
                    EndFindVehicle(handle);
                }
            };

            menu.OnCheckboxChange += (m, item, index, _checked) =>
            {
                if (item == minimapKeybind)
                {
                    UserDefaults.MiscMinimapControls = _checked;
                }
                else if (item == showCores)
                {
                    UserDefaults.MiscAlwaysShowCores = _checked;
                    Function.Call((Hash)0xD4EE21B7CC7FD350, UserDefaults.MiscAlwaysShowCores); // _ALWAYS_SHOW_HORSE_CORES
                    Function.Call((Hash)0x50C803A4CD5932C5, UserDefaults.MiscAlwaysShowCores); // _ALWAYS_SHOW_PLAYER_CORES
                }
                else if (item == objectESP)
                {
                    UserDefaults.ObjectESP = _checked;
                    if (_checked)
                    {
                        StartObjectESP();
                    }
                }
            };

            if (UserDefaults.ObjectESP)
            {
                StartObjectESP();
            }
        }
        private struct NearbyObject
        {
            public int Entity;
            public Vector3 Coords;
            public float Distance;
        }

     
        private static async void StartObjectESP()
        {
            if (isEspRunning) return;
            isEspRunning = true;

            const int maxObjects = 5;
            const float maxDistance = 30.0f;

            while (UserDefaults.ObjectESP)
            {
                Vector3 playerCoords = GetEntityCoords(PlayerPedId(), true, true);
                List<NearbyObject> foundObjects = new List<NearbyObject>();

                int entity = 0;
                int handle = FindFirstObject(ref entity);

                if (handle != -1)
                {
                    try
                    {
                        do
                        {
                            if (DoesEntityExist(entity))
                            {
                                Vector3 objCoords = GetEntityCoords(entity, true, true);
                                float distance = GetDistanceBetweenCoords(playerCoords.X, playerCoords.Y, playerCoords.Z, objCoords.X, objCoords.Y, objCoords.Z, true);

                                if (distance <= maxDistance)
                                {
                                    foundObjects.Add(new NearbyObject
                                    {
                                        Entity = entity,
                                        Coords = objCoords,
                                        Distance = distance
                                    });
                                }
                            }
                        }
                        while (FindNextObject(handle, ref entity));
                    }
                    finally
                    {
                        EndFindObject(handle);
                    }
                }

                // closest first
                foundObjects.Sort((a, b) => a.Distance.CompareTo(b.Distance));

                int countToDraw = Math.Min(foundObjects.Count, maxObjects);
                for (int i = 0; i < countToDraw; i++)
                {
                    var obj = foundObjects[i];
                    int ent = obj.Entity;
                    uint model = (uint)GetEntityModel(ent);
                    float heading = GetEntityHeading(ent);
                    bool isMission = IsEntityAMissionEntity(ent);
                    bool isAttached = IsEntityAttached(ent);
                    bool isNetworked = NetworkGetEntityIsNetworked(ent);
                    int netId = isNetworked ? NetworkGetNetworkIdFromEntity(ent) : -1;

                    // info
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"Model: 0x{model:X8} ({model})");
                    sb.AppendLine($"Entity: {ent} | NetID: {(netId != -1 ? netId.ToString() : "None")}");
                    sb.AppendLine($"Dist: {obj.Distance:0.0}m | Hdg: {heading:0}°");
                    sb.AppendLine($"Pos: {obj.Coords.X:0.1}, {obj.Coords.Y:0.1}, {obj.Coords.Z:0.1}");
                    sb.AppendLine($"Mission: {isMission} | Attached: {isAttached}");

                    string infoText = sb.ToString().TrimEnd('\n');
                    int lineCount = 6;

                    DrawText3DWithBox(obj.Coords.X, obj.Coords.Y, obj.Coords.Z, infoText, lineCount);
                }

                await BaseScript.Delay(0);
            }

            isEspRunning = false;
        }
        private static void DrawText3DWithBox(float x, float y, float z, string text, int lineCount)
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


        public static Menu GetMenu()
        {
            SetupMenu();
            return menu;
        }
    }
}
