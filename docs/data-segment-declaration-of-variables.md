# [MSP - *Mais Simples Possível*](table-of-contents.md)

# The MSP Language

## Syntax and Semantics

### Data Segment - Declaration of Variables

---

As mentioned previously, the Data Segment is defined with the reserved words `MEMORIA DE DADOS` and continues until the beginning of the Code Segment, defined by the reserved word `CODIGO`.

The Data Segment is where variables required by a program are declared, and eventually initialized.

As defined by the [MSP grammar](grammar.md), the declaration of a variable follows the format

`<id_variable> <address> TAM <size> VAL <value1> ... <valueN>`

For example, `x 100 TAM 20 VAL 4` is the declaration of the variable `x`, at address `100`, with the size of `20` bytes, and having the 1st byte initialized with the value `4`. The remaining 19 bytes (addresses 101 to 119) will be initialized, by default, with the value `0`.

The semantics for the declaration of variables comprises a few important aspects:

- each variable identifier is unique; there cannot be two variable declarations using the same identifier;
- variables are not required to be defined contiguously starting from address 0, there can be "holes" in the Data Memory;
- the memory space reserved for one variable cannot overlap with the memory space reserved for another variable, i.e. the range [`<address>`, `<address>` + `<size>` - 1] must be mutually exclusive for all declared variables;
- initialization of the variable's allocated space is optional; by default, the variable's allocated space will be initialized with the value 0;
- variable initialization can be done with any integer value in the interval [-128, 255], with optional sign for positive numbers; as mentioned previously, during the execution of a program, these values will be interpreted based on context;
- it is perfectly legal to not declare any variable in the Data Segment; any access done to the Data Memory must take into account that it will be completely initialized with zero (it is possible to read from, and write to, the Data Memory without having any variable declared...);
