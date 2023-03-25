# [MSP - *Mais Simples Possível*](table-of-contents.md)

# The MSP Language

## Syntax and Semantics

### Code Segment - Instruction Set

---

The Code Segment is initialized with the reserved word `CODIGO` and continues until the end of the text file (`EOF`).

The below links give access to the description of the instruction set for MSP. For each instruction you will be given the following:

- mnemonic;
- numerical operation code (*opcode*);
- arguments (if any) and their type/range (using the notation from the [BNF grammar of MSP](grammar.md));
- the instruction's meaning (an informal description followed by the formal description) in terms of operations on the Stack, Data Memory, Stack Pointer (SP), and Program Counter (PC);

It is assumed that every instruction will increment PC in one unit, immediately after being executed. As such, this will be omitted from the description of the instruction. Instructions which have arguments will analyzed individually.

Be reminded that the Stack grows from the end of its addressable space (address 31999) to the beginning (address 0) and that, therefore, the SP decreases every time something is added to the Stack (`push`) and increases every time something is removed from it (`pop`). The SP always points to the next free memory cell of the Stack.

The instruction set can be divided into 3 groups:

- [Value and address manipulation](code-segment-instruction-set-value-and-address-manipulation-instructions.md)
- Arithmetic and logic operations
  - [Arithmetic](code-segment-instruction-set-arithmetic-instructions.md)
  - [Logic](code-segment-instruction-set-logic-instructions.md)
  - [Bitwise](code-segment-instruction-set-bitwise-instructions.md)
- [Control of the execution sequence of the program](code-segment-instruction-set-control-instructions.md)
