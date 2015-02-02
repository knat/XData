using System;
using System.Collections.Generic;

namespace XData {
    public static class Extensions {
        public const string SystemUri = "http://xdata-io.org";
        public const string DefaultIndentString = "\t";
        public const string DefaultNewLineString = "\n";

        public static IEnumerable<T> Ancestors<T>(this IEnumerable<XObject> source, Func<T, bool> filter = null) where T : XObject {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.Ancestors(filter)) {
                    yield return j;
                }
            }
        }
        #region complex type
        public static IEnumerable<T> SelfAttributes<T>(this IEnumerable<XComplexType> source, Func<T, bool> filter = null) where T : XAttribute {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SelfAttributes(filter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SelfAttributeTypes<T>(this IEnumerable<XComplexType> source, Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SelfAttributeTypes(attributeFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> ChildrenElements<T>(this IEnumerable<XComplexType> source, Func<T, bool> filter = null) where T : XElement {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.ChildrenElements(filter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> ChildrenElementTypes<T>(this IEnumerable<XComplexType> source, Func<XElement, bool> elementFilter = null, Func<T, bool> typeFilter = null) where T : XType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.ChildrenElementTypes(elementFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> ChildrenSimpleChildren<T>(this IEnumerable<XComplexType> source, Func<XElement, bool> elementFilter = null, Func<T, bool> simpleChildFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.ChildrenSimpleChildren(elementFilter, simpleChildFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> ChildrenAttributes<T>(this IEnumerable<XComplexType> source, Func<XElement, bool> elementFilter = null, Func<T, bool> attributeFilter = null) where T : XAttribute {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.ChildrenAttributes(elementFilter, attributeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> ChildrenAttributeTypes<T>(this IEnumerable<XComplexType> source, Func<XElement, bool> elementFilter = null,
            Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.ChildrenAttributeTypes(elementFilter, attributeFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElements<T>(this IEnumerable<XComplexType> source, Func<T, bool> filter = null) where T : XElement {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElements(filter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElementTypes<T>(this IEnumerable<XComplexType> source, Func<XElement, bool> elementFilter = null, Func<T, bool> typeFilter = null) where T : XType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElementTypes(elementFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantSimpleChildren<T>(this IEnumerable<XComplexType> source, Func<XElement, bool> elementFilter = null, Func<T, bool> simpleChildFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantSimpleChildren(elementFilter, simpleChildFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantAttributes<T>(this IEnumerable<XComplexType> source, Func<XElement, bool> elementFilter = null, Func<T, bool> attributeFilter = null) where T : XAttribute {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantAttributes(elementFilter, attributeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantAttributeTypes<T>(this IEnumerable<XComplexType> source, Func<XElement, bool> elementFilter = null,
            Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantAttributeTypes(elementFilter, attributeFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        #endregion complex type
        #region element
        public static IEnumerable<T> SelfAttributes<T>(this IEnumerable<XElement> source, Func<T, bool> filter = null) where T : XAttribute {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SelfAttributes(filter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SelfAttributeTypes<T>(this IEnumerable<XElement> source, Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SelfAttributeTypes(attributeFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> ChildrenElements<T>(this IEnumerable<XElement> source, Func<T, bool> filter = null) where T : XElement {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.ChildrenElements(filter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> ChildrenElementTypes<T>(this IEnumerable<XElement> source, Func<XElement, bool> elementFilter = null, Func<T, bool> typeFilter = null) where T : XType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.ChildrenElementTypes(elementFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> ChildrenSimpleChildren<T>(this IEnumerable<XElement> source, Func<XElement, bool> elementFilter = null, Func<T, bool> simpleChildFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.ChildrenSimpleChildren(elementFilter, simpleChildFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> ChildrenAttributes<T>(this IEnumerable<XElement> source, Func<XElement, bool> elementFilter = null, Func<T, bool> attributeFilter = null) where T : XAttribute {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.ChildrenAttributes(elementFilter, attributeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> ChildrenAttributeTypes<T>(this IEnumerable<XElement> source, Func<XElement, bool> elementFilter = null,
            Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.ChildrenAttributeTypes(elementFilter, attributeFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElements<T>(this IEnumerable<XElement> source, Func<T, bool> filter = null) where T : XElement {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElements(filter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElementTypes<T>(this IEnumerable<XElement> source, Func<XElement, bool> elementFilter = null, Func<T, bool> typeFilter = null) where T : XType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElementTypes(elementFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantSimpleChildren<T>(this IEnumerable<XElement> source, Func<XElement, bool> elementFilter = null, Func<T, bool> simpleChildFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantSimpleChildren(elementFilter, simpleChildFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantAttributes<T>(this IEnumerable<XElement> source, Func<XElement, bool> elementFilter = null, Func<T, bool> attributeFilter = null) where T : XAttribute {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantAttributes(elementFilter, attributeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantAttributeTypes<T>(this IEnumerable<XElement> source, Func<XElement, bool> elementFilter = null,
            Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantAttributeTypes(elementFilter, attributeFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        #endregion element


    }
}
