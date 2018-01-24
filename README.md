# RPGParser
A parser for randomizing formatted text.

This is a recursive parser that reads through strings written in a specific format such that it can randomize
the string content appearing in bracketed lists. It is a visual studio c# project consisting of a single class
for the parser.

example

"[a|b] [c|d|e|f|[1|2] thing|last]" -> "a thing"
or
-> "b c"
or 
- "a 2"

Any section appearing inside '[' and ']' will be parsed. Options inside are dilineated by '|'. The parser is recursive; nested
bracket sections are parsed.

What is this for?

This was made to assist with procedurally generating funny sentences and sayings for characters in my app The Tartle RPG Tool.
Instead of writing methods to pick from lists of choices, I wanted a faster way to write out sentences with nested choices. 

