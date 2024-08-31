namespace Amphasis.Azure.Common.Pages.Shared;

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
			ArgumentOutOfRangeException.ThrowIfLessThan(value, FirstPage, nameof(CurrentPage));
			_currentPage = value;
		}
	}

	public int FirstPage
	{
		get => _firstPage;
		set
		{
			ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(FirstPage));
			ArgumentOutOfRangeException.ThrowIfGreaterThan(value, CurrentPage, nameof(FirstPage));
			_firstPage = value;
		}
	}

	public int NavigationSpread
	{
		get => _navigationSpread;
		set
		{
			ArgumentOutOfRangeException.ThrowIfLessThan(value, 1, nameof(NavigationSpread));
			_navigationSpread = value;
		}
	}
}