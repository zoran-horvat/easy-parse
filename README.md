# Easy Parse - Parser Generator

Library project which helps incorporate parsing and compiling plaintext into other projects. Basic feature include:
* Compiling grammar into a reusable parser definition - Parser generation is an expensive operation, and hence it is done only once per grammar
* Loading parser definition at run time - Building a parser from XML definition is cheap and it can be done over and over again with every execution of the program
* Applying the parser to plaintext - Parser can be applied to a plaintext to build a syntax tree for that text
* Compiling the syntax tree into an object - Caller can supply a custom compiler which will be applied to the syntax tree

Grammar format is intuitive and simple. It mostly resembles what one would write with pencil and paper.
Below is an example of a valid grammar which recognizes arithmetic expressions with addition and subtraction.
Operators are applied from left to right.

