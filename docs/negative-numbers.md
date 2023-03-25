# [MSP - *Mais Simples Possível*](table-of-contents.md)

# The MSP Language

## Negative Numbers

---

With one [byte](https://en.wikipedia.org/wiki/Byte) (8-bits) it is only possible to represent 256 (2<sup>8</sup>) different values.

If a sign bit is not reserved and we assume that all values are positive, then we will be able to represent any number in the interval [0, 255].

If we reserve one bit to indicate the sign of the number, then we are left with only 7 bits and will be limited to only be able to represent values in the interval [-2<sup>7</sup>, 2<sup>7</sup>-1], which equates to [-128, 127].

There are multiple ways to represent the interval [-128, 127] with 8-bits, one of them being the [two's complement](https://en.wikipedia.org/wiki/Two%27s_complement) (2C). In 2C the bit 7 (the most significant) is reserved to codify the sign and the remaining 7 bits (from 0 to 6) are used to represent the modulo of the value.

The conversion mechanism to and from 2C will not be discussed here. What is relevant is that this is the representation that the MSP's Stack Virtual Machine will use to codify 8-bit and 16-bit numbers.

The below table shows how a certain 8-bit binary number can be interpreted in two different ways, whether you assume the 8-bit sequence is a representation without negative sign or with negative sign (in 2C).

| 8-bit binary | integer in [0, 255] | integer in [-128, 127] (in 2C) |
| - | - | - |
| 1 1111111 | 255 | -1 |
| 1 1111110 | 254 | -2 |
| ... | ... | ...|
| 1 0000001 | 129 | -127 |
| 1 0000000 | 128 | -128 |
| 0 1111111 | 127 | 127 |
| 0 1111110 | 126 | 126 |
| ... | ... | ... |
| 0 0000001 | 1 | 1 |
| 0 0000000 | 0 | 0 |

Therefore, there is nothing telling us if the content of a byte is a number with or without sign. A byte can be whatever we want it to be! The context will define how the content of a byte will be interpreted and it is independent from how it was initialized.

As an example, if in the Data Segment a variable is initialized with the value -1 and, during the execution of the program, the content of that variable is used as part of an address, the binary value 11111111 (corresponding to -1) will be interpreted as 255; similarly, if the variable had been initialized with 255 and later it was used as argument of an arithmetic operation, its content would be assumed to be -1!
