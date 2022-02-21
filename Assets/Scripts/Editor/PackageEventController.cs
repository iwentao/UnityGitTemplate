using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace com.lenovo.thinkreality.ipd
{
    public class PackageEventController
    {
        [InitializeOnLoadMethod]
        private static void SubscribeToEvent()
        {
            Events.registeredPackages += RegisteredPackageEventHandler;
        }

        private static void RegisteredPackageEventHandler(PackageRegistrationEventArgs registeredPackageEventHandler)
        {
            foreach (var addedPackage in registeredPackageEventHandler.added)
            {
                Debug.Log($"Added {addedPackage.displayName}");
                EditorApplication.ExecuteMenuItem("Lenovo/Package Manager Utilities/Postprocess Packages");
            }

            foreach (var removedPackage in registeredPackageEventHandler.removed)
            {
                Debug.Log($"Removed {removedPackage.displayName}");
            }

            for (int i = 0; i <= registeredPackageEventHandler.changedFrom.Count; i++)
            {
                var oldPackage = registeredPackageEventHandler.changedFrom[i];
                var newPackage = registeredPackageEventHandler.changedTo[i];

                Debug.Log($"Changed ${oldPackage.displayName} version from ${oldPackage.version} to ${newPackage.version}");
            }
        }
    }
}
