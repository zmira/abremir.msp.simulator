# [MSP - *Mais Simples Possível*](table-of-contents.md)

# The MSP Language

## [Errors and Warnings](errors-and-warnings.md)

## Assembly Warnings

---

### Data Segment warnings

#### **Warning code** 0

#### **Summary** no variables declared

#### **Description** no variable has been declared in the Memory Segment and, due to this, the Data Memory is completely free and initialized with zero

```text
MEMORIA DE DADOS
CODIGO
HALT
```

---

#### **Warning code** 1

#### **Summary** uninitialized Data Memory identifier(s)

#### **Description** a variable declared in the Data Segment has not been initialized or it has only been done partially; non-initialized cells will have the value zero by default

```text
MEMORIA DE DADOS
x 0 TAM 1 ; no initialization
y 1 TAM 3 VAL 1 ; partial initialization (2nd and 3rd cells will have value zero)
CODIGO
HALT
```

---

#### **Warning code** 2

#### **Summary** Data Memory identifier declared, but never referenced

#### **Description** a variable declared in the Data Segment is never referenced in the Code Segment

```text
MEMORIA DE DADOS
a 0 TAM 1 ; the variable "a" is never used in the code
CODIGO
HALT
```

---

### Code Segment warnings

#### **Warning code** 3

#### **Summary** no instructions declared

#### **Description** since no instruction was written in the Code Segment, a warning is raised; similarly to the "empty" Data Memory, all Program Memory cells will have the value zero by default

```text
MEMORIA DE DADOS
CODIGO
```

---

#### **Warning code** 4

#### **Summary** label declared, but never referenced

#### **Description** a label declared in the Code Segment is never referenced in it

```text
MEMORIA DE DADOS
CODIGO
start: IN ; the label "start" is never referenced in the program
OUT
HALT
```

---

#### **Warning code** 5

#### **Summary** Data Memory identifier matches label name

#### **Description** the identifier for a label declared in the Code Segment coincides with the identifier of a label in the Data Segment

```text
MEMORIA DE DADOS
sum 0 TAM 1
CODIGO
sum: HALT ; "sum" identifies both a label and a variable
```

---

#### **Warning code** 6

#### **Summary** `PSHA` instruction references memory space not reserved by any Data Memory identifier

#### **Description** as argument of the `PSHA` instruction a valid 16-bit address is being used (in the range [0, 31999]) but it does not reference any Data Memory cell reserved by a Data Segment variable

```text
MEMORIA DE DADOS
x 0 TAM 100
CODIGO
PSHA 0 ; OK - this is the start address of the declared variable "x"
PSHA 50 ; OK - 50 is an address inside the reserved area for "x"
PSHA 200 ; not OK - there isn't any variable that has reserved Data Memory cell 200
```

---

#### **Warning code** 7

#### **Summary** `HALT` instruction not found

#### **Description** the instruction `HALT` has not been found in the Code Segment, this means the program will run until the Program Counter reaches the end of the addressable for the Program Memory

```text
MEMORIA DE DADOS
CODIGO
PUSH 10
```

---
