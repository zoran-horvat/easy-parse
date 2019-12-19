using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace EasyParse
{
    class TableFormat<TRow, TColumn>
    {
        private int ColumnHeadersLine { get; }
        private int MinimumColumnWidth { get; }
        private ImmutableList<TRow> RowHeaders { get; }
        private ImmutableList<TColumn> ColumnHeaders { get; }
        private ImmutableDictionary<(int row, int column), (string value, int span)> Content { get; }

        public TableFormat()
            : this(0, 5, ImmutableList<TRow>.Empty, ImmutableList<TColumn>.Empty, 
                    ImmutableDictionary<(int row, int column), (string value, int span)>.Empty)
        {
        }

        private TableFormat(
            int columnHeadersLine, int minimumColumnWidth,
            ImmutableList<TRow> rowHeaders, ImmutableList<TColumn> colHeaders,
            ImmutableDictionary<(int row, int column), (string value, int span)> content)
        {
            this.ColumnHeadersLine = columnHeadersLine;
            this.MinimumColumnWidth = minimumColumnWidth;
            this.RowHeaders = rowHeaders;
            this.ColumnHeaders = colHeaders;
            this.Content = content;
        }

        public TableFormat<TRow, TColumn> AddRows(IEnumerable<TRow> rowHeaders) =>
            new TableFormat<TRow, TColumn>(
                this.ColumnHeadersLine, this.MinimumColumnWidth,
                this.RowHeaders.AddRange(rowHeaders), 
                this.ColumnHeaders,
                this.Content.AddRange(rowHeaders.Select((header, index) => 
                    new KeyValuePair<(int row, int column), (string value, int span)>((index + this.ColumnHeadersLine + 1, 0), (header.ToString(), 1)))));

        public TableFormat<TRow, TColumn> AddColumns(IEnumerable<TColumn> columnHeaders) =>
            new TableFormat<TRow, TColumn>(
                this.ColumnHeadersLine, this.MinimumColumnWidth,
                this.RowHeaders, 
                this.ColumnHeaders.AddRange(columnHeaders),
                this.Content.AddRange(columnHeaders.Select((header, index) =>
                    new KeyValuePair<(int row, int column), (string value, int span)>((this.ColumnHeadersLine, index + this.ColumnHeaders.Count + 1), (header.ToString(), 1)))));

        public TableFormat<TRow, TColumn> AddHeader(params (string label, int span)[] labels)
        {
            List<(int row, int column, string value, int span)> header = new List<(int row, int column, string value, int span)>();
            int position = 0;

            foreach ((string label, int span) label in labels)
            {
                header.Add((0, position, label.label, label.span));
                position += label.span;
            }

            return this.AddHeader(header);
        }

        private TableFormat<TRow, TColumn> AddHeader(IEnumerable<(int row, int column, string value, int span)> labels) =>
            new TableFormat<TRow, TColumn>(
                this.ColumnHeadersLine + 1, this.MinimumColumnWidth,
                this.RowHeaders,
                this.ColumnHeaders,
                this.Content
                    .Select(pair => new KeyValuePair<(int, int), (string, int)>((pair.Key.row + 1, pair.Key.column), pair.Value))
                    .Concat(labels.Select(label => 
                        new KeyValuePair<(int, int), (string, int)>((label.row, label.column), (label.value, label.span))))
                    .ToImmutableDictionary());

        public TableFormat<TRow, TColumn> AddContent(IEnumerable<(TRow row, TColumn column, string value)> values) =>
            this.AddContent(values.Select(value => (this.RowIndex(value.row), this.ColumnIndex(value.column), value.value)));

        public TableFormat<TRow, TColumn> AddContent(TRow row, TColumn column, string value) =>
            this.AddContent(new[] {(row, column, value)});

        private int RowIndex(TRow row) =>
            Array.IndexOf(this.RowHeaders.ToArray(), row) + this.ColumnHeadersLine + 1;

        private int ColumnIndex(TColumn column) =>
            Array.IndexOf(this.ColumnHeaders.ToArray(), column) + 1;

        private TableFormat<TRow, TColumn> AddContent(IEnumerable<(int row, int column, string value)> values) =>
            new TableFormat<TRow, TColumn>(
                this.ColumnHeadersLine, this.MinimumColumnWidth,
                this.RowHeaders,
                this.ColumnHeaders,
                this.Content.AddRange(values.Select(tuple => new KeyValuePair<(int, int), (string, int)>((tuple.row, tuple.column), (tuple.value, 1)))));

        public override string ToString() =>
            string.Join(Environment.NewLine, this.Lines());

        private IEnumerable<string> Lines() =>
            this.Lines(this.ColumnWidths());

        private IEnumerable<string> Lines(IDictionary<int, int> columnWidths) =>
            this.RowIndexes
                .SelectMany(row => new[] {this.RowDelimiter(row, columnWidths), this.RowToString(row, columnWidths)})
                .Concat(new[] {this.RowDelimiter(this.RowsCount - 1, columnWidths)});

        private string RowDelimiter(int rowIndex, IDictionary<int, int> columnWidths)
        {
            StringBuilder row = new StringBuilder("+");
            int position = 0;
            int width = this.ColumnsCount;

            while (position < width)
            {
                (string _, int span) = this.CellContent(rowIndex, position);
                int spanWidth = Enumerable.Range(position, span).Sum(col => columnWidths[col]) + span - 1;
                row.Append(new string('-', spanWidth));
                row.Append("+");
                position += span;
            }

            return row.ToString();
        }

        private string RowToString(int rowIndex, IDictionary<int, int> columnWidths)
        {
            StringBuilder row = new StringBuilder("|");
            int position = 0;
            int width = this.ColumnsCount;
            
            while (position < width)
            {
                (string value, int span) = this.CellContent(rowIndex, position);
                row.Append(this.Center(value, position, span, columnWidths));
                row.Append("|");
                position += span;
            }

            return row.ToString();
        }

        private string Center(string content, int columnIndex, int span, IDictionary<int, int> columnWidths) =>
            this.Center(content, Enumerable.Range(columnIndex, span).Sum(col => columnWidths[col]) + span - 1);

        private string Center(string content, int width) =>
            this.CenterCapped(content.Substring(0, Math.Min(content.Length, width)), width);

        private string CenterCapped(string content, int width) =>
            this.CenterCapped(content, (width - content.Length) / 2, width);

        private string CenterCapped(string content, int leftPad, int width) =>
            content.PadLeft(content.Length + leftPad).PadRight(width);

        private (string value, int span) CellContent(int row, int col) =>
            this.Content.TryGetValue((row, col), out (string value, int span) tuple) ? tuple : (string.Empty, 1);

        private IEnumerable<int> RowIndexes => 
            Enumerable.Range(0, this.RowsCount);

        private IDictionary<int, int> ColumnWidths() =>
            this.ColumnWidths(this.RawColumnWidths);

        private IDictionary<int, int> RawColumnWidths =>
            this.Content
                .Where(item => item.Value.span == 1)
                .Select(item => (column: item.Key.column, width: item.Value.value.Length))
                .GroupBy(tuple => tuple.column, tuple => tuple.width)
                .ToDictionary(group => group.Key, group => group.Max());

        private IDictionary<int, int> ColumnWidths(IDictionary<int, int> rawWidths) =>
            this.ColumnIndexes
                .Select(col => (column: col, width: rawWidths.TryGetValue(col, out int width) ? width : 0))
                .ToDictionary(tuple => tuple.column, tuple => Math.Max(tuple.width, this.MinimumColumnWidth));

        private IEnumerable<int> ColumnIndexes =>
            Enumerable.Range(0, this.ColumnsCount);

        private int RowsCount =>
            this.Content.Keys.Max(cell => cell.row) + 1;

        private int ColumnsCount =>
            this.Content.Keys.Max(cell => cell.column) + 1;
    }
}
