## XData

XData is a universal solution for data exchange. It contains four parts: data, schema, data object model, and code generation.

### Data

XData data is a text-based tree-like structure. An example:

```
//single line comment
/*delimited comment*/
alias1:RootElement <alias1 = "http://example.com/project1"
    alias2 = "http://example.com/project2"> = (alias1:MyComplexType)
    [
        Attribute1 = (sys:Int64)-42
        Attribute2 = #[2 3 5 7 11]
        Attribute3
    ]
    {
        alias2:ChildElement1 =
            [
                Attribute1 = true
                Attribute2 = (sys:Guid)"A0E10CD5-BE6C-4DEE-9A5E-F711CD9CB46B"
            ]
            $ (sys:Binary)"MDEyMzQ1Njc4OQ=="
        ChildElement2 = 
        [
            Attribute1 = (sys:DateTimeOffset)"2015-01-24T15:32:43+07:00"
        ]
        ChildElement3
        ChildElement4 <alias1 = "http://other.com"> = 
            {
                alias1:ChildChildElement1 = -42
                ChildChildElement2 = $ (alias1:MyDouble)42.42
            }
    }
```

#### Lexical Grammar

Lexical grammar defines rules that recognize one or more characters as a token.

```
white-space:
unicode-category-Zs | '\u0009' | '\u000B' | '\u000C'
;
new-line:
'\u000D' | '\u000A' | '\u0085' | '\u2028' | '\u2029'
;
white-space-or-new-line-token:
(white-space | new-line)+
;
single-line-comment-token:
'//' (!new-line)*
;
delimited-comment-token:
'/*' (!'*/')* '*/'
; 
name-token:
normal-name-token | verbatim-name-token
;
verbatim-name-token:
'@' normal-name-token
;
normal-name-token:
name-start-char name-part-char*
;
name-start-char:
letter-char | '_'
;
name-part-char:
letter-char | decimal-digit-char | connecting-char | combining-char | formatting-char
;
letter-char:
unicode-category-Lu-Ll-Lt-Lm-Lo-Nl
;
decimal-digit-char:
unicode-category-Nd
;
connecting-char:
unicode-category-Pc
;
combining-char:
unicode-category-Mn-Mc
;
formatting-char:
unicode-category-Cf
;
string-value-token:
normal-string-value-token | verbatim-string-value-token
;
normal-string-value-token:
'"' normal-string-value-char* '"'
;
normal-string-value-char:
!('"' | '\\' | new-line) | simple-escape-sequence | unicode-escape-sequence
;
simple-escape-sequence:
'\\' ('\'' | '"' | '\\' | '0' | 'a' | 'b' | 'f' | 'n' | 'r' | 't' | 'v')
;
unicode-escape-sequence:
'\\u' hex-digit hex-digit hex-digit hex-digit
;
hex-digit:
'0'..'9' | 'A'..'F' | 'a'..'f'
;
verbatim-string-value-token:
'@"' (!'"' | '""')* '"'
;
integer-value-token:
('+' | '-')? decimal-digit+
;
decimal-digit:
'0'..'9'
;
decimal-value-token:
('+' | '-')? decimal-digit* '.' decimal-digit+
;
real-value-token:
('+' | '-')? (decimal-digit* '.')? decimal-digit+ ('E' | 'e') ('+' | '-')? decimal-digit+
;
hash-open-bracket-token:
'#['
;
character-token:
a-single-character-not-recognized-by-the-above-rules
;
```

Token examples:

| Token | Examples |
| ----- | -------- |
|`single-line-comment-token`|`//comment`|
|`delimited-comment-token`|`/*comment*/`|
|`normal-name-token`|`name1` `_1` `名字1` `true`|
|`verbatim-name-token`|`@name1` `@_1` `@名字1` `@true`|
|`normal-string-value-token`|`"abcd\r\nefg\t\u0041\u0042"`|
|`verbatim-string-value-token`|`@"d:\dir\file.txt,""\r\n"`|
|`integer-value-token`|`42` `+042` `-42`|
|`decimal-value-token`|`42.0` `+.42` `-0.42`|
|`real-value-token`|`42.42E7` `+42e-7` `-.42E+7`|
|`character-token`|`<` `>` `(` `)` `[` `]` `{` `}` `:` `=`|

