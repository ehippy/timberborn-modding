using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using UnityEditor;
using UnityEngine;

namespace Timberborn.ModdingTools.ModBuilding {
  public static class PublicizeDlls {
    [MenuItem("Timberborn/Publicize All DLLs")]
    public static void PublicizeAll() {
      PublicizeAllTimberbornDlls();
    }

    public static void PublicizeAllTimberbornDlls() {
      Debug.Log("=== Starting DLL Publicization ===");
      var dllDir = Path.Combine(Application.dataPath, "Plugins", "Timberborn");
      if (!Directory.Exists(dllDir)) {
        Debug.LogError("DLL directory not found: " + dllDir);
        return;
      }

      var timberbornDlls = Directory.GetFiles(dllDir, "Timberborn.*.dll");
      var binditoDlls = Directory.GetFiles(dllDir, "Bindito.*.dll");
      var allDlls = timberbornDlls.Concat(binditoDlls).ToArray();

      Debug.Log($"Found {allDlls.Length} DLLs to publicize");

      foreach (var dllPath in allDlls) {
        try {
          PublicizeSingleDll(dllPath, dllDir);
          Debug.Log($"Publicized: {Path.GetFileName(dllPath)}");
        } catch (Exception e) {
          Debug.LogWarning($"Failed to publicize {Path.GetFileName(dllPath)}: {e.Message}");
        }
      }

      Debug.Log("=== DLL Publicization Complete ===");
      AssetDatabase.Refresh();
    }

    private static void PublicizeSingleDll(string dllPath, string dllDir) {
      var resolver = new DefaultAssemblyResolver();
      resolver.AddSearchDirectory(dllDir);

      using var assembly = AssemblyDefinition.ReadAssembly(dllPath, new AssemblyReaderSettings {
        AssemblyResolver = resolver
      });

      PublicizeTypes(assembly.MainModule.Types);
      assembly.Write(dllPath + ".tmp");
      File.Copy(dllPath + ".tmp", dllPath, true);
      File.Delete(dllPath + ".tmp");
    }

    private static void PublicizeTypes(System.Collections.Generic.IEnumerable<TypeDefinition> types) {
      foreach (var type in types) {
        if (type.Name.Contains("<")) continue; // Skip compiler generated
        PublicizeType(type);
        PublicizeTypes(type.NestedTypes);
      }
    }

    private static void PublicizeType(TypeDefinition type) {
      type.IsPublic = true;
      foreach (var method in type.Methods) {
        if (!method.Name.Contains("<")) {
          method.IsPublic = true;
        }
      }
      foreach (var prop in type.Properties) {
        if (prop.GetMethod != null) prop.GetMethod.IsPublic = true;
        if (prop.SetMethod != null) prop.SetMethod.IsPublic = true;
      }
      foreach (var field in type.Fields) {
        if (!field.Name.Contains("<")) {
          field.IsPublic = true;
        }
      }
    }
  }
}
