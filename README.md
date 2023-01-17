# CFTToPCREConverter

This tool reads a .txt file containg the definition of any contex free grammar and will return a PCRE compatible string that can detect the given grammar.

To compile the program, run the following command in the root folder
```
csc src\*
```

To run the program, enter the following command in the directory containing the `Program.exe`

```
Program.exe [pathToFile.txt]
```
where ```[pathToFile]``` is the relative path from the .exe's directory to the .txt file containing the grammar definition.

# Grammar files

The following grammar definitions are all valid, remeber that terminals are marked by singel quots surrounding them:

```
S -> A

S => A B

S -> A | B

S > 'a'B | A

S -> B | 'a'

S > A | B | A B 'abc'

S => A |
```

if the production rule contains epsilon, create a seperator followed by the end line character, like in the last example.

# Example

## Example input:

```
S -> A S A |'a'B
A -> B|S
B -> 'b'|
```
## Example output:
```
In S -> A S A |'a'B
In A -> B|S
In B -> 'b'|
After Epsilon eliminiation:
S -> S |A S |S A |A S A |'a' B |'a' 
A -> B |S 
B -> 'b' 

After Removing LeftRecursion:
S -> A S S0 |A S A S0 |'a' B S0 |'a' S0 
A -> B |S 
B -> 'b' 
S0 -> A S0 |

After Epsilon eliminiation:
S -> 'a' |'a' S0 |'a' B |'a' B S0 |A S A |A S A S0 |A S |A S S0 
A -> S |B 
B -> 'b' 
S0 -> A |A S0 

After Removing indirect Left reucrsion:
S -> 'a' |'a' S0 |'a' B |'a' B S0 |S S S0 |B S S0 |S S |B S |S S A S0 |B S A S0 |S S A |B S A 
A -> S |B 
B -> 'b' 
S0 -> A |A S0 

Add new prod rule to derivate S0
After Removing LeftRecursion:
S -> 'a' S0 |'a' S0 S0 |'a' B S0 |'a' B S0 S0 |B S S0 S0 |B S S0 |B S A S0 S0 |B S A S0 
A -> S |B 
B -> 'b' 
S0 -> A |A S0 |S A S0 |S A S0 S0 |S S0 |S S0 S0 |

After Epsilon eliminiation:
S -> B S A |B S A S0 |B S A S0 S0 |B S |B S S0 |B S S0 S0 |'a' B |'a' B S0 |'a' B S0 S0 |'a' |'a' S0 |'a' S0 S0 
A -> B |S 
B -> 'b' 
S0 -> S |S S0 |S S0 S0 |S A |S A S0 |S A S0 S0 |A |A S0 

(?(DEFINE)(?<S>(?&B)(?&S)(?&A)|(?&B)(?&S)(?&A)(?&S0)|(?&B)(?&S)(?&A)(?&S0)(?&S0)|(?&B)(?&S)|(?&B)(?&S)(?&S0)|(?&B)(?&S)(?&S0)(?&S0)|a(?&B)|a(?&B)(?&S0)|a(?&B)(?&S0)(?&S0)|a|a(?&S0)|a(?&S0)(?&S0))(?<A>(?&B)|(?&S))(?<B>b)(?<S0>(?&S)|(?&S)(?&S0)|(?&S)(?&S0)(?&S0)|(?&S)(?&A)|(?&S)(?&A)(?&S0)|(?&S)(?&A)(?&S0)(?&S0)|(?&A)|(?&A)(?&S0)))^(?&S)$
```