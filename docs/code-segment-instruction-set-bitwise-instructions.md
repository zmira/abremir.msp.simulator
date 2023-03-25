# [MSP - *Mais Simples Possível*](table-of-contents.md)

# The MSP Language

## Syntax and Semantics

### [Code Segment - Instruction Set](code-segment-instruction-set.md)

### Bitwise instructions

---

#### **ANDB**

##### **Op. code** 30

##### **Description** pops two `<int_8>` from the Stack, and pushes the resulting bitwise conjunction of them to the Stack

```text
SP = SP + 1;
OPERAND2 = Stack[SP];
SP = SP + 1;
OPERAND1 = Stack[SP];
Stack[SP] = OPERAND1 & OPERAND2;
SP = SP - 1;
PC = PC + 1;
```

---

#### **ORB**

##### **Op. code** 31

##### **Description** pops two `<int_8>` from the Stack, and pushes the resulting bitwise disjunction between them to the Stack

```text
SP = SP + 1;
OPERAND2 = Stack[SP];
SP = SP + 1;
OPERAND1 = Stack[SP];
Stack[SP] = OPERAND1 | OPERAND2;
SP = SP - 1;
PC = PC + 1;
```

---

#### **NOTB**

##### **Op. code** 32

##### **Description** pops one `<int_8>` from the Stack, and pushes the bitwise negation (i.e. one's complement) of it to the Stack

```text
SP = SP + 1;
OPERAND = Stack[SP];
Stack[SP] = ~OPERAND;
SP = SP - 1;
PC = PC + 1;
```

---

> #### &gt; bitwise operations are a particular case of logical operations, in which the logical operation is executed between the bits of each operand (bit 0 with bit 0, ... bit 7 with bit 7), and, as for the order of retrieval of operands from the Stack, also the 2nd operand is popped before the 1st operand (with the exception of the unary operand `NOTB`)
