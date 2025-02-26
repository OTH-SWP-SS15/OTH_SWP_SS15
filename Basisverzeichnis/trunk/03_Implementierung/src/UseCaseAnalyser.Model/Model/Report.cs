﻿#region Copyright information
// <summary>
// <copyright file="Report.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>19/05/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using System.Collections.Generic;

namespace UseCaseAnalyser.Model.Model
{
    /// <summary>
    /// The report class
    /// </summary>
    public class Report
    {
        /// <summary>
        ///  The type of the entry
        /// </summary>
        public enum Entrytype
        {            
            /// <summary>
            /// error report entry
            /// </summary>
            ERROR,
            /// <summary>
            /// warning report entry
            /// </summary>
            WARNING, 
            /// <summary>
            /// information report entry
            /// </summary>
            LOG, 
            /// <summary>
            /// default report entry
            /// </summary>
            DEFAULT
        }

        /// <summary>
        /// All error report entries 
        /// </summary>
        public ReportEntry[] ErrorReportEntries
        {
            get { return mErrorReportEntries.ToArray(); }
        }

        /// <summary>
        /// All warning report entries 
        /// </summary>
        public ReportEntry[] WarningReportEntries
        {
            get { return mWarningReportEntries.ToArray(); }
        }

        /// <summary>
        /// All log report entries 
        /// </summary>
        public ReportEntry[] LogReportEntries
        {
            get { return mLogReportEntries.ToArray(); }
        }

        /// <summary>
        /// All error entries
        /// </summary>
        private readonly List<ReportEntry> mErrorReportEntries = new List<ReportEntry>();

        /// <summary>
        /// All warning entries
        /// </summary>
        private readonly List<ReportEntry> mWarningReportEntries = new List<ReportEntry>();

        /// <summary>
        /// All log entries
        /// </summary>
        private readonly List<ReportEntry> mLogReportEntries = new List<ReportEntry>();

        /// <summary>
        /// Adds an entry to the report
        /// </summary>
        /// <param name="entry"></param>
        public void AddReportEntry(ReportEntry entry)
        {
            switch (entry.Type)
            {
                case Entrytype.ERROR: mErrorReportEntries.Add(entry);
                    break;
                case Entrytype.WARNING: mWarningReportEntries.Add(entry);
                    break;
                case Entrytype.LOG: mLogReportEntries.Add(entry);
                    break;
            }
        }

        /// <summary>
        /// Returns all report entries with the specified tag 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="type">optional</param>
        /// <returns></returns>
        public List<ReportEntry> GetEntriesByTag(string tag, Entrytype type = Entrytype.DEFAULT)
        {
            List<ReportEntry> entries = new List<ReportEntry>();
            switch (type)
            {
                case Entrytype.DEFAULT:
                    entries.AddRange(mErrorReportEntries.FindAll(x => string.Equals(x.Tag, tag)));
                    entries.AddRange(mWarningReportEntries.FindAll(x => string.Equals(x.Tag, tag)));
                    entries.AddRange(mLogReportEntries.FindAll(x => string.Equals(x.Tag, tag)));
                    break;
                case Entrytype.ERROR:
                    entries.AddRange(mErrorReportEntries.FindAll(x => string.Equals(x.Tag, tag)));
                    break;
                case Entrytype.LOG:
                    entries.AddRange(mLogReportEntries.FindAll(x => string.Equals(x.Tag, tag)));                    
                    break;
                case Entrytype.WARNING:
                    entries.AddRange(mWarningReportEntries.FindAll(x => string.Equals(x.Tag, tag)));
                    break;
            }
            
            return entries;
        }

        /// <summary>
        /// Data holder
        /// </summary>
        public class ReportEntry
        {
            /// <summary>
            /// A brief summary of the report entry
            /// </summary>
            public string Heading { get; private set; }

            /// <summary>
            /// A description of the report entry
            /// </summary>
            public string Content { get; private set; }

            /// <summary>
            /// The type, e.g. log, error, warning
            /// </summary>
            public Entrytype Type { get; private set; }

            /// <summary>
            /// The tag of the report entry. Used to be one word. By default the tag of an entry is an empty string
            /// </summary>
            public string Tag { get; private set; }

            /// <summary>
            /// creates a new report entry with the given parameters
            /// </summary>
            /// <param name="heading">heading of the report entry</param>
            /// <param name="content">content of the report entry</param>
            /// <param name="type">entry type (error, warning, log)</param>
            /// <param name="tag">optional additional tag of the report entry</param>
            public ReportEntry(string heading, string content, Entrytype type, string tag = "")
            {
                Heading = heading;
                Content = content;
                Type = type;
                Tag = tag;
            }
        }
    }
}
