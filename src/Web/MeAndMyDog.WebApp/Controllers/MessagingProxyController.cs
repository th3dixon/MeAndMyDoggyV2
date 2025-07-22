using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace MeAndMyDog.WebApp.Controllers
{
    /// <summary>
    /// Messaging Proxy Controller - forwards messaging requests to the backend API server
    /// </summary>
    [ApiController]
    [Route("api/v1/messaging")]
    [Authorize]
    public class MessagingProxyController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MessagingProxyController> _logger;

        public MessagingProxyController(IHttpClientFactory httpClientFactory, ILogger<MessagingProxyController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Proxy get conversations requests to the API server
        /// </summary>
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            return await ProxyGetRequest("messaging/conversations", Request.QueryString.Value);
        }

        /// <summary>
        /// Proxy get conversation messages requests to the API server
        /// </summary>
        [HttpGet("conversations/{conversationId}/messages")]
        public async Task<IActionResult> GetConversationMessages(string conversationId)
        {
            return await ProxyGetRequest($"messaging/conversations/{conversationId}/messages", Request.QueryString.Value);
        }

        /// <summary>
        /// Proxy send message requests to the API server
        /// </summary>
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] object messageData)
        {
            return await ProxyRequest("messaging/send", messageData);
        }

        /// <summary>
        /// Proxy upload file requests to the API server
        /// </summary>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile()
        {
            return await ProxyFileUpload("messaging/upload");
        }

        /// <summary>
        /// Proxy message reactions requests to the API server
        /// </summary>
        [HttpPost("messages/{messageId}/reactions")]
        public async Task<IActionResult> ToggleReaction(string messageId, [FromBody] object reactionData)
        {
            return await ProxyRequest($"messaging/messages/{messageId}/reactions", reactionData);
        }

        /// <summary>
        /// Proxy conversation actions (pin, mute, archive) to the API server
        /// </summary>
        [HttpPut("conversations/{conversationId}/pin")]
        public async Task<IActionResult> TogglePin(string conversationId, [FromBody] object pinData)
        {
            return await ProxyRequest($"conversations/{conversationId}/pin", pinData);
        }

        [HttpPut("conversations/{conversationId}/mute")]
        public async Task<IActionResult> ToggleMute(string conversationId, [FromBody] object muteData)
        {
            return await ProxyRequest($"conversations/{conversationId}/mute", muteData);
        }

        [HttpPut("conversations/{conversationId}/archive")]
        public async Task<IActionResult> ToggleArchive(string conversationId, [FromBody] object archiveData)
        {
            return await ProxyRequest($"conversations/{conversationId}/archive", archiveData);
        }

        /// <summary>
        /// Proxy create conversation requests to the API server
        /// </summary>
        [HttpPost("conversations")]
        public async Task<IActionResult> CreateConversation([FromBody] object conversationData)
        {
            return await ProxyRequest("conversations", conversationData);
        }

        /// <summary>
        /// Proxy user search requests to the API server
        /// </summary>
        [HttpGet("users/search")]
        public async Task<IActionResult> SearchUsers()
        {
            return await ProxyGetRequest("users/search", Request.QueryString.Value);
        }

        private async Task<IActionResult> ProxyGetRequest(string endpoint, string? queryString = null)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("API");
                var targetUrl = $"api/v1/{endpoint}";
                if (!string.IsNullOrEmpty(queryString))
                {
                    targetUrl += queryString;
                }

                _logger.LogInformation("Proxying GET request to: {TargetUrl}", targetUrl);

                // Add JWT token from user claims for API authentication
                var jwtToken = User.FindFirst("jwt_token")?.Value;
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
                }

                // Send the GET request
                var response = await httpClient.GetAsync(targetUrl);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("API response: {StatusCode} for {TargetUrl}", response.StatusCode, targetUrl);

                // Return the response with the same status code and content
                return new ContentResult
                {
                    Content = responseContent,
                    ContentType = "application/json",
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while proxying GET request to API server");
                return StatusCode(503, new { message = "Unable to connect to messaging service. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error proxying GET messaging request to API");
                return StatusCode(500, new { message = "Internal server error while processing messaging request" });
            }
        }

        private async Task<IActionResult> ProxyRequest(string endpoint, object data)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("API");
                var targetUrl = $"api/v1/{endpoint}";

                _logger.LogInformation("Proxying POST/PUT request to: {TargetUrl}", targetUrl);

                // Serialize the data
                var json = System.Text.Json.JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Add JWT token from user claims for API authentication
                var jwtToken = User.FindFirst("jwt_token")?.Value;
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
                }

                // Determine HTTP method based on the request
                HttpResponseMessage response;
                if (Request.Method == "PUT")
                {
                    response = await httpClient.PutAsync(targetUrl, content);
                }
                else
                {
                    response = await httpClient.PostAsync(targetUrl, content);
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("API response: {StatusCode} for {TargetUrl}", response.StatusCode, targetUrl);

                // Return the response with the same status code and content
                return new ContentResult
                {
                    Content = responseContent,
                    ContentType = "application/json",
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while proxying request to API server");
                return StatusCode(503, new { message = "Unable to connect to messaging service. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error proxying messaging request to API");
                return StatusCode(500, new { message = "Internal server error while processing messaging request" });
            }
        }

        private async Task<IActionResult> ProxyFileUpload(string endpoint)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("API");
                var targetUrl = $"api/v1/{endpoint}";

                _logger.LogInformation("Proxying file upload request to: {TargetUrl}", targetUrl);

                // Add JWT token from user claims for API authentication
                var jwtToken = User.FindFirst("jwt_token")?.Value;
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
                }

                // Create multipart content from the incoming request
                using var content = new MultipartFormDataContent();
                if (Request.HasFormContentType)
                {
                    foreach (var file in Request.Form.Files)
                    {
                        var fileContent = new StreamContent(file.OpenReadStream());
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
                        content.Add(fileContent, file.Name, file.FileName);
                    }

                    foreach (var formField in Request.Form)
                    {
                        content.Add(new StringContent(formField.Value), formField.Key);
                    }
                }

                // Send the request
                var response = await httpClient.PostAsync(targetUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("API response: {StatusCode} for {TargetUrl}", response.StatusCode, targetUrl);

                // Return the response with the same status code and content
                return new ContentResult
                {
                    Content = responseContent,
                    ContentType = "application/json",
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while proxying file upload request to API server");
                return StatusCode(503, new { message = "Unable to connect to messaging service. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error proxying file upload request to API");
                return StatusCode(500, new { message = "Internal server error while processing file upload request" });
            }
        }
    }
}