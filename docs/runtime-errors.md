# [MSP - *Mais Simples Possível*](table-of-contents.md)

# The MSP Language

## [Errors and Warnings](errors-and-warnings.md)

## Runtime Errors

---

#### **Error code** 100

#### **Summary** divide by zero

#### **Description** a division operation was attempted where the divisor is zero

```text
MEMORIA DE DADOS
CODIGO
PUSH 10
PUSH 0
DIV
```

---

#### **Error code** 101

#### **Summary** underflow error

#### **Description** an operation yields a result too small to store

```text
MEMORIA DE DADOS
CODIGO
PUSH -100
PUSH -100
ADD ; for arithmetic operations, arguments and result must be within the range [-127, 128]
```

---

#### **Error code** 102

#### **Summary** overflow error

#### **Description** an operation yields a result to large to store

```text
MEMORIA DE DADOS
CODIGO
PUSH 100
PUSH 100
ADD ; for arithmetic operations, arguments and result must be within the range [-127, 128]
```

---

#### **Error code** 103

#### **Summary** stack overflow

#### **Description** an attempt was made to `push` a value to an already full stack, but this situation is very rare in practical terms

---

#### **Error code** 104

#### **Summary** stack underflow

#### **Description** an attempt was made to `pop` a value from an empty stack

```text
MEMORIA DE DADOS
CODIGO
LOAD ; the stack is still empty, as such the components needed by the operation are not there
```

---

#### **Error code** 105

#### **Summary** memory address violation

#### **Description** an attempt was made to access the memory through an address that does not sit within its address space [0, 31999]; any instruction that builds an address from the Stack is bound to encounter this error if the MSP and LSB components are such that (MSB * 256 + LSB) does not belong to the addressable space

```text
MEMORIA DE DADOS
CODIGO
PUSH 255
PUSH 255
LOAD ; attempt to load the contents of address 65535 (255 * 256 + 255), which does not belong to the valid range [0, 31999]
```

---

#### **Error code** 106

#### **Summary** unknown instruction

#### **Description** an instruction opcode not belonging to the instruction set has been encountered while running the program; highly unlikely to happen, unless a compiled program is loaded directly to Program Memory

---
