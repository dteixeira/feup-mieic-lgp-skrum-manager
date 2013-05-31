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

    /// <summary>
    /// Represents the type of possible page transitions.
    /// </summary>
    public enum PageTransitionType
    {
        SlideRight,
        SlideLeft,
        SlideUp,
        SlideDown,
        SlideAndFade,
        Fade,
        Grow,
        GrowAndFade,
        Flip,
        FlipAndFade,
        Spin,
        SpinAndFade
    }
}