# Easy Parse: Grammar-based Plaintext Parser with Fluent Builder

Easy Parse is the library project which helps incorporate parsing and compiling plaintext into other projects. Basic feature include:
* Defining grammar by combining literals and regular expressions into complex (possibly recursive) definitions
* Associating functions that transform recognized text into objects
* Generating a compiler object from the grammar and mapping functions
* Applying the compiler to convert any text that satisfies grammar into a graph of objects

## Installing Easy Parse

To add Easy Parse library from Package Manager, execute instruction:

```
Install-Package CodingHelmet.EasyParse
```

To add from .NET CLI, execute instruction:

```
dotnet add package CodingHelmet.EasyParse
```

## Example 1: Defining Grammar and Compilation Rules via Native .NET Class

The simplest and most readable method of defining a grammar is to write a native .NET class which exposes one method per production rule.

In this section, we shall demonstrate use of native grammar class which calculates arithmetic expressions. Follow steps below to get started.

1. Create a .NET 5.0 console application and name it `Calculator`
2. Add `CodingHelmet.EasyParse` package from NuGet
3. Edit the `Program` class to read expressions from the console before constructing the calculator:

```c#
using System;

namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Enter expression (empty to quit): ");
                string expression = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(expression)) break;
                // Here will come evaluation
            }
        }
    }
}
```

### Phase 1: Defining the Minimalistic Grammar

After defining an empty console application, we can start developing the grammar. The process will be iterative, to support the simplest arithmetic constructs first, and then to add more and more complex ones. You will start developing a grammar by defining a class which derives from `EasyParse.Native.NativeGrammar`.

```c#
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EasyParse.Native;
using EasyParse.Native.Annotations;

namespace Calculator
{
    class ArithmeticGrammar : NativeGrammar
    {
        protected override IEnumerable<Regex> IgnorePatterns => 
            new[] {new Regex(@"\s+")};

        [Start] public int Number([R("number", @"\d+")] string value) =>
            int.Parse(value);
    }
}
```

This is the bare minimum that satisfies the rules of [`EasyParse.Native.NativeGrammar`](EasyParse/Native/NativeGrammar.cs) base class. Let's walk through the elements and explain them:

1. Implement the `IgnorePatterns` abstract property - [`EasyParse.Native.NativeGrammar`](EasyParse/Native/NativeGrammar.cs) defines abstract property getter `IgnorePatterns`. Derived class must implement this property getter to return a sequence of regular expressions (`System.Text.RegularExpressions.Regex` objects), each defining a pattern that should be ignored in the input. It is common to ignore whitespace (regular expression '\s+'), or end of line (regular expression '\n').
2. Define start symbol - Class will expose one or more public methods, where each method represents one production rule. Exactly ***one*** of the methods must be endorsed with the [`EasyParse.Native.Annotations.StartAttribute`](EasyParse/Native/Annotations\StartAttribute.cs), to mark the start symbol of the grammar.
3. Define one public method per production rule - Name of each method defines the name of the non-terminal symbol produced by that rule. The `ArithmeticGrammar.Number()` method defines non-terminal symbol `Number`.
4. Specify type of each non-terminal symbol - Return type of the public method will specify the type of the object constructed when non-terminal symbol is compiled. `Name` symbol will be compiled into a `System.Int32` object.
5. Define right-hand side of each production rule - Argument list of a public method represents production rule's body, specifying terminal and non-terminal symbols in their order. An argument can either refer a non-terminal, a literal terminal, or a terminal matched by a regular expression.
6. Specify regular expressions in rule bodies - `string value` argument of the `ArithmeticGrammar.Number()` method defines a terminal symbol matched by a regular expression, as indicated by the `R` attribute applied to the argument (instance of the [`EasyParse.Native.Annotations.RAttribute`](EasyParse/Native/Annotations/RAttribute.cs)). Regular expression is defined by a unique name and pattern. Each time that pattern is matched in the input text, a terminal symbol named `number` will be formed in the parsing tree and, eventually, passed as argument to the `ArithmeticGrammar.Number()` method.

Bottom line is that `ArithmeticGrammar` class is defining a grammar which, quite informally, can be specified as:

```
Lexer:
ignore \s+
number matches \d+

Parser:
Number -> number
```

###  Phase 2: Building the Compiler

The [`EasyParse.Native.NativeGrammar`](EasyParse/Native/NativeGrammar.cs) defines public method `BuildCompiler<T>`:

```c#
class NativeGrammar
{
    ...
    public Compiler<T> BuildCompiler<T>();
    ...
}
```