#### Parsing Grammar

Parsing grammar defines rules that recognize one or more tokens as a node.

```
parsing-unit:
element
;
element:
qualifiable-name uri-aliasings? ('=' element-value)?
;
uri-aliasings:
'<' uri-aliasing* '>'
;
uri-aliasing:
name-token '=' string-value-token
;
qualifiable-name:
(name-token ':')? name-token
;
element-value:
complex-value | simple-value
;
complex-value:
type-indicator? (attributes children? | children | ';')
;
type-indicator:
'(' qualifiable-name ')'
;
attributes:
'[' attribute* ']'
;
attribute:
name-token ('=' attribute-value)?
;
attribute-value:
simple-value
;
children:
simple-child | complex-children
;
simple-child:
'$' simple-value
;
complex-children:
'{' element* '}'
;
simple-value:
type-indicator? (atom-value | list-value)
;
atom-value:
string-value-token | integer-value-token | decimal-value-token | real-value-token
    | 'true' | 'false'
;
list-value:
hash-open-bracket-token simple-value* ']'
;
```

A `parsing-unit` must have one and only one `element`.

A `uri-aliasing` associates a URI with an alias:

```
<alias1 = "http://example.com/project1">
```

The first `name-token` of the `qualifiable-name` is used to reference a URI alias:

```
alias1:Name1
```

If the first `name-token` of the `qualifiable-name` is absent, it references the null URI.

Empty URI equals to null URI.

```
alias1:Element1 <alias1 = ""> //empty URI
```

equals to

```
Element1 //null URI
```

Unlike XML, there is no default URI in XData.

A URI and a name forms a full name. If the URI is not null or empty, we call it qualified full name and can be expressed as `{URI}Name` in semantics:

```
{http://example.com/project1}Name1
```

Otherwise it is a unqualified full name.

Aliases are defined in an element and can be referenced by the self element and descendant nodes, A descendant element can redefine a alias:

```
a1:Element1 <a1 = "http://example.com"> = (a1:MyComplexType)
    {
        a1:Element2 = (sys:Int16)42
        Element2 = $ (a1:MyInt32)42
        a1:Element2 <a1 = "http://other.com"> =
            {
                a1:Element1
                a1:Element1
            }
    }
```

The reserved alias "sys" is used to reference the system URI "http://xdata-io.org", which contains predefined system types.

The data is tightly coupled with the schema. In most cases, `type-indicator` is not required.

An `attribute` is identified by its `name-token`. In an `attributes`, every `attribute` must have a unique `name-token`.

An `attribute` is a name-value pair, the value must be `simple-value`. An `attribute` may have no value:

```
[
    Attribute1 = ...//has value
    Attribute2//no value
]
```

An `element` is identified by its `qualifiable-name`, that is, an `element` is identified by a full name.

An `element` is a name-value pair. An `element` may have no value:

```
Element1 = ...//has value
Element2//no value
```

`element-value` may be `simple-value`:

```
Element1 = 42
```

`element-value` may be `complex-value`, that is, the element has attributes and/or children. `children` may be `simple-child` or `complex-children`:

```
Element1 =
    [//attributes
        Attribute1
        Attribute2
    ]
    $ 42//simple child
Element2 =
    [//attributes
        Attribute1
        Attribute2
    ]
    {//complex children
        ChildElement1
        ChildElement1
    }
```

Consider the following code:

```
Element1 = 42//simple value
Element2 = $ 42//simple child complex value
Element3 = ;//empty complex value
```

`simple-value` maybe `atom-value` or `list-value`:

```
42 //atom value
42.42 //atom value
true //atom value
@"c:\file.txt" //atom value
(sys:DateTimeOffset)"2015-01-24T15:32:43+07:00" //atom value
(a1:MyListType)#[2 (a1:MyInt32)3 true #["abc" 42] 11] //list value
#[] //list value
```

