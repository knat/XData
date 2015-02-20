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
        Attribute2 = (a1:MyListType)#[2 3 5 7 11]
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
|`normal-name-token`|`identifier1` `_id1` `标识符1` `true`|
|`verbatim-name-token`|`@identifier1` `@_id1` `@标识符1` `@true`|
|`normal-string-value-token`|`"abcd\r\nefg\t\u0041\u0042"`|
|`verbatim-string-value-token`|`@"d:\dir1\file.txt,""\r\n"`|
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
uri-alias '=' uri-value
;
uri-alias:
name-token
;
uri-value:
string-value-token
;
qualifiable-name:
(uri-alias ':')? local-name
;
local-name:
name-token
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
local-name ('=' attribute-value)?
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

A `uri-aliasing` associates a URI with an alias:

```
<a1 = "http://example.com/project1">
```

A `qualifiable-name` combines a URI with a local name:

```
a1:LocalName
```

The URI is referenced via the alias.

An empty URI:

```
a1:LocalName <a1 = "">
```

is equal to no URI:

```
LocalName
```

Unlike XML, there is no default URI in XData.

A URI and a local name forms a full name. If the URI is not empty, we call it qualified full name and can be expressed as `{URI}LocalName` in semantics:

```
{http://example.com/project1}LocalName
```

If the URI is empty or has no URI, we call it unqualified full name and can be expressed as `LocalName` in semantics.

Aliases are defined in an element and can be referenced by the self element and descendant nodes, A descendant element can redefine a alias:

```
a1:Element1 <a1 = "http://example.com"> = (a1:MyComplexType)
    {
        a1:Element2 = (sys:Int16)42
        Element2 = $ (a1:MyInt32)42
        a1:Element3 <a1 = "http://other.com"> =
            {
                a1:Element1 = ;
                a1:Element2
            }
    }
```

Alias 'sys' is reserved for the system URI `http://xdata-io.org`. Use `sys` to reference the predefined system types.

The XData data is tightly coupled with the schema. In most cases, `type-indicator` is not required.

An `attribute` is identified by its `local-name`. In an `attributes`, every `attribute` must have a unique `local-name`.

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
    ]
    $ 42//simple child
Element2 =
    [//attributes
    ]
    {//complex children
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
42//atom value
true//atom value
@"c:\file.txt"//atom value
(sys:DateTimeOffset)"2015-01-24T15:32:43+07:00"//atom value
(a1:MyListType)#[2 3 (a1:MyInt32)5 7 11]//list value
#[]//list value
```

`integer-value-token`, `decimal-value-token`, `real-value-token`, `true` and `false` can also be written as `string-value-token`:

```
42 == "42"
42.42 == "42.42"
42.42e7 == "42.42e7"
true == "true"
false == "false"
```

Please review the above grammars to comprehend the XData data.

### Schema

Schema is the contract or specification of your data. An example:

```
//FirstLook.xds
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
(namespace-alias ':')? local-name
;
local-name:
name-token
;
namespace-member:
type | global-element 
;
type:
'type' local-name type-annotations? type-body
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
attributes children? | children
;
attributes:
'[' attribute* ']'
;
attribute:
local-name attribute-annotations? 'as' qualifiable-name
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
child-set | child-sequence
;
child-set:
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
local-name local-element-annotations? 'as' qualifiable-name
;
local-element-annotations:
'<' (nullable | member-name | occurrence-or-delete)* '>'
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
'element' local-name global-element-annotations? 'as' qualifiable-name
;
global-element-annotations:
'<' (nullable | abstract-or-sealed | substitute)* '>'
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

A `compilation-unit` can contain zero or more `namespace`. A `namespace` is identified by a `uri`. If `namespace-member`s want to reference to `namespace-member`s in another `namespace`, `import` is required:

```
namespace "http://example.com/project1"
{
    //...
}
namespace "http://example.com/project2"
{
    import "http://example.com/project1" as p1
    //...
}
namespace "http://example.com/project3"
{
    import "http://example.com/project1" as p1
    import "http://example.com/project2" as p2
    //...
}
```

It is recommended that use `uri-aliasing` to reduce typing:

```
alias "http://example.com/project1" as p1
alias "http://example.com/project2" as p2
alias "http://example.com/project3" as p3

namespace p1
{
    //...
}
namespace p2
{
    import p1 as p1
    //...
}
namespace p3
{
    import p1 as p1
    import p2 as p2
    //...
}
```

`uri-value` can be empty:

```
namespace ""
{
    //...
}
```

`namespace-member` can be `type` or `global-element`, which are identified by a `local-name`. In a `namespace`, every member must has a unique `local-name`:

```
namespace p1
{
    type T1 ...
    element G1 ...
    element T1 ...//ERROR: duplicate member name 'T1'
}
namespace p1
{
    type T2 ...
    element G1 ...//ERROR: duplicate member name 'G1'
}
```

If multiple `namespace`s have a same `uri`, they are merged into a logical namespace by the schema compiler.











=================


If a `qualifiable-name` has no alias, or if the URI is an empty string, we call the full name unqualified full name, otherwise qualified full name.


An element has a local name 

A `parsing-unit` must have one and only one `element`.



