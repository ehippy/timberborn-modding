using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Timberborn.ModdingTools.ModBuilding {
  public static class BuildCritters {
    [MenuItem("Timberborn/Build Critters Mod")]
    public static void Build() {
      BuildCrittersMod();
    }

    public static void BuildCrittersMod() {
      Debug.Log("=== Starting Critters Mod Build ===");

      // Find the mod folder
      var modsDir = "Assets/Mods";
      var modFolder = Directory.GetDirectories(Path.Combine(Application.dataPath, "..", modsDir), "Critters").FirstOrDefault();

      if (string.IsNullOrEmpty(modFolder)) {
        Debug.LogError("Critters mod folder not found!");
        return;
      }

      Debug.Log($"Found mod at: {modFolder}");

      // Build the Unity project first (compiles C# code)
      Debug.Log("Building project (compiling C#)...");
      var buildPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Documents", "Timberborn", "ModsBuild", "TimberbornModExamples");
      var buildOptions = new BuildPlayerOptions {
        target = BuildTarget.StandaloneOSX,
        locationPathName = buildPath,
        scenes = Array.Empty<string>()
      };

      var buildReport = BuildPipeline.BuildPlayer(buildOptions);
      if (buildReport.summary.result != BuildResult.Succeeded) {
        Debug.LogError($"Project build failed: {buildReport.summary.result}");
        return;
      }

      Debug.Log($"Project built to: {buildPath}");

      // Prepare output directory
      var outputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Documents", "Timberborn", "Mods", "Critters");
      Directory.CreateDirectory(outputDir);
      Debug.Log($"Output directory: {outputDir}");

      // Copy manifest
      var manifestSrc = Path.Combine(Path.GetDirectoryName(modFolder), "manifest.json");
      var manifestDst = Path.Combine(outputDir, "manifest.json");
      File.Copy(manifestSrc, manifestDst, true);
      Debug.Log("Copied manifest.json");

      // Copy scripts (compiled DLL will be copied from build)
      var scriptsDir = Path.Combine(Path.GetDirectoryName(modFolder), "Scripts");
      if (Directory.Exists(scriptsDir)) {
        var targetScriptsDir = Path.Combine(outputDir, "Scripts");
        Directory.CreateDirectory(targetScriptsDir);
        foreach (var csFile in Directory.GetFiles(scriptsDir, "*.cs")) {
          var destFile = Path.Combine(targetScriptsDir, Path.GetFileName(csFile));
          File.Copy(csFile, destFile, true);
          Debug.Log($"Copied {Path.GetFileName(csFile)}");
        }
      }

      // Copy Data folder
      var dataDir = Path.Combine(Path.GetDirectoryName(modFolder), "Data");
      if (Directory.Exists(dataDir)) {
        var targetDataDir = Path.Combine(outputDir, "Data");
        CopyDirectory(dataDir, targetDataDir, true);
        Debug.Log("Copied Data folder");
      }

      Debug.Log("=== Critters Mod Build Complete ===");
      Debug.Log($"Mod output: {outputDir}");

      // Open the directory
      UnityEngine.Application.OpenURL(outputDir);
    }

    private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive) {
      Directory.CreateDirectory(destinationDir);
      foreach (var file in Directory.GetFiles(sourceDir)) {
        var destFile = Path.Combine(destinationDir, Path.GetFileName(file));
        File.Copy(file, destFile, true);
      }
      if (recursive) {
        foreach (var subDir in Directory.GetDirectories(sourceDir)) {
          var subDestDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
          CopyDirectory(subDir, subDestDir, true);
        }
      }
    }
  }
}