`integer-value-token`, `decimal-value-token`, `real-value-token`, `true` and `false` can also be written as `string-value-token`:

```
42 == "42"
42.42 == "42.42"
42.42e7 == "42.42e7"
true == "true"
false == "false"
```

Please review the above to comprehend the data.

### Schema

Schema is the specification or contract of your data. An example:

```
alias "http://example.com/project1" as p1
alias "http://example.com/project2" as p2

namespace p1
{

}

namespace p2
{
    import p1 as p1

}
```

#### Lexical Grammar

The schema lexical grammar is a superset of the data lexical grammar. The following rules are added:

```
dot-dot-token:
'..'
;
dollar-open-brace-token:
'${'
;
hash-open-brace-token:
'#{'
;
question-open-brace-token:
'?{'
;
```

#### Parsing Grammar

```
compilation-unit:
uri-aliasing* namespace*
;
uri-aliasing:
'alias' uri-value 'as' uri-alias
;
uri-value:
string-value-token
;
uri-alias:
name-token
;
uri:
uri-value | uri-alias
;
namespace:
'namespace' uri '{' namespace-import* namespace-member* '}'
;
namespace-import:
'import' uri ('as' namespace-alias)?
;
namespace-alias:
name-token
;
qualifiable-name:
(namespace-alias ':')? name-token
;
namespace-member:
type | global-element 
;
type:
'type' name-token type-annotations? type-body
;
type-annotations:
'<' abstract-or-sealed? '>'
;
type-body:
type-list | type-directness | type-extension | type-restriction
;
type-list:
'lists' qualifiable-name facets?
;
type-directness:
attributes-children | ';'
;
type-extension:
'extends' qualifiable-name attributes-children?
;
type-restriction:
'restricts' qualifiable-name (attributes-children | facets)?
;
facets:
dollar-open-brace-token (length-range | precision | scale | value-range | enum |
    pattern | list-item-type)* '}'
;
length-range:
'lengthrange' (integer-value-token dot-dot-token integer-value-token? 
    | dot-dot-token integer-value-token)
;
precision:
'precision' integer-value-token
;
scale:
'scale' integer-value-token
;
value-range:
'valuerange' (lower-value dot-dot-token upper-value? | dot-dot-token upper-value)
;
lower-value:
('[' | '(') literal
;
upper-value:
literal (']' | ')')
;
literal:
string-value-token | integer-value-token | decimal-value-token | real-value-token
    | 'true' | 'false'
;
enum:
'enum' enum-item+
;
enum-item:
literal ('as' name-token)?
;
pattern:
'pattern' string-value-token
;
list-item-type:
'lists' qualifiable-name
;
attributes-children:
attribute-set children? | children
;
attribute-set:
'[' attribute* ']'
;
attribute:
name-token attribute-annotations? 'as' qualifiable-name
;
attribute-annotations:
'<' (optional-or-delete | nullable)* '>'
;
children:
simple-child | complex-children
;
simple-child:
'$' qualifiable-name
;
complex-children:
element-set | child-sequence
;
element-set:
'{' member-element* '}'
;
child-sequence:
hash-open-brace-token member-child* '}'
;
member-element:
local-element | global-element-ref
;
member-child:
member-element | member-child-sequence | member-child-choice
;
local-element:
name-token local-element-annotations? 'as' qualifiable-name
;
local-element-annotations:
'<' (member-name | occurrence-or-delete | nullable)* '>'
;
global-element-ref:
'&' qualifiable-name member-child-annotations?
;
member-child-annotations:
'<' (member-name | occurrence-or-delete)* '>'
;
member-child-sequence:
hash-open-brace-token member-child* '}' member-child-annotations?
;
member-child-choice:
question-open-brace-token member-child* '}' member-child-annotations?
;
global-element:
'element' name-token global-element-annotations? 'as' qualifiable-name
;
global-element-annotations:
'<' (abstract-or-sealed | substitute | nullable)* '>'
;
abstract-or-sealed:
'abstract' | 'sealed'
;
nullable:
'nullable'
;
optional-or-delete:
'?' | 'x'
;
member-name:
'membername' name-token
;
occurrence-or-delete:
occurrence | 'x'
;
occurrence:
(integer-value-token dot-dot-token integer-value-token?)
| '?'
| '*'
| '+'
;
substitute:
'substitutes' qualifiable-name
```

