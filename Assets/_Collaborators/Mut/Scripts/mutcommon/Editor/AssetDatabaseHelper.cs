using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace MutCommon
{
  public static class AssetDatabaseHelper
  {
    public static bool CreateFolderIfNotExists(string path)
    {
      bool created = false;
      int idx = path.LastIndexOf('/');
      if (idx == -1)
      {
        //Debug.LogError("invalid path: " + path);
        return false;
      }

      var parentPath = path.Substring(0, idx);
      var folderName = path.Substring(idx + 1);

      if (!AssetDatabase.IsValidFolder(path))
      {
        CreateFolderIfNotExists(parentPath);
        AssetDatabase.CreateFolder(parentPath, folderName);
        created = true;
      }

      return created;
    }

    public static (T, bool) SaveOrLoadExisting<T>(this T asset, string assetName, string assetPath, string extension, Action<T, string> saveAssetToPath) where T : UnityEngine.Object
    {

      var query = $"{assetName}";
      var res = AssetDatabase.FindAssets(query, new[] { assetPath });

      if (res.Length == 0)
      {
        var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(assetPath);
        saveAssetToPath(asset, $"{assetPath}/{assetName}{extension}");
        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
      }

      res = AssetDatabase.FindAssets(query, new[] { assetPath });

      if (res.Length != 1)
      {
        //UberDebug.LogErrorChannel("mutcommon", $"Something went wrong while creating {assetPath}");
        return (null, false);
      }

      asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(res[0]));
      return (asset, true);
    }

  }
}