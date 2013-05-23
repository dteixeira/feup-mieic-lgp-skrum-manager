using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedTypes
{
    /// <summary>
    /// Represents the application's pages.
    /// </summary>
    public enum ApplicationPages
    {
        RootPage,
        MainPage,
        TaskBoardPage,
        ProjectConfigurationPage,
        ProjectTeamManagementPage,
        ProjectManagementPage,
        PeopleManagementPage,
        BacklogPage,
        ProjectBacklogPage,
        MeetingPage,
        ProjectStatisticsPage,
        PersonStatisticsPage,
        PersonTaskBoardPage
    }

    public enum PageChangeDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}
