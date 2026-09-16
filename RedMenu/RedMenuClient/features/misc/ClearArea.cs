using CitizenFX.Core;
using static CitizenFX.Core.Native.API;


namespace RedMenuClient.features.misc
{
    public static class ClearAreaFeature
    {
        public static void Execute()
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
    }
}
