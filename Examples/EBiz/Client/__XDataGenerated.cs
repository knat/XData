//
//Auto-generated, DO NOT EDIT.
//Visit https://github.com/knat/XData for more information.
//

namespace Client.Common
{
    public partial class String10 : global::XData.XString
    {
        public static implicit operator global::Client.Common.String10(string value)
        {
            return new global::Client.Common.String10()
            {
            Value = value
            }

            ;
        }

        public static readonly global::XData.FacetSetInfo ThisFacetSetInfo = new global::XData.FacetSetInfo(1UL, 10UL, null, null, null, null, null, null);
        new public static readonly global::XData.AtomTypeInfo ThisInfo = new global::XData.AtomTypeInfo(typeof (global::Client.Common.String10), false, new global::XData.FullName("http://example.com/common", "String10"), global::XData.XString.ThisInfo, global::Client.Common.String10.ThisFacetSetInfo, (global::XData.TypeKind)5);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.String10>();
                return ThisInfo;
            }
        }
    }

    public partial class String20 : global::XData.XString
    {
        public static implicit operator global::Client.Common.String20(string value)
        {
            return new global::Client.Common.String20()
            {
            Value = value
            }

            ;
        }

        public static readonly global::XData.FacetSetInfo ThisFacetSetInfo = new global::XData.FacetSetInfo(1UL, 20UL, null, null, null, null, null, null);
        new public static readonly global::XData.AtomTypeInfo ThisInfo = new global::XData.AtomTypeInfo(typeof (global::Client.Common.String20), false, new global::XData.FullName("http://example.com/common", "String20"), global::XData.XString.ThisInfo, global::Client.Common.String20.ThisFacetSetInfo, (global::XData.TypeKind)5);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.String20>();
                return ThisInfo;
            }
        }
    }

    public partial class String40 : global::XData.XString
    {
        public static implicit operator global::Client.Common.String40(string value)
        {
            return new global::Client.Common.String40()
            {
            Value = value
            }

            ;
        }

        public static readonly global::XData.FacetSetInfo ThisFacetSetInfo = new global::XData.FacetSetInfo(1UL, 40UL, null, null, null, null, null, null);
        new public static readonly global::XData.AtomTypeInfo ThisInfo = new global::XData.AtomTypeInfo(typeof (global::Client.Common.String40), false, new global::XData.FullName("http://example.com/common", "String40"), global::XData.XString.ThisInfo, global::Client.Common.String40.ThisFacetSetInfo, (global::XData.TypeKind)5);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.String40>();
                return ThisInfo;
            }
        }
    }

    public partial class NonNegativeInt32 : global::XData.XInt32
    {
        public static implicit operator global::Client.Common.NonNegativeInt32(int value)
        {
            return new global::Client.Common.NonNegativeInt32()
            {
            Value = value
            }

            ;
        }

        public static readonly global::XData.FacetSetInfo ThisFacetSetInfo = new global::XData.FacetSetInfo(null, null, null, null, new global::XData.ValueBoundaryInfo(0, "0", true), null, null, null);
        new public static readonly global::XData.AtomTypeInfo ThisInfo = new global::XData.AtomTypeInfo(typeof (global::Client.Common.NonNegativeInt32), false, new global::XData.FullName("http://example.com/common", "NonNegativeInt32"), global::XData.XInt32.ThisInfo, global::Client.Common.NonNegativeInt32.ThisFacetSetInfo, (global::XData.TypeKind)9);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.NonNegativeInt32>();
                return ThisInfo;
            }
        }
    }

    public partial class PositiveInt32 : global::Client.Common.NonNegativeInt32
    {
        public static implicit operator global::Client.Common.PositiveInt32(int value)
        {
            return new global::Client.Common.PositiveInt32()
            {
            Value = value
            }

            ;
        }

        new public static readonly global::XData.FacetSetInfo ThisFacetSetInfo = new global::XData.FacetSetInfo(null, null, null, null, new global::XData.ValueBoundaryInfo(0, "0", false), null, null, null);
        new public static readonly global::XData.AtomTypeInfo ThisInfo = new global::XData.AtomTypeInfo(typeof (global::Client.Common.PositiveInt32), false, new global::XData.FullName("http://example.com/common", "PositiveInt32"), global::Client.Common.NonNegativeInt32.ThisInfo, global::Client.Common.PositiveInt32.ThisFacetSetInfo, (global::XData.TypeKind)9);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.PositiveInt32>();
                return ThisInfo;
            }
        }
    }

    public partial class PositiveInt32List : global::XData.XListType<global::Client.Common.PositiveInt32>
    {
        new public global::Client.Common.PositiveInt32List AddEx(global::Client.Common.PositiveInt32 item)
        {
            base.Add(item);
            return this;
        }

        new public global::Client.Common.PositiveInt32List AddRange(global::System.Collections.Generic.IEnumerable<global::Client.Common.PositiveInt32> items)
        {
            base.AddRange(items);
            return this;
        }

        new public static readonly global::XData.ListTypeInfo ThisInfo = new global::XData.ListTypeInfo(typeof (global::Client.Common.PositiveInt32List), false, new global::XData.FullName("http://example.com/common", "PositiveInt32List"), global::XData.XListType.ThisInfo, null, global::Client.Common.PositiveInt32.ThisInfo);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.PositiveInt32List>();
                return ThisInfo;
            }
        }
    }

    public partial class Email : global::XData.XString
    {
        public static implicit operator global::Client.Common.Email(string value)
        {
            return new global::Client.Common.Email()
            {
            Value = value
            }

            ;
        }

        public static readonly global::XData.FacetSetInfo ThisFacetSetInfo = new global::XData.FacetSetInfo(null, 40UL, null, null, null, null, null, new global::XData.PatternInfo[]
        {
        new global::XData.PatternInfo("[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,4}")}

        );
        new public static readonly global::XData.AtomTypeInfo ThisInfo = new global::XData.AtomTypeInfo(typeof (global::Client.Common.Email), false, new global::XData.FullName("http://example.com/common", "Email"), global::XData.XString.ThisInfo, global::Client.Common.Email.ThisFacetSetInfo, (global::XData.TypeKind)5);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.Email>();
                return ThisInfo;
            }
        }
    }

    public partial class PhoneKind : global::XData.XString
    {
        public static implicit operator global::Client.Common.PhoneKind(string value)
        {
            return new global::Client.Common.PhoneKind()
            {
            Value = value
            }

            ;
        }

        public const string E_Unknown = "Unknown";
        public const string E_Work = "Work";
        public const string E_Home = "Home";
        public static readonly global::XData.FacetSetInfo ThisFacetSetInfo = new global::XData.FacetSetInfo(null, null, null, null, null, null, new global::XData.EnumInfo(new object[]
        {
        "Unknown", "Work", "Home"
        }

        , "Unknown Work Home"), null);
        new public static readonly global::XData.AtomTypeInfo ThisInfo = new global::XData.AtomTypeInfo(typeof (global::Client.Common.PhoneKind), false, new global::XData.FullName("http://example.com/common", "PhoneKind"), global::XData.XString.ThisInfo, global::Client.Common.PhoneKind.ThisFacetSetInfo, (global::XData.TypeKind)5);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.PhoneKind>();
                return ThisInfo;
            }
        }
    }

    public partial class Phone : global::XData.XComplexType
    {
        public global::Client.Common.Phone.CLS_Attributes.CLS_Kind A_Kind
        {
            get
            {
                return base.TryGetAttribute("Kind") as global::Client.Common.Phone.CLS_Attributes.CLS_Kind;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("Kind");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.Common.Phone.CLS_Attributes.CLS_Kind EnsureA_Kind(bool @try = false)
        {
            return A_Kind ?? (A_Kind = base.CreateAttribute<global::Client.Common.Phone.CLS_Attributes.CLS_Kind>("Kind", @try));
        }

        public global::Client.Common.PhoneKind AT_Kind
        {
            get
            {
                var obj = A_Kind;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_Kind().Type = value;
            }
        }

        new public global::Client.Common.Phone.CLS_Attributes Attributes
        {
            get
            {
                return base.GenericAttributes as global::Client.Common.Phone.CLS_Attributes;
            }

            set
            {
                base.GenericAttributes = value;
            }
        }

        new public global::Client.Common.Phone.CLS_Attributes EnsureAttributes(bool @try = false)
        {
            return base.EnsureAttributes<global::Client.Common.Phone.CLS_Attributes>(@try);
        }

        public partial class CLS_Attributes : global::XData.XAttributeSet
        {
            public global::Client.Common.Phone.CLS_Attributes.CLS_Kind A_Kind
            {
                get
                {
                    return base.TryGetAttribute("Kind") as global::Client.Common.Phone.CLS_Attributes.CLS_Kind;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("Kind");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.Common.Phone.CLS_Attributes.CLS_Kind EnsureA_Kind(bool @try = false)
            {
                return A_Kind ?? (A_Kind = base.CreateAttribute<global::Client.Common.Phone.CLS_Attributes.CLS_Kind>("Kind", @try));
            }

            public global::Client.Common.PhoneKind AT_Kind
            {
                get
                {
                    var obj = A_Kind;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_Kind().Type = value;
                }
            }

            public partial class CLS_Kind : global::XData.XAttribute
            {
                new public global::Client.Common.PhoneKind Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.PhoneKind;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.Common.Phone.CLS_Attributes.CLS_Kind), "{http://example.com/common}Phone.[].Kind", "Kind", false, false, global::Client.Common.PhoneKind.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.Phone.CLS_Attributes.CLS_Kind>();
                        return ThisInfo;
                    }
                }
            }

            public static readonly global::XData.AttributeSetInfo ThisInfo = new global::XData.AttributeSetInfo(typeof (global::Client.Common.Phone.CLS_Attributes), "{http://example.com/common}Phone.[]", new global::XData.AttributeInfo[]
            {
            global::Client.Common.Phone.CLS_Attributes.CLS_Kind.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.Phone.CLS_Attributes>();
                    return ThisInfo;
                }
            }
        }

        new public global::Client.Common.String20 Children
        {
            get
            {
                return base.GenericChildren as global::Client.Common.String20;
            }

            set
            {
                base.GenericChildren = value;
            }
        }

        new public static readonly global::XData.ComplexTypeInfo ThisInfo = new global::XData.ComplexTypeInfo(typeof (global::Client.Common.Phone), false, new global::XData.FullName("http://example.com/common", "Phone"), global::XData.XComplexType.ThisInfo, global::Client.Common.Phone.CLS_Attributes.ThisInfo, global::Client.Common.String20.ThisInfo);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.Phone>();
                return ThisInfo;
            }
        }
    }

    public partial class NormalAddress : global::XData.XComplexType
    {
        public global::Client.Common.NormalAddress.CLS_Attributes.CLS_Country A_Country
        {
            get
            {
                return base.TryGetAttribute("Country") as global::Client.Common.NormalAddress.CLS_Attributes.CLS_Country;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("Country");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.Common.NormalAddress.CLS_Attributes.CLS_Country EnsureA_Country(bool @try = false)
        {
            return A_Country ?? (A_Country = base.CreateAttribute<global::Client.Common.NormalAddress.CLS_Attributes.CLS_Country>("Country", @try));
        }

        public global::Client.Common.String20 AT_Country
        {
            get
            {
                var obj = A_Country;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_Country().Type = value;
            }
        }

        public global::Client.Common.NormalAddress.CLS_Attributes.CLS_State A_State
        {
            get
            {
                return base.TryGetAttribute("State") as global::Client.Common.NormalAddress.CLS_Attributes.CLS_State;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("State");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.Common.NormalAddress.CLS_Attributes.CLS_State EnsureA_State(bool @try = false)
        {
            return A_State ?? (A_State = base.CreateAttribute<global::Client.Common.NormalAddress.CLS_Attributes.CLS_State>("State", @try));
        }

        public global::Client.Common.String20 AT_State
        {
            get
            {
                var obj = A_State;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_State().Type = value;
            }
        }

        public global::Client.Common.NormalAddress.CLS_Attributes.CLS_City A_City
        {
            get
            {
                return base.TryGetAttribute("City") as global::Client.Common.NormalAddress.CLS_Attributes.CLS_City;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("City");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.Common.NormalAddress.CLS_Attributes.CLS_City EnsureA_City(bool @try = false)
        {
            return A_City ?? (A_City = base.CreateAttribute<global::Client.Common.NormalAddress.CLS_Attributes.CLS_City>("City", @try));
        }

        public global::Client.Common.String20 AT_City
        {
            get
            {
                var obj = A_City;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_City().Type = value;
            }
        }

        public global::Client.Common.NormalAddress.CLS_Attributes.CLS_Address A_Address
        {
            get
            {
                return base.TryGetAttribute("Address") as global::Client.Common.NormalAddress.CLS_Attributes.CLS_Address;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("Address");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.Common.NormalAddress.CLS_Attributes.CLS_Address EnsureA_Address(bool @try = false)
        {
            return A_Address ?? (A_Address = base.CreateAttribute<global::Client.Common.NormalAddress.CLS_Attributes.CLS_Address>("Address", @try));
        }

        public global::Client.Common.String40 AT_Address
        {
            get
            {
                var obj = A_Address;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_Address().Type = value;
            }
        }

        public global::Client.Common.NormalAddress.CLS_Attributes.CLS_ZipCode A_ZipCode
        {
            get
            {
                return base.TryGetAttribute("ZipCode") as global::Client.Common.NormalAddress.CLS_Attributes.CLS_ZipCode;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("ZipCode");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.Common.NormalAddress.CLS_Attributes.CLS_ZipCode EnsureA_ZipCode(bool @try = false)
        {
            return A_ZipCode ?? (A_ZipCode = base.CreateAttribute<global::Client.Common.NormalAddress.CLS_Attributes.CLS_ZipCode>("ZipCode", @try));
        }

        public global::Client.Common.String10 AT_ZipCode
        {
            get
            {
                var obj = A_ZipCode;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_ZipCode().Type = value;
            }
        }

        new public global::Client.Common.NormalAddress.CLS_Attributes Attributes
        {
            get
            {
                return base.GenericAttributes as global::Client.Common.NormalAddress.CLS_Attributes;
            }

            set
            {
                base.GenericAttributes = value;
            }
        }

        new public global::Client.Common.NormalAddress.CLS_Attributes EnsureAttributes(bool @try = false)
        {
            return base.EnsureAttributes<global::Client.Common.NormalAddress.CLS_Attributes>(@try);
        }

        public partial class CLS_Attributes : global::XData.XAttributeSet
        {
            public global::Client.Common.NormalAddress.CLS_Attributes.CLS_Country A_Country
            {
                get
                {
                    return base.TryGetAttribute("Country") as global::Client.Common.NormalAddress.CLS_Attributes.CLS_Country;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("Country");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.Common.NormalAddress.CLS_Attributes.CLS_Country EnsureA_Country(bool @try = false)
            {
                return A_Country ?? (A_Country = base.CreateAttribute<global::Client.Common.NormalAddress.CLS_Attributes.CLS_Country>("Country", @try));
            }

            public global::Client.Common.String20 AT_Country
            {
                get
                {
                    var obj = A_Country;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_Country().Type = value;
                }
            }

            public partial class CLS_Country : global::XData.XAttribute
            {
                new public global::Client.Common.String20 Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.String20;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.Common.NormalAddress.CLS_Attributes.CLS_Country), "{http://example.com/common}NormalAddress.[].Country", "Country", false, false, global::Client.Common.String20.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.NormalAddress.CLS_Attributes.CLS_Country>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.Common.NormalAddress.CLS_Attributes.CLS_State A_State
            {
                get
                {
                    return base.TryGetAttribute("State") as global::Client.Common.NormalAddress.CLS_Attributes.CLS_State;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("State");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.Common.NormalAddress.CLS_Attributes.CLS_State EnsureA_State(bool @try = false)
            {
                return A_State ?? (A_State = base.CreateAttribute<global::Client.Common.NormalAddress.CLS_Attributes.CLS_State>("State", @try));
            }

            public global::Client.Common.String20 AT_State
            {
                get
                {
                    var obj = A_State;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_State().Type = value;
                }
            }

            public partial class CLS_State : global::XData.XAttribute
            {
                new public global::Client.Common.String20 Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.String20;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.Common.NormalAddress.CLS_Attributes.CLS_State), "{http://example.com/common}NormalAddress.[].State", "State", true, false, global::Client.Common.String20.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.NormalAddress.CLS_Attributes.CLS_State>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.Common.NormalAddress.CLS_Attributes.CLS_City A_City
            {
                get
                {
                    return base.TryGetAttribute("City") as global::Client.Common.NormalAddress.CLS_Attributes.CLS_City;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("City");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.Common.NormalAddress.CLS_Attributes.CLS_City EnsureA_City(bool @try = false)
            {
                return A_City ?? (A_City = base.CreateAttribute<global::Client.Common.NormalAddress.CLS_Attributes.CLS_City>("City", @try));
            }

            public global::Client.Common.String20 AT_City
            {
                get
                {
                    var obj = A_City;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_City().Type = value;
                }
            }

            public partial class CLS_City : global::XData.XAttribute
            {
                new public global::Client.Common.String20 Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.String20;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.Common.NormalAddress.CLS_Attributes.CLS_City), "{http://example.com/common}NormalAddress.[].City", "City", false, false, global::Client.Common.String20.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.NormalAddress.CLS_Attributes.CLS_City>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.Common.NormalAddress.CLS_Attributes.CLS_Address A_Address
            {
                get
                {
                    return base.TryGetAttribute("Address") as global::Client.Common.NormalAddress.CLS_Attributes.CLS_Address;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("Address");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.Common.NormalAddress.CLS_Attributes.CLS_Address EnsureA_Address(bool @try = false)
            {
                return A_Address ?? (A_Address = base.CreateAttribute<global::Client.Common.NormalAddress.CLS_Attributes.CLS_Address>("Address", @try));
            }

            public global::Client.Common.String40 AT_Address
            {
                get
                {
                    var obj = A_Address;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_Address().Type = value;
                }
            }

            public partial class CLS_Address : global::XData.XAttribute
            {
                new public global::Client.Common.String40 Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.String40;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.Common.NormalAddress.CLS_Attributes.CLS_Address), "{http://example.com/common}NormalAddress.[].Address", "Address", false, false, global::Client.Common.String40.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.NormalAddress.CLS_Attributes.CLS_Address>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.Common.NormalAddress.CLS_Attributes.CLS_ZipCode A_ZipCode
            {
                get
                {
                    return base.TryGetAttribute("ZipCode") as global::Client.Common.NormalAddress.CLS_Attributes.CLS_ZipCode;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("ZipCode");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.Common.NormalAddress.CLS_Attributes.CLS_ZipCode EnsureA_ZipCode(bool @try = false)
            {
                return A_ZipCode ?? (A_ZipCode = base.CreateAttribute<global::Client.Common.NormalAddress.CLS_Attributes.CLS_ZipCode>("ZipCode", @try));
            }

            public global::Client.Common.String10 AT_ZipCode
            {
                get
                {
                    var obj = A_ZipCode;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_ZipCode().Type = value;
                }
            }

            public partial class CLS_ZipCode : global::XData.XAttribute
            {
                new public global::Client.Common.String10 Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.String10;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.Common.NormalAddress.CLS_Attributes.CLS_ZipCode), "{http://example.com/common}NormalAddress.[].ZipCode", "ZipCode", false, false, global::Client.Common.String10.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.NormalAddress.CLS_Attributes.CLS_ZipCode>();
                        return ThisInfo;
                    }
                }
            }

            public static readonly global::XData.AttributeSetInfo ThisInfo = new global::XData.AttributeSetInfo(typeof (global::Client.Common.NormalAddress.CLS_Attributes), "{http://example.com/common}NormalAddress.[]", new global::XData.AttributeInfo[]
            {
            global::Client.Common.NormalAddress.CLS_Attributes.CLS_Country.ThisInfo, global::Client.Common.NormalAddress.CLS_Attributes.CLS_State.ThisInfo, global::Client.Common.NormalAddress.CLS_Attributes.CLS_City.ThisInfo, global::Client.Common.NormalAddress.CLS_Attributes.CLS_Address.ThisInfo, global::Client.Common.NormalAddress.CLS_Attributes.CLS_ZipCode.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.NormalAddress.CLS_Attributes>();
                    return ThisInfo;
                }
            }
        }

        new public static readonly global::XData.ComplexTypeInfo ThisInfo = new global::XData.ComplexTypeInfo(typeof (global::Client.Common.NormalAddress), false, new global::XData.FullName("http://example.com/common", "NormalAddress"), global::XData.XComplexType.ThisInfo, global::Client.Common.NormalAddress.CLS_Attributes.ThisInfo, null);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.NormalAddress>();
                return ThisInfo;
            }
        }
    }

    public partial class SpatialNumber : global::XData.XDecimal
    {
        public static implicit operator global::Client.Common.SpatialNumber(decimal value)
        {
            return new global::Client.Common.SpatialNumber()
            {
            Value = value
            }

            ;
        }

        public static readonly global::XData.FacetSetInfo ThisFacetSetInfo = new global::XData.FacetSetInfo(null, null, (byte)8, (byte)5, null, null, null, null);
        new public static readonly global::XData.AtomTypeInfo ThisInfo = new global::XData.AtomTypeInfo(typeof (global::Client.Common.SpatialNumber), false, new global::XData.FullName("http://example.com/common", "SpatialNumber"), global::XData.XDecimal.ThisInfo, global::Client.Common.SpatialNumber.ThisFacetSetInfo, (global::XData.TypeKind)7);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.SpatialNumber>();
                return ThisInfo;
            }
        }
    }

    public partial class SpatialAddress : global::XData.XComplexType
    {
        public global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Longitude A_Longitude
        {
            get
            {
                return base.TryGetAttribute("Longitude") as global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Longitude;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("Longitude");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Longitude EnsureA_Longitude(bool @try = false)
        {
            return A_Longitude ?? (A_Longitude = base.CreateAttribute<global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Longitude>("Longitude", @try));
        }

        public global::Client.Common.SpatialNumber AT_Longitude
        {
            get
            {
                var obj = A_Longitude;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_Longitude().Type = value;
            }
        }

        public global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Latitude A_Latitude
        {
            get
            {
                return base.TryGetAttribute("Latitude") as global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Latitude;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("Latitude");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Latitude EnsureA_Latitude(bool @try = false)
        {
            return A_Latitude ?? (A_Latitude = base.CreateAttribute<global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Latitude>("Latitude", @try));
        }

        public global::Client.Common.SpatialNumber AT_Latitude
        {
            get
            {
                var obj = A_Latitude;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_Latitude().Type = value;
            }
        }

        new public global::Client.Common.SpatialAddress.CLS_Attributes Attributes
        {
            get
            {
                return base.GenericAttributes as global::Client.Common.SpatialAddress.CLS_Attributes;
            }

            set
            {
                base.GenericAttributes = value;
            }
        }

        new public global::Client.Common.SpatialAddress.CLS_Attributes EnsureAttributes(bool @try = false)
        {
            return base.EnsureAttributes<global::Client.Common.SpatialAddress.CLS_Attributes>(@try);
        }

        public partial class CLS_Attributes : global::XData.XAttributeSet
        {
            public global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Longitude A_Longitude
            {
                get
                {
                    return base.TryGetAttribute("Longitude") as global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Longitude;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("Longitude");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Longitude EnsureA_Longitude(bool @try = false)
            {
                return A_Longitude ?? (A_Longitude = base.CreateAttribute<global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Longitude>("Longitude", @try));
            }

            public global::Client.Common.SpatialNumber AT_Longitude
            {
                get
                {
                    var obj = A_Longitude;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_Longitude().Type = value;
                }
            }

            public partial class CLS_Longitude : global::XData.XAttribute
            {
                new public global::Client.Common.SpatialNumber Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.SpatialNumber;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Longitude), "{http://example.com/common}SpatialAddress.[].Longitude", "Longitude", false, false, global::Client.Common.SpatialNumber.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Longitude>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Latitude A_Latitude
            {
                get
                {
                    return base.TryGetAttribute("Latitude") as global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Latitude;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("Latitude");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Latitude EnsureA_Latitude(bool @try = false)
            {
                return A_Latitude ?? (A_Latitude = base.CreateAttribute<global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Latitude>("Latitude", @try));
            }

            public global::Client.Common.SpatialNumber AT_Latitude
            {
                get
                {
                    var obj = A_Latitude;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_Latitude().Type = value;
                }
            }

            public partial class CLS_Latitude : global::XData.XAttribute
            {
                new public global::Client.Common.SpatialNumber Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.SpatialNumber;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Latitude), "{http://example.com/common}SpatialAddress.[].Latitude", "Latitude", false, false, global::Client.Common.SpatialNumber.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Latitude>();
                        return ThisInfo;
                    }
                }
            }

            public static readonly global::XData.AttributeSetInfo ThisInfo = new global::XData.AttributeSetInfo(typeof (global::Client.Common.SpatialAddress.CLS_Attributes), "{http://example.com/common}SpatialAddress.[]", new global::XData.AttributeInfo[]
            {
            global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Longitude.ThisInfo, global::Client.Common.SpatialAddress.CLS_Attributes.CLS_Latitude.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.SpatialAddress.CLS_Attributes>();
                    return ThisInfo;
                }
            }
        }

        new public static readonly global::XData.ComplexTypeInfo ThisInfo = new global::XData.ComplexTypeInfo(typeof (global::Client.Common.SpatialAddress), false, new global::XData.FullName("http://example.com/common", "SpatialAddress"), global::XData.XComplexType.ThisInfo, global::Client.Common.SpatialAddress.CLS_Attributes.ThisInfo, null);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.SpatialAddress>();
                return ThisInfo;
            }
        }
    }

    public partial class MoneyKind : global::XData.XString
    {
        public static implicit operator global::Client.Common.MoneyKind(string value)
        {
            return new global::Client.Common.MoneyKind()
            {
            Value = value
            }

            ;
        }

        public const string E_USD = "USD";
        public const string E_EUR = "EUR";
        public const string E_CNY = "CNY";
        public static readonly global::XData.FacetSetInfo ThisFacetSetInfo = new global::XData.FacetSetInfo(null, null, null, null, null, null, new global::XData.EnumInfo(new object[]
        {
        "USD", "EUR", "CNY"
        }

        , "USD EUR CNY"), null);
        new public static readonly global::XData.AtomTypeInfo ThisInfo = new global::XData.AtomTypeInfo(typeof (global::Client.Common.MoneyKind), false, new global::XData.FullName("http://example.com/common", "MoneyKind"), global::XData.XString.ThisInfo, global::Client.Common.MoneyKind.ThisFacetSetInfo, (global::XData.TypeKind)5);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.MoneyKind>();
                return ThisInfo;
            }
        }
    }

    public partial class MoneyValue : global::XData.XDecimal
    {
        public static implicit operator global::Client.Common.MoneyValue(decimal value)
        {
            return new global::Client.Common.MoneyValue()
            {
            Value = value
            }

            ;
        }

        public static readonly global::XData.FacetSetInfo ThisFacetSetInfo = new global::XData.FacetSetInfo(null, null, (byte)10, (byte)2, new global::XData.ValueBoundaryInfo(0M, "0", true), null, null, null);
        new public static readonly global::XData.AtomTypeInfo ThisInfo = new global::XData.AtomTypeInfo(typeof (global::Client.Common.MoneyValue), false, new global::XData.FullName("http://example.com/common", "MoneyValue"), global::XData.XDecimal.ThisInfo, global::Client.Common.MoneyValue.ThisFacetSetInfo, (global::XData.TypeKind)7);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.MoneyValue>();
                return ThisInfo;
            }
        }
    }

    public partial class Money : global::XData.XComplexType
    {
        public global::Client.Common.Money.CLS_Attributes.CLS_Kind A_Kind
        {
            get
            {
                return base.TryGetAttribute("Kind") as global::Client.Common.Money.CLS_Attributes.CLS_Kind;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("Kind");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.Common.Money.CLS_Attributes.CLS_Kind EnsureA_Kind(bool @try = false)
        {
            return A_Kind ?? (A_Kind = base.CreateAttribute<global::Client.Common.Money.CLS_Attributes.CLS_Kind>("Kind", @try));
        }

        public global::Client.Common.MoneyKind AT_Kind
        {
            get
            {
                var obj = A_Kind;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_Kind().Type = value;
            }
        }

        new public global::Client.Common.Money.CLS_Attributes Attributes
        {
            get
            {
                return base.GenericAttributes as global::Client.Common.Money.CLS_Attributes;
            }

            set
            {
                base.GenericAttributes = value;
            }
        }

        new public global::Client.Common.Money.CLS_Attributes EnsureAttributes(bool @try = false)
        {
            return base.EnsureAttributes<global::Client.Common.Money.CLS_Attributes>(@try);
        }

        public partial class CLS_Attributes : global::XData.XAttributeSet
        {
            public global::Client.Common.Money.CLS_Attributes.CLS_Kind A_Kind
            {
                get
                {
                    return base.TryGetAttribute("Kind") as global::Client.Common.Money.CLS_Attributes.CLS_Kind;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("Kind");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.Common.Money.CLS_Attributes.CLS_Kind EnsureA_Kind(bool @try = false)
            {
                return A_Kind ?? (A_Kind = base.CreateAttribute<global::Client.Common.Money.CLS_Attributes.CLS_Kind>("Kind", @try));
            }

            public global::Client.Common.MoneyKind AT_Kind
            {
                get
                {
                    var obj = A_Kind;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_Kind().Type = value;
                }
            }

            public partial class CLS_Kind : global::XData.XAttribute
            {
                new public global::Client.Common.MoneyKind Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.MoneyKind;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.Common.Money.CLS_Attributes.CLS_Kind), "{http://example.com/common}Money.[].Kind", "Kind", false, false, global::Client.Common.MoneyKind.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.Money.CLS_Attributes.CLS_Kind>();
                        return ThisInfo;
                    }
                }
            }

            public static readonly global::XData.AttributeSetInfo ThisInfo = new global::XData.AttributeSetInfo(typeof (global::Client.Common.Money.CLS_Attributes), "{http://example.com/common}Money.[]", new global::XData.AttributeInfo[]
            {
            global::Client.Common.Money.CLS_Attributes.CLS_Kind.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.Money.CLS_Attributes>();
                    return ThisInfo;
                }
            }
        }

        new public global::Client.Common.MoneyValue Children
        {
            get
            {
                return base.GenericChildren as global::Client.Common.MoneyValue;
            }

            set
            {
                base.GenericChildren = value;
            }
        }

        new public static readonly global::XData.ComplexTypeInfo ThisInfo = new global::XData.ComplexTypeInfo(typeof (global::Client.Common.Money), false, new global::XData.FullName("http://example.com/common", "Money"), global::XData.XComplexType.ThisInfo, global::Client.Common.Money.CLS_Attributes.ThisInfo, global::Client.Common.MoneyValue.ThisInfo);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.Money>();
                return ThisInfo;
            }
        }
    }

    public partial class ImageValue : global::XData.XBinary
    {
        public static implicit operator global::Client.Common.ImageValue(byte[] value)
        {
            return new global::Client.Common.ImageValue()
            {
            Value = value
            }

            ;
        }

        public static readonly global::XData.FacetSetInfo ThisFacetSetInfo = new global::XData.FacetSetInfo(null, 100000UL, null, null, null, null, null, null);
        new public static readonly global::XData.AtomTypeInfo ThisInfo = new global::XData.AtomTypeInfo(typeof (global::Client.Common.ImageValue), false, new global::XData.FullName("http://example.com/common", "ImageValue"), global::XData.XBinary.ThisInfo, global::Client.Common.ImageValue.ThisFacetSetInfo, (global::XData.TypeKind)19);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.ImageValue>();
                return ThisInfo;
            }
        }
    }

    public partial class Image : global::XData.XComplexType
    {
        public global::Client.Common.Image.CLS_Attributes.CLS_Mime A_Mime
        {
            get
            {
                return base.TryGetAttribute("Mime") as global::Client.Common.Image.CLS_Attributes.CLS_Mime;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("Mime");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.Common.Image.CLS_Attributes.CLS_Mime EnsureA_Mime(bool @try = false)
        {
            return A_Mime ?? (A_Mime = base.CreateAttribute<global::Client.Common.Image.CLS_Attributes.CLS_Mime>("Mime", @try));
        }

        public global::Client.Common.String20 AT_Mime
        {
            get
            {
                var obj = A_Mime;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_Mime().Type = value;
            }
        }

        new public global::Client.Common.Image.CLS_Attributes Attributes
        {
            get
            {
                return base.GenericAttributes as global::Client.Common.Image.CLS_Attributes;
            }

            set
            {
                base.GenericAttributes = value;
            }
        }

        new public global::Client.Common.Image.CLS_Attributes EnsureAttributes(bool @try = false)
        {
            return base.EnsureAttributes<global::Client.Common.Image.CLS_Attributes>(@try);
        }

        public partial class CLS_Attributes : global::XData.XAttributeSet
        {
            public global::Client.Common.Image.CLS_Attributes.CLS_Mime A_Mime
            {
                get
                {
                    return base.TryGetAttribute("Mime") as global::Client.Common.Image.CLS_Attributes.CLS_Mime;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("Mime");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.Common.Image.CLS_Attributes.CLS_Mime EnsureA_Mime(bool @try = false)
            {
                return A_Mime ?? (A_Mime = base.CreateAttribute<global::Client.Common.Image.CLS_Attributes.CLS_Mime>("Mime", @try));
            }

            public global::Client.Common.String20 AT_Mime
            {
                get
                {
                    var obj = A_Mime;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_Mime().Type = value;
                }
            }

            public partial class CLS_Mime : global::XData.XAttribute
            {
                new public global::Client.Common.String20 Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.String20;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.Common.Image.CLS_Attributes.CLS_Mime), "{http://example.com/common}Image.[].Mime", "Mime", false, false, global::Client.Common.String20.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.Image.CLS_Attributes.CLS_Mime>();
                        return ThisInfo;
                    }
                }
            }

            public static readonly global::XData.AttributeSetInfo ThisInfo = new global::XData.AttributeSetInfo(typeof (global::Client.Common.Image.CLS_Attributes), "{http://example.com/common}Image.[]", new global::XData.AttributeInfo[]
            {
            global::Client.Common.Image.CLS_Attributes.CLS_Mime.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.Image.CLS_Attributes>();
                    return ThisInfo;
                }
            }
        }

        new public global::Client.Common.ImageValue Children
        {
            get
            {
                return base.GenericChildren as global::Client.Common.ImageValue;
            }

            set
            {
                base.GenericChildren = value;
            }
        }

        new public static readonly global::XData.ComplexTypeInfo ThisInfo = new global::XData.ComplexTypeInfo(typeof (global::Client.Common.Image), false, new global::XData.FullName("http://example.com/common", "Image"), global::XData.XComplexType.ThisInfo, global::Client.Common.Image.CLS_Attributes.ThisInfo, global::Client.Common.ImageValue.ThisInfo);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.Image>();
                return ThisInfo;
            }
        }
    }

    public partial class Reputation : global::XData.XString
    {
        public static implicit operator global::Client.Common.Reputation(string value)
        {
            return new global::Client.Common.Reputation()
            {
            Value = value
            }

            ;
        }

        public const string E_None = "None";
        public const string E_Bronze = "Bronze";
        public const string E_Silver = "Silver";
        public const string E_Gold = "Gold";
        public const string E_Diamond = "Diamond";
        public const string E_Bad = "Bad";
        public static readonly global::XData.FacetSetInfo ThisFacetSetInfo = new global::XData.FacetSetInfo(null, null, null, null, null, null, new global::XData.EnumInfo(new object[]
        {
        "None", "Bronze", "Silver", "Gold", "Diamond", "Bad"
        }

        , "None Bronze Silver Gold Diamond Bad"), null);
        new public static readonly global::XData.AtomTypeInfo ThisInfo = new global::XData.AtomTypeInfo(typeof (global::Client.Common.Reputation), false, new global::XData.FullName("http://example.com/common", "Reputation"), global::XData.XString.ThisInfo, global::Client.Common.Reputation.ThisFacetSetInfo, (global::XData.TypeKind)5);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.Reputation>();
                return ThisInfo;
            }
        }
    }

    public partial class Gender : global::XData.XString
    {
        public static implicit operator global::Client.Common.Gender(string value)
        {
            return new global::Client.Common.Gender()
            {
            Value = value
            }

            ;
        }

        public const string E_Man = "Man";
        public const string E_Woman = "Woman";
        public const string E_Junior = "Junior";
        public static readonly global::XData.FacetSetInfo ThisFacetSetInfo = new global::XData.FacetSetInfo(null, null, null, null, null, null, new global::XData.EnumInfo(new object[]
        {
        "Man", "Woman", "Junior"
        }

        , "Man Woman Junior"), null);
        new public static readonly global::XData.AtomTypeInfo ThisInfo = new global::XData.AtomTypeInfo(typeof (global::Client.Common.Gender), false, new global::XData.FullName("http://example.com/common", "Gender"), global::XData.XString.ThisInfo, global::Client.Common.Gender.ThisFacetSetInfo, (global::XData.TypeKind)5);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.Gender>();
                return ThisInfo;
            }
        }
    }

    public partial class ShoeUnit : global::XData.XString
    {
        public static implicit operator global::Client.Common.ShoeUnit(string value)
        {
            return new global::Client.Common.ShoeUnit()
            {
            Value = value
            }

            ;
        }

        public const string E_CM = "CM";
        public const string E_EU = "EU";
        public const string E_UK = "UK";
        public const string E_US = "US";
        public static readonly global::XData.FacetSetInfo ThisFacetSetInfo = new global::XData.FacetSetInfo(null, null, null, null, null, null, new global::XData.EnumInfo(new object[]
        {
        "CM", "EU", "UK", "US"
        }

        , "CM EU UK US"), null);
        new public static readonly global::XData.AtomTypeInfo ThisInfo = new global::XData.AtomTypeInfo(typeof (global::Client.Common.ShoeUnit), false, new global::XData.FullName("http://example.com/common", "ShoeUnit"), global::XData.XString.ThisInfo, global::Client.Common.ShoeUnit.ThisFacetSetInfo, (global::XData.TypeKind)5);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.ShoeUnit>();
                return ThisInfo;
            }
        }
    }

    public partial class ShoeSizeValue : global::XData.XDecimal
    {
        public static implicit operator global::Client.Common.ShoeSizeValue(decimal value)
        {
            return new global::Client.Common.ShoeSizeValue()
            {
            Value = value
            }

            ;
        }

        public static readonly global::XData.FacetSetInfo ThisFacetSetInfo = new global::XData.FacetSetInfo(null, null, (byte)3, (byte)1, null, null, null, null);
        new public static readonly global::XData.AtomTypeInfo ThisInfo = new global::XData.AtomTypeInfo(typeof (global::Client.Common.ShoeSizeValue), false, new global::XData.FullName("http://example.com/common", "ShoeSizeValue"), global::XData.XDecimal.ThisInfo, global::Client.Common.ShoeSizeValue.ThisFacetSetInfo, (global::XData.TypeKind)7);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.ShoeSizeValue>();
                return ThisInfo;
            }
        }
    }

    public partial class ShoeSize : global::XData.XComplexType
    {
        public global::Client.Common.ShoeSize.CLS_Attributes.CLS_Unit A_Unit
        {
            get
            {
                return base.TryGetAttribute("Unit") as global::Client.Common.ShoeSize.CLS_Attributes.CLS_Unit;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("Unit");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.Common.ShoeSize.CLS_Attributes.CLS_Unit EnsureA_Unit(bool @try = false)
        {
            return A_Unit ?? (A_Unit = base.CreateAttribute<global::Client.Common.ShoeSize.CLS_Attributes.CLS_Unit>("Unit", @try));
        }

        public global::Client.Common.ShoeUnit AT_Unit
        {
            get
            {
                var obj = A_Unit;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_Unit().Type = value;
            }
        }

        new public global::Client.Common.ShoeSize.CLS_Attributes Attributes
        {
            get
            {
                return base.GenericAttributes as global::Client.Common.ShoeSize.CLS_Attributes;
            }

            set
            {
                base.GenericAttributes = value;
            }
        }

        new public global::Client.Common.ShoeSize.CLS_Attributes EnsureAttributes(bool @try = false)
        {
            return base.EnsureAttributes<global::Client.Common.ShoeSize.CLS_Attributes>(@try);
        }

        public partial class CLS_Attributes : global::XData.XAttributeSet
        {
            public global::Client.Common.ShoeSize.CLS_Attributes.CLS_Unit A_Unit
            {
                get
                {
                    return base.TryGetAttribute("Unit") as global::Client.Common.ShoeSize.CLS_Attributes.CLS_Unit;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("Unit");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.Common.ShoeSize.CLS_Attributes.CLS_Unit EnsureA_Unit(bool @try = false)
            {
                return A_Unit ?? (A_Unit = base.CreateAttribute<global::Client.Common.ShoeSize.CLS_Attributes.CLS_Unit>("Unit", @try));
            }

            public global::Client.Common.ShoeUnit AT_Unit
            {
                get
                {
                    var obj = A_Unit;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_Unit().Type = value;
                }
            }

            public partial class CLS_Unit : global::XData.XAttribute
            {
                new public global::Client.Common.ShoeUnit Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.ShoeUnit;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.Common.ShoeSize.CLS_Attributes.CLS_Unit), "{http://example.com/common}ShoeSize.[].Unit", "Unit", false, false, global::Client.Common.ShoeUnit.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.ShoeSize.CLS_Attributes.CLS_Unit>();
                        return ThisInfo;
                    }
                }
            }

            public static readonly global::XData.AttributeSetInfo ThisInfo = new global::XData.AttributeSetInfo(typeof (global::Client.Common.ShoeSize.CLS_Attributes), "{http://example.com/common}ShoeSize.[]", new global::XData.AttributeInfo[]
            {
            global::Client.Common.ShoeSize.CLS_Attributes.CLS_Unit.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.ShoeSize.CLS_Attributes>();
                    return ThisInfo;
                }
            }
        }

        new public global::Client.Common.ShoeSizeValue Children
        {
            get
            {
                return base.GenericChildren as global::Client.Common.ShoeSizeValue;
            }

            set
            {
                base.GenericChildren = value;
            }
        }

        new public static readonly global::XData.ComplexTypeInfo ThisInfo = new global::XData.ComplexTypeInfo(typeof (global::Client.Common.ShoeSize), false, new global::XData.FullName("http://example.com/common", "ShoeSize"), global::XData.XComplexType.ThisInfo, global::Client.Common.ShoeSize.CLS_Attributes.ThisInfo, global::Client.Common.ShoeSizeValue.ThisInfo);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.Common.ShoeSize>();
                return ThisInfo;
            }
        }
    }
}

