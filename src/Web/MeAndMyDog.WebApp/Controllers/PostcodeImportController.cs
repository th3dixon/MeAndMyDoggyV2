using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace MeAndMyDog.WebApp.Controllers;

/// <summary>
/// Temporary controller for importing postcode data from chunked SQL files
/// </summary>
public class PostcodeImportController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PostcodeImportController> _logger;

    /// <summary>
    /// Initializes a new instance of the PostcodeImportController
    /// </summary>
    /// <param name="configuration">Application configuration</param>
    /// <param name="logger">Logger instance</param>
    public PostcodeImportController(IConfiguration configuration, ILogger<PostcodeImportController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Display the postcode import page
    /// </summary>
    /// <returns>Import view</returns>
    public IActionResult Import()
    {
        return View();
    }

    /// <summary>
    /// Execute the postcode import process using stored procedures
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <param name="filePath">Path to the PostcodeChunks directory</param>
    /// <param name="username">SQL Server username (optional, uses Windows auth if not provided)</param>
    /// <param name="password">SQL Server password (optional, uses Windows auth if not provided)</param>
    /// <returns>JSON result with import status</returns>
    [HttpPost]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3003:Review code for file path injection vulnerabilities", Justification = "This is a temporary administrative controller with controlled file paths")]
    public async Task<IActionResult> ExecuteImport(string connectionString, string filePath = @"C:\temp\PostcodeChunks", string username = null, string password = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return Json(new { success = false, message = "Connection string is required" });
            }

            if (!Directory.Exists(filePath))
            {
                return Json(new { success = false, message = $"Directory not found: {filePath}" });
            }

            var results = new List<object>();
            int successCount = 0;
            int errorCount = 0;
            const int totalChunks = 106;

            _logger.LogInformation("Starting postcode import by executing SQL files directly from {FilePath}", filePath);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            for (int i = 1; i <= totalChunks; i++)
            {
                var chunkFileName = $"PostcodeInserts_{i:000}.sql";
                var chunkFilePath = Path.Combine(filePath, chunkFileName);

                if (!System.IO.File.Exists(chunkFilePath))
                {
                    var errorMsg = $"Chunk file not found: {chunkFileName}";
                    _logger.LogWarning(errorMsg);
                    results.Add(new { chunk = i, status = "error", message = errorMsg });
                    errorCount++;
                    continue;
                }

                try
                {
                    var startTime = DateTime.Now;
                    _logger.LogInformation("Executing chunk {ChunkNumber}/{TotalChunks}: {FileName}", i, totalChunks, chunkFileName);

                    // Build SQLCMD authentication arguments
                    string authArgs;
                    if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
                    {
                        authArgs = $"-U \"{username}\" -P \"{password}\"";
                    }
                    else
                    {
                        authArgs = "-E"; // Windows Authentication
                    }

                    // Use SQLCMD to execute the file directly
                    var sqlcmdArgs = $"-S \"{connection.DataSource}\" -d \"{connection.Database}\" {authArgs} -i \"{chunkFilePath}\" -b -V1";
                    
                    var processInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "sqlcmd",
                        Arguments = sqlcmdArgs,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };

                    using var process = System.Diagnostics.Process.Start(processInfo);
                    await process.WaitForExitAsync();
                    
                    var output = await process.StandardOutput.ReadToEndAsync();
                    var error = await process.StandardError.ReadToEndAsync();
                    var duration = DateTime.Now - startTime;

                    if (process.ExitCode == 0)
                    {
                        var successMsg = $"Chunk {i} completed successfully in {duration.TotalSeconds:F1} seconds";
                        _logger.LogInformation(successMsg);
                        
                        results.Add(new { 
                            chunk = i, 
                            status = "success", 
                            message = successMsg,
                            duration = duration.TotalSeconds,
                            output = output.Trim()
                        });
                        successCount++;
                    }
                    else
                    {
                        var errorMsg = $"Chunk {i} failed with exit code {process.ExitCode}: {error}";
                        _logger.LogError("Error executing chunk {ChunkNumber}: {Error}", i, error);
                        results.Add(new { chunk = i, status = "error", message = errorMsg });
                        errorCount++;
                    }
                }
                catch (Exception ex)
                {
                    var errorMsg = $"Chunk {i} failed: {ex.Message}";
                    _logger.LogError(ex, "Error executing chunk {ChunkNumber}", i);
                    results.Add(new { chunk = i, status = "error", message = errorMsg });
                    errorCount++;
                }
            }

            // Final verification
            var totalRecords = 0;
            try
            {
                using var connection1 = new SqlConnection(connectionString);
                await connection1.OpenAsync();
                using var command = new SqlCommand("SELECT COUNT(*) FROM [dbo].[PostcodeImportStaging]", connection1);
                totalRecords = (int)await command.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not verify total record count");
            }

            var summary = new
            {
                success = errorCount == 0,
                totalChunks = totalChunks,
                successCount = successCount,
                errorCount = errorCount,
                totalRecords = totalRecords,
                results = results
            };

            _logger.LogInformation("Postcode import completed. Success: {SuccessCount}, Errors: {ErrorCount}, Total Records: {TotalRecords}", 
                successCount, errorCount, totalRecords);

            return Json(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error during postcode import");
            return Json(new { success = false, message = $"Fatal error: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get the status of a postcode import (for progress tracking)
    /// </summary>
    /// <returns>Current import status</returns>
    [HttpGet]
    public IActionResult GetImportStatus()
    {
        // This could be enhanced with real-time progress tracking using SignalR
        return Json(new { status = "Not implemented yet" });
    }
}