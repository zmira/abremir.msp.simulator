# [MSP - *Mais Simples Possível*](table-of-contents.md)

# The MSP Language

## Syntax and Semantics

### [Code Segment - Instruction Set](code-segment-instruction-set.md)

### Control instructions

---

#### **JMP**

##### **Op. code** 23

##### **Argument** `<id_label>` or `<address>` or `<relative_address>`

##### **Description** unconditional jump to a program ADDRESS defined by a calculated `<int_16>`, based on the argument; ADDRESS is a Program Memory address that can be tagged with one `<id_label>`, or it may already be an absolute `<address>`, or it may the result of applying an offset, given by `<relative_address>`, to the current Program Counter (see note on this calculation); the program execution continues at the ADDRESS assigned to the Program Counter, calculated based on the argument

```text
PC = ADDRESS;
```

---

#### **JMPF**

##### **Op. code** 24

##### **Argument** `<id_label>` or `<address>` or `<relative_address>`

##### **Description** conditional jump to a program ADDRESS defined by a calculated `<int_16>`, based on the argument; ADDRESS to be assigned to the Program Counter is calculated identically to the `JMP` instruction, but the execution of the jump is conditioned by a logical test against the `<int_8>` value popped from the Stack

```text
SP = SP + 1;
VALUE = Stack[SP];
IF ( VALUE == FALSE ) THEN PC = ADDRESS;
ELSE PC = PC + 3;
```

---

#### **CALL**

##### **Op. code** 25

##### **Argument** `<id_label>` or `<address>` or `<relative_address>`

##### **Description** unconditional call to a sub-routine, defined by a calculated `<int_16>` address, based on the argument; ADDRESS to be assigned to the Program Counter is calculated identically to the `JMP` instruction; after executing the sub-routine (see `RET`), the execution will continue at the instruction following `CALL`, for which it is necessary to push its address to the Stack (`NextPC`)

```text
NextPC = PC + 3; 
MSB = quotient(NextPC / 256);
LSB = remainder(NextPC / 256) ;
Stack[SP] = LSB;
SP = SP - 1;
Stack[SP] = MSB;
SP = SP - 1;
PC = ADDRESS;
```

---

#### **RET**

##### **Op. code** 26

##### **Description** returns from a sub-routine, taking the program's execution to an address previously saved in the Stack; pops two `<int_8>` from the Stack and  calculates an ADDRESS, which is then assigned to the Program Counter

```text
SP = SP + 1;
MSB = Stack[SP];
SP = SP + 1;
LSB = Stack[SP];
ADDRESS = MSB * 256 + LSB;
PC = ADDRESS;
```

---

#### **HALT**

##### **Op. code** 27

##### **Description** aborts the execution of the program, regardless of there being more instructions to execute

---

#### **NOOP**

##### **Op. code** 0

##### **Description** no operation, does not do anything

---

> #### &gt; an absolute address is an address relative to the start (address 0) of the Program Memory;
> #### &gt; when a `<relative_address>` is given as argument to the instructions `JMP`, `JMPF`, and `CALL`, the absolute address is calulated by taking the address of the next instruction (PC + 3) and adding the offset represented by `<relative_address>` (`ADDRESS = PC + 3 + <relative_address>`);
> #### &gt; `HALT` should be used to terminate programs; otherwise, and since the Program Memory is initialized by default with 0 and 0 is the op. code for `NOOP`, the execution of the program will continue until the last cell of the Program Memory (31999); another use of `HALT` is, for example, enforcing the isolation of sub-routine code from the rest of the program
