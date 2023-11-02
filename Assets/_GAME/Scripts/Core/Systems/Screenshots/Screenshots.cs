using UnityEngine;

namespace MemeFight
{
    public class Screenshots
    {
        public const string ScreenshotsDefaultPath = "Screenshots/";

        static string GetScreenshotDefaultName()
        {
            return string.Format("Screenshot_{0}.png", System.DateTime.Now.ToString("MM-dd-yy (HH-mm-ss)"));
        }

        static bool IsOutputValid(string output)
        {
            return !string.IsNullOrEmpty(output);
        }

        static string GetFullOutputPath(string outputName, string outputPath)
        {
            if (!IsOutputValid(outputPath))
                outputPath = ScreenshotsDefaultPath;

            if (!FileManager.DirectoryExists(outputPath))
                FileManager.CreateDirectory(outputPath);

            if (!IsOutputValid(outputName))
                outputName = GetScreenshotDefaultName();

            return FileManager.GetFullPath(outputName, outputPath);
        }

        public static void Capture(string outputName = default, string outputPath = default)
        {
            try
            {
                string screenshotPath = GetFullOutputPath(outputName, outputPath);
                ScreenCapture.CaptureScreenshot(screenshotPath);
                Debug.Log("Screenshot captured. Output: " + screenshotPath);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to capture screenshot with exception: " + e);
            }
        }
    }
}
