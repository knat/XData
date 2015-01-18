﻿using System;

namespace XData {
    public interface IEntityObject {//Attribute and Element impl this interface
        EntityInfo EntityInfo { get; }
        FullName FullName { get; }
        XType Type { get; }
    }

    public abstract class XAttribute : XObject, IEntityObject {
        protected XAttribute() {
            _fullName = GetFullName();
        }
        private readonly FullName _fullName;
        public FullName FullName {
            get {
                return _fullName;
            }
        }
        protected abstract FullName GetFullName();
        private XAttribute _referentialAttribute;
        public XAttribute ReferentialAttribute {
            get {
                return _referentialAttribute;
            }
            set {
                if (value != null) {
                    if (value._fullName != _fullName) {
                        throw new InvalidOperationException("Referential attribute full name '{0}' not equal to '{1}'.".InvFormat(
                            value._fullName.ToString(), _fullName.ToString()));
                    }
                    for (var i = value; i != null; i = i._referentialAttribute) {
                        if (object.ReferenceEquals(this, i)) {
                            throw new InvalidOperationException("Circular reference detected.");
                        }
                    }
                }
                _referentialAttribute = value;
            }
        }
        public XAttribute GenericReferentialAttribute {
            get {
                return _referentialAttribute;
            }
            set {
                ReferentialAttribute = value;
            }
        }
        public bool IsReference {
            get {
                return _referentialAttribute != null;
            }
        }
        public XAttribute EffectiveAttribute {
            get {
                return _referentialAttribute == null ? this : _referentialAttribute.EffectiveAttribute;
            }
        }
        private XSimpleType _type;
        private void SetType(XSimpleType type) {
            _type = SetParentTo(type);
        }
        public XSimpleType Type {
            get {
                return EffectiveAttribute._type;
            }
            set {
                if (_referentialAttribute != null) {
                    _referentialAttribute.Type = value;
                }
                else {
                    SetType(value);
                }
            }
        }
        public bool HasType {
            get {
                return EffectiveAttribute._type != null;
            }
        }
        public XSimpleType GenericType {
            get {
                return Type;
            }
            set {
                Type = value;
            }
        }
        XType IEntityObject.Type {
            get {
                return Type;
            }
        }
        public T EnsureType<T>(bool @try = false) where T : XSimpleType {
            if (_referentialAttribute != null) {
                return _referentialAttribute.EnsureType<T>(@try);
            }
            var obj = _type as T;
            if (obj != null) return obj;
            obj = AttributeInfo.Type.CreateInstance<T>(@try);
            SetType(obj);
            return obj;
        }
        public XSimpleType EnsureType(bool @try = false) {
            return EnsureType<XSimpleType>(@try);
        }
        //public object Value {
        //    get {
        //        var type = Type;
        //        return type == null ? null : type.Value;
        //    }
        //    set { EnsureType().Value = value; }
        //}
        //public object GenericValue {
        //    get {
        //        return Value;
        //    }
        //    set {
        //        Value = value;
        //    }
        //}
        public override XObject DeepClone() {
            var obj = (XAttribute)base.DeepClone();
            obj.SetType(_type);
            return obj;
        }
        public AttributeInfo AttributeInfo {
            get {
                return (AttributeInfo)ObjectInfo;
            }
        }
        EntityInfo IEntityObject.EntityInfo {
            get {
                return AttributeInfo;
            }
        }

    }
}
