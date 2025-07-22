using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeAndMyDog.WebApp.Controllers;

/// <summary>
/// Controller for managing the messaging interface pages
/// </summary>
[Authorize]
public class MessagesController : Controller
{
    private readonly ILogger<MessagesController> _logger;

    /// <summary>
    /// Initialize the messages controller
    /// </summary>
    public MessagesController(ILogger<MessagesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Display the main messaging interface
    /// </summary>
    /// <returns>Messages index view</returns>
    public IActionResult Index()
    {
        ViewData["Title"] = "Messages";
        ViewData["Description"] = "Send and receive messages with service providers and other pet owners.";
        
        return View();
    }

    /// <summary>
    /// Display a specific conversation
    /// </summary>
    /// <param name="conversationId">ID of the conversation to display</param>
    /// <returns>Messages view with selected conversation</returns>
    public IActionResult Conversation(string conversationId)
    {
        if (string.IsNullOrEmpty(conversationId))
        {
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = "Messages - Conversation";
        ViewData["ConversationId"] = conversationId;
        
        return View("Index");
    }

    /// <summary>
    /// Start a new conversation with a specific user
    /// </summary>
    /// <param name="userId">ID of the user to start conversation with</param>
    /// <returns>Redirects to messages with new conversation modal</returns>
    public IActionResult StartConversation(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = "Messages - New Conversation";
        ViewData["StartWithUserId"] = userId;
        
        return View("Index");
    }
}