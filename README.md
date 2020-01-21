# Easy Parse: Parser Generator and Plaintext Compiler

Library project which helps incorporate parsing and compiling plaintext into other projects. Basic feature include:
* Compiling grammar into a reusable parser definition - Parser generation is an expensive operation, and hence it is done only once per grammar
* Loading parser definition at run time - Building a parser from XML definition is cheap and it can be done over and over again with every execution of the program
* Applying the parser to plaintext - Parser can be applied to a plaintext to build a parse tree for that text
* Compiling the parse tree into an object - Caller can supply a custom compiler which will be applied to the parse tree

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
    
    Expr -> Expr @'-' number;  # Constants can also be verbatim strings
                               # They do not support escaping and single quotes

As you may suspect, the # symbol denotes beginning of the line comment. Each definition ends with a semicolon. Besides that, grammar consists of three sections, starting with "lexemes", "start" and "rules" keywords. Lexeme definitions are given as regular expressions.

Some lexemes are ignored and do not appear in the grammar (e.g. whitespace in the grammar above). The rest of the lexemes are true terminal symbols, which will be matched by the parser (e.g. the `number` terminal). Starting symbol definition immediately follows. The rest of the grammar is the list of rules.

## Building a Parser
The simplest way to build a parser definition is to compile it using the `parser.exe` tool (produced when [EasyParse.CommandLineTool](EasyParse.CommandLineTool) is built):

    parser -grammar=AdditionGrammar.txt -compile
    
This command will create `AdditionGrammar.xml` file, which should be included in the project as an embedded resource.

When you wish to parse an input text, use the static [Parser.FromXmlResource method](EasyParse/Parsing/Parser.cs). Just supply the resource name and this method will return a valid instance of the `Parser` classs.

``` csharp
using EasyParse.Parsing;

var parser = Parser.FromXmlResource("EasyParse.CalculatorDemo.AdditionGrammar.xml");
```

## Parsing Text
When parser was constructed, you can use it to recognize text.

``` csharp
ParsingResult result = parser.Parse(line);
Console.WriteLine(result);
```

Parser's `Parse` method is returning the [ParsingResult](EasyParse/Parsing/ParsingResult.cs) object which either indicates an error or a successful match. In case of a success, the `ParsingResult` object will hold a parse tree. For instance, code above produces the following output:

    1++2
    Unexpected input: [+(+)] at 3
    
    1 - 2 + 3
    Expr
    |
    +--- Expr
    |   |
    |   +--- Expr
    |   |   |
    |   |   +--- 1
    |   |
    |   +--- -
    |   |
    |   +--- 2
    |
    +--- +
    |
    +--- 3

Note that parse tree is not accessible via the [ParsingResult](EasyParse/Parsing/ParsingResult.cs) object. Parse tree is expressed in objects of internal classes and cannot be used directly by the consumer. You would have to supply a compiler object to build your custom result out of a parsed text.

## Compiling Text
Ultimate purpose of a parser is to compile the parse tree it generates into a custom object. For a given grammar, you can define a class which compiles it into a custom object:

[[Source: EasyParse.CalculatorDemo/AdditiveCompiler.cs](EasyParse.CalculatorDemo/AdditiveCompiler.cs)]

``` csharp
class AdditiveCompiler : MethodMapCompiler
{
    // Corresponds to grammar line: number matches @'\d+';
    private int TerminalNumber(string value) => int.Parse(value);

    // Corresponds to grammar line: Expr -> number;
    public int Expr(int number) => number;

    // Corresponds to grammar lines:
    // Expr -> Expr '+' number;
    // Expr -> Expr @'-' number;
    public int Expr(int a, string op, int b) =>
        op == "+" ? a + b : a - b;
}
```

The simplest way to code a compiler is to derive it from the [EasyParse.Parsing.MethodMapCompiler](EasyParse/Parsing/MethodMapCompiler.cs) class. `MethodMapCompiler` is inspecting methods added by the derived class and matching them with the grammar. These methods will be invoked to traverse the parse tree in postorder (this order is guaranteed).