namespace Client.EBiz
{
    public abstract partial class Contact : global::XData.XComplexType
    {
        public global::Client.EBiz.Contact.CLS_Attributes.CLS_Id A_Id
        {
            get
            {
                return base.TryGetAttribute("Id") as global::Client.EBiz.Contact.CLS_Attributes.CLS_Id;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("Id");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.EBiz.Contact.CLS_Attributes.CLS_Id EnsureA_Id(bool @try = false)
        {
            return A_Id ?? (A_Id = base.CreateAttribute<global::Client.EBiz.Contact.CLS_Attributes.CLS_Id>("Id", @try));
        }

        public global::Client.Common.PositiveInt32 AT_Id
        {
            get
            {
                var obj = A_Id;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_Id().Type = value;
            }
        }

        public global::Client.EBiz.Contact.CLS_Attributes.CLS_Name A_Name
        {
            get
            {
                return base.TryGetAttribute("Name") as global::Client.EBiz.Contact.CLS_Attributes.CLS_Name;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("Name");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.EBiz.Contact.CLS_Attributes.CLS_Name EnsureA_Name(bool @try = false)
        {
            return A_Name ?? (A_Name = base.CreateAttribute<global::Client.EBiz.Contact.CLS_Attributes.CLS_Name>("Name", @try));
        }

        public global::Client.Common.String10 AT_Name
        {
            get
            {
                var obj = A_Name;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_Name().Type = value;
            }
        }

        public global::Client.EBiz.Contact.CLS_Attributes.CLS_Email A_Email
        {
            get
            {
                return base.TryGetAttribute("Email") as global::Client.EBiz.Contact.CLS_Attributes.CLS_Email;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("Email");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.EBiz.Contact.CLS_Attributes.CLS_Email EnsureA_Email(bool @try = false)
        {
            return A_Email ?? (A_Email = base.CreateAttribute<global::Client.EBiz.Contact.CLS_Attributes.CLS_Email>("Email", @try));
        }

        public global::Client.Common.Email AT_Email
        {
            get
            {
                var obj = A_Email;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_Email().Type = value;
            }
        }

        public global::Client.EBiz.Contact.CLS_Attributes.CLS_RegDate A_RegDate
        {
            get
            {
                return base.TryGetAttribute("RegDate") as global::Client.EBiz.Contact.CLS_Attributes.CLS_RegDate;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("RegDate");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.EBiz.Contact.CLS_Attributes.CLS_RegDate EnsureA_RegDate(bool @try = false)
        {
            return A_RegDate ?? (A_RegDate = base.CreateAttribute<global::Client.EBiz.Contact.CLS_Attributes.CLS_RegDate>("RegDate", @try));
        }

        public global::XData.XDateTimeOffset AT_RegDate
        {
            get
            {
                var obj = A_RegDate;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_RegDate().Type = value;
            }
        }

        new public global::Client.EBiz.Contact.CLS_Attributes Attributes
        {
            get
            {
                return base.GenericAttributes as global::Client.EBiz.Contact.CLS_Attributes;
            }

            set
            {
                base.GenericAttributes = value;
            }
        }

        new public global::Client.EBiz.Contact.CLS_Attributes EnsureAttributes(bool @try = false)
        {
            return base.EnsureAttributes<global::Client.EBiz.Contact.CLS_Attributes>(@try);
        }

        public partial class CLS_Attributes : global::XData.XAttributeSet
        {
            public global::Client.EBiz.Contact.CLS_Attributes.CLS_Id A_Id
            {
                get
                {
                    return base.TryGetAttribute("Id") as global::Client.EBiz.Contact.CLS_Attributes.CLS_Id;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("Id");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.EBiz.Contact.CLS_Attributes.CLS_Id EnsureA_Id(bool @try = false)
            {
                return A_Id ?? (A_Id = base.CreateAttribute<global::Client.EBiz.Contact.CLS_Attributes.CLS_Id>("Id", @try));
            }

            public global::Client.Common.PositiveInt32 AT_Id
            {
                get
                {
                    var obj = A_Id;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_Id().Type = value;
                }
            }

            public partial class CLS_Id : global::XData.XAttribute
            {
                new public global::Client.Common.PositiveInt32 Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.PositiveInt32;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.EBiz.Contact.CLS_Attributes.CLS_Id), "{http://example.com/ebiz}Contact.[].Id", "Id", false, false, global::Client.Common.PositiveInt32.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Contact.CLS_Attributes.CLS_Id>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.EBiz.Contact.CLS_Attributes.CLS_Name A_Name
            {
                get
                {
                    return base.TryGetAttribute("Name") as global::Client.EBiz.Contact.CLS_Attributes.CLS_Name;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("Name");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.EBiz.Contact.CLS_Attributes.CLS_Name EnsureA_Name(bool @try = false)
            {
                return A_Name ?? (A_Name = base.CreateAttribute<global::Client.EBiz.Contact.CLS_Attributes.CLS_Name>("Name", @try));
            }

            public global::Client.Common.String10 AT_Name
            {
                get
                {
                    var obj = A_Name;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_Name().Type = value;
                }
            }

            public partial class CLS_Name : global::XData.XAttribute
            {
                new public global::Client.Common.String10 Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.String10;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.EBiz.Contact.CLS_Attributes.CLS_Name), "{http://example.com/ebiz}Contact.[].Name", "Name", false, false, global::Client.Common.String10.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Contact.CLS_Attributes.CLS_Name>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.EBiz.Contact.CLS_Attributes.CLS_Email A_Email
            {
                get
                {
                    return base.TryGetAttribute("Email") as global::Client.EBiz.Contact.CLS_Attributes.CLS_Email;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("Email");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.EBiz.Contact.CLS_Attributes.CLS_Email EnsureA_Email(bool @try = false)
            {
                return A_Email ?? (A_Email = base.CreateAttribute<global::Client.EBiz.Contact.CLS_Attributes.CLS_Email>("Email", @try));
            }

            public global::Client.Common.Email AT_Email
            {
                get
                {
                    var obj = A_Email;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_Email().Type = value;
                }
            }

            public partial class CLS_Email : global::XData.XAttribute
            {
                new public global::Client.Common.Email Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.Email;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.EBiz.Contact.CLS_Attributes.CLS_Email), "{http://example.com/ebiz}Contact.[].Email", "Email", false, false, global::Client.Common.Email.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Contact.CLS_Attributes.CLS_Email>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.EBiz.Contact.CLS_Attributes.CLS_RegDate A_RegDate
            {
                get
                {
                    return base.TryGetAttribute("RegDate") as global::Client.EBiz.Contact.CLS_Attributes.CLS_RegDate;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("RegDate");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.EBiz.Contact.CLS_Attributes.CLS_RegDate EnsureA_RegDate(bool @try = false)
            {
                return A_RegDate ?? (A_RegDate = base.CreateAttribute<global::Client.EBiz.Contact.CLS_Attributes.CLS_RegDate>("RegDate", @try));
            }

            public global::XData.XDateTimeOffset AT_RegDate
            {
                get
                {
                    var obj = A_RegDate;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_RegDate().Type = value;
                }
            }

            public partial class CLS_RegDate : global::XData.XAttribute
            {
                new public global::XData.XDateTimeOffset Type
                {
                    get
                    {
                        return base.GenericType as global::XData.XDateTimeOffset;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.EBiz.Contact.CLS_Attributes.CLS_RegDate), "{http://example.com/ebiz}Contact.[].RegDate", "RegDate", false, false, global::XData.XDateTimeOffset.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Contact.CLS_Attributes.CLS_RegDate>();
                        return ThisInfo;
                    }
                }
            }

            public static readonly global::XData.AttributeSetInfo ThisInfo = new global::XData.AttributeSetInfo(typeof (global::Client.EBiz.Contact.CLS_Attributes), "{http://example.com/ebiz}Contact.[]", new global::XData.AttributeInfo[]
            {
            global::Client.EBiz.Contact.CLS_Attributes.CLS_Id.ThisInfo, global::Client.EBiz.Contact.CLS_Attributes.CLS_Name.ThisInfo, global::Client.EBiz.Contact.CLS_Attributes.CLS_Email.ThisInfo, global::Client.EBiz.Contact.CLS_Attributes.CLS_RegDate.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Contact.CLS_Attributes>();
                    return ThisInfo;
                }
            }
        }

        public global::Client.EBiz.Contact.CLS_Children.CLS_PhoneList C_PhoneList
        {
            get
            {
                return base.TryGetChild(0) as global::Client.EBiz.Contact.CLS_Children.CLS_PhoneList;
            }

            set
            {
                if (value == null)
                    base.RemoveChild(0);
                else
                    base.AddOrSetChild(value);
            }
        }

        public global::Client.EBiz.Contact.CLS_Children.CLS_PhoneList EnsureC_PhoneList(bool @try = false)
        {
            return C_PhoneList ?? (C_PhoneList = base.CreateChild<global::Client.EBiz.Contact.CLS_Children.CLS_PhoneList>(0, @try));
        }

        public global::Client.EBiz.Contact.CLS_Children.CLS_Address C_Address
        {
            get
            {
                return base.TryGetChild(1) as global::Client.EBiz.Contact.CLS_Children.CLS_Address;
            }

            set
            {
                if (value == null)
                    base.RemoveChild(1);
                else
                    base.AddOrSetChild(value);
            }
        }

        public global::Client.EBiz.Contact.CLS_Children.CLS_Address EnsureC_Address(bool @try = false)
        {
            return C_Address ?? (C_Address = base.CreateChild<global::Client.EBiz.Contact.CLS_Children.CLS_Address>(1, @try));
        }

        new public global::Client.EBiz.Contact.CLS_Children Children
        {
            get
            {
                return base.GenericChildren as global::Client.EBiz.Contact.CLS_Children;
            }

            set
            {
                base.GenericChildren = value;
            }
        }

        new public global::Client.EBiz.Contact.CLS_Children EnsureChildren(bool @try = false)
        {
            return base.EnsureChildren<global::Client.EBiz.Contact.CLS_Children>(@try);
        }

        public partial class CLS_Children : global::XData.XChildSequence
        {
            public partial class CLSITEM_PhoneList : global::XData.XLocalElement
            {
                new public global::Client.Common.Phone Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.Phone;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.ElementInfo ThisInfo = new global::XData.ElementInfo(typeof (global::Client.EBiz.Contact.CLS_Children.CLSITEM_PhoneList), false, "{http://example.com/ebiz}Contact.#{}.PhoneList", (global::XData.ChildKind)2, false, 0, new global::XData.FullName(null, "Phone"), false, global::Client.Common.Phone.ThisInfo, null, null, null, global::XDataProgramInfo.Instance);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Contact.CLS_Children.CLSITEM_PhoneList>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.EBiz.Contact.CLS_Children.CLS_PhoneList C_PhoneList
            {
                get
                {
                    return base.TryGetChild(0) as global::Client.EBiz.Contact.CLS_Children.CLS_PhoneList;
                }

                set
                {
                    if (value == null)
                        base.RemoveChild(0);
                    else
                        base.AddOrSetChild(value);
                }
            }

            public global::Client.EBiz.Contact.CLS_Children.CLS_PhoneList EnsureC_PhoneList(bool @try = false)
            {
                return C_PhoneList ?? (C_PhoneList = base.CreateChild<global::Client.EBiz.Contact.CLS_Children.CLS_PhoneList>(0, @try));
            }

            public partial class CLS_PhoneList : global::XData.XChildList<global::Client.EBiz.Contact.CLS_Children.CLSITEM_PhoneList>
            {
                public global::Client.EBiz.Contact.CLS_Children.CLSITEM_PhoneList CreateItem()
                {
                    return base.CreateItem<global::Client.EBiz.Contact.CLS_Children.CLSITEM_PhoneList>();
                }

                public global::Client.EBiz.Contact.CLS_Children.CLSITEM_PhoneList CreateAndAddItem()
                {
                    return base.CreateAndAddItem<global::Client.EBiz.Contact.CLS_Children.CLSITEM_PhoneList>();
                }

                public global::Client.EBiz.Contact.CLS_Children.CLS_PhoneList Add(global::System.Action<global::Client.EBiz.Contact.CLS_Children.CLSITEM_PhoneList> itemSetter)
                {
                    base.Add<global::Client.EBiz.Contact.CLS_Children.CLSITEM_PhoneList>(itemSetter);
                    return this;
                }

                public global::Client.EBiz.Contact.CLS_Children.CLS_PhoneList AddRange<TItemValue>(global::System.Collections.Generic.IEnumerable<TItemValue> itemValues, global::System.Action<global::Client.EBiz.Contact.CLS_Children.CLSITEM_PhoneList, TItemValue> itemSetter)
                {
                    base.AddRange<global::Client.EBiz.Contact.CLS_Children.CLSITEM_PhoneList, TItemValue>(itemValues, itemSetter);
                    return this;
                }

                public static readonly global::XData.ChildListInfo ThisInfo = new global::XData.ChildListInfo(typeof (global::Client.EBiz.Contact.CLS_Children.CLS_PhoneList), "{http://example.com/ebiz}Contact.#{}.PhoneList", false, 0, 1UL, 5UL, global::Client.EBiz.Contact.CLS_Children.CLSITEM_PhoneList.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Contact.CLS_Children.CLS_PhoneList>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.EBiz.Contact.CLS_Children.CLS_Address C_Address
            {
                get
                {
                    return base.TryGetChild(1) as global::Client.EBiz.Contact.CLS_Children.CLS_Address;
                }

                set
                {
                    if (value == null)
                        base.RemoveChild(1);
                    else
                        base.AddOrSetChild(value);
                }
            }

            public global::Client.EBiz.Contact.CLS_Children.CLS_Address EnsureC_Address(bool @try = false)
            {
                return C_Address ?? (C_Address = base.CreateChild<global::Client.EBiz.Contact.CLS_Children.CLS_Address>(1, @try));
            }

            public partial class CLS_Address : global::XData.XChildChoice
            {
                public global::Client.EBiz.Contact.CLS_Children.CLS_Address.CLS_NormalAddress C_NormalAddress
                {
                    get
                    {
                        return base.TryGetChild(0) as global::Client.EBiz.Contact.CLS_Children.CLS_Address.CLS_NormalAddress;
                    }

                    set
                    {
                        if (value == null)
                            base.RemoveChild(0);
                        else
                            base.AddOrSetChild(value);
                    }
                }

                public global::Client.EBiz.Contact.CLS_Children.CLS_Address.CLS_NormalAddress EnsureC_NormalAddress(bool @try = false)
                {
                    return C_NormalAddress ?? (C_NormalAddress = base.CreateChild<global::Client.EBiz.Contact.CLS_Children.CLS_Address.CLS_NormalAddress>(0, @try));
                }

                public global::Client.Common.NormalAddress CT_NormalAddress
                {
                    get
                    {
                        var obj = C_NormalAddress;
                        if (obj == null)
                            return null;
                        return obj.Type;
                    }

                    set
                    {
                        EnsureC_NormalAddress().Type = value;
                    }
                }

                public partial class CLS_NormalAddress : global::XData.XLocalElement
                {
                    new public global::Client.Common.NormalAddress Type
                    {
                        get
                        {
                            return base.GenericType as global::Client.Common.NormalAddress;
                        }

                        set
                        {
                            base.GenericType = value;
                        }
                    }

                    public static readonly global::XData.ElementInfo ThisInfo = new global::XData.ElementInfo(typeof (global::Client.EBiz.Contact.CLS_Children.CLS_Address.CLS_NormalAddress), false, "{http://example.com/ebiz}Contact.#{}.Address.NormalAddress", (global::XData.ChildKind)2, false, 0, new global::XData.FullName(null, "NormalAddress"), false, global::Client.Common.NormalAddress.ThisInfo, null, null, null, global::XDataProgramInfo.Instance);
                    public override global::XData.ObjectInfo ObjectInfo
                    {
                        get
                        {
                            global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Contact.CLS_Children.CLS_Address.CLS_NormalAddress>();
                            return ThisInfo;
                        }
                    }
                }

                public global::Client.EBiz.Contact.CLS_Children.CLS_Address.CLS_SpatialAddress C_SpatialAddress
                {
                    get
                    {
                        return base.TryGetChild(1) as global::Client.EBiz.Contact.CLS_Children.CLS_Address.CLS_SpatialAddress;
                    }

                    set
                    {
                        if (value == null)
                            base.RemoveChild(1);
                        else
                            base.AddOrSetChild(value);
                    }
                }

                public global::Client.EBiz.Contact.CLS_Children.CLS_Address.CLS_SpatialAddress EnsureC_SpatialAddress(bool @try = false)
                {
                    return C_SpatialAddress ?? (C_SpatialAddress = base.CreateChild<global::Client.EBiz.Contact.CLS_Children.CLS_Address.CLS_SpatialAddress>(1, @try));
                }

                public global::Client.Common.SpatialAddress CT_SpatialAddress
                {
                    get
                    {
                        var obj = C_SpatialAddress;
                        if (obj == null)
                            return null;
                        return obj.Type;
                    }

                    set
                    {
                        EnsureC_SpatialAddress().Type = value;
                    }
                }

                public partial class CLS_SpatialAddress : global::XData.XLocalElement
                {
                    new public global::Client.Common.SpatialAddress Type
                    {
                        get
                        {
                            return base.GenericType as global::Client.Common.SpatialAddress;
                        }

                        set
                        {
                            base.GenericType = value;
                        }
                    }

                    public static readonly global::XData.ElementInfo ThisInfo = new global::XData.ElementInfo(typeof (global::Client.EBiz.Contact.CLS_Children.CLS_Address.CLS_SpatialAddress), false, "{http://example.com/ebiz}Contact.#{}.Address.SpatialAddress", (global::XData.ChildKind)2, false, 1, new global::XData.FullName(null, "SpatialAddress"), false, global::Client.Common.SpatialAddress.ThisInfo, null, null, null, global::XDataProgramInfo.Instance);
                    public override global::XData.ObjectInfo ObjectInfo
                    {
                        get
                        {
                            global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Contact.CLS_Children.CLS_Address.CLS_SpatialAddress>();
                            return ThisInfo;
                        }
                    }
                }

                public static readonly global::XData.ChildStructInfo ThisInfo = new global::XData.ChildStructInfo(typeof (global::Client.EBiz.Contact.CLS_Children.CLS_Address), "{http://example.com/ebiz}Contact.#{}.Address", (global::XData.ChildKind)5, false, 1, new global::XData.ChildInfo[]
                {
                global::Client.EBiz.Contact.CLS_Children.CLS_Address.CLS_NormalAddress.ThisInfo, global::Client.EBiz.Contact.CLS_Children.CLS_Address.CLS_SpatialAddress.ThisInfo
                }

                );
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Contact.CLS_Children.CLS_Address>();
                        return ThisInfo;
                    }
                }
            }

            public static readonly global::XData.ChildStructInfo ThisInfo = new global::XData.ChildStructInfo(typeof (global::Client.EBiz.Contact.CLS_Children), "{http://example.com/ebiz}Contact.#{}", (global::XData.ChildKind)4, false, -1, new global::XData.ChildInfo[]
            {
            global::Client.EBiz.Contact.CLS_Children.CLS_PhoneList.ThisInfo, global::Client.EBiz.Contact.CLS_Children.CLS_Address.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Contact.CLS_Children>();
                    return ThisInfo;
                }
            }
        }

        new public static readonly global::XData.ComplexTypeInfo ThisInfo = new global::XData.ComplexTypeInfo(typeof (global::Client.EBiz.Contact), true, new global::XData.FullName("http://example.com/ebiz", "Contact"), global::XData.XComplexType.ThisInfo, global::Client.EBiz.Contact.CLS_Attributes.ThisInfo, global::Client.EBiz.Contact.CLS_Children.ThisInfo);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                return ThisInfo;
            }
        }
    }

    public partial class OrderDetail : global::XData.XComplexType
    {
        public global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_ProductId A_ProductId
        {
            get
            {
                return base.TryGetAttribute("ProductId") as global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_ProductId;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("ProductId");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_ProductId EnsureA_ProductId(bool @try = false)
        {
            return A_ProductId ?? (A_ProductId = base.CreateAttribute<global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_ProductId>("ProductId", @try));
        }

        public global::Client.Common.PositiveInt32 AT_ProductId
        {
            get
            {
                var obj = A_ProductId;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_ProductId().Type = value;
            }
        }

        public global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_Quantity A_Quantity
        {
            get
            {
                return base.TryGetAttribute("Quantity") as global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_Quantity;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("Quantity");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_Quantity EnsureA_Quantity(bool @try = false)
        {
            return A_Quantity ?? (A_Quantity = base.CreateAttribute<global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_Quantity>("Quantity", @try));
        }

        public global::Client.Common.PositiveInt32 AT_Quantity
        {
            get
            {
                var obj = A_Quantity;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_Quantity().Type = value;
            }
        }

        new public global::Client.EBiz.OrderDetail.CLS_Attributes Attributes
        {
            get
            {
                return base.GenericAttributes as global::Client.EBiz.OrderDetail.CLS_Attributes;
            }

            set
            {
                base.GenericAttributes = value;
            }
        }

        new public global::Client.EBiz.OrderDetail.CLS_Attributes EnsureAttributes(bool @try = false)
        {
            return base.EnsureAttributes<global::Client.EBiz.OrderDetail.CLS_Attributes>(@try);
        }

        public partial class CLS_Attributes : global::XData.XAttributeSet
        {
            public global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_ProductId A_ProductId
            {
                get
                {
                    return base.TryGetAttribute("ProductId") as global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_ProductId;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("ProductId");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_ProductId EnsureA_ProductId(bool @try = false)
            {
                return A_ProductId ?? (A_ProductId = base.CreateAttribute<global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_ProductId>("ProductId", @try));
            }

            public global::Client.Common.PositiveInt32 AT_ProductId
            {
                get
                {
                    var obj = A_ProductId;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_ProductId().Type = value;
                }
            }

            public partial class CLS_ProductId : global::XData.XAttribute
            {
                new public global::Client.Common.PositiveInt32 Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.PositiveInt32;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_ProductId), "{http://example.com/ebiz}OrderDetail.[].ProductId", "ProductId", false, false, global::Client.Common.PositiveInt32.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_ProductId>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_Quantity A_Quantity
            {
                get
                {
                    return base.TryGetAttribute("Quantity") as global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_Quantity;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("Quantity");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_Quantity EnsureA_Quantity(bool @try = false)
            {
                return A_Quantity ?? (A_Quantity = base.CreateAttribute<global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_Quantity>("Quantity", @try));
            }

            public global::Client.Common.PositiveInt32 AT_Quantity
            {
                get
                {
                    var obj = A_Quantity;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_Quantity().Type = value;
                }
            }

            public partial class CLS_Quantity : global::XData.XAttribute
            {
                new public global::Client.Common.PositiveInt32 Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.PositiveInt32;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_Quantity), "{http://example.com/ebiz}OrderDetail.[].Quantity", "Quantity", false, false, global::Client.Common.PositiveInt32.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_Quantity>();
                        return ThisInfo;
                    }
                }
            }

            public static readonly global::XData.AttributeSetInfo ThisInfo = new global::XData.AttributeSetInfo(typeof (global::Client.EBiz.OrderDetail.CLS_Attributes), "{http://example.com/ebiz}OrderDetail.[]", new global::XData.AttributeInfo[]
            {
            global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_ProductId.ThisInfo, global::Client.EBiz.OrderDetail.CLS_Attributes.CLS_Quantity.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.OrderDetail.CLS_Attributes>();
                    return ThisInfo;
                }
            }
        }

        public global::Client.EBiz.OrderDetail.CLS_Children.CLS_UnitPrice C_UnitPrice
        {
            get
            {
                return base.TryGetChild(0) as global::Client.EBiz.OrderDetail.CLS_Children.CLS_UnitPrice;
            }

            set
            {
                if (value == null)
                    base.RemoveChild(0);
                else
                    base.AddOrSetChild(value);
            }
        }

        public global::Client.EBiz.OrderDetail.CLS_Children.CLS_UnitPrice EnsureC_UnitPrice(bool @try = false)
        {
            return C_UnitPrice ?? (C_UnitPrice = base.CreateChild<global::Client.EBiz.OrderDetail.CLS_Children.CLS_UnitPrice>(0, @try));
        }

        public global::Client.Common.Money CT_UnitPrice
        {
            get
            {
                var obj = C_UnitPrice;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureC_UnitPrice().Type = value;
            }
        }

        new public global::Client.EBiz.OrderDetail.CLS_Children Children
        {
            get
            {
                return base.GenericChildren as global::Client.EBiz.OrderDetail.CLS_Children;
            }

            set
            {
                base.GenericChildren = value;
            }
        }

        new public global::Client.EBiz.OrderDetail.CLS_Children EnsureChildren(bool @try = false)
        {
            return base.EnsureChildren<global::Client.EBiz.OrderDetail.CLS_Children>(@try);
        }

        public partial class CLS_Children : global::XData.XChildSequence
        {
            public global::Client.EBiz.OrderDetail.CLS_Children.CLS_UnitPrice C_UnitPrice
            {
                get
                {
                    return base.TryGetChild(0) as global::Client.EBiz.OrderDetail.CLS_Children.CLS_UnitPrice;
                }

                set
                {
                    if (value == null)
                        base.RemoveChild(0);
                    else
                        base.AddOrSetChild(value);
                }
            }

            public global::Client.EBiz.OrderDetail.CLS_Children.CLS_UnitPrice EnsureC_UnitPrice(bool @try = false)
            {
                return C_UnitPrice ?? (C_UnitPrice = base.CreateChild<global::Client.EBiz.OrderDetail.CLS_Children.CLS_UnitPrice>(0, @try));
            }

            public global::Client.Common.Money CT_UnitPrice
            {
                get
                {
                    var obj = C_UnitPrice;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureC_UnitPrice().Type = value;
                }
            }

            public partial class CLS_UnitPrice : global::XData.XLocalElement
            {
                new public global::Client.Common.Money Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.Money;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.ElementInfo ThisInfo = new global::XData.ElementInfo(typeof (global::Client.EBiz.OrderDetail.CLS_Children.CLS_UnitPrice), false, "{http://example.com/ebiz}OrderDetail.#{}.UnitPrice", (global::XData.ChildKind)2, false, 0, new global::XData.FullName(null, "UnitPrice"), false, global::Client.Common.Money.ThisInfo, null, null, null, global::XDataProgramInfo.Instance);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.OrderDetail.CLS_Children.CLS_UnitPrice>();
                        return ThisInfo;
                    }
                }
            }

            public static readonly global::XData.ChildStructInfo ThisInfo = new global::XData.ChildStructInfo(typeof (global::Client.EBiz.OrderDetail.CLS_Children), "{http://example.com/ebiz}OrderDetail.#{}", (global::XData.ChildKind)4, false, -1, new global::XData.ChildInfo[]
            {
            global::Client.EBiz.OrderDetail.CLS_Children.CLS_UnitPrice.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.OrderDetail.CLS_Children>();
                    return ThisInfo;
                }
            }
        }

        new public static readonly global::XData.ComplexTypeInfo ThisInfo = new global::XData.ComplexTypeInfo(typeof (global::Client.EBiz.OrderDetail), false, new global::XData.FullName("http://example.com/ebiz", "OrderDetail"), global::XData.XComplexType.ThisInfo, global::Client.EBiz.OrderDetail.CLS_Attributes.ThisInfo, global::Client.EBiz.OrderDetail.CLS_Children.ThisInfo);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.OrderDetail>();
                return ThisInfo;
            }
        }
    }

    public partial class Order : global::XData.XComplexType
    {
        public global::Client.EBiz.Order.CLS_Attributes.CLS_Id A_Id
        {
            get
            {
                return base.TryGetAttribute("Id") as global::Client.EBiz.Order.CLS_Attributes.CLS_Id;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("Id");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.EBiz.Order.CLS_Attributes.CLS_Id EnsureA_Id(bool @try = false)
        {
            return A_Id ?? (A_Id = base.CreateAttribute<global::Client.EBiz.Order.CLS_Attributes.CLS_Id>("Id", @try));
        }

        public global::Client.Common.PositiveInt32 AT_Id
        {
            get
            {
                var obj = A_Id;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_Id().Type = value;
            }
        }

        public global::Client.EBiz.Order.CLS_Attributes.CLS_OrderDate A_OrderDate
        {
            get
            {
                return base.TryGetAttribute("OrderDate") as global::Client.EBiz.Order.CLS_Attributes.CLS_OrderDate;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("OrderDate");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.EBiz.Order.CLS_Attributes.CLS_OrderDate EnsureA_OrderDate(bool @try = false)
        {
            return A_OrderDate ?? (A_OrderDate = base.CreateAttribute<global::Client.EBiz.Order.CLS_Attributes.CLS_OrderDate>("OrderDate", @try));
        }

        public global::XData.XDateTimeOffset AT_OrderDate
        {
            get
            {
                var obj = A_OrderDate;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_OrderDate().Type = value;
            }
        }

        public global::Client.EBiz.Order.CLS_Attributes.CLS_Urgent A_Urgent
        {
            get
            {
                return base.TryGetAttribute("Urgent") as global::Client.EBiz.Order.CLS_Attributes.CLS_Urgent;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("Urgent");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.EBiz.Order.CLS_Attributes.CLS_Urgent EnsureA_Urgent(bool @try = false)
        {
            return A_Urgent ?? (A_Urgent = base.CreateAttribute<global::Client.EBiz.Order.CLS_Attributes.CLS_Urgent>("Urgent", @try));
        }

        public global::XData.XBoolean AT_Urgent
        {
            get
            {
                var obj = A_Urgent;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_Urgent().Type = value;
            }
        }

        new public global::Client.EBiz.Order.CLS_Attributes Attributes
        {
            get
            {
                return base.GenericAttributes as global::Client.EBiz.Order.CLS_Attributes;
            }

            set
            {
                base.GenericAttributes = value;
            }
        }

        new public global::Client.EBiz.Order.CLS_Attributes EnsureAttributes(bool @try = false)
        {
            return base.EnsureAttributes<global::Client.EBiz.Order.CLS_Attributes>(@try);
        }

        public partial class CLS_Attributes : global::XData.XAttributeSet
        {
            public global::Client.EBiz.Order.CLS_Attributes.CLS_Id A_Id
            {
                get
                {
                    return base.TryGetAttribute("Id") as global::Client.EBiz.Order.CLS_Attributes.CLS_Id;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("Id");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.EBiz.Order.CLS_Attributes.CLS_Id EnsureA_Id(bool @try = false)
            {
                return A_Id ?? (A_Id = base.CreateAttribute<global::Client.EBiz.Order.CLS_Attributes.CLS_Id>("Id", @try));
            }

            public global::Client.Common.PositiveInt32 AT_Id
            {
                get
                {
                    var obj = A_Id;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_Id().Type = value;
                }
            }

            public partial class CLS_Id : global::XData.XAttribute
            {
                new public global::Client.Common.PositiveInt32 Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.PositiveInt32;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.EBiz.Order.CLS_Attributes.CLS_Id), "{http://example.com/ebiz}Order.[].Id", "Id", false, false, global::Client.Common.PositiveInt32.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Order.CLS_Attributes.CLS_Id>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.EBiz.Order.CLS_Attributes.CLS_OrderDate A_OrderDate
            {
                get
                {
                    return base.TryGetAttribute("OrderDate") as global::Client.EBiz.Order.CLS_Attributes.CLS_OrderDate;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("OrderDate");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.EBiz.Order.CLS_Attributes.CLS_OrderDate EnsureA_OrderDate(bool @try = false)
            {
                return A_OrderDate ?? (A_OrderDate = base.CreateAttribute<global::Client.EBiz.Order.CLS_Attributes.CLS_OrderDate>("OrderDate", @try));
            }

            public global::XData.XDateTimeOffset AT_OrderDate
            {
                get
                {
                    var obj = A_OrderDate;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_OrderDate().Type = value;
                }
            }

            public partial class CLS_OrderDate : global::XData.XAttribute
            {
                new public global::XData.XDateTimeOffset Type
                {
                    get
                    {
                        return base.GenericType as global::XData.XDateTimeOffset;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.EBiz.Order.CLS_Attributes.CLS_OrderDate), "{http://example.com/ebiz}Order.[].OrderDate", "OrderDate", false, false, global::XData.XDateTimeOffset.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Order.CLS_Attributes.CLS_OrderDate>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.EBiz.Order.CLS_Attributes.CLS_Urgent A_Urgent
            {
                get
                {
                    return base.TryGetAttribute("Urgent") as global::Client.EBiz.Order.CLS_Attributes.CLS_Urgent;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("Urgent");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.EBiz.Order.CLS_Attributes.CLS_Urgent EnsureA_Urgent(bool @try = false)
            {
                return A_Urgent ?? (A_Urgent = base.CreateAttribute<global::Client.EBiz.Order.CLS_Attributes.CLS_Urgent>("Urgent", @try));
            }

            public global::XData.XBoolean AT_Urgent
            {
                get
                {
                    var obj = A_Urgent;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_Urgent().Type = value;
                }
            }

            public partial class CLS_Urgent : global::XData.XAttribute
            {
                new public global::XData.XBoolean Type
                {
                    get
                    {
                        return base.GenericType as global::XData.XBoolean;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.EBiz.Order.CLS_Attributes.CLS_Urgent), "{http://example.com/ebiz}Order.[].Urgent", "Urgent", true, false, global::XData.XBoolean.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Order.CLS_Attributes.CLS_Urgent>();
                        return ThisInfo;
                    }
                }
            }

            public static readonly global::XData.AttributeSetInfo ThisInfo = new global::XData.AttributeSetInfo(typeof (global::Client.EBiz.Order.CLS_Attributes), "{http://example.com/ebiz}Order.[]", new global::XData.AttributeInfo[]
            {
            global::Client.EBiz.Order.CLS_Attributes.CLS_Id.ThisInfo, global::Client.EBiz.Order.CLS_Attributes.CLS_OrderDate.ThisInfo, global::Client.EBiz.Order.CLS_Attributes.CLS_Urgent.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Order.CLS_Attributes>();
                    return ThisInfo;
                }
            }
        }

        public global::Client.EBiz.Order.CLS_Children.CLS_TotalPrice C_TotalPrice
        {
            get
            {
                return base.TryGetChild(0) as global::Client.EBiz.Order.CLS_Children.CLS_TotalPrice;
            }

            set
            {
                if (value == null)
                    base.RemoveChild(0);
                else
                    base.AddOrSetChild(value);
            }
        }

        public global::Client.EBiz.Order.CLS_Children.CLS_TotalPrice EnsureC_TotalPrice(bool @try = false)
        {
            return C_TotalPrice ?? (C_TotalPrice = base.CreateChild<global::Client.EBiz.Order.CLS_Children.CLS_TotalPrice>(0, @try));
        }

        public global::Client.Common.Money CT_TotalPrice
        {
            get
            {
                var obj = C_TotalPrice;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureC_TotalPrice().Type = value;
            }
        }

        public global::Client.EBiz.Order.CLS_Children.CLS_OrderDetailList C_OrderDetailList
        {
            get
            {
                return base.TryGetChild(1) as global::Client.EBiz.Order.CLS_Children.CLS_OrderDetailList;
            }

            set
            {
                if (value == null)
                    base.RemoveChild(1);
                else
                    base.AddOrSetChild(value);
            }
        }

        public global::Client.EBiz.Order.CLS_Children.CLS_OrderDetailList EnsureC_OrderDetailList(bool @try = false)
        {
            return C_OrderDetailList ?? (C_OrderDetailList = base.CreateChild<global::Client.EBiz.Order.CLS_Children.CLS_OrderDetailList>(1, @try));
        }

        new public global::Client.EBiz.Order.CLS_Children Children
        {
            get
            {
                return base.GenericChildren as global::Client.EBiz.Order.CLS_Children;
            }

            set
            {
                base.GenericChildren = value;
            }
        }

        new public global::Client.EBiz.Order.CLS_Children EnsureChildren(bool @try = false)
        {
            return base.EnsureChildren<global::Client.EBiz.Order.CLS_Children>(@try);
        }

        public partial class CLS_Children : global::XData.XChildSequence
        {
            public global::Client.EBiz.Order.CLS_Children.CLS_TotalPrice C_TotalPrice
            {
                get
                {
                    return base.TryGetChild(0) as global::Client.EBiz.Order.CLS_Children.CLS_TotalPrice;
                }

                set
                {
                    if (value == null)
                        base.RemoveChild(0);
                    else
                        base.AddOrSetChild(value);
                }
            }

            public global::Client.EBiz.Order.CLS_Children.CLS_TotalPrice EnsureC_TotalPrice(bool @try = false)
            {
                return C_TotalPrice ?? (C_TotalPrice = base.CreateChild<global::Client.EBiz.Order.CLS_Children.CLS_TotalPrice>(0, @try));
            }

            public global::Client.Common.Money CT_TotalPrice
            {
                get
                {
                    var obj = C_TotalPrice;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureC_TotalPrice().Type = value;
                }
            }

            public partial class CLS_TotalPrice : global::XData.XLocalElement
            {
                new public global::Client.Common.Money Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.Money;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.ElementInfo ThisInfo = new global::XData.ElementInfo(typeof (global::Client.EBiz.Order.CLS_Children.CLS_TotalPrice), false, "{http://example.com/ebiz}Order.#{}.TotalPrice", (global::XData.ChildKind)2, false, 0, new global::XData.FullName(null, "TotalPrice"), false, global::Client.Common.Money.ThisInfo, null, null, null, global::XDataProgramInfo.Instance);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Order.CLS_Children.CLS_TotalPrice>();
                        return ThisInfo;
                    }
                }
            }

            public partial class CLSITEM_OrderDetailList : global::XData.XLocalElement
            {
                new public global::Client.EBiz.OrderDetail Type
                {
                    get
                    {
                        return base.GenericType as global::Client.EBiz.OrderDetail;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.ElementInfo ThisInfo = new global::XData.ElementInfo(typeof (global::Client.EBiz.Order.CLS_Children.CLSITEM_OrderDetailList), false, "{http://example.com/ebiz}Order.#{}.OrderDetailList", (global::XData.ChildKind)2, false, 1, new global::XData.FullName(null, "OrderDetail"), false, global::Client.EBiz.OrderDetail.ThisInfo, null, null, null, global::XDataProgramInfo.Instance);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Order.CLS_Children.CLSITEM_OrderDetailList>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.EBiz.Order.CLS_Children.CLS_OrderDetailList C_OrderDetailList
            {
                get
                {
                    return base.TryGetChild(1) as global::Client.EBiz.Order.CLS_Children.CLS_OrderDetailList;
                }

                set
                {
                    if (value == null)
                        base.RemoveChild(1);
                    else
                        base.AddOrSetChild(value);
                }
            }

            public global::Client.EBiz.Order.CLS_Children.CLS_OrderDetailList EnsureC_OrderDetailList(bool @try = false)
            {
                return C_OrderDetailList ?? (C_OrderDetailList = base.CreateChild<global::Client.EBiz.Order.CLS_Children.CLS_OrderDetailList>(1, @try));
            }

            public partial class CLS_OrderDetailList : global::XData.XChildList<global::Client.EBiz.Order.CLS_Children.CLSITEM_OrderDetailList>
            {
                public global::Client.EBiz.Order.CLS_Children.CLSITEM_OrderDetailList CreateItem()
                {
                    return base.CreateItem<global::Client.EBiz.Order.CLS_Children.CLSITEM_OrderDetailList>();
                }

                public global::Client.EBiz.Order.CLS_Children.CLSITEM_OrderDetailList CreateAndAddItem()
                {
                    return base.CreateAndAddItem<global::Client.EBiz.Order.CLS_Children.CLSITEM_OrderDetailList>();
                }

                public global::Client.EBiz.Order.CLS_Children.CLS_OrderDetailList Add(global::System.Action<global::Client.EBiz.Order.CLS_Children.CLSITEM_OrderDetailList> itemSetter)
                {
                    base.Add<global::Client.EBiz.Order.CLS_Children.CLSITEM_OrderDetailList>(itemSetter);
                    return this;
                }

                public global::Client.EBiz.Order.CLS_Children.CLS_OrderDetailList AddRange<TItemValue>(global::System.Collections.Generic.IEnumerable<TItemValue> itemValues, global::System.Action<global::Client.EBiz.Order.CLS_Children.CLSITEM_OrderDetailList, TItemValue> itemSetter)
                {
                    base.AddRange<global::Client.EBiz.Order.CLS_Children.CLSITEM_OrderDetailList, TItemValue>(itemValues, itemSetter);
                    return this;
                }

                public static readonly global::XData.ChildListInfo ThisInfo = new global::XData.ChildListInfo(typeof (global::Client.EBiz.Order.CLS_Children.CLS_OrderDetailList), "{http://example.com/ebiz}Order.#{}.OrderDetailList", false, 1, 1UL, 18446744073709551615UL, global::Client.EBiz.Order.CLS_Children.CLSITEM_OrderDetailList.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Order.CLS_Children.CLS_OrderDetailList>();
                        return ThisInfo;
                    }
                }
            }

            public static readonly global::XData.ChildStructInfo ThisInfo = new global::XData.ChildStructInfo(typeof (global::Client.EBiz.Order.CLS_Children), "{http://example.com/ebiz}Order.#{}", (global::XData.ChildKind)4, false, -1, new global::XData.ChildInfo[]
            {
            global::Client.EBiz.Order.CLS_Children.CLS_TotalPrice.ThisInfo, global::Client.EBiz.Order.CLS_Children.CLS_OrderDetailList.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Order.CLS_Children>();
                    return ThisInfo;
                }
            }
        }

        new public static readonly global::XData.ComplexTypeInfo ThisInfo = new global::XData.ComplexTypeInfo(typeof (global::Client.EBiz.Order), false, new global::XData.FullName("http://example.com/ebiz", "Order"), global::XData.XComplexType.ThisInfo, global::Client.EBiz.Order.CLS_Attributes.ThisInfo, global::Client.EBiz.Order.CLS_Children.ThisInfo);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Order>();
                return ThisInfo;
            }
        }
    }

    public partial class Customer : global::Client.EBiz.Contact
    {
        public global::Client.EBiz.Customer.CLS_Attributes.CLS_Reputation A_Reputation
        {
            get
            {
                return base.TryGetAttribute("Reputation") as global::Client.EBiz.Customer.CLS_Attributes.CLS_Reputation;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("Reputation");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.EBiz.Customer.CLS_Attributes.CLS_Reputation EnsureA_Reputation(bool @try = false)
        {
            return A_Reputation ?? (A_Reputation = base.CreateAttribute<global::Client.EBiz.Customer.CLS_Attributes.CLS_Reputation>("Reputation", @try));
        }

        public global::Client.Common.Reputation AT_Reputation
        {
            get
            {
                var obj = A_Reputation;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_Reputation().Type = value;
            }
        }

        new public global::Client.EBiz.Customer.CLS_Attributes Attributes
        {
            get
            {
                return base.GenericAttributes as global::Client.EBiz.Customer.CLS_Attributes;
            }

            set
            {
                base.GenericAttributes = value;
            }
        }

        new public global::Client.EBiz.Customer.CLS_Attributes EnsureAttributes(bool @try = false)
        {
            return base.EnsureAttributes<global::Client.EBiz.Customer.CLS_Attributes>(@try);
        }

        new public partial class CLS_Attributes : global::Client.EBiz.Contact.CLS_Attributes
        {
            public global::Client.EBiz.Customer.CLS_Attributes.CLS_Reputation A_Reputation
            {
                get
                {
                    return base.TryGetAttribute("Reputation") as global::Client.EBiz.Customer.CLS_Attributes.CLS_Reputation;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("Reputation");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.EBiz.Customer.CLS_Attributes.CLS_Reputation EnsureA_Reputation(bool @try = false)
            {
                return A_Reputation ?? (A_Reputation = base.CreateAttribute<global::Client.EBiz.Customer.CLS_Attributes.CLS_Reputation>("Reputation", @try));
            }

            public global::Client.Common.Reputation AT_Reputation
            {
                get
                {
                    var obj = A_Reputation;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_Reputation().Type = value;
                }
            }

            public partial class CLS_Reputation : global::XData.XAttribute
            {
                new public global::Client.Common.Reputation Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.Reputation;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.EBiz.Customer.CLS_Attributes.CLS_Reputation), "{http://example.com/ebiz}Customer.[].Reputation", "Reputation", false, false, global::Client.Common.Reputation.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Customer.CLS_Attributes.CLS_Reputation>();
                        return ThisInfo;
                    }
                }
            }

            new public static readonly global::XData.AttributeSetInfo ThisInfo = new global::XData.AttributeSetInfo(typeof (global::Client.EBiz.Customer.CLS_Attributes), "{http://example.com/ebiz}Customer.[]", new global::XData.AttributeInfo[]
            {
            global::Client.EBiz.Contact.CLS_Attributes.CLS_Id.ThisInfo, global::Client.EBiz.Contact.CLS_Attributes.CLS_Name.ThisInfo, global::Client.EBiz.Contact.CLS_Attributes.CLS_Email.ThisInfo, global::Client.EBiz.Contact.CLS_Attributes.CLS_RegDate.ThisInfo, global::Client.EBiz.Customer.CLS_Attributes.CLS_Reputation.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Customer.CLS_Attributes>();
                    return ThisInfo;
                }
            }
        }

        public global::Client.EBiz.Customer.CLS_Children.CLS_OrderList C_OrderList
        {
            get
            {
                return base.TryGetChild(2) as global::Client.EBiz.Customer.CLS_Children.CLS_OrderList;
            }

            set
            {
                if (value == null)
                    base.RemoveChild(2);
                else
                    base.AddOrSetChild(value);
            }
        }

        public global::Client.EBiz.Customer.CLS_Children.CLS_OrderList EnsureC_OrderList(bool @try = false)
        {
            return C_OrderList ?? (C_OrderList = base.CreateChild<global::Client.EBiz.Customer.CLS_Children.CLS_OrderList>(2, @try));
        }

        new public global::Client.EBiz.Customer.CLS_Children Children
        {
            get
            {
                return base.GenericChildren as global::Client.EBiz.Customer.CLS_Children;
            }

            set
            {
                base.GenericChildren = value;
            }
        }

        new public global::Client.EBiz.Customer.CLS_Children EnsureChildren(bool @try = false)
        {
            return base.EnsureChildren<global::Client.EBiz.Customer.CLS_Children>(@try);
        }

        new public partial class CLS_Children : global::Client.EBiz.Contact.CLS_Children
        {
            public partial class CLSITEM_OrderList : global::XData.XLocalElement
            {
                new public global::Client.EBiz.Order Type
                {
                    get
                    {
                        return base.GenericType as global::Client.EBiz.Order;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.ElementInfo ThisInfo = new global::XData.ElementInfo(typeof (global::Client.EBiz.Customer.CLS_Children.CLSITEM_OrderList), false, "{http://example.com/ebiz}Customer.#{}.OrderList", (global::XData.ChildKind)2, true, 2, new global::XData.FullName(null, "Order"), false, global::Client.EBiz.Order.ThisInfo, null, null, null, global::XDataProgramInfo.Instance);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Customer.CLS_Children.CLSITEM_OrderList>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.EBiz.Customer.CLS_Children.CLS_OrderList C_OrderList
            {
                get
                {
                    return base.TryGetChild(2) as global::Client.EBiz.Customer.CLS_Children.CLS_OrderList;
                }

                set
                {
                    if (value == null)
                        base.RemoveChild(2);
                    else
                        base.AddOrSetChild(value);
                }
            }

            public global::Client.EBiz.Customer.CLS_Children.CLS_OrderList EnsureC_OrderList(bool @try = false)
            {
                return C_OrderList ?? (C_OrderList = base.CreateChild<global::Client.EBiz.Customer.CLS_Children.CLS_OrderList>(2, @try));
            }

            public partial class CLS_OrderList : global::XData.XChildList<global::Client.EBiz.Customer.CLS_Children.CLSITEM_OrderList>
            {
                public global::Client.EBiz.Customer.CLS_Children.CLSITEM_OrderList CreateItem()
                {
                    return base.CreateItem<global::Client.EBiz.Customer.CLS_Children.CLSITEM_OrderList>();
                }

                public global::Client.EBiz.Customer.CLS_Children.CLSITEM_OrderList CreateAndAddItem()
                {
                    return base.CreateAndAddItem<global::Client.EBiz.Customer.CLS_Children.CLSITEM_OrderList>();
                }

                public global::Client.EBiz.Customer.CLS_Children.CLS_OrderList Add(global::System.Action<global::Client.EBiz.Customer.CLS_Children.CLSITEM_OrderList> itemSetter)
                {
                    base.Add<global::Client.EBiz.Customer.CLS_Children.CLSITEM_OrderList>(itemSetter);
                    return this;
                }

                public global::Client.EBiz.Customer.CLS_Children.CLS_OrderList AddRange<TItemValue>(global::System.Collections.Generic.IEnumerable<TItemValue> itemValues, global::System.Action<global::Client.EBiz.Customer.CLS_Children.CLSITEM_OrderList, TItemValue> itemSetter)
                {
                    base.AddRange<global::Client.EBiz.Customer.CLS_Children.CLSITEM_OrderList, TItemValue>(itemValues, itemSetter);
                    return this;
                }

                public static readonly global::XData.ChildListInfo ThisInfo = new global::XData.ChildListInfo(typeof (global::Client.EBiz.Customer.CLS_Children.CLS_OrderList), "{http://example.com/ebiz}Customer.#{}.OrderList", true, 2, 0UL, 18446744073709551615UL, global::Client.EBiz.Customer.CLS_Children.CLSITEM_OrderList.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Customer.CLS_Children.CLS_OrderList>();
                        return ThisInfo;
                    }
                }
            }

            new public static readonly global::XData.ChildStructInfo ThisInfo = new global::XData.ChildStructInfo(typeof (global::Client.EBiz.Customer.CLS_Children), "{http://example.com/ebiz}Customer.#{}", (global::XData.ChildKind)4, false, -1, new global::XData.ChildInfo[]
            {
            global::Client.EBiz.Contact.CLS_Children.CLS_PhoneList.ThisInfo, global::Client.EBiz.Contact.CLS_Children.CLS_Address.ThisInfo, global::Client.EBiz.Customer.CLS_Children.CLS_OrderList.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Customer.CLS_Children>();
                    return ThisInfo;
                }
            }
        }

        new public static readonly global::XData.ComplexTypeInfo ThisInfo = new global::XData.ComplexTypeInfo(typeof (global::Client.EBiz.Customer), false, new global::XData.FullName("http://example.com/ebiz", "Customer"), global::Client.EBiz.Contact.ThisInfo, global::Client.EBiz.Customer.CLS_Attributes.ThisInfo, global::Client.EBiz.Customer.CLS_Children.ThisInfo);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Customer>();
                return ThisInfo;
            }
        }
    }

    public partial class Supplier : global::Client.EBiz.Contact
    {
        public global::Client.EBiz.Supplier.CLS_Attributes.CLS_BankAccount A_BankAccount
        {
            get
            {
                return base.TryGetAttribute("BankAccount") as global::Client.EBiz.Supplier.CLS_Attributes.CLS_BankAccount;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("BankAccount");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.EBiz.Supplier.CLS_Attributes.CLS_BankAccount EnsureA_BankAccount(bool @try = false)
        {
            return A_BankAccount ?? (A_BankAccount = base.CreateAttribute<global::Client.EBiz.Supplier.CLS_Attributes.CLS_BankAccount>("BankAccount", @try));
        }

        public global::Client.Common.String40 AT_BankAccount
        {
            get
            {
                var obj = A_BankAccount;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_BankAccount().Type = value;
            }
        }

        public global::Client.EBiz.Supplier.CLS_Attributes.CLS_ProductIdList A_ProductIdList
        {
            get
            {
                return base.TryGetAttribute("ProductIdList") as global::Client.EBiz.Supplier.CLS_Attributes.CLS_ProductIdList;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("ProductIdList");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.EBiz.Supplier.CLS_Attributes.CLS_ProductIdList EnsureA_ProductIdList(bool @try = false)
        {
            return A_ProductIdList ?? (A_ProductIdList = base.CreateAttribute<global::Client.EBiz.Supplier.CLS_Attributes.CLS_ProductIdList>("ProductIdList", @try));
        }

        public global::Client.Common.PositiveInt32List AT_ProductIdList
        {
            get
            {
                var obj = A_ProductIdList;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_ProductIdList().Type = value;
            }
        }

        new public global::Client.EBiz.Supplier.CLS_Attributes Attributes
        {
            get
            {
                return base.GenericAttributes as global::Client.EBiz.Supplier.CLS_Attributes;
            }

            set
            {
                base.GenericAttributes = value;
            }
        }

        new public global::Client.EBiz.Supplier.CLS_Attributes EnsureAttributes(bool @try = false)
        {
            return base.EnsureAttributes<global::Client.EBiz.Supplier.CLS_Attributes>(@try);
        }

        new public partial class CLS_Attributes : global::Client.EBiz.Contact.CLS_Attributes
        {
            public global::Client.EBiz.Supplier.CLS_Attributes.CLS_BankAccount A_BankAccount
            {
                get
                {
                    return base.TryGetAttribute("BankAccount") as global::Client.EBiz.Supplier.CLS_Attributes.CLS_BankAccount;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("BankAccount");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.EBiz.Supplier.CLS_Attributes.CLS_BankAccount EnsureA_BankAccount(bool @try = false)
            {
                return A_BankAccount ?? (A_BankAccount = base.CreateAttribute<global::Client.EBiz.Supplier.CLS_Attributes.CLS_BankAccount>("BankAccount", @try));
            }

            public global::Client.Common.String40 AT_BankAccount
            {
                get
                {
                    var obj = A_BankAccount;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_BankAccount().Type = value;
                }
            }

            public partial class CLS_BankAccount : global::XData.XAttribute
            {
                new public global::Client.Common.String40 Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.String40;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.EBiz.Supplier.CLS_Attributes.CLS_BankAccount), "{http://example.com/ebiz}Supplier.[].BankAccount", "BankAccount", false, false, global::Client.Common.String40.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Supplier.CLS_Attributes.CLS_BankAccount>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.EBiz.Supplier.CLS_Attributes.CLS_ProductIdList A_ProductIdList
            {
                get
                {
                    return base.TryGetAttribute("ProductIdList") as global::Client.EBiz.Supplier.CLS_Attributes.CLS_ProductIdList;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("ProductIdList");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.EBiz.Supplier.CLS_Attributes.CLS_ProductIdList EnsureA_ProductIdList(bool @try = false)
            {
                return A_ProductIdList ?? (A_ProductIdList = base.CreateAttribute<global::Client.EBiz.Supplier.CLS_Attributes.CLS_ProductIdList>("ProductIdList", @try));
            }

            public global::Client.Common.PositiveInt32List AT_ProductIdList
            {
                get
                {
                    var obj = A_ProductIdList;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_ProductIdList().Type = value;
                }
            }

            public partial class CLS_ProductIdList : global::XData.XAttribute
            {
                new public global::Client.Common.PositiveInt32List Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.PositiveInt32List;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.EBiz.Supplier.CLS_Attributes.CLS_ProductIdList), "{http://example.com/ebiz}Supplier.[].ProductIdList", "ProductIdList", true, false, global::Client.Common.PositiveInt32List.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Supplier.CLS_Attributes.CLS_ProductIdList>();
                        return ThisInfo;
                    }
                }
            }

            new public static readonly global::XData.AttributeSetInfo ThisInfo = new global::XData.AttributeSetInfo(typeof (global::Client.EBiz.Supplier.CLS_Attributes), "{http://example.com/ebiz}Supplier.[]", new global::XData.AttributeInfo[]
            {
            global::Client.EBiz.Contact.CLS_Attributes.CLS_Id.ThisInfo, global::Client.EBiz.Contact.CLS_Attributes.CLS_Name.ThisInfo, global::Client.EBiz.Contact.CLS_Attributes.CLS_Email.ThisInfo, global::Client.EBiz.Contact.CLS_Attributes.CLS_RegDate.ThisInfo, global::Client.EBiz.Supplier.CLS_Attributes.CLS_BankAccount.ThisInfo, global::Client.EBiz.Supplier.CLS_Attributes.CLS_ProductIdList.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Supplier.CLS_Attributes>();
                    return ThisInfo;
                }
            }
        }

        new public static readonly global::XData.ComplexTypeInfo ThisInfo = new global::XData.ComplexTypeInfo(typeof (global::Client.EBiz.Supplier), false, new global::XData.FullName("http://example.com/ebiz", "Supplier"), global::Client.EBiz.Contact.ThisInfo, global::Client.EBiz.Supplier.CLS_Attributes.ThisInfo, global::Client.EBiz.Contact.CLS_Children.ThisInfo);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Supplier>();
                return ThisInfo;
            }
        }
    }

    public abstract partial class Product : global::XData.XComplexType
    {
        public global::Client.EBiz.Product.CLS_Attributes.CLS_Id A_Id
        {
            get
            {
                return base.TryGetAttribute("Id") as global::Client.EBiz.Product.CLS_Attributes.CLS_Id;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("Id");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.EBiz.Product.CLS_Attributes.CLS_Id EnsureA_Id(bool @try = false)
        {
            return A_Id ?? (A_Id = base.CreateAttribute<global::Client.EBiz.Product.CLS_Attributes.CLS_Id>("Id", @try));
        }

        public global::Client.Common.PositiveInt32 AT_Id
        {
            get
            {
                var obj = A_Id;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_Id().Type = value;
            }
        }

        public global::Client.EBiz.Product.CLS_Attributes.CLS_Name A_Name
        {
            get
            {
                return base.TryGetAttribute("Name") as global::Client.EBiz.Product.CLS_Attributes.CLS_Name;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("Name");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.EBiz.Product.CLS_Attributes.CLS_Name EnsureA_Name(bool @try = false)
        {
            return A_Name ?? (A_Name = base.CreateAttribute<global::Client.EBiz.Product.CLS_Attributes.CLS_Name>("Name", @try));
        }

        public global::Client.Common.String20 AT_Name
        {
            get
            {
                var obj = A_Name;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_Name().Type = value;
            }
        }

        public global::Client.EBiz.Product.CLS_Attributes.CLS_StockQuantity A_StockQuantity
        {
            get
            {
                return base.TryGetAttribute("StockQuantity") as global::Client.EBiz.Product.CLS_Attributes.CLS_StockQuantity;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("StockQuantity");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.EBiz.Product.CLS_Attributes.CLS_StockQuantity EnsureA_StockQuantity(bool @try = false)
        {
            return A_StockQuantity ?? (A_StockQuantity = base.CreateAttribute<global::Client.EBiz.Product.CLS_Attributes.CLS_StockQuantity>("StockQuantity", @try));
        }

        public global::Client.Common.NonNegativeInt32 AT_StockQuantity
        {
            get
            {
                var obj = A_StockQuantity;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_StockQuantity().Type = value;
            }
        }

        new public global::Client.EBiz.Product.CLS_Attributes Attributes
        {
            get
            {
                return base.GenericAttributes as global::Client.EBiz.Product.CLS_Attributes;
            }

            set
            {
                base.GenericAttributes = value;
            }
        }

        new public global::Client.EBiz.Product.CLS_Attributes EnsureAttributes(bool @try = false)
        {
            return base.EnsureAttributes<global::Client.EBiz.Product.CLS_Attributes>(@try);
        }

        public partial class CLS_Attributes : global::XData.XAttributeSet
        {
            public global::Client.EBiz.Product.CLS_Attributes.CLS_Id A_Id
            {
                get
                {
                    return base.TryGetAttribute("Id") as global::Client.EBiz.Product.CLS_Attributes.CLS_Id;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("Id");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.EBiz.Product.CLS_Attributes.CLS_Id EnsureA_Id(bool @try = false)
            {
                return A_Id ?? (A_Id = base.CreateAttribute<global::Client.EBiz.Product.CLS_Attributes.CLS_Id>("Id", @try));
            }

            public global::Client.Common.PositiveInt32 AT_Id
            {
                get
                {
                    var obj = A_Id;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_Id().Type = value;
                }
            }

            public partial class CLS_Id : global::XData.XAttribute
            {
                new public global::Client.Common.PositiveInt32 Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.PositiveInt32;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.EBiz.Product.CLS_Attributes.CLS_Id), "{http://example.com/ebiz}Product.[].Id", "Id", false, false, global::Client.Common.PositiveInt32.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Product.CLS_Attributes.CLS_Id>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.EBiz.Product.CLS_Attributes.CLS_Name A_Name
            {
                get
                {
                    return base.TryGetAttribute("Name") as global::Client.EBiz.Product.CLS_Attributes.CLS_Name;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("Name");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.EBiz.Product.CLS_Attributes.CLS_Name EnsureA_Name(bool @try = false)
            {
                return A_Name ?? (A_Name = base.CreateAttribute<global::Client.EBiz.Product.CLS_Attributes.CLS_Name>("Name", @try));
            }

            public global::Client.Common.String20 AT_Name
            {
                get
                {
                    var obj = A_Name;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_Name().Type = value;
                }
            }

            public partial class CLS_Name : global::XData.XAttribute
            {
                new public global::Client.Common.String20 Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.String20;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.EBiz.Product.CLS_Attributes.CLS_Name), "{http://example.com/ebiz}Product.[].Name", "Name", false, false, global::Client.Common.String20.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Product.CLS_Attributes.CLS_Name>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.EBiz.Product.CLS_Attributes.CLS_StockQuantity A_StockQuantity
            {
                get
                {
                    return base.TryGetAttribute("StockQuantity") as global::Client.EBiz.Product.CLS_Attributes.CLS_StockQuantity;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("StockQuantity");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.EBiz.Product.CLS_Attributes.CLS_StockQuantity EnsureA_StockQuantity(bool @try = false)
            {
                return A_StockQuantity ?? (A_StockQuantity = base.CreateAttribute<global::Client.EBiz.Product.CLS_Attributes.CLS_StockQuantity>("StockQuantity", @try));
            }

            public global::Client.Common.NonNegativeInt32 AT_StockQuantity
            {
                get
                {
                    var obj = A_StockQuantity;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_StockQuantity().Type = value;
                }
            }

            public partial class CLS_StockQuantity : global::XData.XAttribute
            {
                new public global::Client.Common.NonNegativeInt32 Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.NonNegativeInt32;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.EBiz.Product.CLS_Attributes.CLS_StockQuantity), "{http://example.com/ebiz}Product.[].StockQuantity", "StockQuantity", false, false, global::Client.Common.NonNegativeInt32.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Product.CLS_Attributes.CLS_StockQuantity>();
                        return ThisInfo;
                    }
                }
            }

            public static readonly global::XData.AttributeSetInfo ThisInfo = new global::XData.AttributeSetInfo(typeof (global::Client.EBiz.Product.CLS_Attributes), "{http://example.com/ebiz}Product.[]", new global::XData.AttributeInfo[]
            {
            global::Client.EBiz.Product.CLS_Attributes.CLS_Id.ThisInfo, global::Client.EBiz.Product.CLS_Attributes.CLS_Name.ThisInfo, global::Client.EBiz.Product.CLS_Attributes.CLS_StockQuantity.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Product.CLS_Attributes>();
                    return ThisInfo;
                }
            }
        }

        public global::Client.EBiz.Product.CLS_Children.CLS_Price C_Price
        {
            get
            {
                return base.TryGetChild(0) as global::Client.EBiz.Product.CLS_Children.CLS_Price;
            }

            set
            {
                if (value == null)
                    base.RemoveChild(0);
                else
                    base.AddOrSetChild(value);
            }
        }

        public global::Client.EBiz.Product.CLS_Children.CLS_Price EnsureC_Price(bool @try = false)
        {
            return C_Price ?? (C_Price = base.CreateChild<global::Client.EBiz.Product.CLS_Children.CLS_Price>(0, @try));
        }

        public global::Client.Common.Money CT_Price
        {
            get
            {
                var obj = C_Price;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureC_Price().Type = value;
            }
        }

        public global::Client.EBiz.Product.CLS_Children.CLS_Image C_Image
        {
            get
            {
                return base.TryGetChild(1) as global::Client.EBiz.Product.CLS_Children.CLS_Image;
            }

            set
            {
                if (value == null)
                    base.RemoveChild(1);
                else
                    base.AddOrSetChild(value);
            }
        }

        public global::Client.EBiz.Product.CLS_Children.CLS_Image EnsureC_Image(bool @try = false)
        {
            return C_Image ?? (C_Image = base.CreateChild<global::Client.EBiz.Product.CLS_Children.CLS_Image>(1, @try));
        }

        public global::Client.Common.Image CT_Image
        {
            get
            {
                var obj = C_Image;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureC_Image().Type = value;
            }
        }

        new public global::Client.EBiz.Product.CLS_Children Children
        {
            get
            {
                return base.GenericChildren as global::Client.EBiz.Product.CLS_Children;
            }

            set
            {
                base.GenericChildren = value;
            }
        }

        new public global::Client.EBiz.Product.CLS_Children EnsureChildren(bool @try = false)
        {
            return base.EnsureChildren<global::Client.EBiz.Product.CLS_Children>(@try);
        }

        public partial class CLS_Children : global::XData.XChildSequence
        {
            public global::Client.EBiz.Product.CLS_Children.CLS_Price C_Price
            {
                get
                {
                    return base.TryGetChild(0) as global::Client.EBiz.Product.CLS_Children.CLS_Price;
                }

                set
                {
                    if (value == null)
                        base.RemoveChild(0);
                    else
                        base.AddOrSetChild(value);
                }
            }

            public global::Client.EBiz.Product.CLS_Children.CLS_Price EnsureC_Price(bool @try = false)
            {
                return C_Price ?? (C_Price = base.CreateChild<global::Client.EBiz.Product.CLS_Children.CLS_Price>(0, @try));
            }

            public global::Client.Common.Money CT_Price
            {
                get
                {
                    var obj = C_Price;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureC_Price().Type = value;
                }
            }

            public partial class CLS_Price : global::XData.XLocalElement
            {
                new public global::Client.Common.Money Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.Money;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.ElementInfo ThisInfo = new global::XData.ElementInfo(typeof (global::Client.EBiz.Product.CLS_Children.CLS_Price), false, "{http://example.com/ebiz}Product.#{}.Price", (global::XData.ChildKind)2, false, 0, new global::XData.FullName(null, "Price"), false, global::Client.Common.Money.ThisInfo, null, null, null, global::XDataProgramInfo.Instance);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Product.CLS_Children.CLS_Price>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.EBiz.Product.CLS_Children.CLS_Image C_Image
            {
                get
                {
                    return base.TryGetChild(1) as global::Client.EBiz.Product.CLS_Children.CLS_Image;
                }

                set
                {
                    if (value == null)
                        base.RemoveChild(1);
                    else
                        base.AddOrSetChild(value);
                }
            }

            public global::Client.EBiz.Product.CLS_Children.CLS_Image EnsureC_Image(bool @try = false)
            {
                return C_Image ?? (C_Image = base.CreateChild<global::Client.EBiz.Product.CLS_Children.CLS_Image>(1, @try));
            }

            public global::Client.Common.Image CT_Image
            {
                get
                {
                    var obj = C_Image;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureC_Image().Type = value;
                }
            }

            public partial class CLS_Image : global::XData.XLocalElement
            {
                new public global::Client.Common.Image Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.Image;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.ElementInfo ThisInfo = new global::XData.ElementInfo(typeof (global::Client.EBiz.Product.CLS_Children.CLS_Image), false, "{http://example.com/ebiz}Product.#{}.Image", (global::XData.ChildKind)2, true, 1, new global::XData.FullName(null, "Image"), false, global::Client.Common.Image.ThisInfo, null, null, null, global::XDataProgramInfo.Instance);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Product.CLS_Children.CLS_Image>();
                        return ThisInfo;
                    }
                }
            }

            public static readonly global::XData.ChildStructInfo ThisInfo = new global::XData.ChildStructInfo(typeof (global::Client.EBiz.Product.CLS_Children), "{http://example.com/ebiz}Product.#{}", (global::XData.ChildKind)4, false, -1, new global::XData.ChildInfo[]
            {
            global::Client.EBiz.Product.CLS_Children.CLS_Price.ThisInfo, global::Client.EBiz.Product.CLS_Children.CLS_Image.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Product.CLS_Children>();
                    return ThisInfo;
                }
            }
        }

        new public static readonly global::XData.ComplexTypeInfo ThisInfo = new global::XData.ComplexTypeInfo(typeof (global::Client.EBiz.Product), true, new global::XData.FullName("http://example.com/ebiz", "Product"), global::XData.XComplexType.ThisInfo, global::Client.EBiz.Product.CLS_Attributes.ThisInfo, global::Client.EBiz.Product.CLS_Children.ThisInfo);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                return ThisInfo;
            }
        }
    }

    public partial class SportEquipment : global::Client.EBiz.Product
    {
        public global::Client.EBiz.SportEquipment.CLS_Attributes.CLS_Applicability A_Applicability
        {
            get
            {
                return base.TryGetAttribute("Applicability") as global::Client.EBiz.SportEquipment.CLS_Attributes.CLS_Applicability;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("Applicability");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.EBiz.SportEquipment.CLS_Attributes.CLS_Applicability EnsureA_Applicability(bool @try = false)
        {
            return A_Applicability ?? (A_Applicability = base.CreateAttribute<global::Client.EBiz.SportEquipment.CLS_Attributes.CLS_Applicability>("Applicability", @try));
        }

        public global::Client.Common.String20 AT_Applicability
        {
            get
            {
                var obj = A_Applicability;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_Applicability().Type = value;
            }
        }

        new public global::Client.EBiz.SportEquipment.CLS_Attributes Attributes
        {
            get
            {
                return base.GenericAttributes as global::Client.EBiz.SportEquipment.CLS_Attributes;
            }

            set
            {
                base.GenericAttributes = value;
            }
        }

        new public global::Client.EBiz.SportEquipment.CLS_Attributes EnsureAttributes(bool @try = false)
        {
            return base.EnsureAttributes<global::Client.EBiz.SportEquipment.CLS_Attributes>(@try);
        }

        new public partial class CLS_Attributes : global::Client.EBiz.Product.CLS_Attributes
        {
            public global::Client.EBiz.SportEquipment.CLS_Attributes.CLS_Applicability A_Applicability
            {
                get
                {
                    return base.TryGetAttribute("Applicability") as global::Client.EBiz.SportEquipment.CLS_Attributes.CLS_Applicability;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("Applicability");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.EBiz.SportEquipment.CLS_Attributes.CLS_Applicability EnsureA_Applicability(bool @try = false)
            {
                return A_Applicability ?? (A_Applicability = base.CreateAttribute<global::Client.EBiz.SportEquipment.CLS_Attributes.CLS_Applicability>("Applicability", @try));
            }

            public global::Client.Common.String20 AT_Applicability
            {
                get
                {
                    var obj = A_Applicability;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_Applicability().Type = value;
                }
            }

            public partial class CLS_Applicability : global::XData.XAttribute
            {
                new public global::Client.Common.String20 Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.String20;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.EBiz.SportEquipment.CLS_Attributes.CLS_Applicability), "{http://example.com/ebiz}SportEquipment.[].Applicability", "Applicability", false, false, global::Client.Common.String20.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.SportEquipment.CLS_Attributes.CLS_Applicability>();
                        return ThisInfo;
                    }
                }
            }

            new public static readonly global::XData.AttributeSetInfo ThisInfo = new global::XData.AttributeSetInfo(typeof (global::Client.EBiz.SportEquipment.CLS_Attributes), "{http://example.com/ebiz}SportEquipment.[]", new global::XData.AttributeInfo[]
            {
            global::Client.EBiz.Product.CLS_Attributes.CLS_Id.ThisInfo, global::Client.EBiz.Product.CLS_Attributes.CLS_Name.ThisInfo, global::Client.EBiz.Product.CLS_Attributes.CLS_StockQuantity.ThisInfo, global::Client.EBiz.SportEquipment.CLS_Attributes.CLS_Applicability.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.SportEquipment.CLS_Attributes>();
                    return ThisInfo;
                }
            }
        }

        new public static readonly global::XData.ComplexTypeInfo ThisInfo = new global::XData.ComplexTypeInfo(typeof (global::Client.EBiz.SportEquipment), false, new global::XData.FullName("http://example.com/ebiz", "SportEquipment"), global::Client.EBiz.Product.ThisInfo, global::Client.EBiz.SportEquipment.CLS_Attributes.ThisInfo, global::Client.EBiz.Product.CLS_Children.ThisInfo);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.SportEquipment>();
                return ThisInfo;
            }
        }
    }

    public partial class Shoe : global::Client.EBiz.Product
    {
        public global::Client.EBiz.Shoe.CLS_Attributes.CLS_Gender A_Gender
        {
            get
            {
                return base.TryGetAttribute("Gender") as global::Client.EBiz.Shoe.CLS_Attributes.CLS_Gender;
            }

            set
            {
                if (value == null)
                    base.RemoveAttribute("Gender");
                else
                    base.AddOrSetAttribute(value);
            }
        }

        public global::Client.EBiz.Shoe.CLS_Attributes.CLS_Gender EnsureA_Gender(bool @try = false)
        {
            return A_Gender ?? (A_Gender = base.CreateAttribute<global::Client.EBiz.Shoe.CLS_Attributes.CLS_Gender>("Gender", @try));
        }

        public global::Client.Common.Gender AT_Gender
        {
            get
            {
                var obj = A_Gender;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureA_Gender().Type = value;
            }
        }

        new public global::Client.EBiz.Shoe.CLS_Attributes Attributes
        {
            get
            {
                return base.GenericAttributes as global::Client.EBiz.Shoe.CLS_Attributes;
            }

            set
            {
                base.GenericAttributes = value;
            }
        }

        new public global::Client.EBiz.Shoe.CLS_Attributes EnsureAttributes(bool @try = false)
        {
            return base.EnsureAttributes<global::Client.EBiz.Shoe.CLS_Attributes>(@try);
        }

        new public partial class CLS_Attributes : global::Client.EBiz.Product.CLS_Attributes
        {
            public global::Client.EBiz.Shoe.CLS_Attributes.CLS_Gender A_Gender
            {
                get
                {
                    return base.TryGetAttribute("Gender") as global::Client.EBiz.Shoe.CLS_Attributes.CLS_Gender;
                }

                set
                {
                    if (value == null)
                        base.RemoveAttribute("Gender");
                    else
                        base.AddOrSetAttribute(value);
                }
            }

            public global::Client.EBiz.Shoe.CLS_Attributes.CLS_Gender EnsureA_Gender(bool @try = false)
            {
                return A_Gender ?? (A_Gender = base.CreateAttribute<global::Client.EBiz.Shoe.CLS_Attributes.CLS_Gender>("Gender", @try));
            }

            public global::Client.Common.Gender AT_Gender
            {
                get
                {
                    var obj = A_Gender;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureA_Gender().Type = value;
                }
            }

            public partial class CLS_Gender : global::XData.XAttribute
            {
                new public global::Client.Common.Gender Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.Gender;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.AttributeInfo ThisInfo = new global::XData.AttributeInfo(typeof (global::Client.EBiz.Shoe.CLS_Attributes.CLS_Gender), "{http://example.com/ebiz}Shoe.[].Gender", "Gender", false, false, global::Client.Common.Gender.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Shoe.CLS_Attributes.CLS_Gender>();
                        return ThisInfo;
                    }
                }
            }

            new public static readonly global::XData.AttributeSetInfo ThisInfo = new global::XData.AttributeSetInfo(typeof (global::Client.EBiz.Shoe.CLS_Attributes), "{http://example.com/ebiz}Shoe.[]", new global::XData.AttributeInfo[]
            {
            global::Client.EBiz.Product.CLS_Attributes.CLS_Id.ThisInfo, global::Client.EBiz.Product.CLS_Attributes.CLS_Name.ThisInfo, global::Client.EBiz.Product.CLS_Attributes.CLS_StockQuantity.ThisInfo, global::Client.EBiz.Shoe.CLS_Attributes.CLS_Gender.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Shoe.CLS_Attributes>();
                    return ThisInfo;
                }
            }
        }

        public global::Client.EBiz.Shoe.CLS_Children.CLS_Size C_Size
        {
            get
            {
                return base.TryGetChild(2) as global::Client.EBiz.Shoe.CLS_Children.CLS_Size;
            }

            set
            {
                if (value == null)
                    base.RemoveChild(2);
                else
                    base.AddOrSetChild(value);
            }
        }

        public global::Client.EBiz.Shoe.CLS_Children.CLS_Size EnsureC_Size(bool @try = false)
        {
            return C_Size ?? (C_Size = base.CreateChild<global::Client.EBiz.Shoe.CLS_Children.CLS_Size>(2, @try));
        }

        public global::Client.Common.ShoeSize CT_Size
        {
            get
            {
                var obj = C_Size;
                if (obj == null)
                    return null;
                return obj.Type;
            }

            set
            {
                EnsureC_Size().Type = value;
            }
        }

        new public global::Client.EBiz.Shoe.CLS_Children Children
        {
            get
            {
                return base.GenericChildren as global::Client.EBiz.Shoe.CLS_Children;
            }

            set
            {
                base.GenericChildren = value;
            }
        }

        new public global::Client.EBiz.Shoe.CLS_Children EnsureChildren(bool @try = false)
        {
            return base.EnsureChildren<global::Client.EBiz.Shoe.CLS_Children>(@try);
        }

        new public partial class CLS_Children : global::Client.EBiz.Product.CLS_Children
        {
            public global::Client.EBiz.Shoe.CLS_Children.CLS_Size C_Size
            {
                get
                {
                    return base.TryGetChild(2) as global::Client.EBiz.Shoe.CLS_Children.CLS_Size;
                }

                set
                {
                    if (value == null)
                        base.RemoveChild(2);
                    else
                        base.AddOrSetChild(value);
                }
            }

            public global::Client.EBiz.Shoe.CLS_Children.CLS_Size EnsureC_Size(bool @try = false)
            {
                return C_Size ?? (C_Size = base.CreateChild<global::Client.EBiz.Shoe.CLS_Children.CLS_Size>(2, @try));
            }

            public global::Client.Common.ShoeSize CT_Size
            {
                get
                {
                    var obj = C_Size;
                    if (obj == null)
                        return null;
                    return obj.Type;
                }

                set
                {
                    EnsureC_Size().Type = value;
                }
            }

            public partial class CLS_Size : global::XData.XLocalElement
            {
                new public global::Client.Common.ShoeSize Type
                {
                    get
                    {
                        return base.GenericType as global::Client.Common.ShoeSize;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.ElementInfo ThisInfo = new global::XData.ElementInfo(typeof (global::Client.EBiz.Shoe.CLS_Children.CLS_Size), false, "{http://example.com/ebiz}Shoe.#{}.Size", (global::XData.ChildKind)2, false, 2, new global::XData.FullName(null, "Size"), false, global::Client.Common.ShoeSize.ThisInfo, null, null, null, global::XDataProgramInfo.Instance);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Shoe.CLS_Children.CLS_Size>();
                        return ThisInfo;
                    }
                }
            }

            new public static readonly global::XData.ChildStructInfo ThisInfo = new global::XData.ChildStructInfo(typeof (global::Client.EBiz.Shoe.CLS_Children), "{http://example.com/ebiz}Shoe.#{}", (global::XData.ChildKind)4, false, -1, new global::XData.ChildInfo[]
            {
            global::Client.EBiz.Product.CLS_Children.CLS_Price.ThisInfo, global::Client.EBiz.Product.CLS_Children.CLS_Image.ThisInfo, global::Client.EBiz.Shoe.CLS_Children.CLS_Size.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Shoe.CLS_Children>();
                    return ThisInfo;
                }
            }
        }

        new public static readonly global::XData.ComplexTypeInfo ThisInfo = new global::XData.ComplexTypeInfo(typeof (global::Client.EBiz.Shoe), false, new global::XData.FullName("http://example.com/ebiz", "Shoe"), global::Client.EBiz.Product.ThisInfo, global::Client.EBiz.Shoe.CLS_Attributes.ThisInfo, global::Client.EBiz.Shoe.CLS_Children.ThisInfo);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.EBiz.Shoe>();
                return ThisInfo;
            }
        }
    }
}

namespace Client.WebApi
{
    public partial class ContactsType : global::XData.XComplexType
    {
        public global::Client.WebApi.ContactsType.CLS_Children.CLS_ContactList C_ContactList
        {
            get
            {
                return base.TryGetChild(0) as global::Client.WebApi.ContactsType.CLS_Children.CLS_ContactList;
            }

            set
            {
                if (value == null)
                    base.RemoveChild(0);
                else
                    base.AddOrSetChild(value);
            }
        }

        public global::Client.WebApi.ContactsType.CLS_Children.CLS_ContactList EnsureC_ContactList(bool @try = false)
        {
            return C_ContactList ?? (C_ContactList = base.CreateChild<global::Client.WebApi.ContactsType.CLS_Children.CLS_ContactList>(0, @try));
        }

        new public global::Client.WebApi.ContactsType.CLS_Children Children
        {
            get
            {
                return base.GenericChildren as global::Client.WebApi.ContactsType.CLS_Children;
            }

            set
            {
                base.GenericChildren = value;
            }
        }

        new public global::Client.WebApi.ContactsType.CLS_Children EnsureChildren(bool @try = false)
        {
            return base.EnsureChildren<global::Client.WebApi.ContactsType.CLS_Children>(@try);
        }

        public partial class CLS_Children : global::XData.XChildSequence
        {
            public partial class CLSITEM_ContactList : global::XData.XLocalElement
            {
                new public global::Client.EBiz.Contact Type
                {
                    get
                    {
                        return base.GenericType as global::Client.EBiz.Contact;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.ElementInfo ThisInfo = new global::XData.ElementInfo(typeof (global::Client.WebApi.ContactsType.CLS_Children.CLSITEM_ContactList), false, "{http://example.com/webapi}ContactsType.#{}.ContactList", (global::XData.ChildKind)2, false, 0, new global::XData.FullName(null, "Contact"), false, global::Client.EBiz.Contact.ThisInfo, null, null, null, global::XDataProgramInfo.Instance);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.WebApi.ContactsType.CLS_Children.CLSITEM_ContactList>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.WebApi.ContactsType.CLS_Children.CLS_ContactList C_ContactList
            {
                get
                {
                    return base.TryGetChild(0) as global::Client.WebApi.ContactsType.CLS_Children.CLS_ContactList;
                }

                set
                {
                    if (value == null)
                        base.RemoveChild(0);
                    else
                        base.AddOrSetChild(value);
                }
            }

            public global::Client.WebApi.ContactsType.CLS_Children.CLS_ContactList EnsureC_ContactList(bool @try = false)
            {
                return C_ContactList ?? (C_ContactList = base.CreateChild<global::Client.WebApi.ContactsType.CLS_Children.CLS_ContactList>(0, @try));
            }

            public partial class CLS_ContactList : global::XData.XChildList<global::Client.WebApi.ContactsType.CLS_Children.CLSITEM_ContactList>
            {
                public global::Client.WebApi.ContactsType.CLS_Children.CLSITEM_ContactList CreateItem()
                {
                    return base.CreateItem<global::Client.WebApi.ContactsType.CLS_Children.CLSITEM_ContactList>();
                }

                public global::Client.WebApi.ContactsType.CLS_Children.CLSITEM_ContactList CreateAndAddItem()
                {
                    return base.CreateAndAddItem<global::Client.WebApi.ContactsType.CLS_Children.CLSITEM_ContactList>();
                }

                public global::Client.WebApi.ContactsType.CLS_Children.CLS_ContactList Add(global::System.Action<global::Client.WebApi.ContactsType.CLS_Children.CLSITEM_ContactList> itemSetter)
                {
                    base.Add<global::Client.WebApi.ContactsType.CLS_Children.CLSITEM_ContactList>(itemSetter);
                    return this;
                }

                public global::Client.WebApi.ContactsType.CLS_Children.CLS_ContactList AddRange<TItemValue>(global::System.Collections.Generic.IEnumerable<TItemValue> itemValues, global::System.Action<global::Client.WebApi.ContactsType.CLS_Children.CLSITEM_ContactList, TItemValue> itemSetter)
                {
                    base.AddRange<global::Client.WebApi.ContactsType.CLS_Children.CLSITEM_ContactList, TItemValue>(itemValues, itemSetter);
                    return this;
                }

                public static readonly global::XData.ChildListInfo ThisInfo = new global::XData.ChildListInfo(typeof (global::Client.WebApi.ContactsType.CLS_Children.CLS_ContactList), "{http://example.com/webapi}ContactsType.#{}.ContactList", false, 0, 1UL, 18446744073709551615UL, global::Client.WebApi.ContactsType.CLS_Children.CLSITEM_ContactList.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.WebApi.ContactsType.CLS_Children.CLS_ContactList>();
                        return ThisInfo;
                    }
                }
            }

            public static readonly global::XData.ChildStructInfo ThisInfo = new global::XData.ChildStructInfo(typeof (global::Client.WebApi.ContactsType.CLS_Children), "{http://example.com/webapi}ContactsType.#{}", (global::XData.ChildKind)4, false, -1, new global::XData.ChildInfo[]
            {
            global::Client.WebApi.ContactsType.CLS_Children.CLS_ContactList.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.WebApi.ContactsType.CLS_Children>();
                    return ThisInfo;
                }
            }
        }

        new public static readonly global::XData.ComplexTypeInfo ThisInfo = new global::XData.ComplexTypeInfo(typeof (global::Client.WebApi.ContactsType), false, new global::XData.FullName("http://example.com/webapi", "ContactsType"), global::XData.XComplexType.ThisInfo, null, global::Client.WebApi.ContactsType.CLS_Children.ThisInfo);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.WebApi.ContactsType>();
                return ThisInfo;
            }
        }
    }

    public partial class Contacts : global::XData.XGlobalElement
    {
        new public global::Client.WebApi.ContactsType Type
        {
            get
            {
                return base.GenericType as global::Client.WebApi.ContactsType;
            }

            set
            {
                base.GenericType = value;
            }
        }

        public static bool TryLoadAndValidate(string filePath, global::System.IO.TextReader reader, global::XData.DiagContext context, out global::Client.WebApi.Contacts result)
        {
            return TryLoadAndValidate<global::Client.WebApi.Contacts>(filePath, reader, context, ThisInfo, out result);
        }

        public static readonly global::XData.ElementInfo ThisInfo = new global::XData.ElementInfo(typeof (global::Client.WebApi.Contacts), false, "{http://example.com/webapi}Contacts", (global::XData.ChildKind)1, false, -1, new global::XData.FullName("http://example.com/webapi", "Contacts"), true, global::Client.WebApi.ContactsType.ThisInfo, null, null, null, global::XDataProgramInfo.Instance);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.WebApi.Contacts>();
                return ThisInfo;
            }
        }
    }

    public partial class ProductsType : global::XData.XComplexType
    {
        public global::Client.WebApi.ProductsType.CLS_Children.CLS_ProductList C_ProductList
        {
            get
            {
                return base.TryGetChild(0) as global::Client.WebApi.ProductsType.CLS_Children.CLS_ProductList;
            }

            set
            {
                if (value == null)
                    base.RemoveChild(0);
                else
                    base.AddOrSetChild(value);
            }
        }

        public global::Client.WebApi.ProductsType.CLS_Children.CLS_ProductList EnsureC_ProductList(bool @try = false)
        {
            return C_ProductList ?? (C_ProductList = base.CreateChild<global::Client.WebApi.ProductsType.CLS_Children.CLS_ProductList>(0, @try));
        }

        new public global::Client.WebApi.ProductsType.CLS_Children Children
        {
            get
            {
                return base.GenericChildren as global::Client.WebApi.ProductsType.CLS_Children;
            }

            set
            {
                base.GenericChildren = value;
            }
        }

        new public global::Client.WebApi.ProductsType.CLS_Children EnsureChildren(bool @try = false)
        {
            return base.EnsureChildren<global::Client.WebApi.ProductsType.CLS_Children>(@try);
        }

        public partial class CLS_Children : global::XData.XChildSequence
        {
            public partial class CLSITEM_ProductList : global::XData.XLocalElement
            {
                new public global::Client.EBiz.Product Type
                {
                    get
                    {
                        return base.GenericType as global::Client.EBiz.Product;
                    }

                    set
                    {
                        base.GenericType = value;
                    }
                }

                public static readonly global::XData.ElementInfo ThisInfo = new global::XData.ElementInfo(typeof (global::Client.WebApi.ProductsType.CLS_Children.CLSITEM_ProductList), false, "{http://example.com/webapi}ProductsType.#{}.ProductList", (global::XData.ChildKind)2, false, 0, new global::XData.FullName(null, "Product"), false, global::Client.EBiz.Product.ThisInfo, null, null, null, global::XDataProgramInfo.Instance);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.WebApi.ProductsType.CLS_Children.CLSITEM_ProductList>();
                        return ThisInfo;
                    }
                }
            }

            public global::Client.WebApi.ProductsType.CLS_Children.CLS_ProductList C_ProductList
            {
                get
                {
                    return base.TryGetChild(0) as global::Client.WebApi.ProductsType.CLS_Children.CLS_ProductList;
                }

                set
                {
                    if (value == null)
                        base.RemoveChild(0);
                    else
                        base.AddOrSetChild(value);
                }
            }

            public global::Client.WebApi.ProductsType.CLS_Children.CLS_ProductList EnsureC_ProductList(bool @try = false)
            {
                return C_ProductList ?? (C_ProductList = base.CreateChild<global::Client.WebApi.ProductsType.CLS_Children.CLS_ProductList>(0, @try));
            }

            public partial class CLS_ProductList : global::XData.XChildList<global::Client.WebApi.ProductsType.CLS_Children.CLSITEM_ProductList>
            {
                public global::Client.WebApi.ProductsType.CLS_Children.CLSITEM_ProductList CreateItem()
                {
                    return base.CreateItem<global::Client.WebApi.ProductsType.CLS_Children.CLSITEM_ProductList>();
                }

                public global::Client.WebApi.ProductsType.CLS_Children.CLSITEM_ProductList CreateAndAddItem()
                {
                    return base.CreateAndAddItem<global::Client.WebApi.ProductsType.CLS_Children.CLSITEM_ProductList>();
                }

                public global::Client.WebApi.ProductsType.CLS_Children.CLS_ProductList Add(global::System.Action<global::Client.WebApi.ProductsType.CLS_Children.CLSITEM_ProductList> itemSetter)
                {
                    base.Add<global::Client.WebApi.ProductsType.CLS_Children.CLSITEM_ProductList>(itemSetter);
                    return this;
                }

                public global::Client.WebApi.ProductsType.CLS_Children.CLS_ProductList AddRange<TItemValue>(global::System.Collections.Generic.IEnumerable<TItemValue> itemValues, global::System.Action<global::Client.WebApi.ProductsType.CLS_Children.CLSITEM_ProductList, TItemValue> itemSetter)
                {
                    base.AddRange<global::Client.WebApi.ProductsType.CLS_Children.CLSITEM_ProductList, TItemValue>(itemValues, itemSetter);
                    return this;
                }

                public static readonly global::XData.ChildListInfo ThisInfo = new global::XData.ChildListInfo(typeof (global::Client.WebApi.ProductsType.CLS_Children.CLS_ProductList), "{http://example.com/webapi}ProductsType.#{}.ProductList", false, 0, 1UL, 18446744073709551615UL, global::Client.WebApi.ProductsType.CLS_Children.CLSITEM_ProductList.ThisInfo);
                public override global::XData.ObjectInfo ObjectInfo
                {
                    get
                    {
                        global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.WebApi.ProductsType.CLS_Children.CLS_ProductList>();
                        return ThisInfo;
                    }
                }
            }

            public static readonly global::XData.ChildStructInfo ThisInfo = new global::XData.ChildStructInfo(typeof (global::Client.WebApi.ProductsType.CLS_Children), "{http://example.com/webapi}ProductsType.#{}", (global::XData.ChildKind)4, false, -1, new global::XData.ChildInfo[]
            {
            global::Client.WebApi.ProductsType.CLS_Children.CLS_ProductList.ThisInfo
            }

            );
            public override global::XData.ObjectInfo ObjectInfo
            {
                get
                {
                    global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.WebApi.ProductsType.CLS_Children>();
                    return ThisInfo;
                }
            }
        }

        new public static readonly global::XData.ComplexTypeInfo ThisInfo = new global::XData.ComplexTypeInfo(typeof (global::Client.WebApi.ProductsType), false, new global::XData.FullName("http://example.com/webapi", "ProductsType"), global::XData.XComplexType.ThisInfo, null, global::Client.WebApi.ProductsType.CLS_Children.ThisInfo);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.WebApi.ProductsType>();
                return ThisInfo;
            }
        }
    }

    public partial class Products : global::XData.XGlobalElement
    {
        new public global::Client.WebApi.ProductsType Type
        {
            get
            {
                return base.GenericType as global::Client.WebApi.ProductsType;
            }

            set
            {
                base.GenericType = value;
            }
        }

        public static bool TryLoadAndValidate(string filePath, global::System.IO.TextReader reader, global::XData.DiagContext context, out global::Client.WebApi.Products result)
        {
            return TryLoadAndValidate<global::Client.WebApi.Products>(filePath, reader, context, ThisInfo, out result);
        }

        public static readonly global::XData.ElementInfo ThisInfo = new global::XData.ElementInfo(typeof (global::Client.WebApi.Products), false, "{http://example.com/webapi}Products", (global::XData.ChildKind)1, false, -1, new global::XData.FullName("http://example.com/webapi", "Products"), true, global::Client.WebApi.ProductsType.ThisInfo, null, null, null, global::XDataProgramInfo.Instance);
        public override global::XData.ObjectInfo ObjectInfo
        {
            get
            {
                global::XData.Extensions.PublicParameterlessConstructorRequired<global::Client.WebApi.Products>();
                return ThisInfo;
            }
        }
    }
}

internal sealed class XDataProgramInfo : global::XData.ProgramInfo
{
    private XDataProgramInfo()
    {
    }

    public static readonly XDataProgramInfo Instance = new XDataProgramInfo();
    protected override global::System.Collections.Generic.List<global::XData.NamespaceInfo> GetNamespaces()
    {
        return new global::System.Collections.Generic.List<global::XData.NamespaceInfo>()
        {
        new global::XData.NamespaceInfo("http://example.com/common", new global::XData.IGlobalObjectInfo[]
        {
        global::Client.Common.String10.ThisInfo, global::Client.Common.String20.ThisInfo, global::Client.Common.String40.ThisInfo, global::Client.Common.NonNegativeInt32.ThisInfo, global::Client.Common.PositiveInt32.ThisInfo, global::Client.Common.PositiveInt32List.ThisInfo, global::Client.Common.Email.ThisInfo, global::Client.Common.PhoneKind.ThisInfo, global::Client.Common.Phone.ThisInfo, global::Client.Common.NormalAddress.ThisInfo, global::Client.Common.SpatialNumber.ThisInfo, global::Client.Common.SpatialAddress.ThisInfo, global::Client.Common.MoneyKind.ThisInfo, global::Client.Common.MoneyValue.ThisInfo, global::Client.Common.Money.ThisInfo, global::Client.Common.ImageValue.ThisInfo, global::Client.Common.Image.ThisInfo, global::Client.Common.Reputation.ThisInfo, global::Client.Common.Gender.ThisInfo, global::Client.Common.ShoeUnit.ThisInfo, global::Client.Common.ShoeSizeValue.ThisInfo, global::Client.Common.ShoeSize.ThisInfo
        }

        ), new global::XData.NamespaceInfo("http://example.com/ebiz", new global::XData.IGlobalObjectInfo[]
        {
        global::Client.EBiz.Contact.ThisInfo, global::Client.EBiz.OrderDetail.ThisInfo, global::Client.EBiz.Order.ThisInfo, global::Client.EBiz.Customer.ThisInfo, global::Client.EBiz.Supplier.ThisInfo, global::Client.EBiz.Product.ThisInfo, global::Client.EBiz.SportEquipment.ThisInfo, global::Client.EBiz.Shoe.ThisInfo
        }

        ), new global::XData.NamespaceInfo("http://example.com/webapi", new global::XData.IGlobalObjectInfo[]
        {
        global::Client.WebApi.ContactsType.ThisInfo, global::Client.WebApi.Contacts.ThisInfo, global::Client.WebApi.ProductsType.ThisInfo, global::Client.WebApi.Products.ThisInfo
        }

        )}

        ;
    }
}