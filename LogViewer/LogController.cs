using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;

namespace LogViewer.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class LogController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public LogController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpGet("directories")]
		public IActionResult GetLogDirectories()
		{
			try
			{
				var logPaths = _configuration.GetSection("LogPaths").Get<string[]>();
				return Ok(logPaths);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpGet("files")]
		public IActionResult GetLogFiles([FromQuery] string directoryPath)
		{
			try
			{
				if (!Directory.Exists(directoryPath))
				{
					return NotFound($"Directory not found: {directoryPath}");
				}

				var logFiles = Directory.GetFiles(directoryPath, "*.log")
					.Select(file => new
					{
						Name = Path.GetFileName(file),
						Path = file,
						LastModified = System.IO.File.GetLastWriteTime(file)
					})
					.OrderByDescending(f => f.LastModified)
					.ToList();

				return Ok(logFiles);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpGet("content")]
		public IActionResult GetLogFileContent([FromQuery] string filePath, [FromQuery] int lines = 100)
		{
			try
			{
				if (!System.IO.File.Exists(filePath))
				{
					return NotFound($"File not found: {filePath}");
				}

				var content = ReadFirstLines(filePath, lines);
				return Ok(new { Content = content });
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		private IEnumerable<string> ReadFirstLines(string filePath, int lines)
		{
			var result = new List<string>();
			using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (var reader = new StreamReader(stream))
			{
				string line;
				int count = 0;

				while ((line = reader.ReadLine()) != null)
				{
					result.Add(line);
					count++;
					if (count >= lines) break;
				}
			}
			return result;
		}
	}
}