A `compilation-unit` can contain zero or more `namespace`. A `namespace` is identified by a `uri`. If multiple `namespace`s have a same URI, they are merged into a logical namespace by the schema compiler.

```
namespace "http://example.com/project1"
{
    //...
}
namespace "http://example.com/project2"
{
    //...
}
namespace "http://example.com/project2"
{
    //...
}
```

`uri-aliasing` can be used to reduce typing:

```
alias "http://example.com/project1" as p1
alias "http://example.com/project2" as p2

namespace p1
{
    //...
}
namespace p2
{
    //...
}
namespace p2
{
    //...
}
```

`uri` can be empty:

```
namespace ""
{
    //...
}
```

`namespace-member` can be `type` or `global-element`, which are identified by a `name-token`. In a logical namespace, every member must have a unique `name-token`:

```
namespace "urn:project1"
{
    type T1 ...
    element E1 ...
    element T1 ...//ERROR: duplicate namespace member 'T1'
}
namespace "urn:project1"
{
    type T2 ...
    element E1 ...//ERROR: duplicate namespace member 'E1'
}
```

If a `namespace-member`s want to reference a `namespace-member` in other `namespace`, `namespace-import` is required.

```
namespace "urn:project1"
{
    //...
}
namespace "urn:project2"
{
    import "urn:project1" as p1
    //we can reference "urn:project1"'s members
}
```

`uri-aliasing` can be used to reduce typing:

```
alias "urn:project1" as p1
alias "urn:project2" as p2

namespace p1
{
    //...
}
namespace p2
{
    import p1 as p1
    //we can reference "urn:project1"'s members
}
```

Use `qualifiable-name` to reference a `namespace-member`. If a `qualifiable-name` has no `namespace-alias`, we call it unqualified `qualifiable-name`, otherwise qualified `qualifiable-name`. To resolve an unqualified `qualifiable-name`, the schema compiler first searches the containing logical namespace, if finds one then uses it, otherwise searches all the imported namespaces, if finds one and only one then uses it, otherwise the unqualified `qualifiable-name` is ambiguous if finds more than one.

```
namespace "urn:project1"
{
    type T1 ...
    type T2 ...
    type T3 ...
}
namespace "urn:project2"
{
    type T3 ...
}
namespace "urn:project3"
{
    import "urn:project1" as p1
    
    type T1 restricts p1:T1//qualified qualifiable-name 'p1:T1' references {urn:project1}T1
}
namespace "urn:project3"
{
    import "urn:project1" as p1
    import "urn:project2" as p2
    
    type TA restricts T1//unqualified qualifiable-name 'T1' references {urn:project3}T1
    type TB restricts T2//unqualified qualifiable-name 'T2' references {urn:project1}T2
    type TC restricts T3//ERROR: unqualified qualifiable-name 'T3' is ambiguous between {urn:project1}T3 and {urn:project2}T3
}
```

There is a system namespace "http://xdata-io.org", which contains predefined system types. System namespace is implicitly imported into every user namespace. The reserved `namespace-alias` "sys" is used to reference the system namespace:

```
namespace "urn:project1"
{
    type Int32 restricts sys:Int32
    type MyInt64 restricts Int64
}
```

Below is the hierarchy of the predefined system types, "<...>" are abstract types, otherwise concrete types:

