using System;
using System.Collections.Generic;
using FileExplorer.Models;
using FileExplorer.ViewModels;

namespace FileExplorer.Converters.Comparers
{
    public class FileInfoViewModelComparer: IComparer<FileInfoViewModel>
    {
        private readonly SortBy _sortBy;
        private readonly Direction _direction;

        public FileInfoViewModelComparer(SortBy sortBy, Direction direction)
        {
            _sortBy = sortBy;
            _direction = direction;
        }

        public int Compare(FileInfoViewModel x, FileInfoViewModel y)
        {
            int result;
            switch (_sortBy)
            {
                case SortBy.Name:
                    result = string.Compare(x.Caption, y.Caption);
                    break;
                case SortBy.Extension:
                    result = string.Compare(x.Extension, y.Extension);
                    break;
                case SortBy.Date:
                    result = DateTime.Compare(x.LastWriteTime, y.LastWriteTime);
                    break;
                case SortBy.Size:
                    result = x.Size.CompareTo(y.Size);
                    break;
                default:
                    result = 0;
                    break;
            }
            return _direction == Direction.Ascending ? result : -result;
        }
    }
}