using System;
using System.Collections.Generic;
using System.Text;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class Grid : ISyncable
    {
        private const int ColumnSizeOnLog = 18;


        private GridColumn[] _columns = new GridColumn[8];
        private string[] _names = { };
        private int _numberOfColumns = 0;
        private int _imageColumnIndex = 0;

        public Grid()
        {
            FillEmptyColumns(_columns, 0, _columns.Length);
        }

        private void FillEmptyColumns(GridColumn[] array, int offset, int length)
        {
            for (var i = 0; i < length; i++)
            {
                array[i + offset] = new GridColumn();
            }
        }

        public IColumn GetColumn(int i)
        {
            if (i >= _numberOfColumns)
            {
                throw new IndexOutOfRangeException(i + " is greater than " + (_numberOfColumns - 1));
            }
            return _columns[i];
        }

        public int NumberOfColumns
        {
            get { return _numberOfColumns; }
        }

        public string[] ColumnNames
        {
            get { return _names; }
        }

        public void PriceImage(int sid, Dictionary<string, object> price)
        {
            // ignore
        }

        public void PriceUpdate(int sid, Dictionary<string, object> price)
        {
            // ignore
        }

        public void PriceStatus(int sid, SubscriptionStatus status, string explanation)
        {
            // ignore
        }

        public void StartGridImage(int sid, int columnCount)
        {
            if (_columns.Length < columnCount)
            {
                ResizeColumns(columnCount);
            }
            _imageColumnIndex = 0;
            _names = new string[columnCount];
            _numberOfColumns = columnCount;
        }

        private void ResizeColumns(int newSize)
        {
            var newLength = _columns.Length;
            while (newLength < newSize)
            {
                newLength *= 2;
            }

            DoResizeColumns(newLength);
        }

        private void DoResizeColumns(int newLength)
        {
            var newColumns = new GridColumn[newLength];
            Array.Copy(_columns, 0, newColumns, 0, _columns.Length);
            FillEmptyColumns(newColumns, _columns.Length, newColumns.Length - _columns.Length);
            _columns = newColumns;
        }

        public void ColumnImage(string name, int rowCount, object[] columnValues)
        {
            _names[_imageColumnIndex] = name;
            _columns[_imageColumnIndex].SetImage(columnValues, rowCount);
            _imageColumnIndex++;
        }

        public void EndGridImage()
        {
            // nothing to do here
        }

        public void StartGridUpdate(int sid, int columnCount)
        {
            //nothing to do here
        }

        public void FullColumnUpdate(string name, int cid, int rowCount, object[] columnValues)
        {
            if (cid >= _numberOfColumns)
            {
                throw new IllegalStateException(
                    "received column update with cid greater than current number of columns: " + cid + ", " +
                    _numberOfColumns);
            }
            _columns[cid].SetImage(columnValues, rowCount);
        }

        public void PartialColumnUpdate(string name, int cid, int rowCount, object[] columnValues, int[] rids)
        {
            UpdateColumn(cid, rowCount, columnValues, rids);
        }

        public void PartialTruncatedColumnUpdate(string name, int cid, int rowCount, object[] columnValues, int[] rids,
            int truncatedRid)
        {
            var gridColumn = UpdateColumn(cid, rowCount, columnValues, rids);
            gridColumn.DeleteFrom(truncatedRid);
        }

        private GridColumn UpdateColumn(int cid, int rowCount, object[] columnValues, int[] rids)
        {
            if (cid >= _numberOfColumns)
            {
                throw new IllegalStateException(
                    "received column update with cid greater than current number of columns: " + cid + ", " +
                    _numberOfColumns);
            }
            var gridColumn = _columns[cid];
            for (var i = 0; i < rowCount; i++)
            {
                gridColumn.Set(rids[i], columnValues[i]);
            }
            return gridColumn;
        }

        public void EndGridUpdate()
        {
            // Nothing to do here
        }
    }
}