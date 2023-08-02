using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

// Put this memu command script into Assets/Editor/

class ProjectTool
{
  static void BuildForIOS()
  {
    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);

    // EditorUserBuildSettings.symlinkLibraries = true;
    EditorUserBuildSettings.development = false;
    EditorUserBuildSettings.allowDebugging = false;

    List<string> scenes = new List<string>();
    for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
    {
      if (EditorBuildSettings.scenes[i].enabled)
      {
        scenes.Add(EditorBuildSettings.scenes[i].path);
      }
    }

    BuildPipeline.BuildPlayer(scenes.ToArray(), "iOSProj", BuildTarget.iOS, BuildOptions.None);
  }
  static void BuildForAndroid()
  {
    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);

    // EditorUserBuildSettings.symlinkLibraries = true;
    EditorUserBuildSettings.development = false;
    EditorUserBuildSettings.allowDebugging = false;

    List<string> scenes = new List<string>();
    for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
    {
      if (EditorBuildSettings.scenes[i].enabled)
      {
        scenes.Add(EditorBuildSettings.scenes[i].path);
      }
    }

    BuildPipeline.BuildPlayer(scenes.ToArray(), "result/im.unity.uikit.apk", BuildTarget.Android, BuildOptions.None);
  }
}