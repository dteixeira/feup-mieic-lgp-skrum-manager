namespace SharedTypes
{
    public interface ITargetPage
    {
        ApplicationPages PageType { get; set; }
        SharedTypes.ApplicationController.DataModificationHandler DataChangeDelegate { get; set; }

        PageChange PageChangeTarget(PageChangeDirection direction);
        void SetupNavigation();
        void UnloadPage();
        void DataChangeHandler(object sender, ServiceLib.NotificationService.NotificationType notification);
    }
}