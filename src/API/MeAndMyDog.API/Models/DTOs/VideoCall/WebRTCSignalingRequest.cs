namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// Request object for WebRTC signaling
/// </summary>
public class WebRTCSignalingRequest
{
    /// <summary>
    /// ID of the video call session
    /// </summary>
    public string CallId { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of signaling message (offer, answer, ice-candidate)
    /// </summary>
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// SDP offer or answer payload
    /// </summary>
    public string? Sdp { get; set; }
    
    /// <summary>
    /// ICE candidate data
    /// </summary>
    public object? Candidate { get; set; }
    
    /// <summary>
    /// Target participant ID for the signaling message
    /// </summary>
    public string? TargetParticipantId { get; set; }
}