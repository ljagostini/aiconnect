using System.Collections.Generic;

namespace Percolore.Core
{
    public class AppPercolore
    {
        List<AppIdentifier> _appList;

        public List<AppIdentifier> AppList
        {
            get { return _appList; }
        }

        public AppPercolore()
        {
            AppIdentifier appIoconnect =
                new AppIdentifier("IOConnect", "7f57aded-e441-4e3c-913b-af955258d361");

            AppIdentifier appIocolore =
                new AppIdentifier("IOColore", "920d99b3-2221-4617-862f-ecb2b6a7cc77");

            _appList = new List<AppIdentifier>();
            _appList.Add(appIoconnect);
            _appList.Add(appIocolore);
        }
    }
}