This method can be used to obtain an object of type [`EasyParse.Parsing.Compiler<T>`](EasyParse/Parsinga/Compiler.cs). This object will later be used to convert plaintext into the specified type `T`.

***Important note:*** Building a compiler is a costly operation. Also, compiler is a stateless object, and hence thread-safe. Combined effect of these two is that one can opt to build a compiler object as a singleton, so that it can be used through the entire lifetime of the application.

To demonstrate building and using arithmetic compiler (a.k.a. the calculator), modify `Program` class as below.

```c#
using System;
using EasyParse.Parsing;

namespace Calculator
{
    class Program
    {
        // Create a singleton compiler
        static Compiler<int> Calculator { get; } =
            new ArithmeticGrammar().BuildCompiler<int>();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Enter expression (empty to quit): ");
                string expression = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(expression)) break;

                // Apply the compiler to obtain either the object, or the error report
                CompilationResult<int> result = Calculator.Compile(expression);
                
                // Print out the object (int value), or error message
                if (result.IsSuccess)
                    Console.WriteLine(result.Result);
                else
                    Console.WriteLine($"ERROR: {result.ErrorMessage}");
            }
        }
    }
}
```

There are several additions made to previous version of the `Program` class:

1. The `Calculator` static property is initialized to the compiler which results from calling `BuildCompiler<int>()` on the `ArithmeticGrammar` object. This property will be used to parse all lines of text through the entire lifetime of the application.
2. When an input line is read from the console, it is subdued to the `Compile` method of the compiler, so to obtain compilation result strongly typed to `System.Int32` result - an instance of the [`EasyParse.Parsing.CompilationResult<T>`](EasyParse/Parsing/CompilationResult.cs) class.
3. Compilation result object is then examined, to see if the process ended in constructing an `int` result, as desired, or in an error. Either way, the corresponding message is formatted for output.

This completes support for arithmetic expressions which only consist of a single non-negative number. Below is the sample output produced by this application when run. Observe how whitespace is ignored in input.

```
Enter expression (empty to quit): 0
0 = 0
Enter expression (empty to quit): 5
5 = 5
Enter expression (empty to quit): 12
12 = 12
Enter expression (empty to quit):      123
123 = 123
Enter expression (empty to quit):    2   +   3
ERROR: Unexpected input at 8: +   3
Enter expression (empty to quit):
```

### Phase 3: Supporting Addition and Subtraction

Now that we have support for plain numbers, we can introduce the first arithmetic operation - addition and subtraction, represented by the `+` or `-` operator. Edit the `ArithmeticGrammar` class to include changes indicated below.

```c#
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EasyParse.Native;
using EasyParse.Native.Annotations;

namespace Calculator
{
    class ArithmeticGrammar : NativeGrammar
    {
        protected override IEnumerable<Regex> IgnorePatterns => 
            new[] {new Regex(@"\s+")};

        public int Number([R("number", @"\d+")] string value) =>                           // <-- Not Start anymore
            int.Parse(value);

        [Start] public int Additive(int number1, [L("+", "-")] string op, int number2) =>  // <-- New method
            op == "+" ? number1 + number2 : number1 - number2;
    }
}
```

Please note that start symbol is now called `Additive()`, and hence the `Number()` method is not adorned with the [`StartAttribute`](EasyParse/Native/Annotations/StartAttribute.cs) anymore.

The `Additive()` method is defining a new non-terminal symbol, with right-hand side consisting of these symbols:

* Non-terminal symbol `Number` - indicated by parameter named `number1` (digit added to distinguish it from parameter `number2`); this parameter will be matched by name (`number`, when digit `1` is stripped off) and by type (`int`) with non-terminal symbol `Number`.
* Literal terminal symbol `+` or `-` - literals are indicated with the [`L`](EasyParse/Native/Annotations/LAttribute.cs) attribute; this attribute receives a list consisting of one or more strings; method will be invoked if any of those strings is matched against input text.
* Another non-terminal symbol `Number` - for the purpose of writing a legal method declaration in C#, two non-terminals are made distinct as `number` and `number2`, but they both refer (by name and by type) to non-terminal defined by the `Number()` method.

The `Additive()` method will be invoked when parser matches `Number`, `+`/`-`, `Number` sequence of symbols in the input, and three values (`int`, `string` and `int`, respectively) will be passed to the `Additive()` method for processing. As this method's body is uncovering, it will either add or subtract the two `int` values to produce the resulting `int`.

We can run the application now, and it would produce output shown below when sample input is used.

```
Enter expression (empty to quit): 12
ERROR: Unexpected end of input
Enter expression (empty to quit): 12+3
12+3 = 15
Enter expression (empty to quit):
```