Concrete compiler defines two kinds of methods. Ones are matching terminal symbols, and their name is always Terminal***, where *** is the name of the terminal from the grammar. Other methods are matching nonterminal symbols from grammar, and their name must match the nonterminal name.

The result of compiling the parse tree using a concrete compiler object is whatever the result was when the last method of the compiler was invoked. That call will correspond to the root node of the parse tree.

Compiling is performed on [ParsingResult](EasyParse/Parsing/ParsingResult.cs), by calling its `Compile` method and passing a concrete compiler:

``` csharp
ParsingResult result = parser.Parse(line);
Console.WriteLine(result.IsSuccess
    ? $"{line} = {result.Compile(new AdditiveCompiler())}"
    : $"Not an additive expression: {result.ErrorMessage}");
```

You have to check whether parsing passed well or not, by testing the `ParsingResult.IsSuccess` property.

Code example above produces output:

    1++2
    Not an additive expression: Unexpected input: [+(+)] at 3
    
    1 - 2 + 3
    1 - 2 + 3 = 2

## Applying Multiple Compilers
You can compile the same parse tree (i.e. [ParsingResult](EasyParse/Parsing/ParsingResult.cs) object) multiple times.

For instance, in addition to the [AdditiveCompiler](EasyParse.CalculatorDemo/AdditiveCompiler.cs) class which calculates an integer value of an additive expression, we can also define a compiler which adds parentheses to the expression:

[[Source: EasyParse.CalculatorDemo/FullAdditiveParenthesizer.cs](EasyParse.CalculatorDemo/FullAdditiveParenthesizer.cs)]

``` csharp
class FullAdditiveParenthesizer : MethodMapCompiler
{
    private string TerminalNumber(string value) => value;

    private string Expr(string value) => value;

    private string Expr(string a, string op, string b) => $"({a} {op} {b})";
}
```

Both compilers can be applied to the same parse tree produced by the parser:

``` csharp
ParsingResult result = parser.Parse(line);
Console.WriteLine(result.IsSuccess
    ? $"{result.Compile(new FullAdditiveParenthesizer())} = {result.Compile(new AdditiveCompiler())}"
    : $"Not an additive expression: {result.ErrorMessage}");
```

After successful parsing, the [FullAdditiveParenthesizer](EasyParse.CalculatorDemo/FullAdditiveParenthesizer.cs) is applied to compile the expression into fully parenthesized form. Then the [AdditiveCompiler](EasyParse.CalculatorDemo/AdditiveCompiler.cs) is applied to the same [ParsingResult](EasyParse/Parsing/ParsingResult.cs) to produce an arithmetic value of the expression.

This code produces output:

    1-2+3-4+5
    ((((1 - 2) + 3) - 4) + 5) = 3

## Parsing Multiline Input
Parser has two variants of the [Parse](EasyParse/Parsing/Parser.cs) method: One receiving a single string, and another receiving a sequence of strings.

``` csharp
class Parser
{
    public ParsingResult Parse(string input);
    public ParsingResult Parse(IEnumerable<string> lines);
}
```

`Parse` method receiving a sequence is used to parse a block of text. Internally, these lines will be glued into a single string, where each separate line ends in a \n character (not \r\n, \n\r, \r or any other end of line combination). Each line, including the last one, is guaranteed to end in \n.

For instance, the following grammar matches separate words from a multiline text, while ignoring whitespace and punctuation.

[[Source: EasyParse.WordAnalysisDemo/Grammar.txt](EasyParse.WordAnalysisDemo/Grammar.txt)]

    lexemes:
    ignore @'[ \t\.,;:!?]+';
    word matches @'[\p{L}-]+';
    
    start: Text;
    
    rules:
    Text -> Line;
    Text -> Text Line;
    Line -> '\n';
    Line -> Words '\n';
    Words -> word;
    Words -> Words word;

Observe that `Line` nonterminal is explicitly matching the end of line character.

For the full example parsing and compiling a multiline text, refer to the [WordAnalysisDemo](EasyParse.WordAnalysisDemo) demo project.
