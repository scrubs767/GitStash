using System;
using Microsoft.TeamFoundation.Controls;

namespace TeamExplorer.Common
{
    public interface ITeamExplorerBase
    {
        Guid ShowNotification(string message, NotificationType type);
    }
}