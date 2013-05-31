using System.Collections.Generic;

namespace SharedTypes
{
    public interface ApplicationWindow
    {
        System.Windows.Threading.Dispatcher WindowDispatcher { get; }

        void SetupNavigation(Dictionary<PageChangeDirection, string> navigation);

        void SetWindowFade(bool fade);

        void TryTransition(PageChange change, PageTransitionType transition = PageTransitionType.Fade);

        void TryTransition(PageChangeDirection direction, PageTransitionType transition = PageTransitionType.Fade);

        void ShowNotificationMessage(string message, System.TimeSpan duration);
    }
}