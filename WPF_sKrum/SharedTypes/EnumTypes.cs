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
        sKrum,
        MainPage,
        ProjectsPage,
        UsersPage,
        TaskBoardPage,
        UserStatsPage
    }

    public enum PageChangeDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}