It appears that we have added one feature, but lost the one we used to have before. When a plain number is entered, parsing error is reported, stating that more input was expected. That is because grammar's start symbol defines that for plaintext to be considered correct, it must consist of two numbers delimited with an addition or subtraction operator. New grammar is dismissing a single number as a valid expression!

To correct the issue, we have to augment definition of the expression. We can accomplish that by specifying that a number is a valid additive expression in its own right, and, on top of that, that additive expressions can be formed by adding or subtracting a number from an existing additive expression.

Edit the `ArithmeticGrammar` class to include changes in the `Additive()` symbol definition.

```c#
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EasyParse.Native;
using EasyParse.Native.Annotations;

namespace Calculator
{
    class ArithmeticGrammar : NativeGrammar
    {
        protected override IEnumerable<Regex> IgnorePatterns => 
            new[] {new Regex(@"\s+")};

        public int Number([R("number", @"\d+")] string value) =>
            int.Parse(value);

        [Start] public int Additive(int number) =>                                 // <-- New method
            number;
        public int Additive(int additive, [L("+", "-")] string op, int number) =>  // <-- Modified
            op == "+" ? additive + number : additive - number;
    }
}	
```

Please note that addition is defined as left-associative in this grammar, i.e., addition and subtraction operators will be applied from left to right. That effect is ensured by placing `additive` symbol first, and only then the `number` in the right-hand side of the `Additive` production rule.

When application is run, you will notice that all kinds of additions and subtractions are supported. Here is the sample output.

```
Enter expression (empty to quit): 12
12 = 12
Enter expression (empty to quit): 4-2
4-2 = 2
Enter expression (empty to quit): 12 + 45  -  26
12 + 45  -  26 = 31
Enter expression (empty to quit):
```

### Phase 4: Supporting Negative Numbers and Parentheses

We have covered large distance so far, and you should be able to follow through the code below without help now.

```c#
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EasyParse.Native;
using EasyParse.Native.Annotations;

namespace Calculator
{
    class ArithmeticGrammar : NativeGrammar
    {
        protected override IEnumerable<Regex> IgnorePatterns => 
            new[] {new Regex(@"\s+")};

        public int Unit([R("number", @"\d+")] string value) =>
            int.Parse(value);
        public int Unit([L("-")] string minus, int unit) =>                             // <-- New method
            -unit;
        public int Unit([L("(")] string open, int additive, [L(")")] string close) =>   // <-- New method
            additive;

        [Start] public int Additive(int unit) =>                                        // <-- Modified
            unit;
        public int Additive(int additive, [L("+", "-")] string op, int unit) =>         // <-- Modified
            op == "+" ? additive + unit : additive - unit;
    }
}
```

Modifications, compared to previous grammar, are as follows:

* What used to be called `Number` so far, will from now on be referred to as `Unit`. A unit will represent a single block of numbers and operations which is indistinguishable from a plain number (clarification follows).
* A `Unit` is defined in one of three ways: a plain non-negative number; a minus sign followed by a unit; an additive expression placed within a pair of parentheses. Any of these is considered a unit: `15`, `-15`, `(15 + 40)`, `-(15 + 40)`. Notice that any of these forms can stand in place of a single number, which clarifies the first bullet point.
* `Additive` symbol is still defined the same way as it used to be, only with a notable distinction that now it references the `Unit` symbol, via method parameters named `unit`. While evolving your grammars through renaming non-terminal symbols, you must never forget to rename corresponding method parameters, or otherwise resulting grammar will not be consistent and you will receive an exception when you try to build the `Compiler<T>` object at run time.

Now that all new elements in this grammar have been explained, we can run the application and subdue some expressions to the new parser:

```
Enter expression (empty to quit): 25
25 = 25
Enter expression (empty to quit): -25
-25 = -25
Enter expression (empty to quit): 12--25
12--25 = 37
Enter expression (empty to quit): -25+12
-25+12 = -13
Enter expression (empty to quit): 12-(2+3)
12-(2+3) = 7
Enter expression (empty to quit): 12-(2-(4+6)+9)
12-(2-(4+6)+9) = 11
Enter expression (empty to quit):
```

### Phase 5: Adding Support for Multiplication and Division

The final step in defining grammar for arithmetic expressions is to add support for multiplication and division. Multiplicative elements are formed by either multiplying/dividing other multiplicative elements with units, or by multiplying/dividing units themselves. Both multiplication and division are left-associative, just like addition and subtraction are. Multiplicative operations have precedence over additive ones, and so multiplicative element will be defined between `Unit` and `Additive` - it will be formed out of `Unit`s, and each `Unit` will be multiplicative, while `Additive` will be formed out of multiplicative elements.

