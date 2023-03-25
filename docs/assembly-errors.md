# [MSP - *Mais Simples Possível*](table-of-contents.md)

# The MSP Language

## [Errors and Warnings](errors-and-warnings.md)

## Assembly Errors

---

### Internal error

#### **Error code** 0

#### **Summary** Data Memory, or Program Memory not available

#### **Description** there was an internal error that prevented the allocation of the Data Memory and/or the Program Memory

---

### Data Segment errors

#### **Error code** 1

#### **Summary** expected `MEMORIA DE DADOS`

#### **Description** the `MEMORIA DE DADOS` pragma was not found; it must occupy a single line, and **must** be included even if no variables are declared

```text
CODIGO
```

---

#### **Error code** 2

#### **Summary** Data Memory exhausted

#### **Description** the existing declarations of variables have exhausted the available capacity of the Data Memory, this not allowing any more variable allocations

```text
MEMORIA DE DADOS
var1 0 TAM 20000
var2 19999 TAM 20000 ; not OK - this will exhaust the 32000 available memory cells
CODIGO
```

---

#### **Error code** 3

#### **Summary** invalid Data Memory address

#### **Description** the initial address assigned to a variable is invalid; this could be caused by an address greater than 31999 or a collision with an area previously reserved by another variable; keep in mind that memory allocation for variables may not always be contiguous, but they are always exclusive; collisions may also originate from the declared size

```text
MEMORIA DE DADOS
var1 100 TAM 20 ; OK - memory [100, 119] reserved
var2 110 TAM 1 ; not OK - collision, memory cell 110 is in the range [100, 119] already reserved
CODIGO
```

---

#### **Error code** 4

#### **Summary** Data Memory identifier already defined

#### **Description** declaration of a variable, in the Data Segment, for which the identifier has already been used for another variable

```text
MEMORIA DE DADOS
x 0 TAM 1 ; OK
x 200 TAM 3 ; not OK - re-use of identifier "x"
CODIGO
```

---

#### **Error code** 5

#### **Summary** unrecognized address of Data Memory

#### **Description** following the declaration of a variable its initial address must be defined; it must be a 16-bit positive integer in the range of the addressable space of the Data Memory ([0, 31999])

```text
MEMORIA DE DADOS
y TAM 1 ; not OK - lacks the initial address of the variable
x 123abc--?? TAM 3 ; not OK - "123abc--??" is not a 16-bit integer in [0, 31999]
CODIGO
```

---

#### **Error code** 6

#### **Summary** expected `TAMANHO`

#### **Description** the required reserved word `TAMANHO` (or one of its abbreviations down to `TAM`) was not found; this reserved word appears between the address and the specification of the size of the variable

```text
MEMORIA DE DADOS
a 0 100 ; not OK - "TAM" not found between "0" and "100"
b 200 T 12 ; not OK - "T" is not recognized as being an alias of "TAMANHO"
CODIGO
```

---

#### **Error code** 7

#### **Summary** expected value for `TAMANHO`

#### **Description** following the reserved word `TAMANHO` it is required to define the size of the variable, which must be a positive integer in the range [1, 32000]

```text
MEMORIA DE DADOS
x 0 TAM 10 ; OK
y 20 TAM ; not OK - variable size not found
CODIGO
```

---

#### **Error code** 8

#### **Summary** invalid value for `TAMANHO`

#### **Description** the size being assigned to the variable is either zero, or it is greater than the size of the Data Memory (32000), or it collides (completely or partially) with a memory area previously reserved by another variable, or it is such that the end of the allocated memory is beyond the addressable space of the Data Memory (i.e. greater than 31999)

```text
MEMORIA DE DADOS
x 0 TAM 0 ; not OK - variable has size 0
y 10 TAM 40000 ; not OK - size greater than 32000
z 100 TAM 10 ; OK
a 90 TAM 50 ; not OK - collision with reserved memory for "z"
b 31000 TAM 1000 ; not OK - "address" + "size" is outside the Data Memory
CODIGO
```

---

#### **Error code** 9

#### **Summary** expected initialization value(s) to be in [-128, 255]

