using CommunityToolkit.Mvvm.ComponentModel;

namespace FileExplorer.Sorting
{
    public class SortingOptions: ObservableRecipient
    {
        private SortBy _sortBy;
        private Direction _direction;

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
}