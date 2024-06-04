using CommunityToolkit.Mvvm.ComponentModel;

namespace FileExplorer.Models;

public class SortingOptions : ObservableRecipient
{
    private Direction _direction;
    private SortBy _sortBy;

    public SortBy SortBy
    {
        get => _sortBy;
        set
        {
            SetProperty(ref _sortBy, value);
            OnPropertyChanged();
        }
    }

    public Direction Direction
    {
        get => _direction;
        set
        {
            SetProperty(ref _direction, value);
            OnPropertyChanged();
        }
    }
}