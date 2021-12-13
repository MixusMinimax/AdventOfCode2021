# Advent of Code 2021

#### My Solutions to [AdventOfCode/2021](https://adventofcode.com/2021)

Written in C#10, dotnet-6.0

A lot of Linq, because it is cool to be able to implement large chunks of logic in a single statement. Useful? No. Fast?
Not necessarily, but I did make sure to optimize what needed to be optimized (i.e. [Day 6](./days/Day06.cs)).

This is my first time working with dotnet for real (ignoring Mono, especially since in Unity the language is very far
behind).

This project uses Reflection to run only the code for the current day. If it is the 10th in the month, Day10.cs will be
executed. Is this useful? Absolutely not. But I needed a reason to use Reflection somewhere in this project.

I decided to make this repository public since it is [allowed](https://adventofcode.com/2021/about#faq_streaming) to do so after the leaderboard of the day is full.
This isn't a problem for me since the leaderboard is long full when I even wake up.

### Example Usage

```powershell
# Run the first step of today with input from a file:
dotnet run .\input\sample_input.txt
dotnet run 1 .\input\sample_input.txt

# Run the second step of today with input from a file:
dotnet run 2 .\input\sample_input.txt

# Run the third step of today with input from a file and extra input:
dotnet run 3 .\input\sample_input.txt 69
```
