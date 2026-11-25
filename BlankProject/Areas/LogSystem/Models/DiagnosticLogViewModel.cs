namespace Food.Areas.LogSystem.Models
{
    public sealed class DiagnosticLogViewModel
    {
        public string Query { get; set; } = "";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 30;
        public int TotalCount { get; set; }
        public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
        public IReadOnlyList<DiagnosticLogRow> Rows { get; set; } = Array.Empty<DiagnosticLogRow>();
    }

    public sealed class DiagnosticLogRow
    {
        public long Id { get; set; }
        public string Reference { get; set; } = "";
        public string Message { get; set; } = "";
        public string Level { get; set; } = "";
        public string Path { get; set; } = "";
        public string Method { get; set; } = "";
        public string Endpoint { get; set; } = "";
        public string TraceId { get; set; } = "";
        public string Username { get; set; } = "";
        public string ExceptionType { get; set; } = "";
        public string ExceptionMessage { get; set; } = "";
        public string StackTrace { get; set; } = "";
        public string PersianDate { get; set; } = "";
        public string Meaning { get; set; } = "";
        public string WhatToCheck { get; set; } = "";
    }
}
