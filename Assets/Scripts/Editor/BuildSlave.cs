using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace com.lenovo.thinkreality.ipd
{
    public class BuildSlave
    {
        static string GAME_NAME = "ipdsetting";
        static string VERSION = Application.version;
        public static string APP_FOLDER = Directory.GetCurrentDirectory();
        public static string ANDROID_FOLDER = string.Format("{0}/Builds/Android/", APP_FOLDER);

        public static string ANDROID_DEVELOPMENT_FILE =
            string.Format("{0}/Builds/Android/{1}.{2}.development.apk", APP_FOLDER, GAME_NAME, VERSION);

        public static string ANDROID_RELEASE_FILE =
            string.Format("{0}/Builds/Android/{1}.{2}.release.apk", APP_FOLDER, GAME_NAME, VERSION);


        [MenuItem("Lenovo/Export Android APK")]
        public static void ExportAndroidAPK()
        {
            AndroidRelease();
        }

        static void AndroidDevelopment()
        {
            // PlayerSettings.SetScriptingBackend (BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
            // PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.Android, "DEV");
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            // EditorUserBuildSettings.development = true;
            // EditorUserBuildSettings.androidETC2Fallback = AndroidETC2Fallback.Quality32Bit;
            BuildReport report = BuildPipeline.BuildPlayer(GetScenes(), ANDROID_DEVELOPMENT_FILE, BuildTarget.Android,
                BuildOptions.None);
            int code = (report.summary.result == BuildResult.Succeeded) ? 0 : 1;
            // EditorApplication.Exit (code);    
        }

        static void AndroidRelease()
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = GetScenes();
            buildPlayerOptions.locationPathName = "ipdsetting";
            buildPlayerOptions.target = BuildTarget.Android;
            buildPlayerOptions.options = BuildOptions.None;

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
            }
        }

        static string[] GetScenes()
        {
            List<string> scenes = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                {
                    scenes.Add(EditorBuildSettings.scenes[i].path);
                }
            }

            return scenes.ToArray();
        }
    }
}