#### **Description** the usage of the reserved word `VALOR` (or one of its abbreviations down to `VAL`) makes it required to define at least one initialization value in the range [-128, 255] (optional + for positive values); the remaining values (in the case where `TAMANHO` is greater than 1) can be omitted, but will be defaulted to zero

```text
MEMORIA DE DADOS
x 0 TAM 1 VAL ; not OK - initialization value not found
y 10 TAM 2 ; OK - all values are initialized with zero
a 20 TAM 3 VAL 100 ; OK - remaining values are initialized with zero
b 40 TAM 1 VAL 1|b%3 ; not OK - "1|b%3" is not a 16-bit integer in [-128, 255]
CODIGO
```

---

#### **Error code** 10

#### **Summary** initialization values exceed reserved memory

#### **Description** the list of values declared to initialize the reserved memory space allocated to a variable contains more items than the number of bytes reserved

```text
MEMORIA DE DADOS
x 0 TAM 2 VAL 1 2 3 ; not OK - allocated size is "2" but there "3" values
CODIGO
```

---

#### **Error code** 11

#### **Summary** 8-bit integer value outside [-128, 255] range

#### **Description** one initialization constant cannot exceed 255, nor can it be less than -128; values above 127 are interpreted based on context

```text
MEMORIA DE DADOS
z 0 TAM 1 VAL 256 ; not OK - value greater than 255
a 1 TAM 1 VAL -129 ; not OK - value less than -128
CODIGO
```

---

### Errors shared between the Data Segment and the Code Segment

#### **Error code** 12

#### **Summary** unknown or unexpected token

#### **Description** a set of characters was encountered that have no meaning for the MSP language; the only recognized tokens are alphanumeric identifiers of variables and labels (if not reserved words), and signed or unsigned 8-bit and 16-bit integers (`+` is optional for positive integers); comments must be preceded by `;`; this error is also raised if following the reserved word `CODIGO`, on the same line, there is anything other than a comment

```text
MEMORIA DE DADOS d7)8/&f8 ; not OK
x 0 TAM 1 d7)8/&f8 ; not OK - expected "VAL"
y 10 TAM 2 VAL 1 88d7)8/&f8 ; not OK - invalid initialization value
CODIGO 88d7)8/&f8 ; not OK
88d7)8/&f8 ; not OK
```

---

#### **Error code** 13

#### **Summary** expected `CODIGO`, or invalid variable identifier

#### **Description** a valid alphanumeric variable identifier, or the delimiter `CODIGO` that begins the Code Segment, was not found

```text
MEMORIA DE DADOS
/gghg% ; not OK - invalid variable identifier
PUSH 10 ; not OK - "PUSH" is a reserved word
CODIGO
```

---

### Code Segment errors

#### **Error code** 14

#### **Summary** end-of-line not found, or instruction expected

#### **Description** the Code Segment allows empty lines, lines with only comments, lines with a single instruction (optionally preceded with a label and/or followed by a comment); two instructions or labels cannot be declared on the same line

```text
MEMORIA DE DADOS
CODIGO
PUSH 10 PSHA x ; not OK - two  instructions on the same line
RET (7lijklsfd ; not OK - "RET" does not take any arguments
Label1: Label2: ; not OK - two labels on the same line
```

---

#### **Error code** 15

#### **Summary** Program Memory exhausted

#### **Description** the preceding instructions have exhausted the Program Memory, not being able to take any more code

---

#### **Error code** 16

#### **Summary** label already defined

#### **Description** declaration of a label identifier that has already been previously defined

```text
MEMORIA DE DADOS
CODIGO
loop: PUSH 10
loop: RET ; not OK - label "loop" already defined above
```

---

#### **Error code** 17

#### **Summary** potential label without colon (`:`)

#### **Description** the MSP language syntax requires that a label must be followed by `:`

```text
MEMORIA DE DADOS
CODIGO
SubProc PSHA x ; not OK - ":" missing, after "SubProc"
End : RET ; OK - there can be a space between the label and ":"
```

---

#### **Error code** 18

#### **Summary** unexpected instruction argument

#### **Description** the instruction does not require more arguments than the ones already defined

