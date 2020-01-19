# Easy Parse - Parser Generator

Library project which helps incorporate parsing and compiling plaintext into other projects. Basic feature include:
* Compiling grammar into a reusable parser definition - Parser generation is an expensive operation, and hence it is done only once per grammar
* Loading parser definition at run time - Building a parser from XML definition is cheap and it can be done over and over again with every execution of the program
* Applying the parser to plaintext - Parser can be applied to a plaintext to build a syntax tree for that text
* Compiling the syntax tree into an object - Caller can supply a custom compiler which will be applied to the syntax tree

Grammar format is intuitive and simple. It mostly resembles what one would write with pencil and paper.
Below is an example of a valid grammar which recognizes arithmetic expressions with addition and subtraction.
Operators are applied from left to right.

## Defining a Grammar
[[Source: EasyParse.CalculatorDemo/AdditionGrammar.txt]](EasyParse.CalculatorDemo/AdditionGrammar.txt)

    lexemes:                   # Mandatory block defining lexical analysis
    ignore @'\s+';             # Ignore text matching regex (i.e. whitespace)
    number matches @'\d+';	   # Match stream of digits as a lexeme called 'number'
    
    start: Expr;               # Mandatory block defining the starting symbol
    
    rules:                     # Mandatory block defining the grammar rules
    
    Expr -> number;            # All rules are in form:
                               # non-terminal, arrow, body, semicolon
    
    Expr -> Expr '+' number;   # Constants can appear in rules under single quotes
                               # Strings can be escaped like in C/C++
    
    Expr -> Expr @'-' number;  # Constants can also be verbatim stringsT
                               # They do not support escaping and single quotes

As you may suspect, the # symbol denotes beginning of the line comment. Each definition ends with a semicolon. Besides that, grammar consists of three sections, starting with "lexemes", "start" and "rules" keywords. Lexeme definitions are given as regular expressions.

Some lexemes are ignored and do not appear in the grammar (e.g. whitespace in the grammar above). The rest of the lexemes are true terminal symbols, which will be matched by the parser (e.g. the `number` terminal). Starting symbol definition immediately follows. The rest of the grammar is the list of rules.

## Building a Parser
The simplest way to build a parser definition is to compile it using the `parser.exe` tool (produced when [EasyParse.CommandLineTool](EasyParse.CommandLineTool) is built):

    parser -grammar=AdditionGrammar.txt -compile
    
This command will create `AdditionGrammar.xml` file, which should be included in the project as an embedded resource.

When you wish to parse an input text, use the static [Parser.FromXmlResource method](EasyParse/Parsing/Parser.cs). Just supply the resource name and this method will return a valid instance of the `Parser` classs.

    using EasyParse.Parsing;
    
    var parser = Parser.FromXmlResource("EasyParse.CalculatorDemo.AdditionGrammar.xml");

