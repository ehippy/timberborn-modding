using System.Collections.Generic;
using Timberborn.ModdingTools.Common;
using Timberborn.ModdingTools.ModBuilding;
using UnityEditor;
using UnityEngine;

namespace Timberborn.ModdingTools {
  public static class BuildCrittersMod {
    [MenuItem("Timberborn/Build Critters (no code)")]
    public static void BuildCrittersNoCode() {
      var settings = new ModBuilderSettings(false, false, true, true, false, "");
      var result = new ModBuilder(new List<ModDefinition> { 
        new ModDefinition("Critters", "Assets/Mods/Critters") 
      }, settings).Build();
      Debug.Log(result ? "Build succeeded" : "Build failed");
    }
  }
}
