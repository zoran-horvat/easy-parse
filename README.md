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

## Quick Demo: Parsing Arithmetic Expressions

Suppose that there is a source of strings, where each string represents one arithmetic expression:

```c#
string s1 = "12*(3+4/2)";
string s2 = "12+-(9/(5-2)+6) * (4-2)";
```

Our goal is to calculate an `int` value of each expression. Using EasyParse library, we need a class like the one below, which derives from [`EasyParse.Native.NativeGrammar`](EasyParse/Native/NativeGrammar.cs) and defines arithmetic rules in form of a context-free grammar.

To try this code out, add the [`CodingHelmet.EasyParse`](https://www.nuget.org/packages/CodingHelmet.EasyParse/) package and paste code below.

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

To apply the grammar, you need to build a corresponding compiler object:

```c#
EasyParse.Parsing.Compiler<int> compiler = new ArithmeticGrammar().BuildCompiler<int>();
```

This is the costly operation - major cost is paid when a compiler object is generated, so that subsequent parsing operations can be as quick as possible. Therefore, you will prefer to build a compiler once during the application's lifetime, and then use it many times over.

To use the compiler object, simply subdue the string to its `Compile` method:

```c#
Console.WriteLine($"{s1} = {compiler.Compile(s1)}");
Console.WriteLine($"{s2} = {compiler.Compile(s2)}");
```

This code prints correct results for both expressions:

```
12*(3 + 4/2) = 60
12+-(9/(5-2)+6) * (4-2) = -6
```

In the remainder of this display, you will see how grammar, lexemes and parsing tree can be displayed for a grammar and input string. But please note that those scenarios are not required to use the EasyParse library. They are shown here for demonstration purposes only, and may be of practical use in debugging.

The only scenario regularly applied in custom code should consist of the three steps outlined above:

1. Define a class deriving from [`EasyParse.Native.NativeGrammar`](EasyParse/Native/NativeGrammar.cs) abstract class.
2. Call `BuildCompiler<T>()` method, specifying type `T` returned by the method indicated as start symbol, to obtain an instance of `Compiler<T>` class; for performance reasons, make sure to only call `BuildCompiler<T>` once if possible, and to use the resulting `Compiler<T>` object as a singleton.
3. Call `Compile()` method on the `Compiler<T>` for every input string or sequence of strings you desire.

### Seeing the Grammar

If you find it easier to ponder over a proper grammar files, rather than a C# class defining its rules, you can easily transform this the `ArithmeticGrammar` object into a more readable form:

```c#
ArithmeticGrammar grammar = new ArithmeticGrammar();
foreach (string line in grammar.ToGrammarFileContent())
    Console.WriteLine(line);
```

This loop will print the following grammar definition:

```
lexemes:
ignore '\s+';
match number is '\d+';

start: Additive;

rules:
Unit -> number;                             #1
Unit -> '-' Unit;                           #2
Unit -> '(' Additive ')';                   #3

Multiplicative -> Unit;                     #4
Multiplicative -> Multiplicative '*' Unit;  #5
Multiplicative -> Multiplicative '/' Unit;  #6

Additive -> Multiplicative;                 #7
Additive -> Additive '+' Multiplicative;    #8
Additive -> Additive '-' Multiplicative;    #9
```

The `ArithmeticGrammar` class is defining both lexer and parser. Lexer will ignore whitespace and isolate terminal symbols that are either sequences of digits, or literals `+`, `-`, `*`, `/`, `(` and `)`.

When it comes to parsing, there are total of nine rules, some of them directly or indirectly recursively depending on themselves. Symbol named `Additive` is the start symbol.

### Seeing the Lexer at Work

If you wished to see tokens produced by the lexer, that can also be done (primarily for debugging purposes). Lexer is accessible from the [`EasyParse.Parsing.Parser`](EasyParse/Parsing/Parser.cs) object. Once the lexer is at hand, pass the input to its `Tokenize()` method as in the code below.

```c#
var parser = grammar.BuildParser();
foreach (var lexeme in  parser.Lexer.Tokenize(Plaintext.Line(s1)))
    Console.Write($"{lexeme} ");
Console.WriteLine();
```

This code sample will tokenize the first test expression, and when it does, it will produce the following output:

```
[number(12)] [*] [(] [number(3)] [+] [number(4)] [/] [number(2)] [)] [End of input]
```

All lexemes except whitespace, which is marked to be ignored, are present in the output.

Once again, lexer is not intended to be used directly, and this code is only shown for demonstration purposes.

### Seeing the Parsing Tree

When a parser is applied to input text, it builds the parsing tree. It is only after that step that methods defined in the grammar class are invoked, one method called exactly once for each node in the parsing tree to which it corresponds.

If you wished to see the parsing tree, as it would be built during the call to `Compiler<T>.Compile()` method, then apply the parser object to input text and print its result out:

```c#
Console.WriteLine(s1);
Console.WriteLine(parser.Parse(s1));
```

When this code executes, it will obtain the parsing tree constructed for the first sample string and print it out to the console.

```
12 * (3 + 4 / 2)
 Additive [rule #7]
 |
 +--- Multiplicative [rule #5]
     |
     +--- Multiplicative [rule #4]
     |   |
     |   +--- Unit [rule #1]
     |       |
     |       +--- 12
     |
     +--- *
     |
     +--- Unit [rule #3]
         |
         +--- (
         |
         +--- Additive [rule #8]
         |   |
         |   +--- Multiplicative [rule #4]
         |   |   |
         |   |   +--- Unit [rule #1]
         |   |       |
         |   |       +--- 3
         |   |
         |   +--- +
         |   |
         |   +--- Additive [rule #7]
         |       |
         |       +--- Multiplicative [rule #6]
         |           |
         |           +--- Multiplicative [rule #4]
         |           |   |
         |           |   +--- Unit [rule #1]
         |           |       |
         |           |       +--- 4
         |           |
         |           +--- /
         |           |
         |           +--- Unit [rule #1]
         |               |
         |               +--- 2
         |
         +--- )
```

The tree is quite verbose, but it is easy to follow the grouping logic that was exercised by the parser when it saw the input string.

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

        [Start] public int Additive(int number1, [L("+", "-")] string op, int number2) =>  // <-- New
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

        [Start] public int Additive(int number) =>                                 // <-- New
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
        public int Unit([L("-")] string minus, int unit) =>                             // <-- New
            -unit;
        public int Unit([L("(")] string open, int additive, [L(")")] string close) =>   // <-- New
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

        public int Multiplicative(int unit) =>                                              // <-- New
            unit;
        public int Multiplicative(int multiplicative, [L("*", "/")] string op, int unit) => // <-- New
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
