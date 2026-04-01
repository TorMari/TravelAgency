using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebProjectServ.Models;

public class ChatHub : Hub
{
   
    private readonly MyDataContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatHub(MyDataContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public class FileUploadMetadata
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
    }
    public async Task SendPrivate(string receiverId, string message, FileUploadMetadata? fileData)
    {
        var senderId = Context.UserIdentifier;
        var senderName = Context.User?.Identity?.Name;

        string? fileName = fileData?.FileName;
        string? fileUrl = fileData?.FileUrl;

        var msg = new Message
        {
            SenderId = senderId,
            SenderName = senderName,
            ReceiverId = receiverId,
            Text = string.IsNullOrWhiteSpace(message) ? null : message,
            SentAt = DateTime.UtcNow, 
            FileName = fileName,
            FileUrl = fileUrl, 
        };

       
        if (string.IsNullOrEmpty(msg.Text) && string.IsNullOrEmpty(msg.FileName))
            return;

        _context.Messages.Add(msg);
        await _context.SaveChangesAsync();

        await Clients.User(receiverId).SendAsync("ReceivePrivate", senderId, senderName, message, fileName, fileUrl, msg.SentAt);

        await Clients.Caller.SendAsync("ReceivePrivate", senderId, senderName, message, fileName, fileUrl, msg.SentAt);
    }


    public async Task JoinTour(int tourId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "tour_" + tourId);
    }

    public async Task SendToTour(int tourId, string message, FileUploadMetadata? fileData)
    {
        var senderId = Context.UserIdentifier;
        var senderName = Context.User?.Identity?.Name;

        string? fileName = fileData?.FileName;
        string? fileUrl = fileData?.FileUrl;

        var msg = new Message
        {
            SenderId = senderId,
            SenderName = senderName,
            TourId = tourId,
            Text = string.IsNullOrWhiteSpace(message) ? null : message,
            SentAt = DateTime.UtcNow,
            FileName = fileName,
            FileUrl = fileUrl,
        };

        if (string.IsNullOrEmpty(msg.Text) && string.IsNullOrEmpty(msg.FileName))
            return;

        _context.Messages.Add(msg);
        await _context.SaveChangesAsync();

        await Clients.Group("tour_" + tourId)
            .SendAsync("ReceiveTour", senderName, message, fileName, fileUrl, msg.SentAt);
    }
}



