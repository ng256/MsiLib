using System;
using System.Diagnostics;
using System.IO;

namespace MsiInstaller
{
    /// <summary>
    /// Class responsible for building MSI files using WiX Toolset (Candle and Light).
    /// </summary>
    public class MsiBuilder
    {
        /// <summary>
        /// Gets the path to the candle.exe tool.
        /// </summary>
        public string CandlePath { get; }

        /// <summary>
        /// Gets the path to the light.exe tool.
        /// </summary>
        public string LightPath { get; }

        /// <summary>
        /// Initializes a new instance of the MsiBuilder class with the paths to Candle and Light executables.
        /// </summary>
        /// <param name="candlePath">The path to the candle.exe tool.</param>
        /// <param name="lightPath">The path to the light.exe tool.</param>
        /// <exception cref="ArgumentException">Thrown when either path is invalid or the executable is not found.</exception>
        public MsiBuilder(string candlePath, string lightPath)
        {
            if (string.IsNullOrWhiteSpace(candlePath) || !File.Exists(candlePath))
                throw new ArgumentException("Invalid path to candle.exe.", nameof(candlePath));

            if (string.IsNullOrWhiteSpace(lightPath) || !File.Exists(lightPath))
                throw new ArgumentException("Invalid path to light.exe.", nameof(lightPath));

            CandlePath = candlePath;
            LightPath = lightPath;
        }

        /// <summary>
        /// Compiles a WiX source file into a WiX object file (.wixobj).
        /// </summary>
        /// <param name="wixFilePath">The path to the WiX source file.</param>
        /// <returns>The path to the generated .wixobj file.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the WiX file is not found.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the .wixobj file is not generated.</exception>
        public string CompileWixFile(string wixFilePath)
        {
            if (string.IsNullOrWhiteSpace(wixFilePath) || !File.Exists(wixFilePath))
                throw new FileNotFoundException("WiX file not found.", wixFilePath);

            string wixObjPath = Path.ChangeExtension(wixFilePath, ".wixobj");

            // Run the Candle tool to compile the WiX file
            RunProcess(CandlePath, string.Format("\"{0}\" -out \"{1}\"", wixFilePath, wixObjPath));

            if (!File.Exists(wixObjPath))
                throw new InvalidOperationException(string.Format("Failed to generate wixobj from {0}.", wixFilePath));

            return wixObjPath;
        }

        /// <summary>
        /// Creates an MSI file from a WiX object file (.wixobj).
        /// </summary>
        /// <param name="wixObjPath">The path to the WiX object file (.wixobj).</param>
        /// <param name="outputMsiPath">The desired output path for the MSI file. If null, the MSI file will be created in the same location as the .wixobj file.</param>
        /// <returns>The path to the generated MSI file.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the WiX object file is not found.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the MSI file is not generated.</exception>
        public string BuildMsi(string wixObjPath, string outputMsiPath = null)
        {
            if (string.IsNullOrWhiteSpace(wixObjPath) || !File.Exists(wixObjPath))
                throw new FileNotFoundException("WiX object file not found.", wixObjPath);

            if (string.IsNullOrWhiteSpace(outputMsiPath))
            {
                outputMsiPath = Path.ChangeExtension(wixObjPath, ".msi");
            }

            // Run the Light tool to create the MSI from the .wixobj file
            RunProcess(LightPath, string.Format("\"{0}\" -out \"{1}\"", wixObjPath, outputMsiPath));

            if (!File.Exists(outputMsiPath))
                throw new InvalidOperationException(string.Format("Failed to generate MSI from {0}.", wixObjPath));

            return outputMsiPath;
        }

        /// <summary>
        /// Executes a process with the specified executable and arguments, capturing output and error messages.
        /// </summary>
        /// <param name="executable">The path to the executable.</param>
        /// <param name="arguments">The arguments to pass to the executable.</param>
        /// <exception cref="InvalidOperationException">Thrown if the process fails with a non-zero exit code.</exception>
        private static void RunProcess(string executable, string arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = executable,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException(string.Format("Process {0} failed with exit code {1}.\nError: {2}", executable, process.ExitCode, stderr));
            }

            Console.WriteLine(stdout); // Optionally log the output

            // Dispose process manually as .NET Framework 4 doesn't support 'using' for Process
            process.Close();
        }
    }
}
