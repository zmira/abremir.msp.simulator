# [MSP - *Mais Simples Possível*](table-of-contents.md)

# The MSP Language

## Grammar

---

The MSP programming language, with which assembly programs will be written for the Stack Virtual Machine, is defined by the BNF ([Backus-Naur Form](https://en.wikipedia.org/wiki/Backus%E2%80%93Naur_form)) grammar presented below.

In short, the BNF notation is composed of:

- **'xyz'**: `xyz` is a well defined value in the language;
- **&lt;xyz&gt;**: sequence of elements equal to, or of type, `xyz`;
- **[xyz]**: `xyz` occurs once, at most;
- **(xyz)\***: `xyz` occurs zero, or more, times;
- **(xyz)+**: `xyz` occurs once, or more, times;

```text
<program>          ::= (<neutral>)* <declarations> (<neutral>)* <instructions> (<neutral>)* <end_of_text>
<neutral>          ::= <comment> | <empty>
<comment>          ::= (<whitespace>)* ';' (<character>)* end_of_line
<whitespace>       ::= ' ' | '\t' | '\r'
<character>        ::= any_ASCII_character_except_end_of_line
<end_of_line>      ::= '\n'
<empty>            ::= (<whitespace>)* end_of_line
<declarations>     ::= 'MEMORIA DE DADOS' (<neutral>)+ (<declaration>)*
<declaration>      ::= (<whitespace>)* <id_variable> (<whitespace>)+ <address> (<whitespace>)+ 
                       ('TAM' | 'TAMA' | 'TAMAN' | 'TAMANH' | 'TAMANHO') 
                       (<whitespace>)+ <size> [ (<whitespace>)+ 
                       ('VAL' | 'VALO' | 'VALOR' | 'VALORE' | 'VALORES') 
                       (<whitespace>)+ ( (<value> | <signed_value>) (<whitespace>)*)+ ] (<neutral>)+
<id_variable>      ::= (<alphanumeric>)+
<alphanumeric>     ::= <digit> | <letter>
<digit>            ::= '0' | '1' | ... | '9'
<letter>           ::= 'a' | ... | 'z' | 'A' | ... | 'Z' | '_'
<address>          ::= <int_16>
<int_16>           ::= integer_16_bit_in[0,31999]
<size>             ::= <int_16>
<value>            ::= <int_8>
<int_8>            ::= integer_8_bit_in_[0,255]
<signed_value>     ::= <sint_8>
<sint_8>           ::= integer_8_bit_in_[-128,127]
<instructions>     ::= 'CODIGO' (<neutral>)+ ( [ (<whitespace>)* <id_label>':'] (<neutral>)* <instruction> )*
<id_label>         ::= (<alphanumeric>)+
<instruction>      ::= (<whitespace>)* <mnemonic> [ (<whitespace>)+ <argument> ] (<neutral>)+
<mnemonic>         ::= 'PUSH' | 'PSHA' | ... | 'NOOP'
<argument>         ::= <value> | <signed_value> | <id_variable> | <id_label> | <address> | <relative_address>
<relative_address> ::= <sint_16>
<sint_16>          ::= integer_16_bit_in_[-31999,31999]
<end_of_text>      ::= '\0'
```

Regarding the proposed grammar for the MSP assembly programming language, the following should be noted:

- the delimiters for the Data and Code segments, respectively `MEMORIA DE DADOS` and `CODIGO` pragmas, are required, even if empty (which would result in a program without variables and/or without code);
- in each line of the Data Segment (lines between `MEMORIA DE DADOS` and `CODIGO`) and of the Code Segment (lines between `CODIGO` and the end of the file (`EOF`)) it is not allowed to declare more than one variable (Data Segment) or label (Code Segment); it is also not allowed to declare more than one instruction per line in the Code Segment;
- empty lines and comments are allowed both in the Data Segment and in the Code Segment;
- a comment starts with `;` and allows any characters until the end of the line (`EOL`);
- variable identifiers or labels must contain at least one `<letter>`, not being possible for them to be only a sequence of digits, underscores (`_`), or the reserved word `CCODIGO`;
- variable and label identifiers can be the same;
- variable identifiers, label identifiers, reserved words (`MEMORIA DE DADOS`, `TAMANHO`, `VALORES`, `CODIGO`, and instruction mnemonics) are not case-sensitive, e.g. `var1` is the same as `VaR1`, `PSHA` is the same as `PSha`, etc.;
- the reserved words `TAMANHO` and `VALORES` can be abbreviated down to the minimum `TAM` and `VAL`, respectively;
- for positive numbers, the `+` sign is optional; for negative numbers, the `-` is required;
