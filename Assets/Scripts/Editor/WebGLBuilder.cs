#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace WillExe.EditorTools {
    public static class WebGLBuilder {
        [MenuItem("WillExe/Build WebGL")]
        public static void Build() {
            string[] scenes = {
                "Assets/Scenes/Boot.unity",
                "Assets/Scenes/Desktop.unity"
            };
            string outDir = Path.Combine(Application.dataPath, "../Builds/WebGL");
            Directory.CreateDirectory(outDir);

            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
            PlayerSettings.WebGL.memorySize = 256;
            PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
            PlayerSettings.SetManagedStrippingLevel(UnityEditor.Build.NamedBuildTarget.WebGL, ManagedStrippingLevel.Minimal);
            PlayerSettings.productName = "WILL.EXE";
            PlayerSettings.companyName = "sudharsan";

            var opts = new BuildPlayerOptions {
                scenes = scenes,
                locationPathName = outDir,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };
            var report = BuildPipeline.BuildPlayer(opts);
            Debug.Log($"[WebGL Build] {report.summary.result} size={report.summary.totalSize} time={report.summary.totalTime} errors={report.summary.totalErrors}");
        }
    }
}
#endif