```
<ComplexType>
<SimpleType>
  |-<ListType> //e.g: #[2 3 5 7 11]
  |-<AtomType>
    |-String
    |-IgnoreCaseString //e.g: "Tank" == "tank"
    |-Decimal //128 bit fixed point number, 28 digit precision
    |  |-Int64 //signed 64 bit integer
    |  |  |-Int32
    |  |     |-Int16
    |  |        |-SByte //signed 8 bit integer
    |  |-UInt64 //unsigned 64 bit integer
    |     |-UInt32
    |        |-UInt16
    |           |-Byte //unsigned 8 bit integer
    |-Double //64 bit double precision floating point number, can be "INF", "-INF" and "NaN"
    |  |-Single //32 bit single precision floating point number, can be "INF", "-INF" and "NaN"
    |-Boolean //true or false
    |-Binary //Base64 encoded, e.g: "MDEyMzQ1Njc4OQ=="
    |-Guid //e.g: "A0E10CD5-BE6C-4DEE-9A5E-F711CD9CB46B"
    |-TimeSpan //e.g: "73.14:08:16.367" 73 days, 14 hours, 8 minutes and 16.367 seconds
    |          // "-00:00:05" negative 5 seconds
    |-DateTimeOffset //e.g: "2015-01-24T15:32:03.367+07:00" "2015-01-01T00:00:00+00:00"
```

That is, `AtomType` is a `SimpleType`, `Int32` is a `Decimal`, `SByte` is a `Decimal`, `SByte` is a `SimpleType`, etc.

Use `type-restriction` to derive a new atom type from an existing atom type:

```
type MyString restricts String
${
    //...
}
type MyString2 restricts MyString
```

`facets`(`${ }`) defines rules that restrict the simple type value:

* `length-range`: Specify character count range of a `String` and `IgnoreCaseString`, byte count range of a `Binary`, item count range of a `ListType`. The left side of `..` is min length, the right side is max length.
* `precision`: Specify total digit count of a `Decimal`.
* `scale`: Specify fraction digit count of a `Decimal`.
* `value-range`: For `String`, `IgnoreCaseString`, numeric types(from `Decimal` to `Single`), `TimeSpan` and `DateTimeOffset`. The type value must be between the value range. `[` or `]` means inclusion, `(` or `)` means exclusion.
* `enum`: For concrete atom type(from `String` to `DateTimeOffset`). The type value must equal to one of the enum values.
* `pattern`: For concrete atom type(from `String` to `DateTimeOffset`). The type value string must match the regular expression.

Examples:

```
type Byte1to20 restricts Binary
${
    lengthrange 1..20 //1 <= byte count <= 20
}
type Char restricts String
${
    lengthrange 1..1
}
type Money restricts Decimal
${
    precision 19
    scale 2
}
type Year2015 restricts DateTimeOffset
${
    valuerange ["2015-01-01T00:00:00+00:00" .. "2016-01-01T00:00:00+00:00")
}
type Color restricts String
${
    enum "Red" "Green" "Blue"
}
type AccessFlags restricts Int32
${
    enum
        0 as None//item names are for programming 
        1 as Read
        2 as Write
        4 as Execute
        7 as All
}
type Email restricts String
${
    lengthrange ..40
    pattern @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}"
}
```

Facets are heritable:

```
type T1 restricts String
${
    lengthrange ..20
    pattern @"[a-h]{1,}"
}
type T2 restricts T1
${
    lengthrange 10..//max length is inherited. That is: 10 <= char count <= 20
    //pattern is inherited
}
```

Derived types can NARROW the base type's facets:

```
type Byte10to20 restricts Byte1to20
${
    lengthrange 10..
}
type SmallMoney restricts Money
${
    precision 9
}
type T1 restricts Int32
${
    valuerange [100..1000)
}
type T2 restricts T1
${
    valuerange (100.. //that is: (100..1000)
}
type RedAndBlue restricts Color
${
    enum "Red" "Blue"
}
type T3 restricts String
${
    pattern @"[a-h]{1,4}"
}
type T4 restricts T3
${
    pattern @"[f-z]{2,}"//the type value string must match the base type's regular expression AND this regular expression
}
```

