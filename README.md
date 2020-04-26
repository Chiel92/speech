# Speech
This repository contains experiments in voice commanded programming.

## StackSpeak

A simple stack-oriented voice-driven programming language that runs on Windows.

In its current form, StackSpeak comes with a live interpreter-environment that has to be
controlled by voice commands. It is highly recommended that you do the speech recognition
improvement program (found at Control Panel -> Train your computer to better understand you) at
least once.

### Vocabulary

Currently the following basic operations are supported.
- `push <int>`: push given number on the stack.
- `pull <int>`: pull the item at the given position on top the stack.
- `add together`: pop the top two numbers from the stack and push their sum on the stack.
- `subtract`: pop the top two numbers from the stack and push their difference on the stack.
- `multiply`: pop the top two numbers from the stack and push their product on the stack.
- `duplicate`: push the current top item on the stack once more.
- `scratch`: pop the current top item from the stack.
- `less than`: pop the top two numbers from the stack and push their comparison on the stack.
  If the first number is less than the second, push a `1`, otherwise a `0`.

StackSpeak supports defining and executing macros with the following commands. Recursion is
supported.
- `define <letter> as <sequence of operations>`: define macro
- `call <letter> now`: execute macro
- `maybe call <letter> now`: conditionally execute macro. Consume the top item on the stack. If
  this was a `0` do nothing else, otherwise execute macro.

StackSpeak supports the following miscellaneous commands.
- `undo`: undo the last change of the stack.
- `uhm`: do nothing. Can be used to stitch operations together when defining a macro.

### Exercises

1. Starting with stack `[1]` and without using the Push operation,
    generate the stack `[25]`.
2. Starting with stack `[1]` and without using the Push operation,
    generate the stack `[1; 2; 3; 4; 5]`.
2. Starting with stack `[1]` and without using the Push operation,
    generate the stack `[1; 2; 3; 4; 5; ...; 10]` *with as few operations as possible*.
3. Starting with stack `[1]` and without using the Push operation,
    generate the first 20 numbers of the fibonacci sequence `1, 1, 2, 3, 5, 8, 13, 21, 34,
    ...`.
4. Starting with stack `[1]` and without using the Push operation,
    generate the first 20 numbers of the quadratic sequence 4, 9, 16, 25...
5. Generate the sequence [1, 2, 3, 4, ..., 100] on the stack.
6. Starting with stack `[1..100]` from the previous exercise, and without using the Push operation,
    generate the sum of all the numbers.
