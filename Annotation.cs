// -----------------------------------------------------------------------
// <copyright file="Annotation.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace UIMA.NET
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Annotation
    {
        public enum AnnotationScope : byte
        {
            Document = 1,
            Sentence = 2,
            Text = 3
        }

        public int ID { get; set; }
        public int? StartPosition { get; set; }
        public int? EndPosition { get; set; }
        public int? EntityLinkID { get; set; }
        public AnnotationScope Scope { get; set; }
        public string Details { get; set; }

        public XmlNode ConvertToNode(string type)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(string.Format("<{0} _id='0' begin='{1}' end='{2}' scope='{3}' linkID='{4}' annotationID='{5}' details='{6}'/>",
                type,
                (StartPosition.HasValue ? StartPosition.Value.ToString() : ""),
                (EndPosition.HasValue ? EndPosition.Value.ToString() : ""),
                ((byte)Scope).ToString(),
                (EntityLinkID.HasValue ? EntityLinkID.Value.ToString() : ""),
                ID.ToString(),
                Details));
            return document.FirstChild;
        }
    }
}