Use `type-list` to create a new list type, which derives from `sys:ListType`. Item type must reference a simple type:

```
type Int32List lists Int32
${
    lengthrange 1.. //1 <= item count
}
```

Use `type-restriction` to derive a new list type from an existing list type. The derived list type's item type must equal to or derive from the base type's item type:

```
type Int32List2 restricts Int32List
${
    lengthrange 10..20 //10 <= item count <= 20
}

type MyInt16 restricts Int16

type MyInt16List restricts Int32List
${
    lists MyInt16
    lengthrange 10..
}
```

Item type can reference any simple types:

```
type SimpleTypeList lists SimpleType
//valid data: #[1 true #["abc" 42.42] #[] -42]

type AtomTypeList restricts SimpleTypeList
${
    lists AtomType
}
//valid data: #[1 true -42]

type Int16List restricts AtomTypeList
${
    lists Int16
}
//valid data: #[1 -42]

type ListList restricts SimpleTypeList
${
    lists ListType
}
//valid data: #[#["abc" 42.42] #[]]
```

Use `type-directness` to create a new complex type, which derives from `sys:ComplexType`:

```
type AttributesOnlyComplexType
[//attribute set
    Attribute1 as Int32
    Attribute2 as SimpleType
]

type SimpleChildComplexType
[
    Attribute1 as Int32
    Attribute2 as AtomType
]
    $ SimpleType

type SimpleChildOnlyComplexType
    $ Int32

type EmptyComplexType
    ;

type ElementSetComplexChildrenComplexType
[
    Attribute1 as Int32
    Attribute2 as ListType
]
{//element set
    Element1 as AttributesOnlyComplexType
    Element2 as SimpleChildComplexType
}

type ChildSequenceComplexChildrenComplexType
#{//child sequence
    Element1<+> as ElementSetComplexChildrenComplexType
    ?{//child choice
        Element2 as EmptyComplexType
        #{//child sequence
            Element3 as SimpleTypeList
            Element4 as Boolean
        }
    }<*>
}
```

A `simple-child` must reference a simple type.

Use `type-extension` or `type-restriction` to derive a new complex type from an existing complex type. If base complex type has no children, `simple-child` can be added:

```
type T1 extends AttributesOnlyComplexType
    //attribute set is inherited
    $ ListType

type T2 extends EmptyComplexType
    $ Int32
```

`simple-child` can be restricted, that is, the derived type's `simple-child` must equal to or derive from the base type's `simple-child`:

```
type T3 restricts SimpleChildComplexType
    //attribute set is inherited
    $ AtomType
type T4 restricts T3
    //attribute set is inherited
    $ String
```

An `attribute` is identified by its `name-token`. In an `attribute-set`, every attribute must have a unique `name-token`. `attribute`s must reference simple types. `attribute`s can be annotated with `?`(optional) and/or `nullable`:

```
//schema
[
    Attribute1 as Int32 //default is required and non-nullable
    Attribute2<?> as Int32 //optional
    Attribute3<nullable> as Int32
    Attribute4<? nullable> as Int32
]

//data
[ //attribute set is unordered
    Attribute2 = 42
    Attribute1 = 42
    Attribute3 //because it is nullable, it may have no value
] //because Attribute4 is optional
```

`attribute-set` can be extended:

```
type T1
[
    Attribute1 as Int32
]

type T2 extends T1
[
    //Attribute1 is inherited
    Attribute2 as Int32
]
/* The result of T2's attribute set is:
[
    Attribute1 as Int32
    Attribute2 as Int32
]
*/
```

`attribute-set` can be restricted:

```
type T1
[
    Attribute1 as Int32
    Attribute2<?> as Int32
    Attribute3<nullable> as Int32
    Attribute4<? nullable> as Int32
]

type T2 restricts T1
[
    //Attribute1 is inherited
    Attribute3 as Int16 //nullable to non-nullable, type is restricted
    Attribute2<x> as Int32 //optional attribute can be deleted
    Attribute4<nullable> as Int32 //optional to required
]
/* The result of T2's attribute set is:
[
    Attribute1 as Int32
    Attribute3 as Int16
    Attribute4<nullable> as Int32
]
*/
```