```text
MEMORIA DE DADOS
CODIGO
ADD 1 ; not OK - "ADD" does not take arguments
PSHA x z ; not OK - "PSHA" only takes one argument
```

---

#### **Error code** 19

#### **Summary** `PUSH` 8-bit integer argument outside [-128, 255] range

#### **Description** the `PUSH` instruction places an 8-bit integer in the Stack; any other value is not valid; not providing a value also raises this error

```text
MEMORIA DE DADOS
CODIGO
PUSH x ; not OK - "x" is not an 8-bit integer
PUSH ; not OK - missing argument
```

---

#### **Error code** 20

#### **Summary** invalid `PSHA` address, 16-bit value must be in [0, 31999]

#### **Description** the argument for `PSHA` was recognized as a 16-bit integer, but its value is not within the Data Memory address space [0, 31999]

```text
MEMORIA DE DADOS
CODIGO
PSHA 32000 ; not OK - "32000" is not within [0, 31999]
```

---

#### **Error code** 21

#### **Summary** use of undeclared Data Memory identifier as `PSHA` argument

#### **Description** the argument of `PSHA` is a variable identifier that has not been declared in the Data Segment

```text
MEMORIA DE DADOS
x 0 TAM 1
CODIGO
PSHA z ; not OK - "z" has not been declared in the Data Segment
```

---

#### **Error code** 22

#### **Summary** use of label, instead of Data Memory identifier, as `PSHA` argument

#### **Description** the argument used for `PSHA` is a label, not a variable, identifier; it is allowed to use the same identifier both as a variable and as a label

```text
MEMORIA DE DADOS
x 0 TAM 1
CODIGO
y: PSHA y ; not OK - "y" is not a variable, even though it is a label
x: PSHA x ; OK - "x" is simultaneously a label and a variable
```

---

#### **Error code** 23

#### **Summary** unknown Data Memory identifier used as `PSHA` argument

#### **Description** the argument for `PSHA` is not valid; valid arguments are 16-bit addresses (not required to be linked to a variable), or a known variable identifier (in lieu of its address)

```text
MEMORIA DE DADOS
x 0 TAM 1
CODIGO
PSHA 1234 ; OK - a valid 16-bit address
PSHA x ; OK - a valid variable identifier
PSHA 0/887&5 ; not OK
```

---

#### **Error code** 24

#### **Summary** undefined label

#### **Description** a label used in a jump instruction has not been defined

```text
MEMORIA DE DADOS
CODIGO
CALL end ; not OK - "end" has not been defined
HALT
```

---

#### **Error code** 25

#### **Summary** attempt to jump outside Program Memory limits ([0, 31999])

#### **Description** the relative or absolute address used as the argument of a jump instruction is pointing to a location outside the address space of the Program Memory [0, 31999]

```text
MEMORIA DE DADOS
CODIGO
JMP -4 ; not OK - jump to absolute address -1 (PC[0] + 3 + -4)
JMP 32000 ; not OK - jump to absolute address greater than 31999
JMP +31992 : not OK - jump to absolute address greater than 31999 (PC[3 + 3] + 3 + 31992)
```

---

#### **Error code** 26

#### **Summary** invalid argument for branch instruction (`JMP`, `JMPF`, `CALL`)

#### **Description** the argument provided for a jump instruction is not valid, or the jump instruction is referencing itself; valid arguments are an address associated with a label, or a 16-bit absolute address in the range [0, 31999], or a relative address (positive or negative) that is translatable to a 16-bit absolute address in the range [0, 31999]

```text
MEMORIA DE DADOS
CODIGO
JMP k(76% ; not OK
loop: CALL loop ; not OK - jump instruction cannot reference itself
```

---

### External error

#### **Error code** 27

#### **Summary** no code to assemble

#### **Description** no text was found to be assembled

---

#### **Error code** 999

#### **Summary** syntax error

#### **Description** other types of syntax errors not already caught and assigned a particular error type

---

#### **Error code** 1000

#### **Summary** other exception

#### **Description** a "catch-all" exception for any type of exception generated while parsing the source code

---
