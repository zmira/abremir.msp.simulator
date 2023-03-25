# [MSP - *Mais Simples Possível*](table-of-contents.md)

# The MSP Language

## Syntax and Semantics

### [Code Segment - Instruction Set](code-segment-instruction-set.md)

### Value and address manipulation instructions

---

#### **PUSH**

##### **Op. code** 1

##### **Argument** `<value>` or `<signed_value>`

##### **Description** pushes either one `<int_8>` or one `<sint_8>` to the Stack

```text
Stack[SP] = <int_8> or Stack[SP] = <sint_8>;
SP = SP - 1;
PC = PC + 2;
```

---

#### **PSHA**

##### **Op. code** 2

##### **Argument** `<value>` or `<signed_value>`

##### **Description** pushes one `<int_16>` to the Stack; if the argument is not `<address>`, `<id_variable>` is converted into its corresponding `<address>`; this address is split into two `<int_8>` (LSB and MSB, such that `<address>` is one `<int_16>` equal to MSB * 256 + LSB) that are pushed to the Stack (first the LSB, then the MSB)

```text
Stack[SP] = LSB;
SP = SP - 1;
Stack[SP] = MSB;
SP = SP - 1;
PC = PC + 3;
```

---

#### **LOAD**

##### **Op. code** 3

##### **Description** pops two `<int_8>` from the Stack, calculates a Data Memory address, and pushes the value at that Data Memory address to the Stack

```text
SP = SP + 1;
MSB = Stack[SP];
SP = SP + 1;
LSB = Stack[SP];
ADDRESS = MSB * 256 + LSB;
Stack[SP] = DataMemory[ADDRESS];
SP = SP - 1;
PC = PC + 1;
```

---

#### **LDA**

##### **Op. code** 4

##### **Description** pops two `<int_8>` from the Stack and calculates a Data Memory address; the value at that Data Memory address, and the value at the following address, become, respectively, the LSB and MSB of another address, which are then pushed to the Stack (first the LSB, then the MSB)

```text
SP = SP + 1;
MSB = Stack[SP];
SP = SP + 1;
LSB = Stack[SP];
ADDRESS = MSB * 256 + LSB;
LSB = DataMemory[ADDRESS];
Stack[SP] = LSB;
SP = SP - 1;
MSB = DataMemory[ADDRESS + 1];
Stack[SP] = MSB;
SP = SP - 1;
PC = PC + 1;
```

---

#### **STORE**

##### **Op. code** 5

##### **Description** pops one `<int_8>` VALUE from the Stack and calculates a Data Memory address with two `<int_8>` also popped from the Stack; VALUE is written at the Data Memory address

```text
SP = SP + 1;
VALUE = Stack[SP];
SP = SP + 1;
MSB = Stack[SP];
SP = SP + 1;
LSB = Stack[SP];
ADDRESS = MSB * 256 + LSB;
DataMemory[ADDRESS] = VALUE;
PC = PC + 1;
```

---

#### **STRA**

##### **Op. code** 6

##### **Description** pops two `<int_8>` from the Stack (possibly the LSB and MSB of a Data Memory address); then pops two more `<int_8>` and calculates a Data Memory address (ADDRESS); the initial two `<int_8>` are written to the Data Memory at ADDRESS and ADDRESS+1

```text
SP = SP + 1;
MSB2 = Stack[SP];
SP = SP + 1;
LSB2 = Stack[SP];
SP = SP + 1;
MSB = Stack[SP];
SP = SP + 1;
LSB = Stack[SP];
ADDRESS = MSB * 256 + LSB;
DataMemory[ADDRESS] = LSB2;
DataMemory[ADDRESS + 1] = MSB2;
PC = PC + 1;
```

---

#### **IN**

##### **Op. code** 7

##### **Description** pushes to the Stack the contents of the special port *input*; this results from reading one `<signed_value>` from the keyboard buffer

```text
Stack[SP] = input;
SP = SP - 1;
PC = PC + 1;
```

---

#### **OUT**

##### **Op. code** 8

##### **Description** writes one `<signed_value>` popped from the Stack to the special port *output*

```text
SP = SP + 1;
output = Stack[SP];
PC = PC + 1;
```

---

#### **INC**

##### **Op. code** 28

##### **Description** pushes to the Stack the contents (ASCII code, of type `<value>`) of the special port *input*; this results from reading one `<character>` from the keyboard buffer

```text
Stack[SP] = input;
SP = SP - 1;
PC = PC + 1;
```

---

#### **OUTC**

##### **Op. code** 29

##### **Description** writes one `<character>` to the special port *output*; this results from popping a `<value>` from the Stack

```text
SP = SP + 1;
output = Stack[SP];
PC = PC + 1;
```

---

> ##### &gt; instructions `PUSH`, `LOAD`, and `STORE` operate on values of one byte, while `PSHA`, `LDA`, and `STRA` operate on 2 byte addresses
