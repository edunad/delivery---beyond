using System;
using System.Diagnostics;
using System.IO;

using UnityEditor;

using Debug = UnityEngine.Debug;

public static class CustomEditorItems {

    [MenuItem("Map/REBUILD-VMF")]
	public static void RebuildVMF() {
        Process process = new Process();

        // redirect the output stream of the child process.
        ProcessStartInfo info = new ProcessStartInfo("cmd.exe");
        info.RedirectStandardOutput = true;
        info.RedirectStandardError = true;
        info.RedirectStandardInput = true;
        info.CreateNoWindow = true;
        info.UseShellExecute = false;
        info.FileName = "./map_mdl_gen/convert.bat";
        info.WorkingDirectory = Directory.GetCurrentDirectory();

        string output = "";

        try {
            process.StartInfo = info;
            process.Start();

            output = process.StandardOutput.ReadToEnd();
            output += process.StandardError.ReadToEnd();

            process.WaitForExit();

            Debug.Log(output);
        } catch(Exception err) {
            Debug.LogError(err.ToString());
        } finally {
            process.Dispose();
            process = null;
        }
	}
}