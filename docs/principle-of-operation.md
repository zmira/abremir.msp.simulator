# [MSP - *Mais Simples Possível*](table-of-contents.md)

# Stack Virtual Machine

## Principle of Operation

---

To start the execution of a program the `PC` must contain the address of the Program Memory where the first instruction is located at. From that point on, and once the virtual machine has been "given" the order to start, its operation is very simple and systematic, based on a loop called *fetch-decode-execute*.

The instruction "pointed to" by the `PC` is loaded into the Decoder, which translated its meaning and sends, to the other units, the necessary commands to complete the action. With this, the `PC` is automatically incremented so that it now contains the address of the next instruction to be executed.

Once the execution of one instruction is completed, the cycle repeats itself: a new instruction is brought to the Decoder and the `PC` is incremented, this way continuing execution of the program until the decoded instruction indicates the ending of execution.

The commands send by the Decoder to the other units correspond to a very limited, and elementary, set of actions but, if combined correctly, are enough to solve any algorithmic problem.

Among others, these actions can be:

- transfer of data between the top of the stack and one of the special cells `input` or `output`;
- execution of one arithmetic (addition, subtraction, multiplication, or division) or logical (conjunction, disjunction, or negation) operation;
- adjustment to the normal execution sequence of the instructions stored in the Program Memory, forcing the `PC` to assume a value different from the it automatically takes after executing one instruction;
