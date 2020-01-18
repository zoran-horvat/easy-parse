# Easy Parse - Parser Generator

Library project which helps incorporate parsing and compiling plaintext into other projects. Basic feature include:
* Compiling grammar into a reusable parser definition - Parser generation is an expensive operation, and hence it is done only once per grammar
* Loading parser definition at run time - Building a parser from XML definition is cheap and it can be done over and over again with every execution of the program
* Applying the parser to plaintext - Parser can be applied to a plaintext to build a syntax tree for that text
* Compiling the syntax tree into an object - Caller can supply a custom compiler which will be applied to the syntax tree

Grammar format is intuitive and simple. It mostly resembles what one would write with pencil and paper.
Below is an example of a valid grammar which recognizes arithmetic expressions with addition and subtraction.
Operators are applied from left to right.

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

Some lexemes are ignored and do not appear in the grammar. You would typically skip whitespace and newline characters (in multiline parsers).

The rest of the lexemes are true terminal symbols, which will be matched by the parser. Each nonterminal symbol has a name and a regular expression which matches it in the input. Name of a nonterminal starts with a lowercase letter, followed by letters, digits or underscores.

Starting symbol definition follows, and it gives name of the grammar's start symbol.

After that, the list of rules is defined. Rules can include nonterminals, terminals and constants. Nonterminals are named with initial uppercase letter, followed by letters, digits or underscores.
