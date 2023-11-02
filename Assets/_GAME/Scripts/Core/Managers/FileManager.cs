using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class FileManager
{
    #region File Handling
    public static bool WriteToFile(string fileName, string directory, object data)
    {
        string fullPath = GetFullPath(fileName, directory);

        try
        {
            using (FileStream stream = File.Open(fullPath, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, data);
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to {fullPath} with exception {e}");
            return false;
        }
    }

    public static bool LoadFromFile(string fileName, string directory, out object result)
    {
        string fullPath = GetFullPath(fileName, directory);

        try
        {
            using (FileStream stream = File.Open(fullPath, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                result = formatter.Deserialize(stream);
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read from {fullPath} with exception {e}");
            result = null;
            return false;
        }
    }

    public static bool MoveFile(string fileName, string oldDirectory, string newFileName, string newDirectory)
    {
        string fullPath = GetFullPath(fileName, oldDirectory);
        string newFullPath = GetFullPath(newFileName, newDirectory);

        try
        {
            DeleteFile(oldDirectory, newFullPath);
            File.Move(fullPath, newFullPath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to move file from {fullPath} to {newFullPath} with exection {e}");
            return false;
        }

        return true;
    }

    public static bool DeleteFile(string fileName, string directory)
    {
        string fullPath = GetFullPath(fileName, directory);

        try
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            else
            {
                Debug.LogWarning($"Unable to delete file {fullPath} because it does not exist!");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Unable to delete file {fullPath} with exeption {e}");
        }

        return false;
    }
    #endregion

    #region Directory Handling
    public static bool CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            return true;
        }

        Debug.LogWarning($"Directory {path} already exists!");
        return false;
    }
    #endregion

    #region Checkers
    public static bool FileExists(string fileName, string directory)
    {
        return File.Exists(GetFullPath(fileName, directory));
    }

    public static bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }
    #endregion

    #region Helpers
    public static string GetFullPath(string fileName, string directory)
    {
        if (string.IsNullOrEmpty(directory))
        {
            throw new Exception("The directory field cannot be null!");
        }

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            Debug.Log($"Since the directory '{directory}' did not exist, it was created.");
        }

        return Path.Combine(directory, fileName);
    }
    #endregion
}
