# [MSP - *Mais Simples Possível*](table-of-contents.md)

# The MSP Language

## Syntax and Semantics

### [Code Segment - Instruction Set](code-segment-instruction-set.md)

### Logic instructions

---

#### **AND**

##### **Op. code** 14

##### **Description** pops two `<int_8>` from the Stack, and pushes the logical conjunction of them to the Stack

```text
SP = SP + 1;
OPERAND2 = Stack[SP];
SP = SP + 1;
OPERAND1 = Stack[SP];
Stack[SP] = OPERAND1 && OPERAND2;
SP = SP - 1;
PC = PC + 1;
```

---

#### **OR**

##### **Op. code** 15

##### **Description** pops two `<int_8>` from the Stack, and pushes the logical disjunction between them to the Stack

```text
SP = SP + 1;
OPERAND2 = Stack[SP];
SP = SP + 1;
OPERAND1 = Stack[SP];
Stack[SP] = OPERAND1 || OPERAND2;
SP = SP - 1;
PC = PC + 1;
```

---

#### **NOT**

##### **Op. code** 16

##### **Description** pops one `<int_8>` from the Stack, and pushes its logical negation to the Stack

```text
SP = SP + 1;
OPERAND = Stack[SP];
Stack[SP] = !OPERAND;
SP = SP - 1;
PC = PC + 1;
```

---

#### **EQ**

##### **Op. code** 17

##### **Description** pops two `<int_8>` from the Stack, checks if they are equal, and pushes the result of that comparison to the Stack

```text
SP = SP + 1;
OPERAND2 = Stack[SP];
SP = SP + 1;
OPERAND1 = Stack[SP];
Stack[SP] = OPERAND1 == OPERAND2;
SP = SP - 1;
PC = PC + 1;
```

---

#### **NE**

##### **Op. code** 18

##### **Description** pops two `<int_8>` from the Stack, checks if they are different, and pushes the result of that comparison to the Stack

```text
SP = SP + 1;
OPERAND2 = Stack[SP];
SP = SP + 1;
OPERAND1 = Stack[SP];
Stack[SP] = OPERAND1 != OPERAND2;
SP = SP - 1;
PC = PC + 1;
```

---

#### **LT**

##### **Op. code** 19

##### **Description** pops two `<int_8>` from the Stack, checks if one is less than the other, and pushes the result of that comparison to the Stack

```text
SP = SP + 1;
OPERAND2 = Stack[SP];
SP = SP + 1;
OPERAND1 = Stack[SP];
Stack[SP] = OPERAND1 < OPERAND2;
SP = SP - 1;
PC = PC + 1;
```

---

#### **LE**

##### **Op. code** 20

##### **Description** pops two `<int_8>` from the Stack, checks if one is less than, or equal to, the other, and pushes the result of that comparison to the Stack

```text
SP = SP + 1;
OPERAND2 = Stack[SP];
SP = SP + 1;
OPERAND1 = Stack[SP];
Stack[SP] = OPERAND1 <= OPERAND2);
SP = SP - 1;
PC = PC + 1;
```

---

#### **GT**

##### **Op. code** 21

##### **Description** pops two `<int_8>` from the Stack, checks if one is greater than the other, and pushes the result of that comparison to the Stack

```text
SP = SP + 1;
OPERAND2 = Stack[SP];
SP = SP + 1;
OPERAND1 = Stack[SP];
Stack[SP] = OPERAND1 > OPERAND2;
SP = SP - 1;
PC = PC + 1;
```

---

#### **GE**

##### **Op. code** 22

##### **Description** pops two `<int_8>` from the Stack, checks if one is greater than, or equal to, the other, and pushes the result of that comparison to the Stack

```text
SP = SP + 1;
OPERAND2 = Stack[SP];
SP = SP + 1;
OPERAND1 = Stack[SP];
Stack[SP] = OPERAND1 >= OPERAND2;
SP = SP - 1;
PC = PC + 1;
```

---

> #### &gt; the result for the logical instructions is 0 (*false*) or 1 (*true*) and, except for the unary instruction `NOT`, also the 2nd operand is popped from the Stack before the 1st operand
