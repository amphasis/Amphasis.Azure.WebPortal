using System;

namespace Amphasis.Azure.WebPortal.Pages.Shared
{
    public class PageNavigationPanelModel
    {
        private int _currentPage;
        private int _firstPage;
        private int _navigationSpread;

        public PageNavigationPanelModel(int currentPage)
        {
            _firstPage = 1;
            _navigationSpread = 5;
            CurrentPage = currentPage;
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (value < FirstPage) throw new ArgumentOutOfRangeException();
                _currentPage = value;
            }
        }

        public int FirstPage
        {
            get => _firstPage;
            set
            {
                if (value < 0 || value > CurrentPage) throw new ArgumentOutOfRangeException();
                _firstPage = value;
            }
        }

        public int NavigationSpread
        {
            get => _navigationSpread;
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException();
                _navigationSpread = value;
            }
        }
    }
}