using System;
using System.Collections.Generic;
using System.Text;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using RedMenuClient.util;

namespace RedMenuClient.features.misc
{
    public static class ObjectESPFeature
    {
        private static bool isEspRunning = false;

        private struct NearbyObject
        {
            public int Entity;
            public Vector3 Coords;
            public float Distance;
        }

        public static async void Execute()
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

                    Util.DrawText3DWithBox(obj.Coords.X, obj.Coords.Y, obj.Coords.Z, infoText, lineCount);
                }

                await BaseScript.Delay(0);
            }

            isEspRunning = false;
        }
    }
}
