using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify;

/// <summary>
/// Specifies the repeat mode for playback.
/// </summary>
public enum RepeatMode
{
    /// <summary>
    /// No repeat mode.
    /// </summary>
    None,

    /// <summary>
    /// Repeat the current item.
    /// </summary>
    One
}