A type can be annotated with `abstract`:

```
//schema
namespace "urn:project1"
{
    type MyComplexTypeBase<abstract>
    [
        Attribute1 as AtomType
        Attribute2 as Double
    ]
        $ Double
    
    type MyComplexType extends MyComplexTypeBase
        //attribute set and simple child are inherited
    
    element GlobalElement as MyComplexTypeBase
}

//data
a1:GlobalElement<a1 = "urn:project1"> = (a1:MyComplexType) //type-indicator is required
                                            // because a1:MyComplexTypeBase is abstract
[
    Attribute2 = 42
    Attribute1 = (sys:Int16)42 //type-indicator is required because sys:AtomType is abstract
]
    $ (sys:Single)42 //type-indicator may be used
```

`type`s can be annotated with `sealed`:

```
type T1<sealed> ...
type T2 extends T1 //ERROR: base type 'T1' is sealed
type T3 restricts T1 //ERROR: base type 'T1' is sealed
```

Like attributes, elements can be annotated with `nullable`. Elements can reference simple or complex types.

`global-element`s are directly defined in `namespace`s. `global-element`s can be `abstract` and/or substituted:

```
namespace "urn:project1"
{
    element GlobalElement1<nullable> as SimpleType
    element GlobalElement2<substitutes GlobalElement1 abstract> as Int32 //nullable to non-nullable, type is restricted
    element GlobalElement3<substitutes GlobalElement2> as Int32

    element GlobalElement4<abstract nullable> as ComplexType
    element GlobalElement5<substitutes GlobalElement4> as MyComplexTypeBase //nullable to non-nullable, type is restricted
    element GlobalElement6<substitutes GlobalElement4 nullable> as MyComplexType //type is restricted
}

`global-element`s can be annotated with `sealed`:

```
element GE1<sealed> ...
element GE2<substitutes GE1> ... //ERROR: base element 'GE1' is sealed
```

`member-child` includes `local-element`, `global-element-ref`, `member-child-sequence` and `member-child-choice`. In a child container(`element-set`, `child-sequence`, `member-child-sequence` and `member-child-choice`), every `member-child` must have a unique member name. If a `member-child` is annotated with a `member-name`, then use it, otherwise, the member name for `local-element` is its `token-name`, `global-element-ref` is the global element's `token-name`, `member-child-sequence` is "Seq", `member-child-choice` is "Choice":

```
type T1
#{//child-sequence
    LocalElement as Int32 //member name: LocalElement
    &GlobalElement1 //global-element-ref. member name: GlobalElement1
    #{
    }//member-child-sequence. member name: Seq
    ?{
    }//member-child-choice. member name: Choice
    LocalElement<membername LocalElement2> as Int32 //member-name annotation required
    &GlobalElement1<membername GlobalElement1_2> //member-name annotation required
    #{
    }<membername Seq2>//member-name annotation required
    ?{
    }<membername Choice2>//member-name annotation required
}
```

`member-child` can be annotated with `occurrence`. The left side of `..` is min occurrence, the right side is max occurrence. If max occurrence is absent, it is infinite. The default value is 1..1.

```
type T1
#{
    LocalElement<3..10> as Int32
    &GlobalElement1<2..> //2..infinite
    #{
    }<?> //0..1
    ?{
    }<*> //0..infinite
    E2<+> as Int32 //1..infinite
    E3 as Int32 //1..1
}
```

If the min occurrence is zero, the `member-child` is optional.

An element is identified by its full name. For a local element, the full name's URI is always null(unqualified full name). For a global element, the full name's URI is the containing namespace's URI. A global element ref is just a pointer to the global element.

`element-set`'s member must be `local-element`s or `global-element-ref`s. The max occurrence must be one. Every member must have a unique full name. The `element-set` is unordered.

```
//schema
namespace "urn:project1"
{
    element GE1<abstract> as Int32
    element GE2<substitutes GE1> as Int32
    element GE3<substitutes GE2> as Int32

