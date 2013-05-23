using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedTypes
{
    public interface ITargetPage
    {
        ApplicationPages PageType { get; set; }
        PageChange PageChangeTarget(PageChangeDirection direction);
        void SetupNavigation();
        void UnloadPage();
    }
}
