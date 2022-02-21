using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEditor.PackageManager.Requests;

namespace com.lenovo.thinkreality.ipd
{
    // Refer to: https://forum.unity.com/threads/solved-removing-packages-in-an-editor-script.697061/
    public static class PackageController
    {
        private static RemoveRequest _removeRequest;
        private static Queue<string> _pkgNameQueue;
        private static AddRequest _addRequest;
        private static Queue<string> _pkgAddQueue;

        [MenuItem("Window/Remove unused packages")]
        public static void RemoveUnusedPackages()
        {
            List<string> packageList = new List<string>();
            packageList.Add("com.unity.ads");
            packageList.Add("com.unity.collab-proxy");
            packageList.Add("com.unity.modules.vehicles");
            packageList.Add("com.unity.modules.terrain");
            packageList.Add("com.unity.modules.unityanalytics");
            packageList.Add("com.unity.timeline");

            string packageListString = string.Join(",", packageList);
            RemovePackages(packageListString);
        }
        
        public static void RemovePackages(string packageList)
        {
            string[] packages = packageList.Split(',');
            if (packages.Length == 0)
            {
                Debug.LogError("No package to remove.");
                return;
            }
            
            _pkgNameQueue = new Queue<string>();
            foreach(var package in packages)
                _pkgNameQueue.Enqueue(package);

            EditorApplication.update += PackageRemovalProgress;
            EditorApplication.LockReloadAssemblies();

            var nextRequestString = _pkgNameQueue.Dequeue();
            _removeRequest = Client.Remove(nextRequestString);
        }

        private static void PackageRemovalProgress()
        {
            if (_removeRequest == null)
                return;
            
            if (_removeRequest.IsCompleted)
            {
                switch (_removeRequest.Status)
                {
                   case StatusCode.Failure:
                       Debug.LogError($"Could not remove package [{_removeRequest.PackageIdOrName}].");
                       break;
                   
                   case StatusCode.InProgress:
                       break;
                   
                   case StatusCode.Success:
                       Debug.Log($"Removed package : {_removeRequest.PackageIdOrName}");
                       break;
                }

                if (_pkgNameQueue.Count > 0)
                {
                    var nextRequestString = _pkgNameQueue.Dequeue();
                    Debug.Log($"Requesting removal of [{nextRequestString}].");
                    _removeRequest = Client.Remove(nextRequestString);
                }
                else
                {
                    EditorApplication.update -= PackageRemovalProgress;
                    EditorApplication.UnlockReloadAssemblies();
                }
            }
        }

        [MenuItem("Window/Add ThinkReality package")]
        public static void AddThinkRealityPackages()
        {
            AddLocalPackages("com.lenovo.thinkreality.core-2.8.66.tgz");
        }

        public static void AddLocalPackages(string packageList)
        {
            string[] packages = packageList.Split(',');
            if(packages.Length == 0)
                Debug.LogError("No package to add");

            _pkgAddQueue = new Queue<string>();
            foreach (var package in packages)
            {
                string packageName = $"file:../Packages/{package}";
                _pkgAddQueue.Enqueue(packageName); 
            }

            EditorApplication.update += PackageAddProgress;
            EditorApplication.LockReloadAssemblies();
            
            string nextRequestString = _pkgAddQueue.Dequeue();
            _addRequest = Client.Add(nextRequestString);
        }

        private static void PackageAddProgress()
        {
            if (_addRequest?.Result == null) return;
            
            if (_addRequest.IsCompleted)
            {
                switch (_addRequest.Status)
                {
                    case StatusCode.Failure:
                        Debug.LogError($"Could not add package [{_addRequest.Result.packageId}].");
                        break;
                   
                    case StatusCode.InProgress:
                        break;
                   
                    case StatusCode.Success:
                        Debug.Log($"Added package [{_addRequest.Result.packageId}]");
                        break;
                }

                if (_pkgAddQueue.Count > 0)
                {
                    var nextRequestString = _pkgAddQueue.Dequeue();
                    Debug.Log($"Requesting add [{nextRequestString}].");
                    _addRequest = Client.Add(nextRequestString);
                }
                else
                {
                    EditorApplication.update -= PackageAddProgress;
                    EditorApplication.UnlockReloadAssemblies();
                }
            } 
        }
    }
}