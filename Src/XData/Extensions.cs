using System;
using System.Collections.Generic;

namespace XData {
    public static class Extensions {
        public const string SystemUri = "http://xdata-io.org";
        public const string DefaultIndentString = "\t";
        public const string DefaultNewLineString = "\n";
        //
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void PublicParameterlessConstructorRequired<T>() where T : new() { }
        //
        public static IEnumerable<T> Ancestors<T>(this IEnumerable<XObject> source,
            Func<T, bool> filter = null) where T : XObject {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.Ancestors(filter)) {
                    yield return j;
                }
            }
        }
        #region complex type
        public static IEnumerable<T> SelfAttributes<T>(this IEnumerable<XComplexType> source,
            Func<T, bool> filter = null) where T : XAttribute {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SelfAttributes(filter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SelfAttributeTypes<T>(this IEnumerable<XComplexType> source,
            Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SelfAttributeTypes(attributeFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubElements<T>(this IEnumerable<XComplexType> source,
            Func<T, bool> filter = null) where T : XElement {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElements(filter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubElementTypes<T>(this IEnumerable<XComplexType> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> typeFilter = null) where T : XType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElementTypes(elementFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubAttributes<T>(this IEnumerable<XComplexType> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> attributeFilter = null) where T : XAttribute {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElementAttributes(elementFilter, attributeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubElementAttributeTypes<T>(this IEnumerable<XComplexType> source,
            Func<XElement, bool> elementFilter = null, Func<XAttribute, bool> attributeFilter = null,
            Func<T, bool> typeFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElementAttributeTypes(elementFilter, attributeFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubElementChildren<T>(this IEnumerable<XComplexType> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> childrenFilter = null) where T : XObject {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElementChildren(elementFilter, childrenFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElements<T>(this IEnumerable<XComplexType> source,
            Func<T, bool> filter = null) where T : XElement {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElements(filter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElementTypes<T>(this IEnumerable<XComplexType> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> typeFilter = null) where T : XType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElementTypes(elementFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElementAttributes<T>(this IEnumerable<XComplexType> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> attributeFilter = null) where T : XAttribute {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElementAttributes(elementFilter, attributeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElementAttributeTypes<T>(this IEnumerable<XComplexType> source,
            Func<XElement, bool> elementFilter = null, Func<XAttribute, bool> attributeFilter = null,
            Func<T, bool> typeFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElementAttributeTypes(elementFilter, attributeFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElementChildren<T>(this IEnumerable<XComplexType> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> childrenFilter = null) where T : XObject {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElementChildren(elementFilter, childrenFilter)) {
                    yield return j;
                }
            }
        }

        #endregion complex type
        #region element
        public static IEnumerable<T> SelfAttributes<T>(this IEnumerable<XElement> source,
            Func<T, bool> filter = null) where T : XAttribute {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SelfAttributes(filter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SelfAttributeTypes<T>(this IEnumerable<XElement> source,
            Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SelfAttributeTypes(attributeFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubElements<T>(this IEnumerable<XElement> source,
            Func<T, bool> filter = null) where T : XElement {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElements(filter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubElementTypes<T>(this IEnumerable<XElement> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> typeFilter = null) where T : XType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElementTypes(elementFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubAttributes<T>(this IEnumerable<XElement> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> attributeFilter = null) where T : XAttribute {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElementAttributes(elementFilter, attributeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubElementAttributeTypes<T>(this IEnumerable<XElement> source,
            Func<XElement, bool> elementFilter = null, Func<XAttribute, bool> attributeFilter = null,
            Func<T, bool> typeFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElementAttributeTypes(elementFilter, attributeFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubElementChildren<T>(this IEnumerable<XElement> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> childrenFilter = null) where T : XObject {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElementChildren(elementFilter, childrenFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElements<T>(this IEnumerable<XElement> source,
            Func<T, bool> filter = null) where T : XElement {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElements(filter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElementTypes<T>(this IEnumerable<XElement> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> typeFilter = null) where T : XType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElementTypes(elementFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElementAttributes<T>(this IEnumerable<XElement> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> attributeFilter = null) where T : XAttribute {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElementAttributes(elementFilter, attributeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElementAttributeTypes<T>(this IEnumerable<XElement> source,
            Func<XElement, bool> elementFilter = null, Func<XAttribute, bool> attributeFilter = null,
            Func<T, bool> typeFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElementAttributeTypes(elementFilter, attributeFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElementChildren<T>(this IEnumerable<XElement> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> childrenFilter = null) where T : XObject {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElementChildren(elementFilter, childrenFilter)) {
                    yield return j;
                }
            }
        }

        #endregion element


    }
}