    type T1
    {//element-set
        E1 as Int32
        E2<?> as Int32
        E3<?> as Int32
        &GE1
    }
    
    element Root as T1
}

//data
a1:Root <a1 = "urn:project1"> =
{//element set is unordered
    E2 = 42 //local element full name's URI is always null
    a1:GE3 = 42 //because a1:GE1 can be substituted by a1:GE2 or a1:GE3
    E1 = 42
}//because E3 is optional
```

`element-set` can be extended:

```
type T1
{
    E1 as Int32
}

type T2 extends T1
{
    //E1 is inherited
    E2 as Int32
}

/* The result of T2's element set is:
{
    E1 as Int32
    E2 as Int32
}
*/
```

`element-set` can be restricted:

```
namespace "urn:project1"
{
    element GE1 as Int32
    element GE2<substitutes GE1> as Int32
    element GE3<substitutes GE2> as Int32

    type T1
    {
        E1 as Int32
        E2<?> as Int32
        E3<nullable> as Int32
        &GE1<?>
    }

    type T2 restricts T1
    {
        //E1 is inherited
        E3 as Int16 //nullable to non-nullable, type is restricted
        E2<x> as Int32 //optional member can be deleted
        &GE2<membername GE1> //optional to required, global element ref is restricted
    }
    /* The result of T2's element set is:
    {
        E1 as Int32
        E3 as Int16
        &GE2<membername GE1>
    }
    */
}
```

`child-sequence` can contain structural children:

```
//schema
namespace "urn:project1"
{
    element GE1 as Int32
    element GE2<substitutes GE1> as Int32
    element GE3<substitutes GE2> as Int32

    type T1
    #{//child-sequence
        E1<*> as Int32
        ?{//member-child-choice
            E2 as Int32
            #{//member-child-sequence
                E3 as Int32
                E4 as Int32
            }<2..>
        }<*>
        &GE1<+>
    }

    element Root as T1
}

//data
a1:Root <a1 = "urn:project1"> =
{
    E1 = 42
    E1 = 42
    E2 = 42
    E2 = 42
    E3 = 42
    E4 = 42
    E3 = 42
    E4 = 42
    E2 = 42
    a1:GE2 = 42
    a1:GE1 = 42
    a1:GE3 = 42
}
```

The schema is "structural", but the data is "flat". The data loader(validator) is a LL(1) parser, it parses element data list into schema structures. An element data is recognized by its full name.

`child-sequence` can be extended:

```
type T1
#{
    E1<*> as Int32
}

type T2 extends T1
#{
    //E1<*> is inherited
    E2<+> as Int32
}

/* The result of T2's child-sequence is:
#{
    E1<*> as Int32
    E2<+> as Int32
}
*/
```

`child-sequence` can be restricted:

```
type T1
#{
    E1 as Int32
    E2<2..10> as Int32
    #{
        E3 as Int32
        E4<nullable> as Int32
        #{
            E5 as Int32
            E6 as Int32
        }<*>
    }<*>
    ?{
        E7 as Int32
        E8 as Int32
        E9 as Int32
    }
}

type T2 restricts T1
#{
    //E1 is inherited
    E2<3..7> as Int16 //occurrence is restricted, type is restricted
    #{
        //E3 is inherited
        E4 as Int32 //nullable to non-nullable
        #{
        }<x> //optional member can be deleted
    }<+> //occurrence is restricted
    ?{
        //E7 and E8 are inherited
        E9<x> as Int32 //a choice member can be deleted even if it is required
    }
}
/* The result of T2's child-sequence is:
#{
    E1 as Int32
    E2<3..7> as Int16
    #{
        E3 as Int32
        E4 as Int32
    }<+>
    ?{
        E7 as Int32
        E8 as Int32
    }
}
*/
```

Please review the above to comprehend the schema and the data.

###Data Object Model