Below is the grammar which defines operations in that way.

```c#
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EasyParse.Native;
using EasyParse.Native.Annotations;

namespace Calculator
{
    class ArithmeticGrammar : NativeGrammar
    {
        protected override IEnumerable<Regex> IgnorePatterns => 
            new[] {new Regex(@"\s+")};

        public int Unit([R("number", @"\d+")] string value) =>
            int.Parse(value);
        public int Unit([L("-")] string minus, int unit) =>
            -unit;
        public int Unit([L("(")] string open, int additive, [L(")")] string close) =>
            additive;

        public int Multiplicative(int unit) =>                                              // <-- New method
            unit;
        public int Multiplicative(int multiplicative, [L("*", "/")] string op, int unit) => // <-- New method
            op == "*" ? multiplicative * unit : multiplicative / unit;

        [Start] public int Additive(int multiplicative) =>                                  // <-- Modified
            multiplicative;
        public int Additive(int additive, [L("+", "-")] string op, int multiplicative) =>   // <-- Modified
            op == "+" ? additive + multiplicative : additive - multiplicative;
    }
}
```

With multiplication and division defined, we have completed grammar definition which supports numbers, unary minus, parentheses, and four basic arithmetic operations, all in accordance to common mathematical rules.

When we run the application, it will produce following output.

```
Enter expression (empty to quit): 12
12 = 12
Enter expression (empty to quit): 12+3
12+3 = 15
Enter expression (empty to quit): 12*4
12*4 = 48
Enter expression (empty to quit): 12*(3+4/2)
12*(3+4/2) = 60
Enter expression (empty to quit): 12+-(9/(5-2)+6)
12+-(9/(5-2)+6) = 3
Enter expression (empty to quit):
```

### Conclusion

In this demonstration, we have constructed a parser which is capable of transforming a string containing a valid arithmetic expression into a number, which is the result of evaluating that expression.

In only 20 lines of code, we have defined all the (natively recurrent) rules of arithmetic expressions, by only using plain C# code and no libraries at all. Except for possessing basic understanding of grammars and parsing in general, one does not need to learn implementation of EasyParse library.

Also, if horizontal space permits, one can organize C# class in form which closely resembles [EBNF](https://en.wikipedia.org/wiki/Extended_Backus%E2%80%93Naur_form) notation. Below is the final, and condensed, implementation of the `ArithmeticGrammar` class.

```c#
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EasyParse.Native;
using EasyParse.Native.Annotations;

namespace Calculator
{
    class ArithmeticGrammar : NativeGrammar
    {
        protected override IEnumerable<Regex> IgnorePatterns => new[] {new Regex(@"\s+")};

        public int Unit([R("number", @"\d+")] string value) => int.Parse(value);
        public int Unit([L("-")] string minus, int unit) => -unit;
        public int Unit([L("(")] string open, int additive, [L(")")] string close) => additive;

        public int Multiplicative(int unit) => unit;
        public int Multiplicative(int multiplicative, [L("*", "/")] string op, int unit) =>
            op == "*" ? multiplicative * unit : multiplicative / unit;

        [Start] public int Additive(int multiplicative) => multiplicative;
        public int Additive(int additive, [L("+", "-")] string op, int multiplicative) =>
            op == "+" ? additive + multiplicative : additive - multiplicative;
    }
}
```

## Use Case 2: Defining Grammar and Compilation Rules via Fluent API

Parsing grammar can be defined by deriving a class from the [`EasyParse.Parsing.Grammar`](EasyParse/Parsing/Grammar.cs) abstract base class.

```c#
public abstract class Grammar
{
    protected abstract IRule Start { get; }
    protected abstract IEnumerable<RegexSymbol> Ignore { get; }

    public Parser BuildParser();
    public Compiler<T> BuildCompiler<T>();
}
```

Implementer must provide the two abstract property getters:

* `Start` - Returns the grammar's start nonterminal symbol
* `Ignore` - Returns a sequence of regular expressions that define lexemes that should be ignored in the input

Once the grammar is defined, the caller can call its `BuildCompiler` method to obtain an instance of `Compiler<T>` class, which can be used to transform any text into strongly-typed object of type `T` by applying the rules defined in the grammar file.

### Building Grammar Rules Using Fluent API

Every grammar class can build rules by first calling the `Rule()` protected method inherited from the [`PartialGrammar`](EasyParse/Parsing/PartialGrammar.cs) base class, and then following its fluent rule building interface.

## Use Case 3: Writing Grammar File and Compiler Class

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
