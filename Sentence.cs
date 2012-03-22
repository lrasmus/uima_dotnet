// -----------------------------------------------------------------------
// <copyright file="Sentence.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace UIMA.NET
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Sentence
    {
        public int ID { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public string Text { get; set; }
        public int SentenceNumber { get; set; }
    }
}
