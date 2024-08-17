using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.DataStructures
{
    public class TransformationTaskCollection : IXmlable, Metalogix.DataStructures.IComparable, ICloneable
    {
        private ArrayList m_list;

        public int Count
        {
            get { return this.m_list.Count; }
        }

        public TransformationTask this[int iIndex]
        {
            get { return (TransformationTask)this.m_list[iIndex]; }
            set { this.m_list.Insert(iIndex, value); }
        }

        public ArrayList TransformationTasks
        {
            get { return this.m_list; }
        }

        public TransformationTaskCollection()
        {
            this.m_list = new ArrayList();
        }

        public TransformationTaskCollection(XmlNode CollectionXML)
        {
            this.m_list = new ArrayList();
            this.FromXML(CollectionXML);
        }

        public virtual object Clone()
        {
            StringBuilder stringBuilder = new StringBuilder();
            this.ToXML(new XmlTextWriter(new StringWriter(stringBuilder)));
            return new TransformationTaskCollection(XmlUtility.StringToXmlNode(stringBuilder.ToString()));
        }

        public void FromXML(XmlNode node)
        {
            foreach (XmlNode childNode in ((node.Name == "TransformationTaskCollection"
                         ? node
                         : node.SelectSingleNode("//TransformationTaskCollection"))).ChildNodes)
            {
                TransformationTask transformationTask = new TransformationTask(childNode);
                this.TransformationTasks.Add(transformationTask);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return this.m_list.GetEnumerator();
        }

        public int GetIndexofTask(System.Type Type, string Pattern)
        {
            int num;
            IEnumerator enumerator = this.TransformationTasks.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    TransformationTask current = (TransformationTask)enumerator.Current;
                    if (current.ApplyTo.GetType() != typeof(FilterExpression) ||
                        !((FilterExpression)current.ApplyTo).AppliesToTypes.Contains(Type.ToString()) ||
                        !(((FilterExpression)current.ApplyTo).Pattern == Pattern))
                    {
                        continue;
                    }

                    num = this.TransformationTasks.IndexOf(current);
                    return num;
                }

                return -1;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            return num;
        }

        public TransformationTask GetTask(object component)
        {
            return this.GetTask(component, (Comparison<object>)null);
        }

        public TransformationTask GetTask(object component, IComparer comparer)
        {
            Comparison<object> comparison = null;
            if (comparer != null)
            {
                IComparer comparer1 = comparer;
                comparison = new Comparison<object>(comparer1.Compare);
            }

            return this.GetTask(component, comparison);
        }

        public TransformationTask GetTask(object component, Comparison<object> comparer)
        {
            TransformationTask transformationTask;
            IEnumerator enumerator = this.m_list.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    TransformationTask current = (TransformationTask)enumerator.Current;
                    if (!current.ApplyTo.Evaluate(component, comparer))
                    {
                        continue;
                    }

                    transformationTask = current;
                    return transformationTask;
                }

                return null;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            return transformationTask;
        }

        public string GetTransformationValue(object objectTaskAppliesTo, string sAttributeToChange)
        {
            return this.GetTransformationValue(objectTaskAppliesTo, sAttributeToChange, (Comparison<object>)null);
        }

        public string GetTransformationValue(object objectTaskAppliesTo, string sAttributeToChange, IComparer comparer)
        {
            Comparison<object> comparison = null;
            if (comparer != null)
            {
                IComparer comparer1 = comparer;
                comparison = new Comparison<object>(comparer1.Compare);
            }

            return this.GetTransformationValue(objectTaskAppliesTo, sAttributeToChange, comparison);
        }

        public string GetTransformationValue(object objectTaskAppliesTo, string sAttributeToChange,
            Comparison<object> comparer)
        {
            TransformationTask transformationTask = null;
            return this.GetTransformationValue(objectTaskAppliesTo, sAttributeToChange, out transformationTask);
        }

        public string GetTransformationValue(object objectTaskAppliesTo, string sAttributeToChange,
            out TransformationTask applicableTask)
        {
            return this.GetTransformationValue(objectTaskAppliesTo, sAttributeToChange, (Comparison<object>)null,
                out applicableTask);
        }

        public string GetTransformationValue(object objectTaskAppliesTo, string sAttributeToChange, IComparer comparer,
            out TransformationTask applicableTask)
        {
            Comparison<object> comparison = null;
            if (comparer != null)
            {
                IComparer comparer1 = comparer;
                comparison = new Comparison<object>(comparer1.Compare);
            }

            return this.GetTransformationValue(objectTaskAppliesTo, sAttributeToChange, comparison, out applicableTask);
        }

        public string GetTransformationValue(object objectTaskAppliesTo, string sAttributeToChange,
            Comparison<object> comparer, out TransformationTask applicableTask)
        {
            string item = null;
            applicableTask = this.GetTask(objectTaskAppliesTo, comparer);
            if (applicableTask != null && applicableTask.ChangeOperations.ContainsKey(sAttributeToChange))
            {
                item = applicableTask.ChangeOperations[sAttributeToChange];
            }

            return item;
        }

        public bool IsEqual(Metalogix.DataStructures.IComparable targetComparable, DifferenceLog differencesOutput,
            ComparisonOptions options)
        {
            bool flag;
            if (!(targetComparable is TransformationTaskCollection))
            {
                differencesOutput.Write("Target comparable is not a compatible type.");
                return false;
            }

            TransformationTaskCollection transformationTaskCollection =
                targetComparable as TransformationTaskCollection;
            if (transformationTaskCollection.Count != this.Count)
            {
                differencesOutput.Write("Different amount of Tasks on Source than on Target", "Tasks");
                return false;
            }

            IEnumerator enumerator = transformationTaskCollection.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    TransformationTask current = (TransformationTask)enumerator.Current;
                    bool flag1 = false;
                    int num = 0;
                    IEnumerator enumerator1 = this.m_list.GetEnumerator();
                    try
                    {
                        while (true)
                        {
                            if (enumerator1.MoveNext())
                            {
                                TransformationTask transformationTask = (TransformationTask)enumerator1.Current;
                                if (!current.ApplyTo.Equals(transformationTask.ApplyTo) ||
                                    !current.IsEqual(transformationTask, differencesOutput, options))
                                {
                                    num++;
                                }
                                else
                                {
                                    flag1 = true;
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    finally
                    {
                        IDisposable disposable = enumerator1 as IDisposable;
                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }

                    if (flag1 || num != this.Count)
                    {
                        continue;
                    }

                    differencesOutput.Write("Task from Target not found on Source", "Task", DifferenceStatus.Missing);
                    flag = false;
                    return flag;
                }

                return true;
            }
            finally
            {
                IDisposable disposable1 = enumerator as IDisposable;
                if (disposable1 != null)
                {
                    disposable1.Dispose();
                }
            }

            return flag;
        }

        public string ToXML()
        {
            StringBuilder stringBuilder = new StringBuilder();
            this.ToXML(new XmlTextWriter(new StringWriter(stringBuilder)));
            return stringBuilder.ToString();
        }

        public void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("TransformationTaskCollection");
            foreach (TransformationTask transformationTask in this.TransformationTasks)
            {
                transformationTask.ToXML(xmlWriter);
            }

            xmlWriter.WriteEndElement();
        }
    }
}