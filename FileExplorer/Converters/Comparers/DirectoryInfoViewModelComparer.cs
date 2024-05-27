using System;
using System.Collections.Generic;
using FileExplorer.Models;
using FileExplorer.ViewModels;

namespace FileExplorer.Converters.Comparers
{
    public class DirectoryInfoViewModelComparer: IComparer<DirectoryInfoViewModel>
    {
        private readonly SortBy _sortBy;
        private readonly Direction _direction;

        public DirectoryInfoViewModelComparer(SortBy sortBy, Direction direction)
        {
            _sortBy = sortBy;
            _direction = direction;
        }

        public int Compare(DirectoryInfoViewModel x, DirectoryInfoViewModel y)
        {
            int result;
            switch (_sortBy)
            {
                case SortBy.Name:
                    result = string.Compare(x.Caption, y.Caption);
                    break;
                case SortBy.Date:
                    result = DateTime.Compare(x.LastWriteTime, y.LastWriteTime);
                    break;
                case SortBy.Size:
                    result = x.Count.CompareTo(y.Count);
                    break;
                default:
                    result = 0;
                    break;
            }
            return _direction == Direction.Ascending ? result : -result;
        }
    }
}