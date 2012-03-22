using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace UIMA.NET
{
    public class Document : XmlDocument
    {
        private Dictionary<string, string> typeMap = new Dictionary<string, string>();

        public Document()
        {
            typeMap.Add("Sentence", "edu.mayo.bmi.uima.core.type.Sentence");
            typeMap.Add("Annotation", "edu.northwestern.uima.core.type.Annotation");
        }

        public string GetText()
        {
            return this.SelectSingleNode(".//uima.cas.Sofa").Attributes["sofaString"].InnerText;
        }

        public Sentence[] GetSentences()
        {
            if (!typeMap.ContainsKey("Sentence"))
            {
                throw new Exception("Not configured for Sentences");
            }

            string text = GetText();
            XmlNodeList sentenceNodes = this.SelectNodes(".//" + typeMap["Sentence"]);
            List<Sentence> sentences = new List<Sentence>();
            foreach (XmlNode node in sentenceNodes)
            {
                int startIndex = int.Parse(node.Attributes["begin"].InnerText);
                int endIndex = int.Parse(node.Attributes["end"].InnerText);
                Sentence sentence = new Sentence()
                {
                    ID = int.Parse(node.Attributes["_id"].InnerText),
                    StartIndex = startIndex,
                    EndIndex = endIndex,
                    Text = text.Substring(startIndex, endIndex - startIndex)
                };
                sentences.Add(sentence);
            }
            return sentences.ToArray<Sentence>();
        }

        public Annotation UpdateOrAddAnnotation(Annotation annotation)
        {
            if (!typeMap.ContainsKey("Annotation"))
            {
                throw new Exception("Not configured for annotations");
            }

            // Does an annotation exist in the document with the ID?
            XmlNode annotationNode = this.SelectSingleNode(string.Format(".//{0}[@annotationID='{1}'][@linkID='{2}']", 
                typeMap["Annotation"], 
                annotation.ID, 
                annotation.EntityLinkID));
            if (annotationNode == null)
            {
                XmlNodeList list = this.SelectNodes(string.Format(".//{0}", typeMap["Annotation"]));
                annotation.ID = GetMaxID(list, "annotationID");
                annotationNode = annotation.ConvertToNode(typeMap["Annotation"]);
                FirstChild.AppendChild(ImportNode(annotationNode, true));
            }
            else
            {
                // Why replace instead of update?  This is just a faster way by reusing existing code.
                FirstChild.ReplaceChild(
                    ImportNode(annotation.ConvertToNode(typeMap["Annotation"]), true), 
                    annotationNode);
            }

            return annotation;
        }

        public Annotation[] GetAnnotations()
        {
            if (!typeMap.ContainsKey("Annotation"))
            {
                throw new Exception("Not configured for annotations");
            }

            string text = GetText();
            XmlNodeList annotationNodes = this.SelectNodes(".//" + typeMap["Annotation"]);
            List<Annotation> annotations = new List<Annotation>();
            foreach (XmlNode node in annotationNodes)
            {
                Annotation sentence = new Annotation()
                {
                    ID = int.Parse(node.Attributes["annotationID"].Value),
                    StartPosition = (node.Attributes["begin"].Value == string.Empty ? null : new Nullable<int>(int.Parse(node.Attributes["begin"].Value))),
                    EndPosition = (node.Attributes["end"].Value == string.Empty ? null : new Nullable<int>(int.Parse(node.Attributes["end"].Value))),
                    Scope = (UIMA.NET.Annotation.AnnotationScope)byte.Parse(node.Attributes["scope"].Value),
                    EntityLinkID = SafeGetAttribute(node, "linkID", null),
                    Details = (node.Attributes["details"] != null ? node.Attributes["details"].Value : "")
                };
                annotations.Add(sentence);
            }
            return annotations.ToArray<Annotation>();
        }

        public void DeleteAnnotation(Annotation annotation)
        {
            if (!typeMap.ContainsKey("Annotation"))
            {
                throw new Exception("Not configured for annotations");
            }

            // Does an annotation exist in the document with the ID?
            XmlNodeList annotationNodes = this.SelectNodes(string.Format(".//{0}[@annotationID='{1}'][@linkID='{2}']", 
                typeMap["Annotation"], 
                annotation.ID, 
                annotation.EntityLinkID));
            foreach (XmlNode annotationNode in annotationNodes)
            {
                // Why replace instead of update?  This is just a faster way by reusing existing code.
                FirstChild.RemoveChild(annotationNode);
            }
        }

        #region Helper methods
        private int? SafeGetAttribute(XmlNode node, string attribute, int? defaultValue)
        {
            XmlAttribute attr = node.Attributes[attribute];
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                return int.Parse(attr.Value);
            }

            return defaultValue;
        }


        private int GetMaxID(XmlNodeList list, string idAttribute)
        {
            int max = 1;
            foreach (XmlNode node in list)
            {
                if (node.Attributes[idAttribute] != null)
                {
                    int id = int.Parse(node.Attributes[idAttribute].InnerText);
                    max = Math.Max(max, id);
                }
            }

            return max;
        }
        #endregion

    }
}
