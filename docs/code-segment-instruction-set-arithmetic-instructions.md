# [MSP - *Mais Simples Possível*](table-of-contents.md)

# The MSP Language

## Syntax and Semantics

### [Code Segment - Instruction Set](code-segment-instruction-set.md)

### Arithmetic instructions

---

#### **ADD**

##### **Op. code** 9

##### **Description** pops two `<sint_8>` from the Stack, and pushes the addition of them to the Stack

```text
SP = SP + 1;
OPERAND2 = Stack[SP];
SP = SP + 1;
OPERAND1 = Stack[SP];
Stack[SP] = OPERAND1 + OPERAND2;
SP = SP - 1;
PC = PC + 1;
```

---

#### **SUB**

##### **Op. code** 10

##### **Description** pops two `<sint_8>` from the Stack, and pushes the subtraction of them to the Stack

```text
SP = SP + 1;
OPERAND2 = Stack[SP];
SP = SP + 1;
OPERAND1 = Stack[SP];
Stack[SP] = OPERAND1 - OPERAND2;
SP = SP - 1;
PC = PC + 1;
```

---

#### **MUL**

##### **Op. code** 11

##### **Description** pops two `<sint_8>` from the Stack, and pushes the product of them to the Stack

```text
SP = SP + 1;
OPERAND2 = Stack[SP];
SP = SP + 1;
OPERAND1 = Stack[SP];
Stack[SP] = OPERAND1 * OPERAND2;
SP = SP - 1;
PC = PC + 1;
```

---

#### **DIV**

##### **Op. code** 12

##### **Description** pops two `<sint_8>` from the Stack, and pushes the resulting quotient of the division with them to the Stack

```text
SP = SP + 1;
OPERAND2 = Stack[SP];
SP = SP + 1;
OPERAND1 = Stack[SP];
Stack[SP] = OPERAND1 / OPERAND2;
SP = SP - 1;
PC = PC + 1;
```

---

#### **ADDA**

##### **Op. code** 13

##### **Description** pops one `<sint_8>` from the Stack (OFFSET) and calculates an ADDRESS with two `<int_8>` also popped from the Stack; OFFSET is added to ADDRESS and the resulting address is decomposed, into LSB and MSB, and pushed to the Stack

```text
SP = SP + 1;
OFFSET = Stack[SP];
SP = SP + 1;
MSB = Stack[SP];
SP = SP + 1;
LSB = Stack[SP];
ADDRESS = MSB * 256 + LSB + OFFSET;
MSB = quotient(ADDRESS / 256) ;
LSB = remainder(ADDRESS / 256);
Stack[SP] = LSB;
SP = SP - 1;
Stack[SP] = MSB;
SP = SP - 1;
PC = PC + 1;
```

---

> #### &gt; with the exception of `ADDA`, who's result is of type `<address>`, result of the remaining arithmetic instructions is of type `<sint_8>`, and the 2nd operand is popped from the Stack before the 1st operand
