using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Models.DTOs;

/// <summary>
/// Data Transfer Object for Notifications.
/// </summary>
public class NotificationDTO : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the message of the notification.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the ID of the artist associated with the notification.
    /// </summary>
    public string ArtistId { get; set; }

    /// <summary>
    /// Gets or sets the name of the artist associated with the notification.
    /// </summary>
    public string ArtistName { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the notification.
    /// </summary>
    public DateTime Timestamp { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}

